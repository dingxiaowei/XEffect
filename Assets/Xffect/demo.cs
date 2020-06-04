using UnityEngine;
using System.Collections;
using Xft;

public class demo : MonoBehaviour
{
    public XffectComponent explode_energy;
    public XffectComponent ice_impact;
    public CompositeXffect suckblood;
    public CompositeXffect firestorm;
    public XffectComponent phantomsword;
    public CompositeXffect cyclonestorm;
    public XffectComponent radial_energy;
    public CompositeXffect lightning_storm;
    public XffectComponent glow_per_obj;


    //pre-init xffect objs to avoid lags when they get activated.
    void Start()
    {
        explode_energy.Initialize();
        ice_impact.Initialize();
        suckblood.Initialize();
        firestorm.Initialize();
        phantomsword.Initialize();
        cyclonestorm.Initialize();
        radial_energy.Initialize();
        lightning_storm.Initialize();
        glow_per_obj.Initialize();
    }

    void Reset()
    {
        explode_energy.DeActive();
        ice_impact.DeActive();
        suckblood.DeActive();
        firestorm.DeActive();
        phantomsword.DeActive();
        cyclonestorm.DeActive();
        radial_energy.DeActive();
        lightning_storm.DeActive();
        glow_per_obj.DeActive();
    }

    void OnGUI()
    {

        GUI.Label(new Rect(60, 0, 500, 30), "Requires unity pro to get the best result.");

        if (GUI.Button(new Rect(0, 0, 50, 30), "1"))
        {
            explode_energy.Active();
        }
        if (GUI.Button(new Rect(0, 30, 50, 30), "2"))
        {
            ice_impact.Active();
        }

        if (GUI.Button(new Rect(0, 60, 50, 30), "3"))
        {
            suckblood.Active();
        }

        if (GUI.Button(new Rect(0, 90, 50, 30), "4"))
        {
            firestorm.Active();
        }

        if (GUI.Button(new Rect(0, 120, 50, 30), "5"))
        {
            phantomsword.Active();
        }

        if (GUI.Button(new Rect(0, 150, 50, 30), "6"))
        {
            cyclonestorm.Active();
        }

        if (GUI.Button(new Rect(0, 180, 50, 30), "7"))
        {
            radial_energy.Active();
        }

        if (GUI.Button(new Rect(0, 210, 50, 30), "8"))
        {
            lightning_storm.Active();
        }

        if (GUI.Button(new Rect(0, 240, 50, 30), "9"))
        {
            glow_per_obj.Active();
        }

        if (GUI.Button(new Rect(0, 270, 50, 30), "Reset"))
        {
            Reset();
        }
    }
}