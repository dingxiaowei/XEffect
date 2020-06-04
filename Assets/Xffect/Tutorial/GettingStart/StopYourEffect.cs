using UnityEngine;
using System.Collections;
using Xft;


public class StopYourEffect : MonoBehaviour
{
    public XffectComponent SampleLoop;



    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 350, 30), "Reset"))
        {
            SampleLoop.Active();
        }

        if (GUI.Button(new Rect(0, 30, 350, 30), "stop effect immediately"))
        {
            SampleLoop.DeActive();
        }

        if (GUI.Button(new Rect(0, 60, 350, 30), "stop effect softly"))
        {
            SampleLoop.StopSmoothly(1f);
        }
    }

}


