//----------------------------------------------
//            Xffect Editor
// Copyright © 2012- Shallway Studio
// http://shallway.net
//----------------------------------------------

using UnityEngine;
using System.Collections;

namespace Xft
{

    public enum AFFECTORTYPE
    {
        RotateAffector,
        UVAffector,
        UVRotAffector,
        ScaleAffector,
        ColorAffector,
        GravityAffector,
        BombAffector,
        AirFieldAffector,
        JetAffector,
        VortexAffector,
        TurbulenceAffector,
        DragAffector,
        SineAffector,
        UVScaleAffector,
    }

    public class Affector
    {
        public EffectNode Node;
        public AFFECTORTYPE Type;
        public Affector(EffectNode node, AFFECTORTYPE type)
        {
            Node = node;
            Type = type;
        }


        public virtual void Update(float deltaTime)
        {

        }

        public virtual void Reset()
        {
        }
    }
    public class RotateAffector : Affector
    {
        protected RSTYPE RType;
        protected float Delta = 0f;
  
        protected bool IsFirst = true;
        
        public RotateAffector(RSTYPE type, EffectNode node)
            : base(node,AFFECTORTYPE.RotateAffector)
        {
            RType = type;
        }

        public RotateAffector(float delta, EffectNode node)
            : base(node, AFFECTORTYPE.RotateAffector)
        {
            RType = RSTYPE.SIMPLE;
            Delta = delta;
        }
  
        
        public override void Reset ()
        {
            IsFirst = true;
            Node.RotateAngle = 0f;
        }

        public override void Update(float deltaTime)
        {
            if (IsFirst)
            {
                if (RType == RSTYPE.RANDOM)
                    Delta = Random.Range(Node.Owner.RotateSpeedMin,Node.Owner.RotateSpeedMax);
                else
                    Delta = Node.Owner.DeltaRot;
                IsFirst = false;
            }
            
            
            float time = Node.GetElapsedTime();
            if (RType == RSTYPE.CURVE)
                Node.RotateAngle = (int)Node.Owner.RotateCurve.Evaluate(time);
            else if (RType == RSTYPE.SIMPLE)
            {
                float angle = Node.RotateAngle + Delta * deltaTime;
                Node.RotateAngle = angle;
            }
            else if (RType == RSTYPE.RANDOM)
            {
                Node.RotateAngle = Node.RotateAngle + Delta * deltaTime;
            }
            else
            {
                float tlen = Node.Owner.RotateCurveTime;
                if (tlen < 0f)
                {
                    tlen = Node.GetLifeTime();
                }
                
                float t = time / tlen;
                
                if (t > 1f)
                {
                    if (Node.Owner.RotateCurveWrap == WRAP_TYPE.CLAMP)
                    {
                        t = 1f;
                    }  
                    else if (Node.Owner.RotateCurveWrap == WRAP_TYPE.LOOP)
                    {
                        int d = Mathf.FloorToInt(t);
                        t = t - (float)d;
                    }
                    else
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
                }
                Node.RotateAngle = (int)(Node.Owner.RotateCurve01.Evaluate(t) * Node.Owner.RotateCurveMaxValue);
            }
        }
    }

    //注意，如果贴图不是整张，那么uv scroll还是在整张范围内scroll的
    public class UVRotAffector : Affector
    {
        protected float RotXSpeed;
        protected float RotYSpeed;
        protected bool FirstUpdate = true;
        public UVRotAffector(float rotXSpeed, float rotYSpeed, EffectNode node)
            : base(node, AFFECTORTYPE.UVRotAffector)
        {
            RotXSpeed = rotXSpeed;
            RotYSpeed = rotYSpeed;
        }

        public override void Reset()
        {
            FirstUpdate = true;
        }

