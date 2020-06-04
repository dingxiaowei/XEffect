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
    public struct XTransform
    {
        public Vector3 position;
        public Quaternion rotation;

        public void Reset()
        {
            position = Vector3.zero;
            rotation = Quaternion.identity;
        }
        public void LookAt(Vector3 dir, Vector3 up)
        {
            //Vector3 relativePos = target - position;
            rotation = Quaternion.LookRotation(dir, up);
        }
    }

    public class XftSprite : RenderObject
    {
        protected Vector2 LowerLeftUV;
        protected Vector2 UVDimensions;

        public XTransform MyTransform;
        public Vector3 v1 = Vector3.zero;
        public Vector3 v2 = Vector3.zero;
        public Vector3 v3 = Vector3.zero;
        public Vector3 v4 = Vector3.zero;
        protected VertexPool.VertexSegment Vertexsegment;

        public Color Color;
        Vector3 ScaleVector;
        Quaternion Rotation;
        Matrix4x4 LocalMat;
        Matrix4x4 WorldMat;

        protected float ElapsedTime;

        //public Camera MainCamera;

        protected bool UVChanged;
        protected bool ColorChanged;

        protected Matrix4x4 LastMat;
        protected Vector3 RotateAxis;
        STYPE Type;
        ORIPOINT OriPoint;
        // int UVStretch;//0:VERTICAL  1: HORIZONTAL

        // if is  simple sprite, it will not use Transform() in order to optimize performance.
        bool Simple = false;


        //for custom grid tile use.
        public bool UseCustomHeight = false;
        public float h1;
        public float h2;
        public float h3;
        public float h4;


        #region override

        //x for displacement control
        //y for dissolve control.
        public override void ApplyShaderParam(float x, float y)
        {
            VertexPool pool = Vertexsegment.Pool;
            int index = Vertexsegment.VertStart;
            Vector2 param = Vector2.one;
            param.x = x;
            param.y = y;
            pool.UVs2[index + 0] = param;
            pool.UVs2[index + 1] = param;
            pool.UVs2[index + 2] = param;
            pool.UVs2[index + 3] = param;

            Vertexsegment.Pool.UV2Changed = true;
        }

        public override void Initialize(EffectNode node)
        {
            base.Initialize(node);

            SetUVCoord(node.LowerLeftUV, node.UVDimensions);
            SetColor(Node.Color);
            if (Simple)
            {
                //simple sprite only update once here!
                node.Update(0f);
                Transform();
            }

            //direction is synced to client rotation, except Sphere direction.
            if (node.Owner.DirType != DIRECTION_TYPE.Sphere)
                SetRotationTo(node.Owner.ClientTransform.rotation * node.OriDirection);
            else
                SetRotationTo(node.OriDirection);
        }


        public override void Reset()
        {
            SetRotation(Node.OriRotateAngle);
            SetPosition(Node.Position);
            SetColor(Color.clear);
            Update(true, 0f);
        }


        public override void Update(float deltaTime)
        {
            if (Node.Owner.AlwaysSyncRotation)
            {
                if (Node.Owner.DirType != DIRECTION_TYPE.Sphere)
                    SetRotationTo(Node.Owner.ClientTransform.rotation * Node.OriDirection);
            }

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



        public XftSprite(VertexPool.VertexSegment segment, float width, float height, STYPE type, ORIPOINT oripoint, float maxFps, bool simple)
        {
            UVChanged = ColorChanged = false;
            MyTransform.position = Vector3.zero;
            MyTransform.rotation = Quaternion.identity;
            LocalMat = WorldMat = Matrix4x4.identity;
            Vertexsegment = segment;
            //UVStretch = uvStretch;

            LastMat = Matrix4x4.identity;
            ElapsedTime = 0f;
            //Fps = 1f / maxFps;
            Simple = simple;

            OriPoint = oripoint;

            RotateAxis = Vector3.zero;
            SetSizeXZ(width, height);
            RotateAxis.y = 1;
            Type = type;
            ResetSegment();
        }

        public void ResetSegment()
        {
            VertexPool pool = Vertexsegment.Pool;
            int index = Vertexsegment.IndexStart;
            int vindex = Vertexsegment.VertStart;
            // Clockwise winding
            pool.Indices[index + 0] = vindex + 0;    //	0_ 1			0 ___ 3
            pool.Indices[index + 1] = vindex + 3;    //  | /		Verts:	 |	/|
            pool.Indices[index + 2] = vindex + 1;    // 2|/				1|/__|2
            pool.Indices[index + 3] = vindex + 3;    //	  3
            pool.Indices[index + 4] = vindex + 2;    //   /|
            pool.Indices[index + 5] = vindex + 1;    // 5/_|4

            pool.Vertices[vindex + 0] = Vector3.zero;
            pool.Vertices[vindex + 1] = Vector3.zero;
            pool.Vertices[vindex + 2] = Vector3.zero;
            pool.Vertices[vindex + 3] = Vector3.zero;

            pool.Colors[vindex + 0] = Color.white;
            pool.Colors[vindex + 1] = Color.white;
            pool.Colors[vindex + 2] = Color.white;
            pool.Colors[vindex + 3] = Color.white;

            pool.UVs[vindex + 0] = Vector2.zero;
            pool.UVs[vindex + 1] = Vector2.zero;
            pool.UVs[vindex + 2] = Vector2.zero;
            pool.UVs[vindex + 3] = Vector2.zero;

            pool.UVChanged = pool.IndiceChanged = pool.ColorChanged = pool.VertChanged = true;
        }


        public void SetUVCoord(Vector2 lowerleft, Vector2 dimensions)
        {
            LowerLeftUV = lowerleft;
            UVDimensions = dimensions;


            XftTools.TopLeftUVToLowerLeft(ref LowerLeftUV, ref UVDimensions);

            UVChanged = true;
        }

        public void SetPosition(Vector3 pos)
        {
            MyTransform.position = pos;
        }

        public void SetRotation(Quaternion q)
        {
            MyTransform.rotation = q;
        }

        //将XZ面的Sprite转到与dir垂直
        public void SetRotationFaceTo(Vector3 dir)
        {
            MyTransform.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        }

        //将XZ面的Sprite转到与dir共面
        //首先将sprite以Y轴转
        //再将坐标系由Y转移到dir
        public void SetRotationTo(Vector3 dir)
        {
            if (dir == Vector3.zero)
                return;
            Quaternion rot = Quaternion.identity;
            Vector3 from = dir;
            from.y = 0f;
            if (from == Vector3.zero)
                from = Vector3.up;
            if (OriPoint == ORIPOINT.CENTER)
            {
                Quaternion rot1 = Quaternion.FromToRotation(new Vector3(0, 0, 1), from);
                Quaternion rot2 = Quaternion.FromToRotation(from, dir);
                rot = rot2 * rot1;
            }
            else if (OriPoint == ORIPOINT.LEFT_UP)
            {
                Quaternion rot1 = Quaternion.FromToRotation(LocalMat.MultiplyPoint3x4(v3), from);
                Quaternion rot2 = Quaternion.FromToRotation(from, dir);
                rot = rot2 * rot1;
            }
            else if (OriPoint == ORIPOINT.LEFT_BOTTOM)
            {
                Quaternion rot1 = Quaternion.FromToRotation(LocalMat.MultiplyPoint3x4(v4), from);
                Quaternion rot2 = Quaternion.FromToRotation(from, dir);
                rot = rot2 * rot1;
            }
            else if (OriPoint == ORIPOINT.RIGHT_BOTTOM)
            {
                Quaternion rot1 = Quaternion.FromToRotation(LocalMat.MultiplyPoint3x4(v1), from);
                Quaternion rot2 = Quaternion.FromToRotation(from, dir);
                rot = rot2 * rot1;
            }
            else if (OriPoint == ORIPOINT.RIGHT_UP)
            {
                Quaternion rot1 = Quaternion.FromToRotation(LocalMat.MultiplyPoint3x4(v2), from);
                Quaternion rot2 = Quaternion.FromToRotation(from, dir);
                rot = rot2 * rot1;
            }
            else if (OriPoint == ORIPOINT.BOTTOM_CENTER)
            {
                Quaternion rot1 = Quaternion.FromToRotation(new Vector3(0, 0, 1), from);
                Quaternion rot2 = Quaternion.FromToRotation(from, dir);
                rot = rot2 * rot1;
            }
            else if (OriPoint == ORIPOINT.TOP_CENTER)
            {
                Quaternion rot1 = Quaternion.FromToRotation(new Vector3(0, 0, -1), from);
                Quaternion rot2 = Quaternion.FromToRotation(from, dir);
                rot = rot2 * rot1;
            }
            else if (OriPoint == ORIPOINT.RIGHT_CENTER)
            {
                Quaternion rot1 = Quaternion.FromToRotation(new Vector3(-1, 0, 0), from);
                Quaternion rot2 = Quaternion.FromToRotation(from, dir);
                rot = rot2 * rot1;
            }
            else if (OriPoint == ORIPOINT.LEFT_CENTER)
            {
                Quaternion rot1 = Quaternion.FromToRotation(new Vector3(1, 0, 0), from);
                Quaternion rot2 = Quaternion.FromToRotation(from, dir);
                rot = rot2 * rot1;
            }

            MyTransform.rotation = rot;
        }

        public void SetSizeXZ(float width, float height)
        {
            v1 = new Vector3(-width / 2, 0, height / 2);	// Upper-left
            v2 = new Vector3(-width / 2, 0, -height / 2);	// Lower-left
            v3 = new Vector3(width / 2, 0, -height / 2);	// Lower-right
            v4 = new Vector3(width / 2, 0, height / 2);	// Upper-right

            Vector3 offset = Vector3.zero;
            if (OriPoint == ORIPOINT.LEFT_UP)
                offset = v3;
            else if (OriPoint == ORIPOINT.LEFT_BOTTOM)
                offset = v4;
            else if (OriPoint == ORIPOINT.RIGHT_BOTTOM)
                offset = v1;
            else if (OriPoint == ORIPOINT.RIGHT_UP)
                offset = v2;
            else if (OriPoint == ORIPOINT.BOTTOM_CENTER)
                offset = new Vector3(0, 0, height / 2);
            else if (OriPoint == ORIPOINT.TOP_CENTER)
                offset = new Vector3(0, 0, -height / 2);
            else if (OriPoint == ORIPOINT.LEFT_CENTER)
                offset = new Vector3(width / 2, 0, 0);
            else if (OriPoint == ORIPOINT.RIGHT_CENTER)
                offset = new Vector3(-width / 2, 0, 0);
            v1 += offset;
            v2 += offset;
            v3 += offset;
            v4 += offset;
        }

        public void UpdateUV()
        {
            VertexPool pool = Vertexsegment.Pool;
            int index = Vertexsegment.VertStart;
            if (UVDimensions.y > 0)
            {//From     Lower-Left
                pool.UVs[index + 0] = LowerLeftUV + Vector2.up * UVDimensions.y;   // Upper-left
                pool.UVs[index + 1] = LowerLeftUV;                                 // Lower-left
                pool.UVs[index + 2] = LowerLeftUV + Vector2.right * UVDimensions.x;// Lower-right
                pool.UVs[index + 3] = LowerLeftUV + UVDimensions;                  // Upper-right
            }
            else
            {// From Upper Left
                pool.UVs[index + 0] = LowerLeftUV;
                pool.UVs[index + 1] = LowerLeftUV + Vector2.up * UVDimensions.y;
                pool.UVs[index + 2] = LowerLeftUV + UVDimensions;
                pool.UVs[index + 3] = LowerLeftUV + Vector2.right * UVDimensions.x;
            }
            //Debug.LogWarning(pool.UVs[index + 0] + ":" + pool.UVs[index + 1] + ":" + pool.UVs[index + 2] + ":" + pool.UVs[index + 3]);
            Vertexsegment.Pool.UVChanged = true;
        }

        public void UpdateColor()
        {
            VertexPool pool = Vertexsegment.Pool;
            int index = Vertexsegment.VertStart;
            pool.Colors[index + 0] = Color;
            pool.Colors[index + 1] = Color;
            pool.Colors[index + 2] = Color;
            pool.Colors[index + 3] = Color;

            Vertexsegment.Pool.ColorChanged = true;
        }


        public void SetCustomHeight(float[] h)
        {
            UseCustomHeight = true;
            h1 = h[0];
            h2 = h[1];
            h3 = h[2];
            h4 = h[3];
            Transform();
        }

        //this function should be very optimized
        public void Transform()
        {
            LocalMat.SetTRS(Vector3.zero, Rotation, ScaleVector);

            Transform t = Node.MyCamera.transform;
            if (Type == STYPE.BILLBOARD)
            {
                MyTransform.LookAt(t.rotation * Vector3.up, t.rotation * Vector3.back);
            }
            else if (Type == STYPE.BILLBOARD_Y)
            {
                Vector3 diff = t.position - MyTransform.position;
                diff.y = 0f;
                MyTransform.LookAt(Vector3.up, diff);
            }
            WorldMat.SetTRS(MyTransform.position, MyTransform.rotation, Vector3.one);
            Matrix4x4 mat = WorldMat * LocalMat;

            VertexPool pool = Vertexsegment.Pool;
            int index = Vertexsegment.VertStart;

            Vector3 v1w = mat.MultiplyPoint3x4(v1);
            Vector3 v2w = mat.MultiplyPoint3x4(v2);
            Vector3 v3w = mat.MultiplyPoint3x4(v3);
            Vector3 v4w = mat.MultiplyPoint3x4(v4);


            if (Type == STYPE.BILLBOARD_SELF)
            {
                Vector3 headElem = Vector3.zero;
                Vector3 tailElem = Vector3.zero;
                float vWidth = 0f;

                Vector3 mscale = Vector3.one * (Node.Owner.Owner.Scale);

                if (Node.Owner.SpriteUVStretch == UV_STRETCH.Vertical)
                {
                    headElem = (v1w + v4w) / 2;
                    tailElem = (v2w + v3w) / 2;
                    vWidth = (v4w - v1w).magnitude;
                }
                else
                {
                    headElem = (v1w + v2w) / 2;
                    tailElem = (v4w + v3w) / 2;
                    vWidth = (v2w - v1w).magnitude;
                }

                Vector3 chainTangent = headElem - tailElem;

                //fixed ver 2.1.2: the scale may influence the direction!
                //Vector3 vP1ToEye = MainCamera.transform.position - headElem;
                Vector3 vP1ToEye = Node.MyCamera.transform.position - Vector3.Scale(headElem, mscale);
                Vector3 vPerpendicular1 = Vector3.Cross(chainTangent, vP1ToEye);
                vPerpendicular1.Normalize();
                vPerpendicular1 *= (vWidth * 0.5f);

                //Vector3 vP2ToEye = MainCamera.transform.position - tailElem;
                Vector3 vP2ToEye = Node.MyCamera.transform.position - Vector3.Scale(tailElem, mscale);
                Vector3 vPerpendicular2 = Vector3.Cross(chainTangent, vP2ToEye);
                vPerpendicular2.Normalize();
                vPerpendicular2 *= (vWidth * 0.5f);

                //Debug.DrawRay(MainCamera.transform.position,-vP2ToEye * 1000f,Color.red,10f);

                if (Node.Owner.SpriteUVStretch == UV_STRETCH.Vertical)
                {
                    v1w = headElem - vPerpendicular1;
                    v4w = headElem + vPerpendicular1;
                    v2w = tailElem - vPerpendicular2;
                    v3w = tailElem + vPerpendicular2;
                }
                else
                {
                    v1w = headElem - vPerpendicular1;
                    v2w = headElem + vPerpendicular1;
                    v4w = tailElem - vPerpendicular2;
                    v3w = tailElem + vPerpendicular2;
                }
            }

            pool.Vertices[index + 0] = v1w;
            pool.Vertices[index + 1] = v2w;
            pool.Vertices[index + 2] = v3w;
            pool.Vertices[index + 3] = v4w;

            if (UseCustomHeight)
            {
                pool.Vertices[index + 0].y = h1;
                pool.Vertices[index + 1].y = h2;
                pool.Vertices[index + 2].y = h3;
                pool.Vertices[index + 3].y = h4;
            }

        }

        public void SetRotation(float angle)
        {
            Rotation = Quaternion.AngleAxis(angle, RotateAxis);
        }

        public void SetScale(float width, float height)
        {
            ScaleVector.x = width;
            ScaleVector.z = height;
        }

        public void Update(bool force, float deltaTime)
        {
            ElapsedTime += deltaTime;
            if (ElapsedTime > Fps || force)
            {
                if (!Simple || force)
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

        public void SetColor(Color c)
        {
            Color = c;
            ColorChanged = true;
        }
    }
}


