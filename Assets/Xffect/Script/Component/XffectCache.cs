//----------------------------------------------
//            Xffect Editor
// Copyright © 2012- Shallway Studio
// http://shallway.net
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Xft
{
    [AddComponentMenu("Xffect/XffectCache")]
    public class XffectCache : MonoBehaviour
    {

        #region local methods
        Dictionary<string, List<XffectComponent>> EffectDic = new Dictionary<string, List<XffectComponent>>();
        Dictionary<string, List<CompositeXffect>> CompEffectDic = new Dictionary<string, List<CompositeXffect>>();

        protected bool mInited = false;

        void Awake()
        {
            mInited = false;
            Init();
        }

        public bool IsInited()
        {
            return mInited;
        }

        public void Init()
        {
            foreach (Transform child in transform)
            {
                XffectComponent xft = child.GetComponent<XffectComponent>();
                if (xft != null)
                {
                    //make sure all children is inited.
                    xft.Initialize();
                    if (!EffectDic.ContainsKey(child.name))
                        EffectDic[child.name] = new List<XffectComponent>();
                    EffectDic[child.name].Add(xft);
                }
                CompositeXffect cxft = child.GetComponent<CompositeXffect>();
                if (cxft != null)
                {
                    cxft.Initialize();
                    if (!CompEffectDic.ContainsKey(child.name))
                        CompEffectDic[child.name] = new List<CompositeXffect>();
                    CompEffectDic[child.name].Add(cxft);
                }
            }
            mInited = true;
        }

        public XffectComponent AddXffect(string name)
        {
            Transform baseobj = transform.Find(name);
            if (baseobj == null)
            {
                Debug.Log("object:" + name + "doesn't exist!");
                return null;
            }
            Transform newobj = Instantiate(baseobj, Vector3.zero, Quaternion.identity) as Transform;
            newobj.parent = transform;
            XffectComponent.SetActive(newobj.gameObject, false);

            newobj.gameObject.name = baseobj.gameObject.name;

            XffectComponent xft = newobj.GetComponent<XffectComponent>();
            EffectDic[name].Add(xft);
            if (xft != null)
                xft.Initialize();
            return xft;
        }
        #endregion

        #region API
        public bool ContainsEffect(string name)
        {
            return EffectDic.ContainsKey(name);
        }
        public XffectComponent GetEffect(string name)
        {

            if (!EffectDic.ContainsKey(name))
            {
                Debug.LogError("there is no effect:" + name + " in effect cache!");
                return null;
            }
            List<XffectComponent> cache = EffectDic[name];
            if (cache == null)
            {
                return null;
            }

            foreach (XffectComponent xft in cache)
            {
                //IMPORTANT: xft might be destroyed if you used it in other parent. always check it.
                if (xft == null)
                {
                    continue;
                }

                if (!XffectComponent.IsActive(xft.gameObject))
                {
                    return xft;
                }
            }
            return AddXffect(name);
        }
        #endregion

        #region non-useful API

        //not support auto extend.
        public CompositeXffect GetCompositeXffect(string name)
        {
            List<CompositeXffect> cache = CompEffectDic[name];
            if (cache == null)
            {
                return null;
            }
            foreach (CompositeXffect xftc in cache)
            {
                if (!XffectComponent.IsActive(xftc.gameObject))
                {
                    return xftc;
                }
            }
            return null;
        }

        //manually emit effect node by expected position.
        public EffectNode EmitNode(string eftName, Vector3 pos)
        {
            List<XffectComponent> cache = EffectDic[eftName];
            if (cache == null)
            {
                Debug.LogError(name + ": cache doesnt exist!");
                return null;
            }
            if (cache.Count > 1)
            {
                Debug.LogWarning("EmitNode() only support only-one xffect cache!");
            }
            XffectComponent xft = (XffectComponent)cache[0];
            if (!XffectComponent.IsActive(xft.gameObject))
                xft.Active();

            EffectNode ret = xft.EmitByPos(pos);

            if (ret == null)
                Debug.LogError("emit node error! may be node max count is not enough:" + eftName);

            return ret;
        }

        //release effect by world position. in this case, the client transform's position should be zero.
        //notice: if the xffect is not auto-deactive, you should remember to manually deactive it.
        //** automaticaly set client to EffectCache, so you should make sure EffectCache's position is always zero.
        public XffectComponent ReleaseEffect(string name, Vector3 pos)
        {
            XffectComponent xft = GetEffect(name);
            if (xft == null)
            {
                Debug.LogWarning("can't find available effect in cache!:" + name);
                return null;
            }
            xft.Active();
            xft.SetClient(transform);
            xft.SetEmitPosition(pos);
            return xft;
        }

        //release effect with a new client transform.
        //note: if the xffect is not auto-deactive, you should remember to manually deactive it.
        public XffectComponent ReleaseEffect(string name, Transform client)
        {
            XffectComponent xft = GetEffect(name);
            if (xft == null)
            {
                Debug.LogWarning("can't find available effect in cache!:" + name);
                return null;
            }
            xft.Active();
            xft.SetClient(client);
            return xft;
        }

        public XffectComponent ReleaseEffect(string name)
        {
            XffectComponent xft = GetEffect(name);
            if (xft == null)
            {
                //Debug.LogWarning("can't find available effect in cache!:" + name);
                return null;
            }
            xft.Active();
            return xft;
        }

        public void StopEffect(string eftName, float fadeTime)
        {
            List<XffectComponent> cache = EffectDic[eftName];
            if (cache == null)
            {
                Debug.LogError(name + ": cache doesnt exist!");
                return;
            }

            for (int i = 0; i < cache.Count; i++)
            {
                XffectComponent xft = cache[i];
                if (!XffectComponent.IsActive(xft.gameObject))
                    continue;
                xft.StopSmoothly(fadeTime);
            }
        }

        public void DeActiveEffect(string eftName)
        {
            List<XffectComponent> cache = EffectDic[eftName];
            if (cache == null)
            {
                Debug.LogError(name + ": cache doesnt exist!");
                return;
            }
            //if (cache.Count > 1)
            //{
            //Debug.LogWarning("DeActive() only support one Xffect cache!");
            //}

            for (int i = 0; i < cache.Count; i++)
            {
                XffectComponent xft = cache[i];
                if (!XffectComponent.IsActive(xft.gameObject))
                    continue;
                xft.DeActive();
            }
        }
        //for only-one effect
        public bool IsEffectActive(string eftName)
        {
            List<XffectComponent> cache = EffectDic[eftName];
            if (cache == null)
            {
                Debug.LogError(name + ": cache doesnt exist!");
                return true;
            }
            if (cache.Count > 1)
            {
                Debug.LogWarning("DeActive() only support one Xffect cache!");
            }
            XffectComponent xft = cache[0];

            return XffectComponent.IsActive(xft.gameObject);
        }

        //deactive the effect smoothly.
        public void StopEffect(string eftName)
        {
            List<XffectComponent> cache = EffectDic[eftName];
            if (cache == null)
            {
                Debug.LogError(name + ": cache doesnt exist!");
                return;
            }
            if (cache.Count > 1)
            {
                Debug.LogWarning("DeActive() only support one Xffect cache!");
            }
            XffectComponent xft = cache[0];
            if (!XffectComponent.IsActive(xft.gameObject))
                return;
            xft.StopEmit();
        }


        public int GetAvailableEffectCount(string eftName)
        {
            List<XffectComponent> cache = EffectDic[eftName];
            if (cache == null)
                return 0;
            int ret = 0;
            for (int i = 0; i < cache.Count; i++)
            {
                XffectComponent xftcomp = cache[i];
                if (xftcomp == null)
                    continue;
                if (!XffectComponent.IsActive(xftcomp.gameObject))
                    ret++;
            }

            return ret;
        }

        #endregion
    }
}