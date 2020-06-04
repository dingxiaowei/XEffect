using UnityEngine;
using System.Collections;
using Xft;
using System.Collections.Generic;

namespace Xft
{
    public class CustomMesh : RenderObject
    {
        protected VertexPool.VertexSegment Vertexsegment;
        
        
        public Mesh MyMesh;
        
        public Vector3[] MeshVerts;

        public Color MyColor;
        public Vector3 MyPosition = Vector3.zero;
        public Vector2 MyScale = Vector2.one;
        public Quaternion MyRotation = Quaternion.identity;
        public  Vector3 MyDirection;
        
        
        Matrix4x4 LocalMat;
        Matrix4x4 WorldMat;
       // float Fps = 0.016f;
        float ElapsedTime = 0f;
        
       // EffectNode Owner;

        public bool ColorChanged = false;
        public bool UVChanged = false;
        
        protected Vector2 LowerLeftUV;
        protected Vector2 UVDimensions;
        
        protected Vector2[] m_oriUvs = null;
        



        #region override


        //x for displacement control
        //y for dissolve control.
        public override void ApplyShaderParam(float x, float y)
        {
            Vector2 param = Vector2.one;
            param.x = x;
            param.y = y;

            VertexPool pool = Vertexsegment.Pool;
            int index = Vertexsegment.VertStart;

            for (int i = 0; i < Vertexsegment.VertCount; i++)
            {
                pool.UVs2[index + i] = param;
            }

            Vertexsegment.Pool.UV2Changed = true;

        }

        public override void Initialize(EffectNode node)
        {
            base.Initialize(node);
            SetColor(Node.Color);
            SetRotation(Node.OriRotateAngle);
            SetScale(Node.OriScaleX, Node.OriScaleY);
            SetUVCoord(Node.LowerLeftUV, Node.UVDimensions);

            //if (Node.Owner.DirType != DIRECTION_TYPE.Sphere)
            //    SetDirection(Node.Owner.ClientTransform.rotation * Node.OriDirection);
            //else
                SetDirection(Node.OriDirection);
        }

        public override void Reset()
        {
            SetColor(Color.clear);
            SetRotation(Node.OriRotateAngle);
            Update(true, 0f);
        }

        public override void Update(float deltaTime)
        {
            SetScale(Node.Scale.x * Node.OriScaleX, Node.Scale.y * Node.OriScaleY);
            //if (Node.Owner.ColorAffectorEnable || Node.mIsFade)
                SetColor(Node.Color);

            if (Node.Owner.UVAffectorEnable || Node.Owner.UVRotAffectorEnable || Node.Owner.UVScaleAffectorEnable)
                SetUVCoord(Node.LowerLeftUV, Node.UVDimensions);
            SetRotation((float)Node.OriRotateAngle + Node.RotateAngle);
            SetPosition(Node.CurWorldPos);
            Update(false, deltaTime);
        }

        #endregion


        public CustomMesh(VertexPool.VertexSegment segment, Mesh mesh,Vector3 dir, float maxFps)
        {
   
            MyMesh = mesh;
            
            MeshVerts =  new Vector3[mesh.vertices.Length];
            
            mesh.vertices.CopyTo(MeshVerts,0);

            Vertexsegment = segment;
            MyDirection = dir;
            //Fps = 1f / maxFps;
            SetPosition(Vector3.zero);
            InitVerts();
        }
        
        
        public void SetDirection(Vector3 dir)
        {
            MyDirection = dir;
        }
        
        public void SetUVCoord(Vector2 lowerleft, Vector2 dimensions)
        {
            LowerLeftUV = lowerleft;
            UVDimensions = dimensions;


            XftTools.TopLeftUVToLowerLeft(ref LowerLeftUV, ref UVDimensions);

            //Debug.LogWarning(LowerLeftUV + ":" + dimensions);

            UVChanged = true;
        }
        
        public void SetColor(Color c)
        {
            MyColor = c;
            ColorChanged = true;
        }

