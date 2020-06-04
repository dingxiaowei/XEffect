using UnityEngine;
using System.Collections;

namespace Xft
{
    public class ColorInverseEvent : CameraEffectEvent
    {
        protected float m_strength;

        public Shader ColorInverseShader;

        protected Material m_material;


        public Material MyMaterial
        {
            get
            {
                if (m_material == null)
                {
                    m_material = new Material(ColorInverseShader);
                    m_material.hideFlags = HideFlags.HideAndDontSave;
                }
                return m_material;
            }
        }

        public ColorInverseEvent (XftEventComponent owner)
            : base(CameraEffectEvent.EType.ColorInverse, owner)
        {
            ColorInverseShader = owner.ColorInverseShader;
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

        public override void Update (float deltaTime)
        {
            m_elapsedTime += deltaTime;
            m_strength = m_owner.CIStrengthCurve.Evaluate(m_elapsedTime);
        }


        public override void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            MyMaterial.SetFloat("_Strength", m_strength);
            Graphics.Blit(source, destination, MyMaterial);
        }

    }
}

