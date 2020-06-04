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

    public class VertexPool
    {
        public class VertexSegment
        {
            public int VertStart;
            public int IndexStart;
            public int VertCount;
            public int IndexCount;
            public VertexPool Pool;

            public VertexSegment(int start, int count, int istart, int icount, VertexPool pool)
            {
                VertStart = start;
                VertCount = count;
                IndexCount = icount;
                IndexStart = istart;
                Pool = pool;
            }


            public void ClearIndices()
            {
                for (int i = IndexStart; i < IndexStart + IndexCount; i++)
                {
                    Pool.Indices[i] = 0;
                }

                Pool.IndiceChanged = true;
            }

        }

        public Vector3[] Vertices;
        public int[] Indices;
        public Vector2[] UVs;
        public Color[] Colors;

        //added in version 4.0.0, to store additional parameter.
        public Vector2[] UVs2;

        public bool IndiceChanged;
        public bool ColorChanged;
        public bool UVChanged;
        public bool VertChanged;
        public bool UV2Changed;



        public Mesh Mesh;
        public Material Material;

        protected int VertexTotal;
        protected int VertexUsed;
        protected int IndexTotal = 0;
        protected int IndexUsed = 0;
        public bool FirstUpdate = true;

        protected bool VertCountChanged;


        public const int BlockSize = 108;

        public float BoundsScheduleTime = 1f;
        public float ElapsedTime = 0f;


        public void RecalculateBounds()
        {
            Mesh.RecalculateBounds();
        }

        public VertexPool(Mesh mesh, Material material)
        {
            VertexTotal = VertexUsed = 0;
            VertCountChanged = false;
            Mesh = mesh;
            Material = material;
            InitArrays();

            //***BUG FIXED Vertices又初始化为Mesh的，那么InitArray等于没有用了，VertexTotal增加。
            //搞清楚功能，是Vertex LateUpdate赋值给Mesh
            //Vertices = Mesh.vertices;
            //Indices = Mesh.triangles;
            //Colors = Mesh.colors;
            //UVs = Mesh.uv;
            IndiceChanged = ColorChanged = UVChanged = UV2Changed = VertChanged = true;
        }
  
        public CustomMesh AddCustomMesh(Mesh mesh,Vector3 dir, float maxFps)
        {
            VertexSegment segment = GetVertices(mesh.vertices.Length, mesh.triangles.Length);
            CustomMesh cmesh = new CustomMesh(segment,mesh,dir,maxFps);
            return cmesh;
        }
        
        public Cone AddCone(Vector2 size, int numSegment, float angle, Vector3 dir, int uvStretch, float maxFps, bool usedelta, AnimationCurve deltaAngle)
        {
            VertexSegment segment = GetVertices((numSegment + 1) * 2, numSegment * 6);
            Cone f = new Cone(segment, size, numSegment, angle, dir, uvStretch, maxFps);
            f.UseDeltaAngle = usedelta;
            f.CurveAngle = deltaAngle;
            return f;
        }

        public XftSprite AddSprite(float width, float height, STYPE type, ORIPOINT ori, float maxFps, bool simple)
        {
            VertexSegment segment = GetVertices(4, 6);
            XftSprite s = new XftSprite(segment, width, height, type, ori, maxFps, simple);
            return s;
        }

        public RibbonTrail AddRibbonTrail(bool useFaceObj, Transform faceobj,float width, int maxelemnt, float len, Vector3 pos, float maxFps)
        {
            VertexSegment segment = GetVertices(maxelemnt * 2, (maxelemnt - 1) * 6);
            RibbonTrail trail = new RibbonTrail(segment,useFaceObj,faceobj, width, maxelemnt, len, pos, maxFps);
            return trail;
        }

        public VertexSegment GetRopeVertexSeg(int maxcount)
        {
            VertexSegment segment = GetVertices(maxcount * 2, (maxcount - 1) * 6);
            return segment;
        }

        public Rope AddRope()
        {
            Rope rope = new Rope();
            return rope;
        }

        public SphericalBillboard AddSphericalBillboard()
        {
            VertexSegment segment = GetVertices(4, 6);
            SphericalBillboard sb = new SphericalBillboard(segment);
            
            return sb;
        }

        public Material GetMaterial()
        {
            return Material;
        }

        public VertexSegment GetVertices(int vcount, int icount)
        {
            int vertNeed = 0;
            int indexNeed = 0;
            if (VertexUsed + vcount >= VertexTotal)
            {
                vertNeed = (vcount / BlockSize + 1) * BlockSize;
            }
            if (IndexUsed + icount >= IndexTotal)
            {
                indexNeed = (icount / BlockSize + 1) * BlockSize;
            }
            VertexUsed += vcount;
            IndexUsed += icount;
            if (vertNeed != 0 || indexNeed != 0)
            {
                EnlargeArrays(vertNeed, indexNeed);
                VertexTotal += vertNeed;
                IndexTotal += indexNeed;
            }
            return new VertexSegment(VertexUsed - vcount, vcount, IndexUsed - icount, icount, this);
        }


        void InitDefaultShaderParam(Vector2[] uv2)
        {
            for (int i = 0; i < uv2.Length; i++)
            {
                //default displacement param
                uv2[i].x = 1f;

                //default dissolve param;
                uv2[i].y = 0f;
            }
        }

        protected void InitArrays()
        {
            Vertices = new Vector3[4];
            UVs = new Vector2[4];
            UVs2 = new Vector2[4];
            Colors = new Color[4];
            Indices = new int[6];
            VertexTotal = 4;
            IndexTotal = 6;

            InitDefaultShaderParam(UVs2);
        }



        public void EnlargeArrays(int count, int icount)
        {
            Vector3[] tempVerts = Vertices;
            Vertices = new Vector3[Vertices.Length + count];
            tempVerts.CopyTo(Vertices, 0);

            Vector2[] tempUVs = UVs;
            UVs = new Vector2[UVs.Length + count];
            tempUVs.CopyTo(UVs, 0);

            Vector2[] tempUVs2 = UVs2;
            UVs2 = new Vector2[UVs2.Length + count];
            tempUVs2.CopyTo(UVs2, 0);
            InitDefaultShaderParam(UVs2);

            Color[] tempColors = Colors;
            Colors = new Color[Colors.Length + count];
            tempColors.CopyTo(Colors, 0);

            int[] tempTris = Indices;
            Indices = new int[Indices.Length + icount];
            tempTris.CopyTo(Indices, 0);

            VertCountChanged = true;
            IndiceChanged = true;
            ColorChanged = true;
            UVChanged = true;
            VertChanged = true;
            UV2Changed = true;
        }

        public void LateUpdate()
        {
            if (VertCountChanged)
            {
                Mesh.Clear();
            }

            // we assume the vertices are always changed.
            Mesh.vertices = Vertices;
            if (UVChanged)
            {
                Mesh.uv = UVs;
            }

            if (UV2Changed)
            {
                Mesh.uv2 = UVs2;
            }

            if (ColorChanged)
            {
                Mesh.colors = Colors;
            }

            if (IndiceChanged)
            {
                Mesh.triangles = Indices;
            }

            ElapsedTime += Time.deltaTime;
            if (ElapsedTime > BoundsScheduleTime || FirstUpdate)
            {
                RecalculateBounds();
                ElapsedTime = 0f;
            }

            //how to recognise the first update?..
            if (ElapsedTime > BoundsScheduleTime)
                FirstUpdate = false;

            VertCountChanged = false;
            IndiceChanged = false;
            ColorChanged = false;
            UVChanged = false;
            UV2Changed = false;
            VertChanged = false;
        }
    }
}