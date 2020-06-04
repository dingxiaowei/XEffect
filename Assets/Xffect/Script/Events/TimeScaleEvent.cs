using UnityEngine;
using System.Collections;

namespace Xft
{
    public class TimeScaleEvent : XftEvent
    {
        protected bool mIsFirst = true;
        protected float mOriScale = 1f;
        
        public TimeScaleEvent (XftEventComponent owner)
         : base(XEventType.TimeScale, owner)
        {
        }

			
        public override void Reset ()
        {
			base.Reset();
            if (mIsFirst)
                mOriScale = Time.timeScale;
            Time.timeScale = mOriScale;
            mIsFirst = false;
        }
  
		
		public override void OnBegin ()
		{
			base.OnBegin ();
			Time.timeScale = m_owner.TimeScale;
		}

        public override void Update (float deltaTime)
        {
            m_elapsedTime += deltaTime;
			
			float elapsed = m_elapsedTime; /*- m_owner.StartTime * m_owner.TimeScale*/
			
			//Debug.LogWarning(elapsed);
			
			if (elapsed / m_owner.TimeScale > m_owner.TimeScaleDuration)
			{
				Time.timeScale = mOriScale;
			}

        }
    }
}

