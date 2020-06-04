//----------------------------------------------
//            Xffect Editor
// Copyright © 2012- Shallway Studio
// http://shallway.net
//----------------------------------------------
using UnityEngine;
using System.Collections;
using Xft;

namespace Xft
{
    public class SineAffector : Affector
    {
        public SineAffector(EffectNode node)
            : base(node, AFFECTORTYPE.SineAffector)
        {

        }

        public override void Update(float deltaTime)
        {
            float time = Node.Owner.SineTime;
            float mag = Node.Owner.SineMagnitude;

            if (Node.Owner.SineMagType != MAGTYPE.Fixed)
                mag = Node.Owner.SineMagCurveX.Evaluate(Node.GetElapsedTime());

            float t = Node.GetElapsedTime() / time * 2f * Mathf.PI;

            Vector3 delta = Mathf.Sin(t) * Node.Owner.SineForce * mag ;

            if (Node.Owner.SineIsAccelarate)
                Node.Velocity += delta;
            else
                Node.Position += delta;
        }
    }
}

