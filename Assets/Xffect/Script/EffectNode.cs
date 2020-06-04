//----------------------------------------------
//            Xffect Editor
// Copyright © 2012- Shallway Studio
// http://shallway.net
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using Xft;
using System.Collections;
using System;

namespace Xft
{
 
    public class CollisionParam
    {
        protected GameObject m_collideObject;
        protected Vector3 m_collidePos;
        protected Vector3 m_direction;
        
        public GameObject CollideObject
        {
            get {return m_collideObject;}
            set {m_collideObject = value;}
        }
        
        public Vector3 CollidePos
        {
            get {return m_collidePos;}
            set {m_collidePos = value;}
        }

        public Vector3 CollideDir
        {
            get
            {
                return m_direction;
            }
        }

        public CollisionParam(GameObject obj, Vector3 pos, Vector3 dir)
        {
            m_collideObject = obj;
            m_collidePos = pos;
            m_direction = dir;
        }
    }

    public class EffectNode : IComparable<EffectNode>
    {

        public RenderObject RenderObj;


        //constructor
        protected int Type;  //1 sprite, 2 ribbon trail, 3 cone, 4, custom mesh.
        public int Index;
        public ulong TotalIndex;
        public Transform ClientTrans;
        public bool SyncClient;
        public EffectLayer Owner;

        //internal using
        protected Vector3 CurDirection;
        protected Vector3 LastWorldPos = Vector3.zero;
        public Vector3 CurWorldPos;
        protected float ElapsedTime;

        //affect by affector
        public Vector3 Position;
        public Vector2 LowerLeftUV;
        public Vector2 UVDimensions;
        public Vector3 Velocity;
        public Vector2 Scale;
        public float RotateAngle;
        public Color Color;
  
        
        public XffectComponent SubEmitter = null;


        public Camera MyCamera
        {
            get
            {
                if (Owner == null)
                {
                    Debug.LogError("something wrong with camera init!");
                    return null;
                }
                return Owner.MyCamera;
            }
        }

        //reset
        public List<Affector> AffectorList;
        public Vector3 OriDirection;
        public float LifeTime;
        public int OriRotateAngle;
        public float OriScaleX;
        public float OriScaleY;

        public bool SimpleSprite = false;


        public Color StartColor;

        //added 2012 6.3 for collision use

        protected bool IsCollisionEventSended = false;
        protected Vector3 LastCollisionDetectDir = Vector3.zero;
        
        public bool mIsFade = false;


        public int CompareTo(EffectNode other)
        {
            return this.TotalIndex.CompareTo(other.TotalIndex);
        }

        public EffectNode(int index, Transform clienttrans, bool sync, EffectLayer owner)
        {
            Index = index;
            ClientTrans = clienttrans;
            SyncClient = sync;
            Owner = owner;
            LowerLeftUV = Vector2.zero;
            UVDimensions = Vector2.one;
            Scale = Vector2.one;
            RotateAngle = 0;
            Color = Color.white;
        }

        public void SetAffectorList(List<Affector> afts)
        {
            AffectorList = afts;
        }

        public List<Affector> GetAffectorList()
        {
            return AffectorList;
        }

        public void Init(Vector3 oriDir, float speed, float life, int oriRot, float oriScaleX, float oriScaleY, Color oriColor, Vector2 oriLowerUv, Vector2 oriUVDimension)
        {
            //OriDirection = ClientTrans.TransformDirection(oriDir);
            OriDirection = oriDir;
            LifeTime = life;
            OriRotateAngle = oriRot;
            OriScaleX = oriScaleX;
            OriScaleY = oriScaleY;

            StartColor = oriColor;

            Color = oriColor;
            ElapsedTime = 0f;
            // direction is synced to client rotation, except sphere dir
            if (Owner.DirType != DIRECTION_TYPE.Sphere)
                Velocity = Owner.ClientTransform.rotation * OriDirection * speed;
            else
                Velocity = OriDirection * speed;
            LowerLeftUV = oriLowerUv;
            UVDimensions = oriUVDimension;
            IsCollisionEventSended = false;
            RenderObj.Initialize(this);

        }
        
        

        
        public float GetElapsedTime()
        {
            return ElapsedTime;
        }

