using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Xft
{
    public class RopeData
    {
        public List<EffectNode> NodeList = new List<EffectNode>();

        public VertexPool.VertexSegment Vertexsegment;

        public EffectLayer Owner;

        public EffectNode dummyNode;//will not be added to the activenode list. so will not be rendered, just to calc uv anim.

        public void Init(EffectLayer owner)
        {
            Owner = owner;
            Vertexsegment = owner.GetVertexPool().GetRopeVertexSeg(owner.MaxENodes);

            dummyNode = new EffectNode(0, owner.ClientTransform, false, owner);
            List<Affector> afts = owner.InitAffectors(dummyNode);
            dummyNode.SetAffectorList(afts);
            dummyNode.SetRenderType(4);

            //use infinite life.
            dummyNode.Init(Vector3.zero, 0f, -1f, 0, 1f, 1f, Color.clear, Vector2.zero, Vector2.one);
        }

        protected void RefreshData()
        {
            NodeList.Clear();
            for (int i = 0; i < Owner.MaxENodes; i++)
            {
                EffectNode node = Owner.ActiveENodes[i];
                if (node == null)
                    continue;
                NodeList.Add(node);
            }

            NodeList.Sort();
        }

        public void Update(float deltaTime)
        {

            RefreshData();

            if (NodeList.Count < 2)
                return;
            //just use for calculating uv change.
            dummyNode.Update(deltaTime);

            //force first node to the start point
            //if (Owner.RopeFixStartPos)
            //{
            //    EffectNode firstNode = NodeList[NodeList.Count - 1];
            //    firstNode.CurWorldPos = Owner.ClientTransform.position + Owner.EmitPoint;
            //}


            ClearDeadVerts();
            UpdateVertices();
            UpdateIndices();
        }

        protected void ClearNodeVert(EffectNode node)
        {
            int baseIdx = Vertexsegment.VertStart + node.Index * 2;

            VertexPool pool = Vertexsegment.Pool;

            pool.Vertices[baseIdx] = Owner.ClientTransform.position;
            pool.Colors[baseIdx] = Color.clear;

            pool.Vertices[baseIdx + 1] = Owner.ClientTransform.position;
            pool.Colors[baseIdx + 1] = Color.clear;

            pool.VertChanged = true;
            pool.ColorChanged = true;
        }

        public void ClearDeadVerts()
        {
            for (int i = 0; i < Owner.MaxENodes; i++)
            {
                EffectNode node = Owner.AvailableENodes[i];
                if (node == null)
                    continue;
                ClearNodeVert(node);
            }

            Vertexsegment.ClearIndices();
        }

        public void UpdateIndices()
        {
            int ecount = 0;
            VertexPool pool = Vertexsegment.Pool;
            for (int i = NodeList.Count - 1; i >= 0; i--)
            {
                EffectNode node = NodeList[i];

                EffectNode nextNode = i - 1 >= 0 ? NodeList[i - 1] : null;

                if (nextNode == null)
                    break;

                int lastBaseIdx = Vertexsegment.VertStart + node.Index * 2;
                int baseIdx = Vertexsegment.VertStart + nextNode.Index * 2;
                int iidx = Vertexsegment.IndexStart + ecount * 6;

                pool.Indices[iidx + 0] = lastBaseIdx;
                pool.Indices[iidx + 1] = lastBaseIdx + 1;
                pool.Indices[iidx + 2] = baseIdx;
                pool.Indices[iidx + 3] = lastBaseIdx + 1;
                pool.Indices[iidx + 4] = baseIdx + 1;
                pool.Indices[iidx + 5] = baseIdx;

                ecount++;
            }

            pool.IndiceChanged = true;
        }

        public void UpdateVertices()
        {
            float uvSegment = 0f;
            float uvLen = 0f;

            //NOTE: ONLY USE THE DUMMY NODE'S UV CHANGE
            Vector2 LowerLeftUV = dummyNode.LowerLeftUV;
            Vector2 UVDimensions = dummyNode.UVDimensions;

            // change to lower left coord?
            UVDimensions.y = -UVDimensions.y;
            LowerLeftUV.y = 1f - LowerLeftUV.y;


            float totalUVLen = Owner.RopeUVLen;

            if (Owner.RopeFixUVLen)
            {
                float t = 0;
                for (int i = 0; i < NodeList.Count - 1; i++)
                {
                    t += (NodeList[i + 1].GetWorldPos() - NodeList[i].GetWorldPos()).magnitude;
                }
                totalUVLen = t;
            }


            for (int i = NodeList.Count - 1; i >= 0; i--)
            {
                EffectNode node = NodeList[i];

                EffectNode prevNode = i + 1 < NodeList.Count ? NodeList[i + 1] : null;

                EffectNode nextNode = i - 1 >= 0 ? NodeList[i - 1] : null;

                Vector3 chainTangent;

                if (nextNode == null)
                {
                    //tail node
                    chainTangent = node.GetWorldPos() - prevNode.GetWorldPos();
                }
                else if (prevNode == null)
                {
                    //head node
                    chainTangent = nextNode.GetWorldPos() - node.GetWorldPos();
                }
                else
                {
                    chainTangent = nextNode.GetWorldPos() - prevNode.GetWorldPos();
                }

                

                Vector3 eyePos = Owner.MyCamera.transform.position;
                Vector3 vP1ToEye = eyePos - node.GetWorldPos();

                Vector3 vPerpendicular = Vector3.Cross(chainTangent, vP1ToEye);
                vPerpendicular.Normalize();
                vPerpendicular *= (Owner.RopeWidth * 0.5f * node.Scale.x);

                //Debug.DrawRay(node.GetWorldPos(), vPerpendicular, Color.red, 1f);

                Vector3 pos0 = node.GetWorldPos() - vPerpendicular;
                Vector3 pos1 = node.GetWorldPos() + vPerpendicular;

                VertexPool pool = Vertexsegment.Pool;
                //if (Owner.StretchType == 0)
                    uvSegment = (uvLen / totalUVLen) * Mathf.Abs(UVDimensions.y);
               // else
                   // uvSegment = (uvLen / totalUVLen) * Mathf.Abs(UVDimensions.x);
                Vector2 uvCoord = Vector2.zero;
                int baseIdx = Vertexsegment.VertStart + node.Index * 2;

                pool.Vertices[baseIdx] = pos0;
                pool.Colors[baseIdx] = node.Color;
                //if (Owner.StretchType == 0)
               // {
                    uvCoord.x = LowerLeftUV.x + UVDimensions.x;
                    uvCoord.y = LowerLeftUV.y - uvSegment;
               // }
               // else
               // {
                   // uvCoord.x = LowerLeftUV.x + uvSegment;
                   // uvCoord.y = LowerLeftUV.y;
               // }
                pool.UVs[baseIdx] = uvCoord;


                //pos1
                pool.Vertices[baseIdx + 1] = pos1;
                pool.Colors[baseIdx + 1] = node.Color;
                //if (Owner.StretchType == 0)
               // {
                    uvCoord.x = LowerLeftUV.x;
                    uvCoord.y = LowerLeftUV.y - uvSegment;
               // }
                //else
                //{
                  //  uvCoord.x = LowerLeftUV.x + uvSegment;
                   // uvCoord.y = LowerLeftUV.y - Mathf.Abs(UVDimensions.y);
               // }
                pool.UVs[baseIdx + 1] = uvCoord;

                if (nextNode != null)
                    uvLen += (nextNode.GetWorldPos() - node.GetWorldPos()).magnitude;
                else
                    uvLen += (node.GetWorldPos() - prevNode.GetWorldPos()).magnitude;
            }

            Vertexsegment.Pool.UVChanged = true;
            Vertexsegment.Pool.VertChanged = true;
            Vertexsegment.Pool.ColorChanged = true;
        }

    }
}


