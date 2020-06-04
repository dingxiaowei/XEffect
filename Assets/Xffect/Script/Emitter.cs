﻿//----------------------------------------------
//            Xffect Editor
// Copyright © 2012- Shallway Studio
// http://shallway.net
//----------------------------------------------

using UnityEngine;
using System.Collections;
using Xft;

namespace Xft
{
 
    public enum EEmitWay
    {
        ByRate,
        ByDistance,
        ByCurve
    }
    
    public class Emitter
    {
        public EffectLayer Layer;

        public float EmitterElapsedTime = 0f;
        protected float mElapsedTime = 0f;
        //float EmitDelayTime = 0f;
        bool IsFirstEmit = true;
        public float EmitLoop = 0f;
        public float EmitDist = 1f;
        public bool CurveEmitDone = false;
        Vector3 LastClientPos = Vector3.zero;

        //added 2012 5.15
        protected int m_emitCount = 0;
        
        protected float m_curveCountTime = 0f;

        public Emitter(EffectLayer owner)
        {
            Layer = owner;
            EmitLoop = Layer.EmitLoop;
            EmitDist = Layer.DiffDistance;
            LastClientPos = Layer.ClientTransform.position;
        }
        public void Reset()
        {
            EmitterElapsedTime = 0f;
            //EmitDelayTime = 0f;
            IsFirstEmit = true;
            EmitLoop = Layer.EmitLoop;
            EmitDist = Layer.DiffDistance;
            m_emitCount = 0;
            CurveEmitDone = false;
            m_curveCountTime = 0f;
        }
  
        
        public void StopEmit()
        {
            EmitLoop = 0;
            EmitterElapsedTime = 999999f;
            EmitDist = 9999999f;
            mElapsedTime = 0f;
        }
        
        
        protected int EmitByCurve(float deltaTime)
        {
            XCurveParam curve = Layer.EmitterCurveX;
            if (curve == null)
            {
                Debug.LogWarning("emitter hasn't set a curve yet!");
                return 0;
            }
            
            EmitterElapsedTime += deltaTime;
            
            int numToEmit = (int)curve.Evaluate(EmitterElapsedTime) - m_emitCount;;

            int needToEmit = 0;
            if (numToEmit > Layer.AvailableNodeCount)
                needToEmit = Layer.AvailableNodeCount;
            else
                needToEmit = numToEmit;
            if (needToEmit < 0)
                needToEmit = 0;
            
            m_emitCount += needToEmit;
            
            //if there haven't any node be emitted during 1 second, then we assume it's over.
            if (needToEmit == 0)
            {
                m_curveCountTime += deltaTime;
                if (m_curveCountTime > 1f)
                    CurveEmitDone = true;
            }
            else
                m_curveCountTime = 0f;
            
            if (CurveEmitDone)
                return 0;
            
            return needToEmit;
        }
        
        protected int EmitByDistance()
        {
            
            if (Layer.mStopped)
                return 0;
            
            Vector3 diff = Layer.ClientTransform.position - LastClientPos;
            if (diff.magnitude >= EmitDist)
            {
                LastClientPos = Layer.ClientTransform.position;
                return 1;
            }
            else
            {
                return 0;
            }
        }
  
        
        protected int EmitByBurst(float deltaTime)
        {
            if (!IsFirstEmit)
            {
                EmitLoop = 0;
                return 0;
            }
            
            IsFirstEmit = false;
            
            int numToEmit = (int)Layer.EmitRate;
            
            int needToEmit = 0;
            if (numToEmit > Layer.AvailableNodeCount)
                needToEmit = Layer.AvailableNodeCount;
            else
                needToEmit = numToEmit;
            if (needToEmit <= 0)
                return 0;
            
            return needToEmit;
        }


        //must be continious
        //protected int RopeEmit(float deltaTime)
        //{

        //    if (Layer.IsBurstEmit)
        //        return EmitByBurst(deltaTime);

        //    EmitterElapsedTime += deltaTime;

        //    //Debug.LogWarning(EmitterElapsedTime + ":" + 1f / Layer.EmitRate);

        //    if (EmitterElapsedTime < 1f / Layer.EmitRate || Layer.AvailableNodeCount == 0)
        //    {
        //        return 0;
        //    }

        //    EmitterElapsedTime = 0f;
            

