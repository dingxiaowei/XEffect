using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Xft
{
	public class CameraEffectEvent : XftEvent, System.IComparable<CameraEffectEvent>
	{
		public enum EType
		{
			RadialBlur,
			RadialBlurMask,
			Glow,
			GlowPerObj,
			ColorInverse,
            Glitch,
		}
		protected EType m_effectType;

        protected XftCameraEffectComp m_comp;

        protected bool m_supported = true;


        public int CompareTo(CameraEffectEvent otherObj)
        {
            return m_owner.CameraEffectPriority.CompareTo(otherObj.m_owner.CameraEffectPriority);
        }

        public XftCameraEffectComp CameraComp
        {
            get
            {
                m_comp = MyCamera.gameObject.GetComponent<XftCameraEffectComp>();

                if (m_comp == null)
                {
                    m_comp = MyCamera.gameObject.AddComponent<XftCameraEffectComp>();
                }

                return m_comp;
            }
        }

        public EType EffectType
        {
            get { return m_effectType; }
        }

		public CameraEffectEvent(EType etype, XftEventComponent owner)
            : base(XEventType.CameraEffect, owner)
        {
			m_effectType = etype;
        }


        public override void Initialize()
        {
            if (!CheckSupport())
            {
                Debug.LogWarning("camera effect is not supported on this device:" + m_effectType);
                m_supported = false;
                return;
            }
        }


        public override void OnBegin()
        {
            if (!m_supported)
                return;
            base.OnBegin();
            CameraComp.AddEvent(this);
        }


        public override void Reset()
        {
            base.Reset();
            CameraComp.ResetEvent(this);
        }

		public virtual void ToggleCameraComponent(bool flag)
		{
			
		}


        public virtual bool CheckSupport()
        {
            return true;
        }

        public virtual void OnPreRender()
        {
        }

        public virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
        {

        }
		
	}
}

