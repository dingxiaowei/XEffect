using UnityEngine;
using System.Collections;

namespace Xft
{
    public class GlowPerObjEvent : CameraEffectEvent
    {

        public Shader ReplacementShader;

        protected GameObject m_shaderCamera;
        protected Camera ShaderCamera
        {
            get
            {
                if (m_shaderCamera == null)
                {
                    m_shaderCamera = new GameObject("ShaderCamera", typeof(Camera));
                    m_shaderCamera.GetComponent<Camera>().enabled = false;
                    m_shaderCamera.hideFlags = HideFlags.HideAndDontSave;
                }

                return m_shaderCamera.GetComponent<Camera>();
            }
        }

        protected RenderTexture TempRenderTex;
        protected RenderTexture TempRenderGlow;

        protected Color m_tint;

        #region property
        public Shader compositeShader;
        Material m_CompositeMaterial = null;
        protected Material compositeMaterial
        {
            get
            {
                if (m_CompositeMaterial == null)
                {
                    m_CompositeMaterial = new Material(compositeShader);
                    m_CompositeMaterial.hideFlags = HideFlags.HideAndDontSave;
                }
                return m_CompositeMaterial;
            }
        }


        public Shader blurShader;
        Material m_BlurMaterial = null;
        protected Material blurMaterial
        {
            get
            {
                if (m_BlurMaterial == null)
                {
                    m_BlurMaterial = new Material(blurShader);
                    m_BlurMaterial.hideFlags = HideFlags.HideAndDontSave;
                }
                return m_BlurMaterial;
            }
        }


        public Shader downsampleShader;
        Material m_DownsampleMaterial = null;
        protected Material downsampleMaterial
        {
            get
            {
                if (m_DownsampleMaterial == null)
                {
                    m_DownsampleMaterial = new Material(downsampleShader);
                    m_DownsampleMaterial.hideFlags = HideFlags.HideAndDontSave;
                }
                return m_DownsampleMaterial;
            }
        }

        //final blend the temp render tecture and dest texture.
        public Shader blendShader;
        Material m_blendMaterial = null;
        protected Material blendMaterial
        {
            get
            {
                if (m_blendMaterial == null)
                {
                    m_blendMaterial = new Material(blendShader);
                    m_blendMaterial.hideFlags = HideFlags.HideAndDontSave;
                }
                return m_blendMaterial;
            }
        }
        #endregion

        public GlowPerObjEvent(XftEventComponent owner)
            : base(CameraEffectEvent.EType.GlowPerObj, owner)
        {
            ReplacementShader = owner.GlowPerObjReplacementShader;
            blendShader = owner.GlowPerObjBlendShader;
            downsampleShader = owner.GlowDownSampleShader;
            compositeShader = owner.GlowCompositeShader;
            blurShader = owner.GlowBlurShader;
        }

        #region verride functions
        public override void Initialize()
        {
            base.Initialize();
            //Note: In forward lightning path, the depth texture is not automatically generated.
            if (MyCamera.depthTextureMode == DepthTextureMode.None)
                MyCamera.depthTextureMode = DepthTextureMode.Depth;
        }
        public override bool CheckSupport()
        {
            bool ret = true;
            if (!SystemInfo.supportsImageEffects)
            {
                ret = false;
            }
            if (!blurMaterial.shader.isSupported)
                ret = false;
            if (!compositeMaterial.shader.isSupported)
                ret = false;
            if (!downsampleMaterial.shader.isSupported)
                ret = false;

            return ret;
        }
        public override void OnPreRender()
        {
            PrepareRenderTex();

            Camera cam = ShaderCamera;
            cam.CopyFrom(MyCamera);
            cam.backgroundColor = Color.black;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.targetTexture = TempRenderTex;
            cam.RenderWithShader(ReplacementShader, "XftEffect");
        }
        public override void Update(float deltaTime)
        {
            m_elapsedTime += deltaTime;
            float t = m_owner.ColorCurve.Evaluate(m_elapsedTime);
            m_tint = Color.Lerp(m_owner.GlowColorStart, m_owner.GlowColorEnd, t);
        }
        public override void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            //only glow the particle "layer"
            RenderGlow(TempRenderTex, TempRenderGlow);

