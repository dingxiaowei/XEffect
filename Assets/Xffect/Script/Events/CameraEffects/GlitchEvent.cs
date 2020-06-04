using UnityEngine;
using System.Collections;

namespace Xft
{
    //modified from here:http://va.lent.in/blog/2010/01/25/how-i-made-the-tv-glitch-effect-for-va-lent-in/
    public class WaveRandom
    {
        protected int seed = 0;
        protected float[] dSeeds = new float[3];
        protected float[] seeds = new float[3];
        protected Vector3 disp = Vector3.zero;
        
        public void Reset ()
        {
            seeds [0] = Random.Range (1f, 2f);
            seeds [1] = Random.Range (1f, 2f);
            seeds [2] = Random.Range (1f, 2f);
            seed = 0;
        }       
        
        
        //must call Reset Before!
        public Vector3 GetRandom(float minAmp, float maxAmp, float minRand, float maxRand, int len)
        {
            
            float difAmp = maxAmp - minAmp;
            seed++;
            if (seed >= len) {
                seed = 0;
            }
     
            float v = Mathf.PI / len * seed;
            float sin = 1.27323954f * v - 0.405284735f * v * v;
            float amp = minAmp + sin * difAmp;
     
            float pi2 = 6.28318531f;
     
            
            for (int i = 0; i < 3; i++) {
                if (seeds [i] >= 1f) {
                    seeds [i] = seeds [i] - 1f;
                    dSeeds [i] = Random.Range (minRand, maxRand);
                }
                
                seeds [i] += dSeeds [i];
                v = seeds [i] * pi2;
                
                if (v > Mathf.PI)
                    v -= pi2;
                if (v < 0f)
                    sin = 1.27323954f * v + 0.405284735f * v * v;
                else
                    sin = 1.27323954f * v - 0.405284735f * v * v;
                disp [i] = sin * amp;
            }
            
            return disp;
        }
    }
    
    
    
    public class GlitchEvent : CameraEffectEvent
    {
        protected Vector3 m_offset;
        protected WaveRandom m_random;
        protected Material m_material;


        public Shader GlitchShader;
        public Texture2D Mask;


        public Material GlitchMaterial
        {
            get
            {
                if (m_material == null)
                {
                    m_material = new Material(GlitchShader);
                    m_material.hideFlags = HideFlags.HideAndDontSave;
                }
                return m_material;
            }
        }
     
        public GlitchEvent (XftEventComponent owner)
            : base(CameraEffectEvent.EType.Glitch, owner)
        {
            m_random = new WaveRandom();
            Mask = owner.GlitchMask;
            GlitchShader = owner.GlitchShader;
        }


        public override bool CheckSupport()
        {
            bool ret = true;
            if (!SystemInfo.supportsImageEffects)
                ret = false;

            if (!GlitchMaterial.shader.isSupported)
                ret = false;

            return ret;
        }
        public override void Initialize ()
        {
            base.Initialize();
            m_random.Reset();
        }
        public override void Reset ()
        {
            base.Reset();
            m_random.Reset();
        }
        public override void Update (float deltaTime)
        {
            m_elapsedTime += deltaTime;
            m_offset = m_random.GetRandom(m_owner.MinAmp, m_owner.MaxAmp, m_owner.MinRand, m_owner.MaxRand, m_owner.WaveLen);
        }

        public override void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (Mask == null)
                return;

            GlitchMaterial.SetVector("_displace", m_offset);
            GlitchMaterial.SetTexture("_Mask", Mask);
            Graphics.Blit(source, destination, GlitchMaterial);
        }
    }
}

