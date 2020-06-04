using UnityEngine;
using System.Collections;

namespace Xft
{
    public class RenderObject
    {
        public EffectNode Node;

        protected float Fps
        {
            get 
            {
                float fps = 1f / Node.Owner.Owner.MaxFps;
                if (!Node.Owner.Owner.IgnoreTimeScale)
                    fps *= Time.timeScale;
                return fps;
            }
        }

        public virtual void Initialize(EffectNode node)
        {
            Node = node;
        }

        public virtual void Reset()
        {

        }

        public virtual void Update(float deltaTime)
        {

        }

        public virtual void ApplyShaderParam(float x, float y)
        {

        }

    }
}

