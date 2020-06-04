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
    public class Cone : RenderObject
    {

        public  Vector2 Size;
        public  Vector3 Direction;
        //public  int     UVStretch;
        public int NumSegment = 4;
        public float SpreadAngle = 0f;
        public float OriSpreadAngle;
        public float OriRotAngle = 45;

        public bool UseDeltaAngle = false;
        public AnimationCurve CurveAngle;



        protected int NumVerts;
        protected float SegmentAngle;

        protected VertexPool.VertexSegment Vertexsegment;
        //protected float Fps;

        protected Vector3[] Verts;

        protected Vector3[] VertsTemp;
        protected float ElapsedTime = 0f;

        protected bool UVChanged = true;
        protected bool ColorChanged = true;

        protected float OriUVX = 0f;


        //affected
        public Vector3 Position = Vector3.zero;
        public  Color   Color;
        public  Vector2 Scale;
        protected Vector2 LowerLeftUV = Vector2.zero;
        protected Vector2 UVDimensions = Vector2.one;


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

            for (int i = 0; i < NumVerts; i++)
            {
                pool.UVs2[index + i] = param;
            }

            Vertexsegment.Pool.UV2Changed = true;

        }

        public override void Initialize(EffectNode node)
        {
            base.Initialize(node);
            SetUVCoord(Node.LowerLeftUV, Node.UVDimensions);
            SetColor(Node.Color);
            SetRotation(Node.OriRotateAngle);
        }

        public override void Reset()
        {
            SetRotation(Node.OriRotateAngle);
            SetColor(Color.clear);
            SetPosition(Node.Position);
            ResetAngle();
            MyUpdate(true, 0f);
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
            MyUpdate(false, deltaTime);
        }

        #endregion



        public Cone(VertexPool.VertexSegment segment, Vector2 size, int numseg, float angle, Vector3 dir, int uvStretch, float maxFps)
        {
            Vertexsegment = segment;
            Size = size;
            Direction = dir;
            //UVStretch = uvStretch;
            //Fps = 1f / maxFps;
            SetColor(Color.white);

            NumSegment = numseg;
            SpreadAngle = angle;
            OriSpreadAngle = angle;
            
            InitVerts();
        }


        //注意这里有点乱，与Sprite，CustomMesh这些不一样，UV计算是自定义的。
        //所以先全局坐标转换为lower left后，还需要局部转换。
        public void SetUVCoord(Vector2 topleft, Vector2 dimensions)
        {
            LowerLeftUV = topleft;
            UVDimensions = dimensions;

            XftTools.TopLeftUVToLowerLeft(ref LowerLeftUV, ref UVDimensions);

            //注意这里还需颠倒UV。这里是局部坐标转换

            //还原为从顶部开始
            LowerLeftUV.y -= dimensions.y;
            UVDimensions.y = -UVDimensions.y;

            UVChanged = true;
        }

        public void SetColor(Color c)
        {
            Color = c;
            ColorChanged = true;
        }

        public void SetPosition(Vector3 pos)
        {
            Position = pos;
        }

        public void SetScale(float width, float height)
        {
            Scale.x = width;
            Scale.y = height;
        }

    
        public void SetRotation(float angle)
        {
            //LowerLeftUV.x =  OriUVX + (angle%360) / 360f;
            //UVChanged = true;
            OriRotAngle = angle;
        }

        public void ResetAngle()
        {
            SpreadAngle = OriSpreadAngle;
        }


        protected void UpdateRotAngle(float deltaTime)
        {
            if (!UseDeltaAngle)
                return;
            SpreadAngle = CurveAngle.Evaluate(Node.GetElapsedTime());

            for (int i = NumVerts / 2; i < NumVerts; i++)
            {
                Verts[i] = Verts[i - NumVerts / 2] + Vector3.up * Size.y;
                Verts[i] = Vector3.RotateTowards(Verts[i], Verts[i - NumVerts / 2], SpreadAngle * Mathf.Deg2Rad, 0);
            }
        }

        public void UpdateVerts()
        {
            Vector3 v = Vector3.forward * Size.x;

            for (int i = 0; i < NumVerts / 2; i++)
            {
                // ... don't use Quanternion.EulerAngles()....
                Verts[i] = Quaternion.Euler(0, OriRotAngle + SegmentAngle * i, 0) * v;
            }

            for (int i = NumVerts / 2; i < NumVerts; i++)
            {
                Verts[i] = Verts[i - NumVerts / 2] + Vector3.up * Size.y;
                Verts[i] = Vector3.RotateTowards(Verts[i], Verts[i - NumVerts / 2], SpreadAngle * Mathf.Deg2Rad, 0);
            }
        }

        public void InitVerts()
        {
            NumVerts = (NumSegment + 1) * 2;
            SegmentAngle = 360f / NumSegment;

            Verts = new Vector3[NumVerts];
            VertsTemp = new Vector3[NumVerts];

            UpdateVerts();

            //init indices
            VertexPool pool = Vertexsegment.Pool;
            int index = Vertexsegment.IndexStart;
            int vindex = Vertexsegment.VertStart;
            for (int i = 0; i < NumSegment; i++)
            {
                int myIndex = index + i * 6;
                int myVindex = vindex + i;

                pool.Indices[myIndex + 0] = myVindex + NumSegment + 1;       //			5 ___6___7___8___9
                pool.Indices[myIndex + 1] = myVindex + NumSegment + 2;      //	Verts:	 |	 |   |   |   |
                pool.Indices[myIndex + 2] = myVindex + 0;                    //		     0___1___2___3___4
                pool.Indices[myIndex + 3] = myVindex + NumSegment + 2;    
                pool.Indices[myIndex + 4] = myVindex + 1;   
                pool.Indices[myIndex + 5] = myVindex + 0;

                //Debug.Log(pool.Vertices.Length + ":" + vindex);
                pool.Vertices[myVindex + NumSegment + 1] = Vector3.zero;
                pool.Vertices[myVindex + NumSegment + 2] = Vector3.zero;
                pool.Vertices[myVindex + 0] = Vector3.zero;
                pool.Vertices[myVindex + 1] = Vector3.zero;
            }
        }

        public void UpdateUV()
        {
            VertexPool pool = Vertexsegment.Pool;
            int index = Vertexsegment.VertStart;
            float uvSeg = UVDimensions.x / NumSegment;

            //Debug.LogWarning(LowerLeftUV + ":" + UVDimensions);


            for (int i = 0; i < NumSegment + 1; i++)
            {
                pool.UVs[index + i] = LowerLeftUV;
                pool.UVs[index + i].x += i*uvSeg;
            }
            for (int i = NumSegment + 1; i < NumVerts; i++)
            {
                pool.UVs[index + i] = LowerLeftUV + Vector2.up * UVDimensions.y;
                pool.UVs[index + i].x += (i - NumSegment - 1) * uvSeg;
            }
            Vertexsegment.Pool.UVChanged = true;
        }

        public void UpdateColor()
        {
            VertexPool pool = Vertexsegment.Pool;
            int index = Vertexsegment.VertStart;
            for (int i = 0; i < NumVerts; i++)
            {
                pool.Colors[index + i] = Color;
            }
            Vertexsegment.Pool.ColorChanged = true;
        }


        public void Transform()
        {
            if (Node.Owner.RotAffectorEnable || OriRotAngle != 0f)
                UpdateVerts();

            Quaternion rot;

            if (Node.Owner.AlwaysSyncRotation)
                rot = Quaternion.FromToRotation(Vector3.up, Node.Owner.transform.rotation * Direction);
            else
                rot = Quaternion.FromToRotation(Vector3.up,Direction);

            for (int i = 0; i < NumSegment + 1; i++)
            {
                VertsTemp[i] = Verts[i] * Scale.x;
				VertsTemp[i] = rot * VertsTemp[i];
				VertsTemp[i] = VertsTemp[i] + Position;
            }

            for (int i = NumSegment + 1; i < NumVerts; i++)
            {//注意只扩大Y轴。
                //VertsTemp[i] = Verts[i];
                VertsTemp[i] = Verts[i] * Scale.x;
                VertsTemp[i].y = Verts[i].y * Scale.y;
				VertsTemp[i] = rot * VertsTemp[i];
                VertsTemp[i] = VertsTemp[i] + Position;
            }
            VertexPool pool = Vertexsegment.Pool;
            int index = Vertexsegment.VertStart;

            for (int i = 0; i < NumVerts; i++)
            {
                pool.Vertices[index + i] = VertsTemp[i];
            }
        }

        public void MyUpdate(bool force,float deltaTime)
        {
            ElapsedTime += deltaTime;
            if (ElapsedTime > Fps || force)
            {
                UpdateRotAngle(deltaTime);
                Transform();
                if (UVChanged)
                    UpdateUV();
                if (ColorChanged)
                    UpdateColor();
                UVChanged = ColorChanged = false;
                if (!force)
                    ElapsedTime -= Fps;
            }
        }
    }
}
