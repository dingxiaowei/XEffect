using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Xft;

public class XEditorTool
{
    
    public static float LabelWidth = 130f;
	
	public static float MinHeight = 26f;
	
	public static float MinHeightBig = 29f;
    
	public static int ColorTexWidth = 512;
	public static int ColorTexHeight = 32;
	
    public enum EArea
    {
        CheckBox,
        Texture,
		AlwaysToggle,
        None,
    }
    
    public Dictionary<string, XArea> XAreas = new Dictionary<string, XArea>();
    
    public EffectLayerCustom MyEditor;
    
    public class XArea
    {
        public bool Open = false;
        public Rect LastRect;
        public bool Enable = false;
    }
    
	
	static public string GetXffectPath()
	{
		Shader temp	= Shader.Find ("Xffect/PP/radial_blur_mask");
		string assetPath = AssetDatabase.GetAssetPath(temp);
		int index = assetPath.LastIndexOf("Xffect");
		string basePath = assetPath.Substring (0,index+7);
		
		return basePath;
	}
	
	public XArea BeginXArea(string id,GUIStyle style, float minHeight,EArea type,SerializedProperty bobj)
	{
		return BeginXArea(id,style,minHeight,type,bobj,0f);
	}
	
	
    public XArea BeginXArea(string id,GUIStyle style, float minHeight,EArea type,SerializedProperty bobj, float offset)
    {
		
		GUIStyle buttonStyle = EffectLayerCustom.Xbutton;

        if (!XAreas.ContainsKey(id))
        {
            XAreas.Add(id,new XArea());
        }
        
        //find my area.
        XArea m = XAreas[id];
        style.stretchWidth = true;
        Rect gotLastRect = GUILayoutUtility.GetRect (new GUIContent (),style,GUILayout.Height (m.LastRect.height));


        if (m.Open)
        {
            buttonStyle = EffectLayerCustom.XbuttonClicked;
        }
        
        GUILayout.BeginArea (m.LastRect,style);
        Rect newRect = EditorGUILayout.BeginVertical();
        
        //head bar
        EditorGUILayout.BeginHorizontal();
        if (type == EArea.CheckBox || type == EArea.AlwaysToggle)
        {
			if (type == EArea.CheckBox)
			{
				//little trick.
				m.Enable = bobj.boolValue;
				m.Enable = GUILayout.Toggle(m.Enable,GUIContent.none,EffectLayerCustom.Xtoggle, GUILayout.Width(18f),GUILayout.Height(18f));
				bobj.boolValue = m.Enable;
			}
            else
			{
				m.Open = GUILayout.Toggle(m.Open,GUIContent.none,EffectLayerCustom.XToggle2, GUILayout.Width(18f),GUILayout.Height(18f));
				m.Enable = true;
			}

            if (GUILayout.Button(id, buttonStyle, GUILayout.Height(20f)))
            {
                MyEditor.Repaint();
                m.Open = !m.Open;
            }
                
        }
        else if(type == EArea.Texture)
        {
            
            Texture tex = null;
            string mname = "no material";
			
            if (MyEditor.Script.Material != null)
			{
				tex = MyEditor.Script.Material.mainTexture;
				mname = MyEditor.Script.Material.name;
			}
			
            GUILayout.Label(new GUIContent(tex),EffectLayerCustom.XTexture,GUILayout.Width(25f),GUILayout.Height(25f));

            if (GUILayout.Button(mname, buttonStyle, GUILayout.Height(24f)))
            {
                MyEditor.Repaint();
                m.Open = !m.Open;
            }

            m.Enable = true;
        }
        
		//increase a bit, need to do this.
		newRect.height += 3f;
		
		newRect.height += offset;

        EditorGUILayout.EndHorizontal();
        GUI.enabled = m.Enable;
        if (!m.Open)
        {
            newRect.height = minHeight;
        }
            
        //calculate area size.
        if (Event.current.type == EventType.Repaint || Event.current.type == EventType.ScrollWheel) {
         newRect.x = gotLastRect.x;
         newRect.y = gotLastRect.y;
         newRect.width = gotLastRect.width;
         newRect.height += style.padding.top+ style.padding.bottom;

         if (m.LastRect != newRect) {
                m.LastRect = newRect;
                MyEditor.Repaint ();
            }
        }
        
        return m;
    }
    