        public void SetPosition(Vector3 pos)
        {
            MyPosition = pos;
        }
        
        public void SetScale(float width, float height)
        {
            MyScale.x = width;
            MyScale.y = height;
        }
        
        public void SetRotation(float angle)
        {
            MyRotation = Quaternion.AngleAxis(angle, Node.Owner.MeshRotateAxis);
        }
 
        public void InitVerts()
        {
            VertexPool pool = Vertexsegment.Pool;
            int index = Vertexsegment.IndexStart;
            int vindex = Vertexsegment.VertStart;

            for (int i = 0; i < MeshVerts.Length; i++)
            {
                //pool.Vertices[vindex + i] = MeshVerts[i];
                pool.Vertices[vindex + i] = Vector3.zero;
            }
            
            
            int[] indices = MyMesh.triangles;
            for (int i = 0; i < Vertexsegment.IndexCount; i++)
            {
                pool.Indices[i + index] = indices[i] + vindex;
            }
            
            m_oriUvs = MyMesh.uv;
            for (int i = 0; i < m_oriUvs.Length; i++)
            {
                pool.UVs[i + vindex] = m_oriUvs[i];
            }
            
            Color[] colors = MyMesh.colors;
            for (int i = 0; i < colors.Length; i++)
            {
                //pool.Colors[i + vindex] = colors[i];
                pool.Colors[i + vindex] = Color.clear;
            }
        }
        
        public void UpdateUV()
        {
            VertexPool pool = Vertexsegment.Pool;
            int vindex = Vertexsegment.VertStart;
            
            for (int i = 0; i < m_oriUvs.Length; i++)
            {
                Vector2 curScale = LowerLeftUV + Vector2.Scale(m_oriUvs[i] , UVDimensions);
                
                //fix wrap : repeat.
                if (curScale.x > UVDimensions.x +  LowerLeftUV.x)
                {
                    float delta = curScale.x - UVDimensions.x - LowerLeftUV.x;
                    
                    delta = Mathf.Repeat(delta,UVDimensions.x + LowerLeftUV.x);

                    curScale.x = LowerLeftUV.x + delta * UVDimensions.x;
                }
                
                pool.UVs[i + vindex] = curScale;
            }
            
            Vertexsegment.Pool.UVChanged = true;
        }
        
        public void UpdateColor()
        {
            VertexPool pool = Vertexsegment.Pool;
            int index = Vertexsegment.VertStart;
            for (int i = 0; i < Vertexsegment.VertCount; i++)
            {
                pool.Colors[index + i] = MyColor;
            }
            Vertexsegment.Pool.ColorChanged = true;
        }
        
        
        public void Transform()
        {
            Quaternion rot;
            if (Node.Owner.AlwaysSyncRotation)
                rot = Quaternion.FromToRotation(Vector3.up, Node.Owner.transform.rotation * MyDirection);
            else
                rot = Quaternion.FromToRotation(Vector3.up, MyDirection);

            Vector3 scale = Vector3.one;
            scale.x = scale.z = MyScale.x;
            scale.y = MyScale.y;

            LocalMat.SetTRS(Vector3.zero, rot * MyRotation, scale);
            WorldMat.SetTRS(MyPosition, Quaternion.identity, Vector3.one);
            Matrix4x4 mat = WorldMat * LocalMat;

            VertexPool pool = Vertexsegment.Pool;

            for (int i = Vertexsegment.VertStart; i < Vertexsegment.VertStart + Vertexsegment.VertCount; i++)
            {
                pool.Vertices[i] = mat.MultiplyPoint3x4(MeshVerts[i - Vertexsegment.VertStart]);
            }
            
        }
        

        public void Update(bool force,float deltaTime)
        {
            ElapsedTime += deltaTime;
            if (ElapsedTime > Fps || force)
            {
                Transform();
                if (ColorChanged)
                    UpdateColor();
                if (UVChanged)
                    UpdateUV();
                ColorChanged = UVChanged = false;
                if (!force)
                    ElapsedTime -= Fps;
            }
        }
    }
}

