using UnityEngine;
using System.Collections;
using Xft;

public class GravityDemo : MonoBehaviour
{
    public XffectComponent Effect;
    public GameObject Target;
    public XffectCache EffectCache;


    void SetUpMissile()
    {
        //must call Active() first!
        Effect.Active();
        Effect.SetCollisionGoalPos(Target.transform, "soul");
        Effect.SetGravityGoal(Target.transform, "soul");
    }


    //collision callback, here we release some explode effect at the collison pos.
    void OnMissileCollision(Xft.CollisionParam param)
    {
        XffectComponent eft = EffectCache.GetEffect("explode");
        eft.transform.position = param.CollidePos;
        eft.Active();
    }


    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 350, 30), "Fire!"))
        {
            SetUpMissile();
        }
    }
}


