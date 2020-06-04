using UnityEngine;
using System.Collections;
using Xft;

//in this example, we use a 'Rope Renderer' and 'Gravity Modifier' to simulate a dynamic rope.
//about how to use rope renderer, please check the video tutorial.
public class XftRopeDemo : MonoBehaviour
{
    public XffectComponent Effect;
    public GameObject Target;


    void SetUpMagicChain()
    {
        //must call Active First!
        Effect.Active();
        //this API only affect 'Gravity Modifier'.
        Effect.SetGravityGoal(Target.transform, "line");
    }


    void OnMissileCollision()
    {

    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 350, 30), "Reset"))
        {
            SetUpMagicChain();
        }
    }
}