        public float GetLifeTime()
        {
            return LifeTime;
        }
  
        
        
        public void SetLocalPosition(Vector3 pos)
        {
            //ribbon trail needs to reset the head.
            if (Type == 1)
            {
                RibbonTrail rt = RenderObj as RibbonTrail;
                if (!SyncClient)
                    rt.OriHeadPos = pos;
                else
                    rt.OriHeadPos = GetRealClientPos() + pos;
            }

            // collison may get through. use dir to detect.
            //if (Owner.UseCollisionDetection && Owner.CollisionType != COLLITION_TYPE.CollisionLayer)
                //LastCollisionDetectDir = Vector3.zero;

            Position = pos;
        }
        public Vector3 GetLocalPosition()
        {
            return Position;
        }
  
        public Vector3 GetRealClientPos()
        {
            Vector3 mscale = Vector3.one * Owner.Owner.Scale;
            Vector3 clientPos = Vector3.zero;
            clientPos.x = ClientTrans.position.x / mscale.x;
            clientPos.y = ClientTrans.position.y / mscale.y;
            clientPos.z = ClientTrans.position.z / mscale.z;
            return clientPos;
        }
        
        //back to original scale pos.
        public Vector3 GetOriginalPos()
        {
            Vector3 ret;
            Vector3 clientPos = GetRealClientPos();
            if (!SyncClient)
                ret = Position - clientPos + ClientTrans.position;
            else
                ret = Position + ClientTrans.position;
            return ret;
        }

        public Vector3 GetWorldPos()
        {
            return CurWorldPos;
        }


        //added to optimize grid effect..if simpe no Transform() every circle.
        protected bool IsSimpleSprite()
        {
            bool ret = false;
            if (Owner.SpriteType == (int)STYPE.XY && Owner.OriVelocityAxis == Vector3.zero&& Owner.ScaleAffectorEnable == false && Owner.RotAffectorEnable == false &&
                Owner.OriSpeed < 1e-04 && Owner.GravityAffectorEnable == false&& Owner.AirAffectorEnable == false && Owner.TurbulenceAffectorEnable == false && Owner.BombAffectorEnable == false&&Owner.UVRotAffectorEnable == false && Owner.UVScaleAffectorEnable == false&&
                Mathf.Abs(Owner.OriRotationMax - Owner.OriRotationMin) < 1e-04 && Mathf.Abs(Owner.OriScaleXMin - Owner.OriScaleXMax) < 1e-04 &&
                Mathf.Abs(Owner.OriScaleYMin - Owner.OriScaleYMax) < 1e-04 && Owner.SpeedMin < 1e-04)
            {
                ret = true;
                //Debug.Log("detected simple sprite which not transformed!");
            }
            return ret;
        }