	public XArea BeginCommonArea(string id,string name,Editor editor,bool forceOpen)
    {
		GUIStyle buttonStyle = EffectLayerCustom.Xbutton;
		GUIStyle style = EffectLayerCustom.XArea;
		
        if (!XAreas.ContainsKey(id))
        {
            XAreas.Add(id,new XArea());
        }
        
        //find my area.
        XArea m = XAreas[id];
        style.stretchWidth = true;
        Rect gotLastRect = GUILayoutUtility.GetRect (new GUIContent (),style,GUILayout.Height (m.LastRect.height));
		
        GUILayout.BeginArea (m.LastRect,style);
        Rect newRect = EditorGUILayout.BeginVertical();
        
        //head bar
        EditorGUILayout.BeginHorizontal();

		m.Open = GUILayout.Toggle(m.Open,GUIContent.none,EffectLayerCustom.XToggle2, GUILayout.Width(18f),GUILayout.Height(18f));
		m.Enable = true;
		if (GUILayout.Button (name,buttonStyle,GUILayout.Height(20f)))
			m.Open = !m.Open;
		
		if (forceOpen)
			m.Open = true;
		
		//increase a bit, need to do this.
		newRect.height += 3f;
		
		
        EditorGUILayout.EndHorizontal();
        GUI.enabled = m.Enable;
        if (!m.Open)
        {
            newRect.height = MinHeight;
        }
            
        //calculate area size.
        if (Event.current.type == EventType.Repaint || Event.current.type == EventType.ScrollWheel) {
         newRect.x = gotLastRect.x;
         newRect.y = gotLastRect.y;
         newRect.width = gotLastRect.width;
         newRect.height += style.padding.top+ style.padding.bottom;

         if (m.LastRect != newRect) {
                m.LastRect = newRect;
                editor.Repaint ();
            }
        }
		
        return m;
    }

    public void EndXArea()
    {
        GUI.enabled = true;
        EditorGUILayout.EndVertical();
        GUILayout.EndArea();
    }
	
    public void DrawEnumMagType(string label, string tooltip, SerializedProperty obj)
    {
        
        GUIContent content = null;
        
        if (string.IsNullOrEmpty(tooltip))
            content = new GUIContent(label);
        else
            content = new GUIContent(label,tooltip);
        
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(content,EffectLayerCustom.XLabelField,GUILayout.Width(XEditorTool.LabelWidth));
		obj.enumValueIndex = (int)(MAGTYPE)EditorGUILayout.EnumPopup((MAGTYPE)obj.enumValueIndex);
		EditorGUILayout.EndHorizontal();
    }		
	
	
    public void DrawInt(string label, string tooltip, SerializedProperty obj)
    {
        
        GUIContent content = null;
        
        if (string.IsNullOrEmpty(tooltip))
            content = new GUIContent(label);
        else
            content = new GUIContent(label,tooltip);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(content,EffectLayerCustom.XLabelField,GUILayout.Width(LabelWidth));
        obj.intValue = EditorGUILayout.IntField(obj.intValue);
        EditorGUILayout.EndHorizontal();
    }	
    
	public void DrawVector3Field(string label, string tooltip, SerializedProperty obj)
    {
        obj.vector3Value = EditorGUILayout.Vector3Field(label,obj.vector3Value);
    }
	
	public void DrawVector2Field(string label, string tooltip, SerializedProperty obj)
    {
        obj.vector2Value = EditorGUILayout.Vector2Field(label,obj.vector2Value);
    }
	
