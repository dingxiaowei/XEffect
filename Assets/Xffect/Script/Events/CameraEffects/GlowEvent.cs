using UnityEngine;
using System.Collections;

namespace Xft
{
    public class GlowEvent : CameraEffectEvent
    {

        #region member
        protected Color m_glowTint = new Color(1, 1, 1, 1);
        #endregion

        #region property
        // --------------------------------------------------------
        // The final composition shader:
        //   adds (glow color * glow alpha * amount) to the original image.
        // In the combiner glow amount can be only in 0..1 range; we apply extra
        // amount during the blurring phase.
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


        // --------------------------------------------------------
        // The blur iteration shader.
        // Basically it just takes 4 texture samples and averages them.
        // By applying it repeatedly and spreading out sample locations
        // we get a Gaussian blur approximation.
        // The alpha value in _Color would normally be 0.25 (to average 4 samples),
        // however if we have glow amount larger than 1 then we increase this.

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

        // --------------------------------------------------------
        // The image downsample shaders for each brightness mode.
        // It is in external assets as it's quite complex and uses Cg.
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

        #endregion

        public GlowEvent(XftEventComponent owner)
            : base(CameraEffectEvent.EType.Glow, owner)
        {

            downsampleShader = owner.GlowDownSampleShader;
            compositeShader = owner.GlowCompositeShader;
            blurShader = owner.GlowBlurShader;
        }


        #region helper functions
        // Performs one blur iteration.
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

        // Downsamples the texture to a quarter resolution.
        void DownSample4x(RenderTexture source, RenderTexture dest)
        {
            downsampleMaterial.color = new Color(m_glowTint.r, m_glowTint.g, m_glowTint.b, m_glowTint.a / 4.0f);
            Graphics.Blit(source, dest, downsampleMaterial);
        }

        void BlitGlow(RenderTexture source, RenderTexture dest, float intensity)
        {
            compositeMaterial.color = new Color(1F, 1F, 1F, Mathf.Clamp01(intensity));
            Graphics.Blit(source, dest, compositeMaterial);
        }

        #endregion

        #region override functions
        public override bool CheckSupport()
        {
            bool ret = true;
            if (!SystemInfo.supportsImageEffects)
            {
                ret = false;
            }

            // Disable the effect if no downsample shader is setup
            if (downsampleShader == null)
            {
                Debug.Log("No downsample shader assigned! Disabling glow.");
                ret = false;
            }
            // Disable if any of the shaders can't run on the users graphics card
            else
            {
                if (!blurMaterial.shader.isSupported)
                    ret = false;
                if (!compositeMaterial.shader.isSupported)
                    ret = false;
                if (!downsampleMaterial.shader.isSupported)
                    ret = false;
            }

            return ret;
        }
        public override void OnRenderImage(RenderTexture source, RenderTexture destination)
        {

            int blurIterations = m_owner.GlowBlurIterations;
            float blurSpread = m_owner.GlowBlurSpread;
            float glowIntensity = m_owner.GlowIntensity;

            // Clamp parameters to sane values
            glowIntensity = Mathf.Clamp(glowIntensity, 0.0f, 10.0f);
            blurIterations = Mathf.Clamp(blurIterations, 0, 30);
            blurSpread = Mathf.Clamp(blurSpread, 0.5f, 1.0f);

            RenderTexture buffer = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0);
            RenderTexture buffer2 = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0);

            // Copy source to the 4x4 smaller texture.
            DownSample4x(source, buffer);

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
        public override void Update(float deltaTime)
        {
            m_elapsedTime += deltaTime;

            float t = m_owner.ColorCurve.Evaluate(m_elapsedTime);
            m_glowTint = Color.Lerp(m_owner.GlowColorStart, m_owner.GlowColorEnd, t);
        }
        #endregion
    }
}