        public void SetRenderType(int type)
        {

            Type = type;

            if (type == 0)
            {
                RenderObj = Owner.GetVertexPool().AddSprite(Owner.SpriteWidth, Owner.SpriteHeight,
                    (STYPE)Owner.SpriteType, (ORIPOINT)Owner.OriPoint, 60f, IsSimpleSprite());
            }
            else if (type == 1)
            {
                float rwidth = Owner.RibbonWidth;
                float rlen = Owner.RibbonLen;
                if (Owner.UseRandomRibbon)
                {
                    rwidth = UnityEngine.Random.Range(Owner.RibbonWidthMin, Owner.RibbonWidthMax);
                    rlen = UnityEngine.Random.Range(Owner.RibbonLenMin, Owner.RibbonLenMax);
                }
                RenderObj = Owner.GetVertexPool().AddRibbonTrail(Owner.FaceToObject, Owner.FaceObject,
                    rwidth, Owner.MaxRibbonElements, rlen, Owner.ClientTransform.position + Owner.EmitPoint, 60f);
            }
            else if (type == 2)
            {
                RenderObj = Owner.GetVertexPool().AddCone(Owner.ConeSize, Owner.ConeSegment,
                    Owner.ConeAngle, Owner.OriVelocityAxis, 0, 60f, Owner.UseConeAngleChange, Owner.ConeDeltaAngle);
            }
            else if (type == 3)
            {
                if (Owner.CMesh == null)
                    Debug.LogError("custom mesh layer has no mesh to display!", Owner.gameObject);
                Vector3 dir = Vector3.zero;
                if (Owner.OriVelocityAxis == Vector3.zero)
                {
                    Owner.OriVelocityAxis = Vector3.up;
                }

                dir = Owner.OriVelocityAxis;

                RenderObj = Owner.GetVertexPool().AddCustomMesh(Owner.CMesh, dir, 60f);
            }

            else if (type == 4)
            {
                RenderObj = Owner.GetVertexPool().AddRope();
            }
            else if (type == 5)
            {
                RenderObj = Owner.GetVertexPool().AddSphericalBillboard();
            }

            RenderObj.Node = this;
        }


        public void Reset()
        {
            //activate on death subemitter.
            if (Owner.UseSubEmitters && !string.IsNullOrEmpty(Owner.DeathSubEmitter))
            {
                XffectComponent sub = Owner.SpawnCache.GetEffect(Owner.DeathSubEmitter);
                if (sub == null)
                {
                    return;
                }
                
                sub.transform.position = CurWorldPos;
                sub.Active();
            }
            
            //Position = Vector3.up * -9999;
            Position = Owner.ClientTransform.position;
            Velocity = Vector3.zero;
            ElapsedTime = 0f;
   
            
            CurWorldPos = Owner.transform.position;
            
            LastWorldPos = CurWorldPos;

            IsCollisionEventSended = false;

            if (Owner.IsRandomStartColor)
            {
                //StartColor = Owner.RandomColorParam.GetGradientColor(UnityEngine.Random.Range(0f, 1f));
                StartColor = Owner.RandomColorGradient.Evaluate(UnityEngine.Random.Range(0f, 1f));
            }

            //do not use foreach in script!
            for (int i = 0; i < AffectorList.Count; i++)
            {
                Affector aft = AffectorList[i];
                aft.Reset();
            }
   
            Scale = Vector3.one;
            
            
            mIsFade = false;


            RenderObj.Reset();

            if (Owner.UseSubEmitters 
                && SubEmitter != null && XffectComponent.IsActive(SubEmitter.gameObject)
                && Owner.SubEmitterAutoStop)
            {
                SubEmitter.StopEmit();
            }
        }

        public void Remove()
        {
            //RenderObj.OnRemove();
            Owner.RemoveActiveNode(this);
        }
  
        public void Stop()
        {
            Reset();
            Remove();
        }
  
        
        public void Fade(float time)
        {
            ColorAffector ca = null;
            
            for (int i = 0; i < AffectorList.Count; i++)
            {
                if (AffectorList[i].Type == AFFECTORTYPE.ColorAffector)
                {
                    ca = (ColorAffector)AffectorList[i];
                    break;
                }
            }
            
            if (ca == null)
            {
                ca = new ColorAffector(Owner,this);
                AffectorList.Add(ca);
            }
            
            mIsFade = true;
            
            ca.Fade(time);
        }



