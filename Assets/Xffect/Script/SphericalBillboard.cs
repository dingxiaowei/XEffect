using UnityEngine;
using System.Collections;

namespace Xft
{
    //NOTE: THIS RENDER TYPE IS CONTROLLED BY SHADER!
    public class SphericalBillboard : RenderObject
    {
        protected Vector2 LowerLeftUV;
        protected Vector2 UVDimensions;


        protected Vector3 v1 = Vector3.zero;
        protected Vector3 v2 = Vector3.zero;
        protected Vector3 v3 = Vector3.zero;
        protected Vector3 v4 = Vector3.zero;
        protected VertexPool.VertexSegment Vertexsegment;


        public Color Color;
        protected bool UVChanged;
        protected bool ColorChanged;

        protected Vector3 mCenterPos = Vector3.zero;
        protected float mRadius = 0f;


        public void SetUVCoord(Vector2 lowerleft, Vector2 dimensions)
        {
            LowerLeftUV = lowerleft;
            UVDimensions = dimensions;


            XftTools.TopLeftUVToLowerLeft(ref LowerLeftUV, ref UVDimensions);

            UVChanged = true;
        }


        public void SetColor(Color c)
        {
            Color = c;
            ColorChanged = true;
        }

        public void SetPosition(Vector3 pos)
        {
            mCenterPos = pos;
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


        public SphericalBillboard(VertexPool.VertexSegment segment)
        {
            UVChanged = ColorChanged = false;
            Vertexsegment = segment;
            ResetSegment();
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

        public void UpdateCenterPos()
        {
            VertexPool pool = Vertexsegment.Pool;
            int vindex = Vertexsegment.VertStart;
            pool.Vertices[vindex + 0] = mCenterPos;
            pool.Vertices[vindex + 1] = mCenterPos;
            pool.Vertices[vindex + 2] = mCenterPos;
            pool.Vertices[vindex + 3] = mCenterPos;

            Vertexsegment.Pool.VertChanged = true;

        }

        public override void Update(float deltaTime)
        {
            //if (Node.Owner.ColorAffectorEnable || Node.mIsFade)
                SetColor(Node.Color);
            if (Node.Owner.UVAffectorEnable || Node.Owner.UVRotAffectorEnable || Node.Owner.UVScaleAffectorEnable)
                SetUVCoord(Node.LowerLeftUV, Node.UVDimensions);
            SetPosition(Node.CurWorldPos);

            if (UVChanged)
                UpdateUV();
            if (ColorChanged)
                UpdateColor();
            UVChanged = ColorChanged = false;
            UpdateCenterPos();

            mRadius = Node.Scale.x * Node.Owner.SpriteWidth * Node.OriScaleX;
            //set radius to shader!!!
            VertexPool pool = Vertexsegment.Pool;
            int index = Vertexsegment.VertStart;
            Vector2 param = Vector2.one;
            param.x = mRadius;

            //set rotation!
            param.y = ((float)Node.OriRotateAngle + Node.RotateAngle) * Mathf.PI / 180f;


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

            if (Node.Owner.MyCamera.depthTextureMode == DepthTextureMode.None)
            {
                Node.Owner.MyCamera.depthTextureMode = DepthTextureMode.Depth;
            }
        }

        public override void Reset()
        {
            SetPosition(Node.Position);
            SetColor(Color.clear);
            mRadius = 0f;
            Update(0f);
        }

    }
}