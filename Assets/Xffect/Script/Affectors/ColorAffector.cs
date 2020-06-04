using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Xft{
	[System.Serializable]
	public class ColorKey : System.IComparable
	{
		public float t;
		public Color Color;
		public ColorKey(float age, Color color)
		{
			t=age;
			Color=color;
		}
		public int CompareTo(object obj)
		{
			return -((ColorKey)obj).t.CompareTo(t);
		}
	}
	
	[System.Serializable]
	public class ColorParameter
	{
		public List<ColorKey> Colors;

		public ColorParameter()
		{
			Colors = new List<ColorKey>();
			AddColorKey(0f,Color.white);
		}
		
		public Color GetGradientColor(float t)
		{
			if (Colors.Count == 1)
			{
				//no gradient
				return Colors[0].Color;
			}
			
			if (Colors.Count == 0)
			{
				//Debug.LogWarning("color keys can't be empty!");
				return Color.black;
			}
			
			for (int k2=1;k2<Colors.Count;k2++) {
            if (t <= Colors[k2].t) {
					int k = k2 - 1;
					return Color.Lerp(Colors[k].Color, Colors[k2].Color, (t - Colors[k].t) / (Colors[k2].t - Colors[k].t));
				}
			}
			//clamp to the last key color.
			return Colors[Colors.Count - 1].Color;
		}
	
		public ColorKey AddColorKey(float t, Color color)
		{
			ColorKey k = new ColorKey(t, color);
			Colors.Add(k);
			Colors.Sort();
			return k;
		}
		
	}
	
	
    public class ColorAffector : Affector
    {
        protected float ElapsedTime = 0f;
        protected bool IsNodeLife = false;
		protected EffectLayer mOwner;
		
		//protected float mRandomKey = 0f;
		
        
        protected bool mFade = false;
        protected float mFadingTime = 0f;
        protected Color mFadeStartColor;
        
        public ColorAffector(EffectLayer owner, EffectNode node)
            : base(node, AFFECTORTYPE.ColorAffector)
        {
			mOwner = owner;
            if (owner.ColorGradualTimeLength < 0)
                IsNodeLife = true;
			
            //if (mOwner.ColorChangeType == COLOR_CHANGE_TYPE.Random)
            //{
            //    mRandomKey = Random.Range(0f,1f);
            //}
			
        }
  
        
        public void Fade(float time)
        {
            mFade = true;
            ElapsedTime = 0f;
            mFadeStartColor = Node.Color;
            mFadingTime = time;
        }
        
        
        void UpdateFading()
        {
            float t = ElapsedTime / mFadingTime;
            Node.Color = Color.Lerp(mFadeStartColor,Color.clear,t);
            if (t >= 1)
            {
                Node.Stop();
            }
        }
        
        public override void Reset()
        {
            
            mFade = false;
            
            ElapsedTime = 0;
			
			if (IsNodeLife && mOwner.IsNodeLifeLoop && !mOwner.mStopped)
			{
				Debug.LogWarning("invalid color gradual time, loop node can't be gradient by 'gradual time':" + mOwner.ColorGradualTimeLength + mOwner.Owner.name);
			}
			
			
            //if (mOwner.ColorChangeType == COLOR_CHANGE_TYPE.Random)
            //{
            //    mRandomKey = Random.Range(0f,1f);
            //}
        }

        public override void Update(float deltaTime)
        {
            ElapsedTime += deltaTime;
			
            
            if (mFade)
            {
                UpdateFading();
                return;
            }

			float gradualLen = mOwner.ColorGradualTimeLength;
			
            if (IsNodeLife)
            {
                gradualLen = Node.GetLifeTime();
            }
			
            if (gradualLen <= 0f)// warning.
                return;
			
			float t = 0f;
			
			if (mOwner.ColorGradualType == COLOR_GRADUAL_TYPE.CURVE)
			{
				t = mOwner.ColorGradualCurve.Evaluate(ElapsedTime);
			}
			else
			{
				t = ElapsedTime / gradualLen;
			}
			
            //if (mOwner.ColorChangeType == COLOR_CHANGE_TYPE.Random)
            //{
            //    t = mRandomKey;
            //}
			
			if (t > 1f)
			{
				if (mOwner.ColorGradualType == COLOR_GRADUAL_TYPE.CLAMP)
				{
					//Node.Color = Node.StartColor * mOwner.ColorParam.GetGradientColor(1f);
                    Node.Color = Node.StartColor * mOwner.ColorGradient.Evaluate(1f);
					return;
				}
				else if (mOwner.ColorGradualType == COLOR_GRADUAL_TYPE.LOOP)
				{
					ElapsedTime = 0f;
					return;
				}
				else if (mOwner.ColorGradualType == COLOR_GRADUAL_TYPE.REVERSE)
				{
					int n = Mathf.CeilToInt(t);
					int d = Mathf.FloorToInt(t);
					if (n % 2 == 0)
					{
						t = (float)n - t;
					}
					else
					{
						t = t - (float)d;
					}
					
				}
				else
				{
					// no need to change t.
				}
			}
            //Node.Color = Node.StartColor * mOwner.ColorParam.GetGradientColor(t);
            Node.Color = Node.StartColor * mOwner.ColorGradient.Evaluate(t);
        }
    }
}