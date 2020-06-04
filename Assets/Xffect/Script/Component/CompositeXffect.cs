using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Xft
{
    public class CompositeXffect : MonoBehaviour
    {

        List<XffectComponent> XffectList = new List<XffectComponent>();
        void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            foreach (Transform child in transform)
            {
                XffectComponent xft = child.GetComponent<XffectComponent>();
                if (xft == null)
                {
                    XffectCache xc = child.GetComponent<XffectCache>();
                    if (xc != null)
                        xc.Init();
                    continue;
                }
                    
                xft.Initialize();
                XffectList.Add(xft);
            }
        }

        public void Active()
        {
            gameObject.SetActive(true);
            foreach (XffectComponent xft in XffectList)
            {
                xft.Active();
            }
        }

        public void DeActive()
        {
            gameObject.SetActive(false);
            foreach (XffectComponent xft in XffectList)
            {
                xft.DeActive();
            }
        }

        //void LateUpdate()
        //{
        //    for (int i = 0; i < XffectList.Count; i++)
        //    {
        //        XffectComponent xft = (XffectComponent)XffectList[i];
        //        if (XffectComponent.IsActive(xft.gameObject))
        //            return;
        //    }
        //    //if all children is done
        //    DeActive();
        //}
    }
}

