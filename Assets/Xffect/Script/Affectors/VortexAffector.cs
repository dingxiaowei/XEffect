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
	public class VortexAffector : Affector
	{

		protected Vector3 Direction;
		protected Transform VortexObj;

		protected bool InheritRotation;

        
        protected bool IsFirst = true;
        protected float OriginalRadius = 0f;
        
        public override void Reset ()
        {
            IsFirst = true;
            OriginalRadius = 0f;

            if (Node.Owner.IsRandomVortexDir)
            {
                Direction.x = Random.Range(-1f, 1f);
                Direction.y = Random.Range(-1f, 1f);
                Direction.z = Random.Range(-1f, 1f);
            }
            Direction.Normalize();
        }

		public VortexAffector (Transform obj, Vector3 dir, bool inhRot, EffectNode node)
            : base(node, AFFECTORTYPE.VortexAffector)
		{

			Direction = dir;
			InheritRotation = inhRot;
			VortexObj = obj;



			//ver 1.2.1
			if (node.Owner.IsRandomVortexDir) {
				Direction.x = Random.Range (-1f, 1f);
				Direction.y = Random.Range (-1f, 1f);
				Direction.z = Random.Range (-1f, 1f);
			}
			Direction.Normalize ();
            IsFirst = true;
		}

		public override void Update (float deltaTime)
		{
			Vector3 diff;

			diff = Node.GetOriginalPos() - VortexObj.position;


            //if is random dir, we assume the track is around the sphere.
            if (Node.Owner.IsRandomVortexDir && IsFirst)
            {
                Direction = Vector3.Cross(diff, Direction);
            }

            Vector3 direction = Direction;
             if (InheritRotation)
                 direction = Node.Owner.ClientTransform.rotation * direction;
            
            if (IsFirst)
            {
                IsFirst = false;
                OriginalRadius = (diff - Vector3.Project(diff,direction)).magnitude;
            }

			float distanceSqr = diff.sqrMagnitude;

			if (distanceSqr < 1e-06f)
				return;
			
			if (!Node.Owner.UseVortexMaxDistance || (distanceSqr <= Node.Owner.VortexMaxDistance * Node.Owner.VortexMaxDistance)) 
			{

				float segParam = Vector3.Dot (direction, diff);
				diff -= segParam * direction;

				Vector3 deltaV = Vector3.zero;
				if (diff == Vector3.zero) {
					deltaV = diff;
				} else {
					deltaV = Vector3.Cross (direction, diff).normalized;
				}
				float time = Node.GetElapsedTime ();
				float magnitude;
                if (Node.Owner.VortexMagType == MAGTYPE.Curve_OBSOLETE)
                    magnitude = Node.Owner.VortexCurve.Evaluate(time);
                else if (Node.Owner.VortexMagType == MAGTYPE.Fixed)
                    magnitude = Node.Owner.VortexMag;
                else
                    magnitude = Node.Owner.VortexCurveX.Evaluate(time);
				
				if (Node.Owner.VortexAttenuation < 1e-04f)
				{
					deltaV *= magnitude * deltaTime;
				}
				else
				{
					deltaV *= magnitude * deltaTime / Mathf.Pow(Mathf.Sqrt(distanceSqr),Node.Owner.VortexAttenuation);
				}
				
				if (Node.Owner.IsVortexAccelerate)
                {
                    Node.Velocity += deltaV;
                }	
				else
                {
                    if (Node.Owner.IsFixedCircle)
                    {
                        Vector3 dist = Node.GetOriginalPos() + deltaV - VortexObj.position;
                        Vector3 proj = Vector3.Project(dist,direction);
                        Vector3 fixedDir = dist - proj;
                        if (Node.Owner.SyncClient)
                        {
                            Node.Position = fixedDir.normalized * OriginalRadius + proj;
                        } 
                        else
                        {
                            Node.Position = Node.GetRealClientPos() + fixedDir.normalized * OriginalRadius + proj;
                        }    
                    }
                    else
                    {
                        Node.Position += deltaV;
                    } 
                }
					
			}


		}
	}
}