        //    return 1;
        //}

        
        protected int EmitByRate(float deltaTime)
        {
   
            //deprecated.
            //check chance to this emit;
            //int random = Random.Range(0, 100);
            //if (random >= 0 && random > Layer.ChanceToEmit)
            //{
                //return 0;
            //}
   
            
            if (Layer.IsBurstEmit)
                return EmitByBurst(deltaTime);
            EmitterElapsedTime += deltaTime;

            //check delay
            //EmitDelayTime += deltaTime;
            //if (EmitDelayTime < Layer.EmitDelay && !IsFirstEmit)
            //{
                //return 0;
            //}
            
            //time expired
            if (Layer.EmitDuration > 0 && EmitterElapsedTime >= Layer.EmitDuration)
            {
                if (EmitLoop > 0)
                    EmitLoop--;
                m_emitCount = 0;
                EmitterElapsedTime = 0;
                //EmitDelayTime = 0;
                IsFirstEmit = false;
                mElapsedTime = 0f;
            }
            if (EmitLoop == 0)
            {
                return 0;
            }
            //no available nodes
            if (Layer.AvailableNodeCount == 0)
            {
                return 0;
            }


            mElapsedTime += deltaTime;

            int numToEmit = (int)(mElapsedTime * Layer.EmitRate);

            //obsolete the below emit method, not good.
            //decides how many nodes to emit
            //int numToEmit = (int)(EmitterElapsedTime * Layer.EmitRate) - m_emitCount;

            //ugly hack, some times the emition is not continious in low rate, don't know why.
            //if (Layer.EmitRate <= 60 && numToEmit > 1)
            //{
            //    //Debug.LogWarning(numToEmit);
            //    //fix to emit only one node at one frame.
            //    numToEmit = 1;
            //    EmitterElapsedTime = (m_emitCount + 1) * Layer.EmitRate;
            //}

            //Debug.LogWarning(numToEmit);

            int needToEmit = 0;
            if (numToEmit > Layer.AvailableNodeCount)
                needToEmit = Layer.AvailableNodeCount;
            else
                needToEmit = numToEmit;
            if (needToEmit <= 0)
                return 0;

            //mElapsedTime -= needToEmit / Layer.EmitRate;
            mElapsedTime = 0;
            

            m_emitCount += needToEmit;
            return needToEmit;
        }
  