        public override void Update(float deltaTime)
        {
            if (FirstUpdate)
            {
                if (Node.Owner.RandomStartFrame)
                {
                    Node.LowerLeftUV.x += Random.Range(-1f, 1f);
                    Node.LowerLeftUV.y += Random.Range(-1f, 1f);
                }
                else
                {
                    Node.LowerLeftUV = Node.Owner.UVRotStartOffset;
                }
                FirstUpdate = false;
            }
            Vector2 nodeuv = Node.LowerLeftUV;
            nodeuv.x = nodeuv.x + RotXSpeed * deltaTime;
            nodeuv.y = nodeuv.y + RotYSpeed * deltaTime;
            Node.LowerLeftUV = nodeuv;
        }
    }

    public class UVScaleAffector : Affector
    {

        public UVScaleAffector(EffectNode node)
            : base(node, AFFECTORTYPE.UVScaleAffector)
        {
        }

        public override void Update(float deltaTime)
        {

            Vector2 dimensions = Node.UVDimensions;
            dimensions.x = dimensions.x + Node.Owner.UVScaleXSpeed * deltaTime;
            dimensions.y = dimensions.y + Node.Owner.UVScaleYSpeed * deltaTime;
            Node.UVDimensions = dimensions;
        }
    }

    public class UVAffector : Affector
    {
        protected UVAnimation Frames;
        protected float ElapsedTime;
        protected float UVTime;
        protected bool RandomStartFrame = false;
        protected bool FirstUpdate = true;

        public UVAffector(UVAnimation frame, float time, EffectNode node, bool randomStart)
            : base(node, AFFECTORTYPE.UVAffector)
        {
            Frames = frame;
            UVTime = time;
            RandomStartFrame = randomStart;
            if (RandomStartFrame)
            {
                Frames.curFrame = Random.Range(0, Frames.frames.Length - 1);
            }
        }

        public override void Reset()
        {
            ElapsedTime = 0;
            FirstUpdate = true;
            Frames.curFrame = 0;
            Frames.numLoops = 0;
            if (RandomStartFrame)
            {
                Frames.curFrame = Random.Range(0, Frames.frames.Length - 1);
            }
        }
        public override void Update(float deltaTime)
        {
            ElapsedTime += deltaTime;
            float framerate;
            if (UVTime <= 0f)
            {
                framerate = Node.GetLifeTime() / Frames.frames.Length;
            }
            else
            {
                framerate = UVTime / Frames.frames.Length;
            }
            if (ElapsedTime >= framerate || FirstUpdate)
            {
                Vector2 uv = Vector2.zero;
                Vector2 dm = Vector2.zero;
                Frames.GetNextFrame(ref uv, ref dm);

				
                //if (Node.Owner.RenderType == 2 || Node.Owner.RenderType == 3)
                //{
                //    uv.y = 1f - uv.y;
                //    dm.y = -dm.y;
                //}
				
				Node.LowerLeftUV = uv;
                Node.UVDimensions = dm;
				
                ElapsedTime -= framerate;
            }

            FirstUpdate = false;
        }
    }

    public class ScaleAffector : Affector
    {
        protected RSTYPE SType;
        protected float DeltaX = 0f;
        protected float DeltaY = 0f;
  
        protected bool IsFirst = true;
        
        public ScaleAffector(RSTYPE type, EffectNode node)
            : base(node, AFFECTORTYPE.ScaleAffector)
        {
            SType = type;
        }

        public ScaleAffector(float x, float y, EffectNode node)
            : base(node, AFFECTORTYPE.ScaleAffector)
        {
            SType = RSTYPE.SIMPLE;
            DeltaX = x;
            DeltaY = y;
        }
  
