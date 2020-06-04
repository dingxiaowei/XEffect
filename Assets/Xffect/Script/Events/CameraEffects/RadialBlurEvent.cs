using UnityEngine;
using System.Collections;


namespace Xft
{
    public class RadialBlurEvent : CameraEffectEvent
    {

        protected Material m_material;
        public Shader RadialBlurShader;
        public Vector3 Center = new Vector3(0.5f, 0.5f, 0f);

        protected float m_strength;

        public Material MyMaterial
        {
            get
            {
                if (m_material == null)
                {
                    m_material = new Material(RadialBlurShader);
                    m_material.hideFlags = HideFlags.HideAndDontSave;
                }
                return m_material;
            }
        }
        public RadialBlurEvent(XftEventComponent owner)
            : base(CameraEffectEvent.EType.RadialBlur, owner)
        {
            RadialBlurShader = owner.RadialBlurShader;
        }

        public override bool CheckSupport()
        {
            bool ret = true;
            if (!SystemInfo.supportsImageEffects)
                ret = false;

            if (!MyMaterial.shader.isSupported)
                ret = false;

            return ret;
        }
        public override void Update(float deltaTime)
        {
            m_elapsedTime += deltaTime;

            Vector3 pos = MyCamera.WorldToScreenPoint(m_owner.RadialBlurObj.position);
            Center = pos;

            if (m_owner.RBStrengthType == MAGTYPE.Fixed)
                m_strength = m_owner.RBSampleStrength;
            else if (m_owner.RBStrengthType == MAGTYPE.Curve_OBSOLETE)
                m_strength = m_owner.RBSampleStrengthCurve.Evaluate(m_elapsedTime);
            else
                m_strength = m_owner.RBSampleStrengthCurveX.Evaluate(m_elapsedTime);
        }

        public override void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            MyMaterial.SetFloat("_SampleDist", m_owner.RBSampleDist);
            MyMaterial.SetFloat("_SampleStrength", m_strength);
            Vector4 center = Vector4.zero;
            center.x = Center.x / Screen.width;
            center.y = Center.y / Screen.height;
            MyMaterial.SetVector("_Center", center);
            Graphics.Blit(source, destination, m_material);
        }
    }
}

