//----------------------------------------------
//            Xffect Editor
// Copyright © 2012- Shallway Studio
// http://shallway.net
//----------------------------------------------

using UnityEngine;
using System.Collections;

namespace Xft
{
    public enum BOMBTYPE
    {
        Planar,
        Spherical,
        Cylindrical
    }

    public enum BOMBDECAYTYPE
    {
        None,
        Linear,
        Exponential
    }

    public class BombAffector : Affector
    {
        protected BOMBTYPE BombType;

        protected BOMBDECAYTYPE DecayType;
        protected float Magnitude;

        protected float Decay;
        protected Vector3 BombAxis;
        protected Transform BombObj;

        protected float ElapsedTime = 0f;

        public BombAffector(Transform obj, BOMBTYPE gtype,BOMBDECAYTYPE dtype, float mag,
            float decay, Vector3 axis, EffectNode node)
            : base(node, AFFECTORTYPE.BombAffector)
        {
            BombType = gtype;

            DecayType = dtype;
            Magnitude = mag;

            Decay = decay;
            BombAxis = axis;
            BombAxis.Normalize();
            BombObj = obj;
        }


        public override void Reset()
        {
            ElapsedTime = 0f;
        }

        public override void Update(float deltaTime)
        {
            
            //fixed at 60fps?
            deltaTime = 0.01666f;

            float strength = Magnitude;

            Vector3 bombDir = BombObj.rotation * BombAxis;
            Vector3 dir;
            dir = Node.GetOriginalPos() - BombObj.position;
            float dist = dir.magnitude;
            Vector3 force = Vector3.zero;

            if (dir == Vector3.zero)
            {
               // Debug.LogWarning("you need to set the bomb obj's pos differ to the emit pos!");
            }

            if (DecayType == BOMBDECAYTYPE.None || dist <= Decay)
            {
                switch (BombType)
                {
                    case BOMBTYPE.Spherical:
                        force = dir / dist;
                        break;
                    case BOMBTYPE.Planar:
                        dist = Vector3.Dot(bombDir, dir);
                        if (dist < 0.0f)
                        {
                            dist = -dist;
                            force = -bombDir;
                        }
                        else
                        {
                            force = bombDir;
                        }
                        break;
                    case BOMBTYPE.Cylindrical:
                        // Subtract off the vector component along the
                        // cylinder axis
                        dist = Vector3.Dot(bombDir, dir);

                        force = dir - dist * bombDir;
                        dist = force.magnitude;
                        if (dist != 0.0f)
                        {
                            force /= dist;
                        }
                        break;
                    default:
                        Debug.LogError("wrong bomb type!");
                        break;
                }

                float decay = 1.0f;
                if (DecayType == BOMBDECAYTYPE.Linear)
                {
                    decay = (Decay - dist) / Decay;
                }
                else if (DecayType == BOMBDECAYTYPE.Exponential)
                {
                    decay = Mathf.Exp(-dist / Decay);
                }

                ElapsedTime += deltaTime;

                strength = strength / (ElapsedTime * ElapsedTime);
                
                Node.Velocity += decay * strength * deltaTime * force;
            }
        }
    }
}