        public void CollisionDetection()
        {
            if (!Owner.UseCollisionDetection || IsCollisionEventSended)
                return;

            bool collided = false;
   
            GameObject collideObject = null;
            
            if (Owner.CollisionType == COLLITION_TYPE.Sphere && Owner.CollisionGoal != null)
            {
                Vector3 diff = CurWorldPos - Owner.CollisionGoal.position;
                float range = Owner.ColliisionPosRange + Owner.ParticleRadius;
                if (diff.sqrMagnitude <= range * range /*|| //fixed, don't check get through.
                    Vector3.Dot(diff, LastCollisionDetectDir) < 0*/)
                {
                    collided = true;
                    collideObject = Owner.CollisionGoal.gameObject;
                }
                LastCollisionDetectDir = diff;
            }
            else if (Owner.CollisionType == COLLITION_TYPE.CollisionLayer)
            {
                int layer = 1 << Owner.CollisionLayer;
                
                RaycastHit hit;
                Vector3 p1 = GetOriginalPos();
                if (Physics.SphereCast(p1, Owner.ParticleRadius, Velocity.normalized, out hit, Owner.ParticleRadius,layer))
                {
                    collided = true;
                    collideObject = hit.collider.gameObject;
                }
                
                //if (Physics.CheckSphere(GetOriginalPos(), Owner.ParticleRadius, layer))
                //{
                    //collided = true;
                //}
            }
            else if (Owner.CollisionType == COLLITION_TYPE.Plane)
            {
                if (!Owner.CollisionPlane.GetSide(CurWorldPos -Owner.PlaneDir.normalized * Owner.ParticleRadius))
                {
                    collided = true;
                    collideObject = Owner.gameObject;
                }
            }
            else
            {
                //Debug.LogError("invalid collision target!");
            }

            if (collided)
            {
                if (Owner.EventHandleFunctionName != "" && Owner.EventReceiver != null)
                {
                    //Owner.EventReceiver.SendMessage(Owner.EventHandleFunctionName, Owner.CollisionGoal);
                    Owner.EventReceiver.SendMessage(Owner.EventHandleFunctionName, new CollisionParam(collideObject,GetOriginalPos(),Velocity.normalized));
                }
                    
                IsCollisionEventSended = true;
                if (Owner.CollisionAutoDestroy)
                {
                    //distroy.
                    ElapsedTime = Mathf.Infinity;
                }
                

                //activate on collision subemitter.
                if (Owner.UseSubEmitters && !string.IsNullOrEmpty(Owner.CollisionSubEmitter))
                {
                    XffectComponent sub = Owner.SpawnCache.GetEffect(Owner.CollisionSubEmitter);
                    if (sub == null)
                    {
                        return;
                    }
                    
                    sub.transform.position = CurWorldPos;
                    sub.Active();
                }
                
            }
        }

        public void Update(float deltaTime)
        {
            ElapsedTime += deltaTime;
            for (int i = 0; i < AffectorList.Count; i++)
            {
                Affector aft = AffectorList[i];
                aft.Update(deltaTime);
            }

            Position += Velocity * deltaTime;

            

            //if (Mathf.Abs(Acceleration) > 1e-04)
            //{
                //Velocity += Velocity.normalized * Acceleration * deltaTime;
            //}

            if (SyncClient)
            {
                //NOTICE: only sync position.
                CurWorldPos = Position + GetRealClientPos(); 
            }
            else
            {
                CurWorldPos = Position;
            }
            
            
            CollisionDetection();
            
            //sync subemitters position.
            if (Owner.UseSubEmitters && SubEmitter != null && XffectComponent.IsActive(SubEmitter.gameObject))
            {
                SubEmitter.transform.position = CurWorldPos;
            }
            

            RenderObj.Update(deltaTime);

            if (Owner.UseShaderCurve2 || Owner.UseShaderCurve1)
            {
                float x = Owner.UseShaderCurve1 ? Owner.ShaderCurveX1.Evaluate(GetElapsedTime(), this) : 1f;

                float y = Owner.UseShaderCurve2 ? Owner.ShaderCurveX2.Evaluate(GetElapsedTime(), this) : 0f;

                RenderObj.ApplyShaderParam(x, y);
            }


            LastWorldPos = CurWorldPos;

            if (ElapsedTime > LifeTime && LifeTime > 0)
            {
                Reset();
                Remove();
            }
        }
    }
}