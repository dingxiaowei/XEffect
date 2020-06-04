using UnityEngine;
using System.Collections;
using Xft;

//IMPRTANT NOTE:
//To make your custom line work properly, you need to ensure these things:
//1, make sure the line sprite's height is exactly 1.0f.
//2, make sure the XffectComponent's scale option is exactly 1.0f.
//and if you want a more realistic magic chain, please check the rope renderer tutorial.

[ExecuteInEditMode]
public class XftMagicChainDemo : MonoBehaviour 
{
    public XffectComponent MagicChain;
    public Transform Target;
    
    void Update()
    {
        //calculate the distance to target
        float distance = (Target.transform.position - MagicChain.transform.position).magnitude;
        
        //set corresponding scale of MagicChain
        Vector2 scale = new Vector2(1f,distance);
        //API SetScale()
        MagicChain.SetScale(scale,"line");
        
        //adjust rotation
        Vector3 direction = Target.transform.position - MagicChain.transform.position;
        MagicChain.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }
    
    
    void OnGUI()
    {
        GUI.Label(new Rect(150, 0, 400, 25), "move around Sphere2 in the editor scene to see how it works!");
    }
}