        public override void Reset ()
        {
            IsFirst = true;
            Node.Scale = Vector3.one;
        }

        
        public override void Update(float deltaTime)
        {
            if (IsFirst)
            {
                if (SType == RSTYPE.RANDOM){
                    DeltaX = Random.Range(Node.Owner.DeltaScaleX,Node.Owner.DeltaScaleXMax);
                    DeltaY = Random.Range(Node.Owner.DeltaScaleY,Node.Owner.DeltaScaleYMax);
                }
                else
                {
                    DeltaX = Node.Owner.DeltaScaleX;
                    DeltaY = Node.Owner.DeltaScaleY;
                }
                IsFirst = false;
            }
            
            
            float time = Node.GetElapsedTime();
            if (SType == RSTYPE.CURVE)
            {
                //deprecated.
                if (Node.Owner.UseSameScaleCurve)
                {
                    float curs = Node.Owner.ScaleXCurve.Evaluate(time);
                    Node.Scale.x = curs;
                    Node.Scale.y = curs;
                }
                else
                {
                    Node.Scale.x = Node.Owner.ScaleXCurve.Evaluate(time);
                    Node.Scale.y = Node.Owner.ScaleYCurve.Evaluate(time);
                }
            }
            else if (SType == RSTYPE.RANDOM)
            {
                float scalex = Node.Scale.x + DeltaX * deltaTime;
                float scaley = Node.Scale.y + DeltaY * deltaTime;
                    if (scalex > 0)
                        Node.Scale.x = scalex;
                    if (scaley > 0)
                        Node.Scale.y = scaley;
            }
            else if (SType == RSTYPE.CURVE01)
            {
                float tlen = Node.Owner.ScaleCurveTime;
                if (tlen < 0f)
                {
                    tlen = Node.GetLifeTime();
                }
                
                float t = time / tlen;
                
                if (t > 1f)
                {
                    if (Node.Owner.ScaleWrapMode == WRAP_TYPE.CLAMP)
                    {
                        t = 1f;
                    }  
                    else if (Node.Owner.ScaleWrapMode == WRAP_TYPE.LOOP)
                    {
                        int d = Mathf.FloorToInt(t);
                        t = t - (float)d;
                    }
                    else
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
                }
                if (Node.Owner.UseSameScaleCurve)
                {
                    float curs = Node.Owner.ScaleXCurveNew.Evaluate(t);
                    curs *= Node.Owner.MaxScaleCalue;
                    Node.Scale.x = curs;
                    Node.Scale.y = curs;
                }
                else
                {
                    Node.Scale.x = Node.Owner.ScaleXCurveNew.Evaluate(t) * Node.Owner.MaxScaleCalue;
                    Node.Scale.y = Node.Owner.ScaleYCurveNew.Evaluate(t) * Node.Owner.MaxScaleValueY;
                }
            }
            else if (SType == RSTYPE.SIMPLE)
            {
                float scalex = Node.Scale.x + DeltaX * deltaTime;
                float scaley = Node.Scale.y + DeltaY * deltaTime;
                if (scalex * Node.Scale.x > 0)
                    Node.Scale.x = scalex;
                if (scaley * Node.Scale.y > 0)
                    Node.Scale.y = scaley;
            }
        }
    }



    public class JetAffector : Affector
    {
        protected float Mag;
        protected MAGTYPE MType;
        protected AnimationCurve MagCurve;
        
        public JetAffector(float mag, MAGTYPE type, AnimationCurve curve,EffectNode node)
            : base(node, AFFECTORTYPE.JetAffector)
        {
            Mag = mag;
            MType = type;
            MagCurve = curve;
        }

        public override void Update(float deltaTime)
        {
            Vector3 delta = Vector3.zero;
            if (MType == MAGTYPE.Fixed)
                delta = Node.Velocity.normalized * Mag * deltaTime;
            else if (MType == MAGTYPE.Curve_OBSOLETE)
                delta = Node.Velocity.normalized * MagCurve.Evaluate(Node.GetElapsedTime());
            else
                delta = Node.Velocity.normalized * Node.Owner.JetCurveX.Evaluate(Node.GetElapsedTime());
            
            Vector3 goalV = Node.Velocity + delta;
            if (Vector3.Dot(goalV,Node.Velocity) <= 0)
            {
                //don't change the dir
                Node.Velocity = Vector3.zero;
                return;
            }
            
            Node.Velocity = goalV;
        }
    }
}