using UnityEngine;
using System.Collections;
using Xft;


public class ReleaseYourEffect : MonoBehaviour
{
    public XffectComponent YourEffect;

    public XffectCache EffectPool;



    void Start()
    {
        YourEffect.Initialize();
    }

    void InstantiateEffect()
    {
        GameObject go = Instantiate(YourEffect.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
        XffectComponent eft = go.GetComponent<XffectComponent>();
        //after you instantiate a Xffect Object, you need to call Active() to activate it
        //and when there are no active nodes in the scene, this Xffect Object will automatically become non-active
        eft.Active();
    }

    void ReleasePooledEffect(string effectName)
    {
        XffectComponent eft = EffectPool.GetEffect(effectName);
        eft.Active();
    }


    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 350, 30), "instantiate a new effect"))
        {
            InstantiateEffect();
        }

        if (GUI.Button(new Rect(0, 30, 350, 30), "instantiate effect from EffectCache"))
        {
            ReleasePooledEffect("sample_effect");
        }
    }
}


