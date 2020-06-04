using UnityEngine;
using System.Collections;
using Xft;

public class SphericalBillboardDemo : MonoBehaviour {

    public XffectComponent QuadExplode;
    public XffectComponent SphericalExplode;
    public XffectComponent QuadFogs;
    public XffectComponent SphericalFogs;


	void Start () 
    {
        //pre init here to avoid lag.
        QuadExplode.Initialize();
        SphericalExplode.Initialize();
        QuadFogs.Initialize();
        SphericalFogs.Initialize();
	}


    void ActiveEffect(XffectComponent xft)
    {
        QuadExplode.DeActive();
        SphericalExplode.DeActive();
        QuadFogs.DeActive();
        SphericalFogs.DeActive();
        xft.Active();
    }


    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 200, 30), "Spherical Explode"))
        {
            ActiveEffect(SphericalExplode);
        }

        if (GUI.Button(new Rect(0, 30, 200, 30), "QuadExplode"))
        {
            ActiveEffect(QuadExplode);
        }

        if (GUI.Button(new Rect(0, 60, 200, 30), "Spherical Fogs"))
        {
            ActiveEffect(SphericalFogs);
        }

        if (GUI.Button(new Rect(0, 90, 200, 30), "Quad Fogs"))
        {
            ActiveEffect(QuadFogs);
        }

    }

}