            blendMaterial.SetTexture("_GlowTex", TempRenderGlow);

            //blendMaterial.SetTexture("_MainTex",source);
            Graphics.Blit(source, destination, blendMaterial);

            //Graphics.Blit (TempRenderGlow, destination);

            ReleaseRenderTex();
        }
        #endregion

        #region helper functions
        void ReleaseRenderTex()
        {
            if (TempRenderGlow != null)
            {
                RenderTexture.ReleaseTemporary(TempRenderGlow);
                TempRenderGlow = null;
            }

            if (TempRenderTex != null)
            {
                RenderTexture.ReleaseTemporary(TempRenderTex);
                TempRenderTex = null;
            }


        }
        void PrepareRenderTex()
        {

            if (TempRenderTex == null)
            {
                TempRenderTex = RenderTexture.GetTemporary((int)MyCamera.pixelWidth, (int)MyCamera.pixelHeight, 0);
            }

            if (TempRenderGlow == null)
            {
                TempRenderGlow = RenderTexture.GetTemporary((int)MyCamera.pixelWidth, (int)MyCamera.pixelHeight, 0);
            }
        }
        void FourTapCone(RenderTexture source, RenderTexture dest, int iteration, float spread)
        {
            float off = 0.5f + iteration * spread;
            Graphics.BlitMultiTap(source, dest, blurMaterial,
                new Vector2(off, off),
                new Vector2(-off, off),
                new Vector2(off, -off),
                new Vector2(-off, -off)
            );
        }

        void DownSample4x(RenderTexture source, RenderTexture dest, Color tint)
        {
            downsampleMaterial.color = new Color(tint.r, tint.g, tint.b, tint.a / 4.0f);
            Graphics.Blit(source, dest, downsampleMaterial);
        }

        void RenderGlow(RenderTexture source, RenderTexture destination)
        {

            float glowIntensity = m_owner.GlowIntensity;
            int blurIterations = m_owner.GlowBlurIterations;
            float blurSpread = m_owner.GlowBlurSpread;

            // Clamp parameters to sane values
            glowIntensity = Mathf.Clamp(glowIntensity, 0.0f, 10.0f);
            blurIterations = Mathf.Clamp(blurIterations, 0, 30);
            blurSpread = Mathf.Clamp(blurSpread, 0.5f, 1.0f);

            RenderTexture buffer = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0);
            RenderTexture buffer2 = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0);

            // Copy source to the 4x4 smaller texture.
            DownSample4x(source, buffer, m_tint);

            // Blur the small texture
            float extraBlurBoost = Mathf.Clamp01((glowIntensity - 1.0f) / 4.0f);
            blurMaterial.color = new Color(1F, 1F, 1F, 0.25f + extraBlurBoost);

            bool oddEven = true;
            for (int i = 0; i < blurIterations; i++)
            {
                if (oddEven)
                {
                    FourTapCone(buffer, buffer2, i, blurSpread);
                    buffer.DiscardContents();
                }
                else
                {
                    FourTapCone(buffer2, buffer, i, blurSpread);
                    buffer2.DiscardContents();
                }
                    
                oddEven = !oddEven;
            }
            Graphics.Blit(source, destination);

            if (oddEven)
                BlitGlow(buffer, destination, glowIntensity);
            else
                BlitGlow(buffer2, destination, glowIntensity);

            RenderTexture.ReleaseTemporary(buffer);
            RenderTexture.ReleaseTemporary(buffer2);
        }

        void BlitGlow(RenderTexture source, RenderTexture dest, float intensity)
        {
            compositeMaterial.color = new Color(1F, 1F, 1F, Mathf.Clamp01(intensity));
            Graphics.Blit(source, dest, compositeMaterial);
        }

        #endregion

    }
}