	public void DrawCurve(string label, string tooltip, SerializedProperty obj)
	{
		DrawCurve(label,tooltip,obj,false);
	}
	
	public void DrawCurve(string label, string tooltip, SerializedProperty obj, bool limit)
    {
        
        GUIContent content = null;
        
        if (string.IsNullOrEmpty(tooltip))
            content = new GUIContent(label);
        else
            content = new GUIContent(label,tooltip);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(content,EffectLayerCustom.XLabelField,GUILayout.Width(LabelWidth));
		if (!limit)
        	obj.animationCurveValue = EditorGUILayout.CurveField(obj.animationCurveValue);
		else
			obj.animationCurveValue = EditorGUILayout.CurveField(obj.animationCurveValue,Color.green, new Rect(0f,0f,99f,1f));
        EditorGUILayout.EndHorizontal();
    }
    
    public void DrawCurve01(string label, string tooltip, SerializedProperty obj)
    {
        
        GUIContent content = null;
        
        if (string.IsNullOrEmpty(tooltip))
            content = new GUIContent(label);
        else
            content = new GUIContent(label,tooltip);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(content,EffectLayerCustom.XLabelField,GUILayout.Width(LabelWidth));
        obj.animationCurveValue = EditorGUILayout.CurveField(obj.animationCurveValue,Color.green, new Rect(0f,0f,1f,1f));
        EditorGUILayout.EndHorizontal();
    }
	
	public void DrawSlider(string label, string tooltip, SerializedProperty obj,float min, float max)
    {
        
        GUIContent content = null;
        
        if (string.IsNullOrEmpty(tooltip))
            content = new GUIContent(label);
        else
            content = new GUIContent(label,tooltip);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(content,EffectLayerCustom.XLabelField,GUILayout.Width(LabelWidth));
        obj.floatValue = EditorGUILayout.Slider(obj.floatValue,min,max);
        EditorGUILayout.EndHorizontal();
    }
	
    
    public void DrawText(string label, string tooltip, SerializedProperty obj)
    {
        
        GUIContent content = null;
        
        if (string.IsNullOrEmpty(tooltip))
            content = new GUIContent(label);
        else
            content = new GUIContent(label,tooltip);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(content,EffectLayerCustom.XLabelField,GUILayout.Width(LabelWidth));
        obj.stringValue = EditorGUILayout.TextField(obj.stringValue);
        EditorGUILayout.EndHorizontal();
    }
    
    
    public void DrawFloat(string label, string tooltip, SerializedProperty obj)
    {
        
        GUIContent content = null;
        
        if (string.IsNullOrEmpty(tooltip))
            content = new GUIContent(label);
        else
            content = new GUIContent(label,tooltip);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(content,EffectLayerCustom.XLabelField,GUILayout.Width(LabelWidth));
        obj.floatValue = EditorGUILayout.FloatField(obj.floatValue);
        EditorGUILayout.EndHorizontal();
    }
    
    public void DrawColor(string label, string tooltip, SerializedProperty obj)
    {
        
        GUIContent content = null;
        
        if (string.IsNullOrEmpty(tooltip))
            content = new GUIContent(label);
        else
            content = new GUIContent(label,tooltip);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(content,EffectLayerCustom.XLabelField,GUILayout.Width(LabelWidth));
        obj.colorValue = EditorGUILayout.ColorField(obj.colorValue);
        EditorGUILayout.EndHorizontal();
    }

    public bool DrawToggle(string label, string tooltip, SerializedProperty obj)
    {
        
        GUIContent content = null;
        
        if (string.IsNullOrEmpty(tooltip))
            content = new GUIContent(label);
        else
            content = new GUIContent(label,tooltip);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(content,EffectLayerCustom.XLabelField,GUILayout.Width(LabelWidth));
        obj.boolValue = EditorGUILayout.Toggle(obj.boolValue);
        EditorGUILayout.EndHorizontal();
		
		return obj.boolValue;
    }
    