        public Vector3 GetEmitRotation(EffectNode node)
        {
            Vector3 ret = Vector3.zero;
            //Set Direction:
            if (Layer.DirType == DIRECTION_TYPE.Sphere)
            {
                ret = node.GetOriginalPos() - Layer.ClientTransform.position;
				if (ret == Vector3.zero)
				{
					//use random rotation.
					Vector3 r = Vector3.up;
                	Quaternion rot = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                	ret = rot * r;
				}
            }
            else if (Layer.DirType == DIRECTION_TYPE.Planar)
            {
                ret = Layer.OriVelocityAxis; 
            }
            else if (Layer.DirType == DIRECTION_TYPE.Cone)
            {
                //if emit uniform circle, the rotation should be spread from the center.
                if (Layer.EmitType == (int)EMITTYPE.CIRCLE && Layer.EmitUniform)
                {
                    Vector3 dir;
                    if (!Layer.SyncClient)
                        dir = node.Position - (node.GetRealClientPos() + Layer.EmitPoint);
                    else
                        dir = node.Position - Layer.EmitPoint;
                    
                    int coneAngle = Layer.AngleAroundAxis;
                    if (Layer.UseRandomDirAngle)
                    {
                        coneAngle = Random.Range(Layer.AngleAroundAxis,Layer.AngleAroundAxisMax);
                    }
                    
                    Vector3 target = Vector3.RotateTowards(dir, Layer.CircleDir, (90 - coneAngle) * Mathf.Deg2Rad, 1);
                    Quaternion rot = Quaternion.FromToRotation(dir, target);
                    ret = rot * dir;
                }
                else
                {
                    //first, rotate y around z 30 degrees
                    Quaternion rotY = Quaternion.Euler(0, 0, Layer.AngleAroundAxis);
                    //second, rotate around y 360 random dir;
                    Quaternion rotAround = Quaternion.Euler(0, Random.Range(0, 360), 0);
                    //last, rotate the dir to OriVelocityAxis
                    Quaternion rotTo = Quaternion.FromToRotation(Vector3.up, Layer.OriVelocityAxis);
                    ret = rotTo * rotAround * rotY * Vector3.up;
                }
            }
            else if (Layer.DirType == DIRECTION_TYPE.Cylindrical)
            {
                Vector3 dir = node.GetOriginalPos() - Layer.ClientTransform.position;
                

                if (dir == Vector3.zero)
                {
                 //use random rotation.
                 Vector3 r = Vector3.up;
                 Quaternion rot = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                 dir = rot * r;
                }
                
                float dist = Vector3.Dot(Layer.OriVelocityAxis, dir);
                ret = dir - dist * Layer.OriVelocityAxis.normalized;
                
                
                
            }
            return  ret;
        }
        public void SetEmitPosition(EffectNode node)
        {
            Vector3 retPos = Vector3.zero;
            Vector3 clientPos = node.GetRealClientPos();

            
            if (Layer.EmitType == (int)EMITTYPE.BOX)
            {
                Vector3 center = Layer.EmitPoint;
                float x = Random.Range(center.x - Layer.BoxSize.x / 2, center.x + Layer.BoxSize.x / 2);
                float y = Random.Range(center.y - Layer.BoxSize.y / 2, center.y + Layer.BoxSize.y / 2);
                float z = Random.Range(center.z - Layer.BoxSize.z / 2, center.z + Layer.BoxSize.z / 2);
                retPos.x = x; retPos.y = y; retPos.z = z;
                
                if (!Layer.SyncClient)
                {
                    //if (!Layer.BoxInheritRotation)
                       // retPos = clientPos + retPos;
                    //else
                        retPos = Layer.ClientTransform.rotation * retPos  + clientPos;
                }
                else
                {
                    //if (Layer.BoxInheritRotation)
                        retPos = Layer.ClientTransform.rotation * retPos;
                }
            }
            else if (Layer.EmitType == (int)EMITTYPE.POINT)
            {
                retPos = Layer.EmitPoint;
                if (!Layer.SyncClient)
                {
                    retPos = clientPos + Layer.EmitPoint;
                }
            }
            else if (Layer.EmitType == (int)EMITTYPE.SPHERE)
            {
                retPos = Layer.EmitPoint;
                if (!Layer.SyncClient)
                {//同步的话在NodeUpdate里会更新位置
                    retPos = clientPos + Layer.EmitPoint;
                }
                Vector3 r = Vector3.up * Layer.Radius;
                Quaternion rot = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                retPos = rot * r + retPos;
            }
            //Line direction is based on client transform's rotation.
            else if (Layer.EmitType == (int)EMITTYPE.LINE)
            {
                Vector3 left = Layer.LineStartObj.position;
                Vector3 right = Layer.LineEndObj.position;
                Vector3 dir = right - left;

                float length;
                if (Layer.EmitUniform)
                {
                    float p = (float)(node.Index + 1) / Layer.MaxENodes;
                    length = dir.magnitude * p;
                }
                else
                {
                    length = Random.Range(0, dir.magnitude);
                }

                retPos = left + dir.normalized * length - clientPos;
                if (!Layer.SyncClient)
                    retPos = clientPos + retPos;
            }
            else if (Layer.EmitType == (int)EMITTYPE.CIRCLE)
            {
                float rangle;
                if (Layer.EmitUniform)
                {

                    int index = node.Index;
                    float p = (float)(index + 1) / Layer.MaxENodes;
                    rangle = 360 * p;
                }
                else
                {
                    rangle = Random.Range(0, 360);
                }
                
                float radius = Layer.Radius;
                if (Layer.UseRandomCircle)
                {
                    radius = Random.Range(Layer.CircleRadiusMin,Layer.CircleRadiusMax);
                }
                
                Quaternion rotY = Quaternion.Euler(0, rangle, 0);
                Vector3 v = rotY * (Vector3.right * radius);
                Quaternion rotTo = Quaternion.FromToRotation(Vector3.up, Layer.ClientTransform.rotation * Layer.CircleDir);
                retPos = rotTo * v;
                if (!Layer.SyncClient)
                    retPos = clientPos + retPos + Layer.EmitPoint;
                else
                    retPos = retPos + Layer.EmitPoint;
            }
            else if (Layer.EmitType == (int)EMITTYPE.Mesh)
            {
                if (Layer.EmitMesh == null)
                {
                    Debug.LogWarning("please set a mesh to the emitter.");
                    return;
                }  
                int index = 0;
                if (Layer.EmitMeshType == 0)
                {
                    int vertCount = Layer.EmitMesh.vertexCount;
                    if (Layer.EmitUniform)
                        index = (node.Index) % (vertCount - 1);
                    else
                        index = Random.Range(0, vertCount - 1);
                    retPos = Layer.EmitMesh.vertices[index];
                    if (!Layer.SyncClient)
                        retPos = clientPos + retPos + Layer.EmitPoint;
                    else
                        retPos = retPos + Layer.EmitPoint;
                }
                else if (Layer.EmitMeshType == 1)
                {
                    Vector3[] verts = Layer.EmitMesh.vertices;
                    int triCount = Layer.EmitMesh.triangles.Length / 3;
                    if (Layer.EmitUniform)
                        index = (node.Index) % (triCount - 1);
                    else
                        index = Random.Range(0, triCount - 1);
                    int vid1 = Layer.EmitMesh.triangles[index * 3 + 0];
                    int vid2 = Layer.EmitMesh.triangles[index * 3 + 1];
                    int vid3 = Layer.EmitMesh.triangles[index * 3 + 2];
                    retPos = (verts[vid1] + verts[vid2] + verts[vid3]) / 3;
                    if (!Layer.SyncClient)
                        retPos = clientPos + retPos + Layer.EmitPoint;
                    else
                        retPos = retPos + Layer.EmitPoint;
                }
            }
            node.SetLocalPosition(retPos);
        }
        public int GetNodes(float deltaTime)
        {
            //for downward compatibility
            //if (Layer.IsEmitByDistance)

            if (Layer.EmitWay == EEmitWay.ByRate)
                return EmitByRate(deltaTime);
            else if (Layer.EmitWay == EEmitWay.ByCurve)
                return EmitByCurve(deltaTime);
            else
                return EmitByDistance();
        }
    }
}