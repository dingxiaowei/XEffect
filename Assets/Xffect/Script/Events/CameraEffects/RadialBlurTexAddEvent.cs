using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Xft
{
    public class RadialBlurTexAddEvent : CameraEffectEvent
    {

        protected float m_strength;

        protected Material m_material;
        public Shader RadialBlurShader;
        public Texture2D Mask;

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


        public RadialBlurTexAddEvent (XftEventComponent owner)
            : base(CameraEffectEvent.EType.RadialBlurMask, owner)
        {
            RadialBlurShader = owner.RadialBlurTexAddShader;
            Mask = owner.RadialBlurMask;
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


        public override void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (m_material == null)
            {
                return;
            }
            MyMaterial.SetTexture("_Mask", Mask);
            MyMaterial.SetFloat("_SampleDist", m_owner.RBMaskSampleDist);
            MyMaterial.SetFloat("_SampleStrength", m_strength);
            Graphics.Blit(source, destination, m_material);
        }

        public override void Update (float deltaTime)
        {
            m_elapsedTime += deltaTime;
			
			if (m_owner.RBMaskStrengthType == MAGTYPE.Fixed)
                m_strength = m_owner.RBMaskSampleStrength;
            else if (m_owner.RBMaskStrengthType == MAGTYPE.Curve_OBSOLETE)
                m_strength = m_owner.RBMaskSampleStrengthCurve.Evaluate(m_elapsedTime);
            else
                m_strength = m_owner.RBMaskSampleStrengthCurveX.Evaluate(m_elapsedTime);
		}
    }

}