    public void DrawTransform(string label, string tooltip, SerializedProperty obj)
    {
        
        GUIContent content = null;
        
        if (string.IsNullOrEmpty(tooltip))
            content = new GUIContent(label);
        else
            content = new GUIContent(label,tooltip);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(content,EffectLayerCustom.XLabelField,GUILayout.Width(LabelWidth));
        obj.objectReferenceValue = EditorGUILayout.ObjectField(obj.objectReferenceValue,typeof(Transform),true);
        EditorGUILayout.EndHorizontal();
    }
    
	public void DrawTexture(string label, string tooltip, SerializedProperty obj)
    {
		
        obj.objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent(label,tooltip), obj.objectReferenceValue,typeof(Texture2D),true);

    }
    
    public void DrawMaterial(string label, string tooltip, SerializedProperty obj)
    {
        
        GUIContent content = null;
        
        if (string.IsNullOrEmpty(tooltip))
            content = new GUIContent(label);
        else
            content = new GUIContent(label,tooltip);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(content,EffectLayerCustom.XLabelField,GUILayout.Width(LabelWidth));
        obj.objectReferenceValue = EditorGUILayout.ObjectField(obj.objectReferenceValue,typeof(Material),false);
        EditorGUILayout.EndHorizontal();
    }
    
    public void DrawMesh(string label, string tooltip, SerializedProperty obj)
    {
        
        GUIContent content = null;
        
        if (string.IsNullOrEmpty(tooltip))
            content = new GUIContent(label);
        else
            content = new GUIContent(label,tooltip);
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(content,EffectLayerCustom.XLabelField,GUILayout.Width(LabelWidth));
        obj.objectReferenceValue = EditorGUILayout.ObjectField(obj.objectReferenceValue,typeof(Mesh),false);
        EditorGUILayout.EndHorizontal();
    }
	
    public void DrawSeparator()
    {
        //EditorGUILayout.LabelField(GUIContent.none,EffectLayerCustom.XSeparator);
        EditorGUILayout.LabelField(GUIContent.none,EffectLayerCustom.XLabelField,GUILayout.Width(LabelWidth));
    }
	
	
	public void DrawInfo(string info)
	{
		//BeginXArea(info,EffectLayerCustom.XInfoArea,50f,EArea.None,null);
		GUILayout.Label(info,EffectLayerCustom.XInfoArea);
		//EndXArea();
	}
	
	
	
	static public void RefreshGradientTex(ref Texture2D tex, ColorParameter cp, Editor editor)
	{
        
        if (editor == null)
            return;
        
		if (tex == null)
		{
			tex =new Texture2D(ColorTexWidth,ColorTexHeight, TextureFormat.RGBA32, false);
			tex.wrapMode = TextureWrapMode.Clamp;
		}
		
		RefreshEditorGradient(tex,cp);
		
		
		//editor.Repaint();
	}
	
	
	static protected void RefreshEditorGradient (Texture2D tex, ColorParameter p)
    {
        Color col;
        for (int x = 0; x < tex.width; x++) {
            col = p.GetGradientColor(x / (float)tex.width);
            for (int y=0;y<tex.height;y++)
                tex.SetPixel(x,y,col);
        }
        tex.Apply();
    }


    static public void PatchColorGradient(ColorParameter c1, Gradient c2)
    {

        GradientColorKey[] ck = new GradientColorKey[c1.Colors.Count];
        GradientAlphaKey[] ak = new GradientAlphaKey[c1.Colors.Count];

        for (int i = 0; i < c1.Colors.Count; i++)
        {
            ColorKey k = c1.Colors[i];

            ck[i].color = k.Color;
            ck[i].time = k.t;

            ak[i].alpha = k.Color.a;
            ak[i].time = k.t;
        }

        c2.SetKeys(ck, ak);
    }

}
