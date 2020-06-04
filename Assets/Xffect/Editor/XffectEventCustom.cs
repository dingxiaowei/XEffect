using UnityEngine;
using System.Collections;
using UnityEditor;
using Xft;

[CustomEditor(typeof(XftEventComponent))]
public class XffectEventCustom : Editor 
{
	
	public XftEventComponent Script;
	
	protected XEditorTool mXEditor;
	
	public XEditorTool XEditor
	{
		get{
			if (mXEditor == null)
			{
				mXEditor = new XEditorTool();
			}
			return mXEditor;
		}
	}
	
	protected SerializedProperty Type;
    protected SerializedProperty StartTime;
    protected SerializedProperty EndTime;
	
	protected SerializedProperty CameraEffectType;
    protected SerializedProperty CameraEffectPriority;

    protected SerializedProperty RadialBlurObj;
    protected SerializedProperty RBSampleDist;
	protected SerializedProperty RBStrengthType;
	protected SerializedProperty RBSampleStrength;
	protected SerializedProperty RBSampleStrengthCurve;
    protected SerializedProperty RBSampleStrengthCurveX;
	
	
    protected SerializedProperty RadialBlurMask;
    protected SerializedProperty RBMaskSampleDist;
    protected SerializedProperty RBMaskStrengthType;
    protected SerializedProperty RBMaskSampleStrength;
    protected SerializedProperty RBMaskSampleStrengthCurve;
    protected SerializedProperty RBMaskSampleStrengthCurveX;
	
	protected SerializedProperty GlowIntensity;
	protected SerializedProperty GlowBlurIterations;
	protected SerializedProperty GlowBlurSpread;
	protected SerializedProperty GlowColorStart;
	protected SerializedProperty GlowColorEnd;
    protected SerializedProperty ColorCurve;
	
	
	//public Shader ColorInverseShader;
    protected SerializedProperty CIStrengthCurve;
    
    
    protected SerializedProperty GlitchShader;
    protected SerializedProperty GlitchMask;
    protected SerializedProperty MinAmp;
    protected SerializedProperty MaxAmp;
    protected SerializedProperty MinRand;
    protected SerializedProperty MaxRand;
    protected SerializedProperty WaveLen;
    
	
	protected SerializedProperty PositionForce;
	protected SerializedProperty RotationForce;
	protected SerializedProperty PositionStifness;
	protected SerializedProperty PositionDamping;
	protected SerializedProperty RotationStiffness;
	protected SerializedProperty RotationDamping;
	protected SerializedProperty UseEarthQuake; string TUseEarthQuake = "whether use a constant force or not.";
	protected SerializedProperty EarthQuakeMagnitude;
	protected SerializedProperty EarthQuakeMagTye;
	protected SerializedProperty EarthQuakeMagCurve;
	protected SerializedProperty EarthQuakeTime;
	protected SerializedProperty EarthQuakeCameraRollFactor;
    protected SerializedProperty EarthQuakeMagCurveX;
    protected SerializedProperty PositionCurve;
    protected SerializedProperty RotationCurve;
    protected SerializedProperty CameraShakeType;
    protected SerializedProperty ShakeCurveTime;
	
	protected SerializedProperty Clip;
	protected SerializedProperty Volume;
	protected SerializedProperty Pitch;
    protected SerializedProperty IsSoundLoop;
	
	
	protected SerializedProperty LightComp;
	protected SerializedProperty LightIntensity;
	protected SerializedProperty LightIntensityType;
	protected SerializedProperty LightIntensityCurve;
    protected SerializedProperty LightIntensityCurveX;
	protected SerializedProperty LightRange;
	protected SerializedProperty LightRangeType;
	protected SerializedProperty LightRangeCurve;
    protected SerializedProperty LightRangeCurveX;
	
	
	protected SerializedProperty TimeScale;
    protected SerializedProperty TimeScaleDuration;
	
	void InitSerializedProperty()
	{
		Type = serializedObject.FindProperty("Type");
		StartTime = serializedObject.FindProperty("StartTime");
		EndTime = serializedObject.FindProperty("EndTime");
		
		CameraEffectType = serializedObject.FindProperty("CameraEffectType");
        CameraEffectPriority = serializedObject.FindProperty("CameraEffectPriority");
		RadialBlurObj = serializedObject.FindProperty("RadialBlurObj");
		RBSampleDist = serializedObject.FindProperty("RBSampleDist");
		RBStrengthType = serializedObject.FindProperty("RBStrengthType");
		RBSampleStrength = serializedObject.FindProperty("RBSampleStrength");
		RBSampleStrengthCurve = serializedObject.FindProperty("RBSampleStrengthCurve");
        RBSampleStrengthCurveX = serializedObject.FindProperty("RBSampleStrengthCurveX");

		RadialBlurMask = serializedObject.FindProperty("RadialBlurMask");
		RBMaskSampleDist = serializedObject.FindProperty("RBMaskSampleDist");
		RBMaskStrengthType = serializedObject.FindProperty("RBMaskStrengthType");
		RBMaskSampleStrength = serializedObject.FindProperty("RBMaskSampleStrength");
		RBMaskSampleStrengthCurve = serializedObject.FindProperty("RBMaskSampleStrengthCurve");
        RBMaskSampleStrengthCurveX = serializedObject.FindProperty("RBMaskSampleStrengthCurveX");

		GlowIntensity = serializedObject.FindProperty("GlowIntensity");
		GlowBlurIterations = serializedObject.FindProperty("GlowBlurIterations");
		GlowBlurSpread = serializedObject.FindProperty("GlowBlurSpread");
		GlowColorStart = serializedObject.FindProperty("GlowColorStart");
		GlowColorEnd = serializedObject.FindProperty("GlowColorEnd");
		ColorCurve = serializedObject.FindProperty("ColorCurve");
		
		CIStrengthCurve = serializedObject.FindProperty("CIStrengthCurve");
		
		PositionForce = serializedObject.FindProperty("PositionForce");
		RotationForce = serializedObject.FindProperty("RotationForce");
		PositionStifness = serializedObject.FindProperty("PositionStifness");
		PositionDamping = serializedObject.FindProperty("PositionDamping");
		RotationStiffness = serializedObject.FindProperty("RotationStiffness");
		RotationDamping = serializedObject.FindProperty("RotationDamping");
		UseEarthQuake = serializedObject.FindProperty("UseEarthQuake");
		EarthQuakeMagnitude = serializedObject.FindProperty("EarthQuakeMagnitude");
		EarthQuakeMagTye = serializedObject.FindProperty("EarthQuakeMagTye");
		EarthQuakeMagCurve = serializedObject.FindProperty("EarthQuakeMagCurve");
		EarthQuakeTime = serializedObject.FindProperty("EarthQuakeTime");
		EarthQuakeCameraRollFactor = serializedObject.FindProperty("EarthQuakeCameraRollFactor");
        EarthQuakeMagCurveX = serializedObject.FindProperty("EarthQuakeMagCurveX");

		Clip = serializedObject.FindProperty("Clip");
		Volume = serializedObject.FindProperty("Volume");
		Pitch = serializedObject.FindProperty("Pitch");
        IsSoundLoop = serializedObject.FindProperty("IsSoundLoop");
		
		LightComp = serializedObject.FindProperty("LightComp");
		LightIntensity = serializedObject.FindProperty("LightIntensity");
		LightIntensityType = serializedObject.FindProperty("LightIntensityType");
		LightIntensityCurve = serializedObject.FindProperty("LightIntensityCurve");
		LightRange = serializedObject.FindProperty("LightRange");
		LightRangeType = serializedObject.FindProperty("LightRangeType");
		LightRangeCurve = serializedObject.FindProperty("LightRangeCurve");
        LightIntensityCurveX = serializedObject.FindProperty("LightIntensityCurveX");
        LightRangeCurveX = serializedObject.FindProperty("LightRangeCurveX");
		
		TimeScale = serializedObject.FindProperty("TimeScale");
		TimeScaleDuration = serializedObject.FindProperty("TimeScaleDuration");
        
        GlitchShader = serializedObject.FindProperty("GlitchShader");
        GlitchMask = serializedObject.FindProperty("GlitchMask");
        MinAmp = serializedObject.FindProperty("MinAmp");
        MaxAmp = serializedObject.FindProperty("MaxAmp");
        MinRand = serializedObject.FindProperty("MinRand");
        MaxRand = serializedObject.FindProperty("MaxRand");
        WaveLen = serializedObject.FindProperty("WaveLen");

        PositionCurve = serializedObject.FindProperty("PositionCurve");
        RotationCurve = serializedObject.FindProperty("RotationCurve");
        CameraShakeType = serializedObject.FindProperty("CameraShakeType");
        ShakeCurveTime = serializedObject.FindProperty("ShakeCurveTime");
	}
	
	void LoadStyle()
	{
		if (EffectLayerCustom.IsSkinLoaded)
			return;
		EffectLayerCustom.LoadStyle();
	}
	
	
	void OnEnable()
	{
		Script = target as XftEventComponent;
		InitSerializedProperty();
		LoadStyle();
	}
	
	
	void DrawTimeScale()
	{
		XEditor.BeginCommonArea("event timrescale","Time Scale",this,true);
		
		XEditor.DrawFloat("time scale:","",TimeScale);
		XEditor.DrawFloat("duration:","",TimeScaleDuration);
		
		XEditor.EndXArea();
	}
	
	void DrawLight()
	{
		
		XEditor.BeginCommonArea("event light","Light",this,true);
		
		LightComp.objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent("light:",""),LightComp.objectReferenceValue,typeof(Light),true);
		
		XEditor.DrawEnumMagType("intensity type:","",LightIntensityType);
		
		if ((MAGTYPE)LightIntensityType.enumValueIndex == MAGTYPE.Fixed)
		{
			XEditor.DrawFloat("intensity:","",LightIntensity);
		}
        else if ((MAGTYPE)LightIntensityType.enumValueIndex == MAGTYPE.Curve_OBSOLETE)
        {
            XEditor.DrawCurve("intensity curve:", "", LightIntensityCurve);
        }
        else
        {
            DrawCurveX(LightIntensityCurveX, "time length:");
        }
		
		XEditor.DrawSeparator();
		
		XEditor.DrawEnumMagType("range type:","",LightRangeType);
		if ((MAGTYPE)LightRangeType.enumValueIndex == MAGTYPE.Fixed)
		{
			XEditor.DrawFloat("range:","",LightRange);
		}
        else if ((MAGTYPE)LightRangeType.enumValueIndex == MAGTYPE.Curve_OBSOLETE)
        {
            XEditor.DrawCurve("range curve:", "", LightRangeCurve);
        }
        else
        {
            DrawCurveX(LightRangeCurveX, "time length:");
        }
		
		XEditor.EndXArea();
	}
	
	void DrawSound()
	{
		XEditor.BeginCommonArea("event sound","Sound",this,true);

		Clip.objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent("audio clip:",""),Clip.objectReferenceValue,typeof(AudioClip),false);
		
		XEditor.DrawFloat("volume:","",Volume);
		XEditor.DrawFloat("pitch:","",Pitch);
		
        XEditor.DrawToggle("looping?","",IsSoundLoop);
        
		XEditor.EndXArea();
	}
	
	void DrawCameraShake()
	{
		XEditor.BeginCommonArea("event camera shake","Camera Shake",this,true);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("camera shake type:", EffectLayerCustom.XLabelField, GUILayout.Width(XEditorTool.LabelWidth));
        CameraShakeType.enumValueIndex = (int)(XCameraShakeType)EditorGUILayout.EnumPopup((XCameraShakeType)CameraShakeType.enumValueIndex);
        EditorGUILayout.EndHorizontal();


        XEditor.DrawVector3Field("position force:", "", PositionForce);
        XEditor.DrawVector3Field("rotation force:", "", RotationForce);
        XEditor.DrawSeparator();

        if ((XCameraShakeType)CameraShakeType.enumValueIndex == XCameraShakeType.Curve)
        {

            XEditor.DrawInfo("Note: the curve value will be mapped to [-1,1]\nso you may need to set the curve's start-key and end-key to 0.5f, or the camera will not shake back to the original state.");

            XEditor.DrawFloat("time len:", "", ShakeCurveTime);
            PositionCurve.animationCurveValue = EditorGUILayout.CurveField("position curve:", PositionCurve.animationCurveValue, Color.green, new Rect(0f, 0f, 1f, 1f));
            RotationCurve.animationCurveValue = EditorGUILayout.CurveField("rotation curve:", RotationCurve.animationCurveValue, Color.green, new Rect(0f, 0f, 1f, 1f));
        }
        else
        {
            XEditor.DrawSlider("position stiffness", "", PositionStifness, 0f, 1f);

            XEditor.DrawSlider("position damping", "", PositionDamping, 0f, 1f);

            XEditor.DrawSeparator();

            XEditor.DrawSlider("rotation stiffness", "", RotationStiffness, 0f, 1f);
            XEditor.DrawSlider("rotation damping", "", RotationDamping, 0f, 1f);

            XEditor.DrawSeparator();


            if (XEditor.DrawToggle("use earthquake?", TUseEarthQuake, UseEarthQuake))
            {
                XEditor.DrawFloat("duration:", "", EarthQuakeTime);
                XEditor.DrawFloat("roll factor:", "", EarthQuakeCameraRollFactor);
                XEditor.DrawEnumMagType("magnitude type:", "", EarthQuakeMagTye);

                if ((MAGTYPE)EarthQuakeMagTye.enumValueIndex == MAGTYPE.Fixed)
                {
                    XEditor.DrawFloat("magnitude:", "", EarthQuakeMagnitude);
                }
                else if ((MAGTYPE)EarthQuakeMagTye.enumValueIndex == MAGTYPE.Curve_OBSOLETE)
                {
                    XEditor.DrawCurve("magnitude curve:", "", EarthQuakeMagCurve);
                }
                else
                {
                    DrawCurveX(EarthQuakeMagCurveX, "time length:");
                }
            }
        }

		XEditor.EndXArea();
	}


    void DrawCurveX(SerializedProperty curve, string tinfo)
    {
        SerializedProperty MaxValue = curve.FindPropertyRelative("MaxValue");
        SerializedProperty TimeLen = curve.FindPropertyRelative("TimeLen");
        SerializedProperty WrapType = curve.FindPropertyRelative("WrapType");
        SerializedProperty Curve01 = curve.FindPropertyRelative("Curve01");

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("wrap mode:", ""), EffectLayerCustom.XLabelField, GUILayout.Width(XEditorTool.LabelWidth));
        WrapType.enumValueIndex = (int)(WRAP_TYPE)EditorGUILayout.EnumPopup((WRAP_TYPE)WrapType.enumValueIndex);
        EditorGUILayout.EndHorizontal();

        XEditor.DrawFloat("time length", "", TimeLen);
        XEditor.DrawFloat("max value:", "", MaxValue);
        XEditor.DrawCurve01("curve:", "", Curve01);

    }
	void DrawColorInverse()
	{
		if (Script.ColorInverseShader == null)
		{
			Script.ColorInverseShader = Shader.Find ("Xffect/PP/color_inverse");
		}
		
		XEditor.DrawCurve("strength curve:","",CIStrengthCurve,true);
		
	}
	
	void DrawCheckGlowPerObjMaterial()
	{
		
		if ((CameraEffectEvent.EType)CameraEffectType.enumValueIndex != CameraEffectEvent.EType.GlowPerObj)
			return;
		
		Transform parent = Script.transform.parent;
		
		if (parent.gameObject.GetComponent<XffectComponent>() == null)
			return;
		
		foreach(Transform child in parent)
		{
			EffectLayer el = child.gameObject.GetComponent<EffectLayer>();
			if (el == null || el.Material == null)
				continue;
			if (el.Material.shader.name.Contains("glow_per_obj"))
			{
				return;
			}
				
		}
		//come to here means no replacement shader find.
		
		XEditor.DrawInfo("can't find GlowPerObj shader in this Xffect Obj, you might need to assign a shader in 'Xffect/PP/glow_per_obj' category to one of this obj's EffectLayer.");
	}
	
	void DrawGlow()
	{
		
		DrawCheckGlowPerObjMaterial();
		if (Script.GlowCompositeShader == null)
			Script.GlowCompositeShader = Shader.Find ("Xffect/PP/glow_compose");
		if (Script.GlowDownSampleShader == null)
			Script.GlowDownSampleShader = Shader.Find ("Xffect/PP/glow_downsample");
		if (Script.GlowBlurShader == null)
			Script.GlowBlurShader = Shader.Find ("Xffect/PP/glow_conetap");
		
		XEditor.DrawFloat("intensity:","",GlowIntensity);
		XEditor.DrawInt("blur iterations:","",GlowBlurIterations);
		XEditor.DrawSlider("blur spread","",GlowBlurSpread,0.5f,1f);
		
		XEditor.DrawSeparator();
		
		XEditor.DrawColor("color start:","",GlowColorStart);
		XEditor.DrawColor("color end:","",GlowColorEnd);
		
		XEditor.DrawCurve("change curve:","",ColorCurve,true);
	}
	
    
    void DrawGlitch()
    {
        if (Script.GlitchShader == null)
        {
            Script.GlitchShader = Shader.Find ("Xffect/PP/glitch");
        }
        string assetPath = AssetDatabase.GetAssetPath(Script.GlitchShader);
        
        int index = assetPath.LastIndexOf("Xffect");
        string basePath = assetPath.Substring (0,index+7);
        string texPath = basePath + "Resources/PostProcess/Textures/Noise.png";
        
        if (Script.GlitchMask == null)
            Script.GlitchMask = (Texture2D)AssetDatabase.LoadAssetAtPath(texPath,typeof(Texture2D));
        XEditor.DrawTexture("mask:","",GlitchMask);
        
        XEditor.DrawSlider("amplitude min:","",MinAmp,0f,1f);
        XEditor.DrawSlider("amplitude max:","",MaxAmp,0f,1f);
        
        XEditor.DrawSeparator();
        
        XEditor.DrawSlider("random min:","",MinRand,0f,1f);
        XEditor.DrawSlider("random max:","",MaxRand,0f,1f);
        
        XEditor.DrawInt("wave len:","",WaveLen);
        
        
    }
    
	void DrawRadialBlurMask()
	{
		if (Script.RadialBlurTexAddShader == null)
		{
			Script.RadialBlurTexAddShader = Shader.Find ("Xffect/PP/radial_blur_mask");
		}

		string assetPath = AssetDatabase.GetAssetPath(Script.RadialBlurTexAddShader);
		int index = assetPath.LastIndexOf("Xffect");
		string basePath = assetPath.Substring (0,index+7);
		string texPath = basePath + "Resources/PostProcess/Textures/radial_mask1.psd";
		
		if (Script.RadialBlurMask == null)
			Script.RadialBlurMask = (Texture2D)AssetDatabase.LoadAssetAtPath(texPath,typeof(Texture2D));
		
		XEditor.DrawTexture("mask:","",RadialBlurMask);
		
		XEditor.DrawFloat("sample dist:","",RBMaskSampleDist);
		
		XEditor.DrawEnumMagType("magnitude type:","",RBMaskStrengthType);
		if ((MAGTYPE)RBMaskStrengthType.enumValueIndex == MAGTYPE.Fixed)
		{
			XEditor.DrawFloat("magnitude:","",RBMaskSampleStrength);
		}
        else if ((MAGTYPE)RBMaskStrengthType.enumValueIndex == MAGTYPE.Curve_OBSOLETE)
        {
            XEditor.DrawCurve("magnitude curve:", "", RBMaskSampleStrengthCurve);
        }
        else
        {
            DrawCurveX(RBMaskSampleStrengthCurveX, "time length:");
        }
	}
	
	void DrawRadialBlur()
	{
		
		if (Script.RadialBlurShader == null)
		{
			Script.RadialBlurShader = Shader.Find ("Xffect/PP/radial_blur");
		}
		
		XEditor.DrawTransform("center:","",RadialBlurObj);
		
		XEditor.DrawFloat("sample dist:","",RBSampleDist);
		
		XEditor.DrawEnumMagType("magnitude type:","",RBStrengthType);
		
		if ((MAGTYPE)RBStrengthType.enumValueIndex == MAGTYPE.Fixed)
		{
			XEditor.DrawFloat("magnitude:","",RBSampleStrength);
		}
        else if ((MAGTYPE)RBStrengthType.enumValueIndex == MAGTYPE.Curve_OBSOLETE)
        {
            XEditor.DrawCurve("magnitude curve:", "", RBSampleStrengthCurve);
        }
        else
        {
            DrawCurveX(RBSampleStrengthCurveX, "time length");
        }
	}
	void DrawCameraEffectConfig()
	{
		XEditor.BeginCommonArea("event camera effect","Camera Effect",this,true);
		
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("effet type:",EffectLayerCustom.XLabelField,GUILayout.Width(XEditorTool.LabelWidth));
        CameraEffectType.enumValueIndex = (int)(CameraEffectEvent.EType)EditorGUILayout.EnumPopup((CameraEffectEvent.EType)CameraEffectType.enumValueIndex);
        EditorGUILayout.EndHorizontal();

        XEditor.DrawInt("priority:", "", CameraEffectPriority);

        XEditor.DrawSeparator();

		if ((CameraEffectEvent.EType)CameraEffectType.enumValueIndex == CameraEffectEvent.EType.RadialBlur)
		{
			DrawRadialBlur();
		}
		else if ((CameraEffectEvent.EType)CameraEffectType.enumValueIndex == CameraEffectEvent.EType.RadialBlurMask)
		{
			DrawRadialBlurMask();
		}
		else if ((CameraEffectEvent.EType)CameraEffectType.enumValueIndex == CameraEffectEvent.EType.Glow
			|| (CameraEffectEvent.EType)CameraEffectType.enumValueIndex == CameraEffectEvent.EType.GlowPerObj)
		{
			DrawGlow();
		}
		else if ((CameraEffectEvent.EType)CameraEffectType.enumValueIndex == CameraEffectEvent.EType.ColorInverse)
		{
			DrawColorInverse();
		}
        else if ((CameraEffectEvent.EType)CameraEffectType.enumValueIndex == CameraEffectEvent.EType.Glitch)
        {
            DrawGlitch();
        }
		XEditor.EndXArea();
	}
	
	
	void DrawMainConfig()
	{
		XEditor.BeginCommonArea("event main","Event",this,true);
		
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("event type:",EffectLayerCustom.XLabelField,GUILayout.Width(XEditorTool.LabelWidth));
        Type.enumValueIndex = (int)(XEventType)EditorGUILayout.EnumPopup((XEventType)Type.enumValueIndex);
        EditorGUILayout.EndHorizontal();
		
		
		XEditor.DrawFloat("start time:","",StartTime);
		XEditor.DrawFloat("duration(-1 infinite):","",EndTime);
		
		XEditor.EndXArea();
	}
	
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		
		DrawMainConfig();
		
		if ((XEventType)Type.enumValueIndex == XEventType.CameraEffect)
		{
			DrawCameraEffectConfig();
		}
		else if ((XEventType)Type.enumValueIndex == XEventType.CameraShake)
		{
			DrawCameraShake();
		}
		else if ((XEventType)Type.enumValueIndex == XEventType.Sound)
		{
			 DrawSound();
		}
		else if ((XEventType)Type.enumValueIndex == XEventType.Light)
		{
			DrawLight();
		}
		else if ((XEventType)Type.enumValueIndex == XEventType.TimeScale)
		{
			DrawTimeScale();
		}
		serializedObject.ApplyModifiedProperties();
	}
	
	/*public static bool DisplayRadialBlurConfig = false;
    public static bool DisplayRadialBlurTexAddConfig = false;
	public static bool DisplayGlowConfig = false;
    public static bool DisplayColorInverseConfig = false;
    public static bool DisplayTimeScaleConfig = false;
	public static bool DisplaySoundConfig = false;
	public static bool DisplayCameraShakeConfig = false;
	public static bool DisplayLightConfig = false;
	
	public override void OnInspectorGUI()
	{
		XftEventComponent ctarget = (XftEventComponent)target;
		EditorGUILayout.BeginVertical();
		
		ctarget.EventType = (XftEventType)EditorGUILayout.EnumPopup("event type:", ctarget.EventType);
		
		ctarget.StartTime = EditorGUILayout.FloatField("start time:", ctarget.StartTime);
		ctarget.EndTime = EditorGUILayout.FloatField("end time:", ctarget.EndTime);
		
		switch (ctarget.EventType)
		{
		case XftEventType.CameraShake:
			CameraShakeConfig(ctarget);
			break;
		case XftEventType.Sound:
			SoundConfig(ctarget);
			break;
		case XftEventType.Light:
			LightConfig(ctarget);
			break;
		case XftEventType.CameraRadialBlur:
			RadialBlurConfig(ctarget);
			break;
        case XftEventType.CameraRadialBlurMask:
            RadialBlurTexAddConfig(ctarget);
            break;
		case XftEventType.CameraGlow:
			GlowConfig(ctarget);
			break;
        case XftEventType.CameraColorInverse:
            ColorInverseConfig(ctarget);
            break;
        case XftEventType.TimeScale:
            TimeScaleEventConfig(ctarget);
            break;
		default:
			break;
		}
		
		EditorGUILayout.EndVertical();
		
		if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
	}
	
    
    protected void TimeScaleEventConfig(XftEventComponent ctarget)
    {
        DisplayTimeScaleConfig = EditorGUILayout.Foldout(DisplayTimeScaleConfig, "Time Scale Configuration");
        //if (DisplayTimeScaleConfig)
        {
            EditorGUILayout.BeginVertical();
            ctarget.TimeScale = EditorGUILayout.FloatField("time scale:",ctarget.TimeScale);
            ctarget.TimeScaleDuration = EditorGUILayout.FloatField("duration:",ctarget.TimeScaleDuration);
            EditorGUILayout.EndVertical();
        }
    }
    
    protected void ColorInverseConfig(XftEventComponent ctarget)
    {
        DisplayColorInverseConfig = EditorGUILayout.Foldout(DisplayColorInverseConfig, "Color Inverse Configuration");
     
        //if (DisplayColorInverseConfig)
        {
            EditorGUILayout.BeginVertical();

            ctarget.ColorInverseShader = (Shader)EditorGUILayout.ObjectField("color inverse shader:",ctarget.ColorInverseShader, typeof(Shader),true);
            
            ctarget.CIStrengthCurve = EditorGUILayout.CurveField("strength curve:", ctarget.CIStrengthCurve,
                Color.green, new Rect(0f,0f,99f,1f));
            EditorGUILayout.EndVertical();
        }
    }
    
	protected void RadialBlurConfig(XftEventComponent ctarget)
	{
		DisplayRadialBlurConfig = EditorGUILayout.Foldout(DisplayRadialBlurConfig, "RadialBlur Configuration");
		
		//if (DisplayRadialBlurConfig)
        {
            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.LabelField("[note:if you want to use radial blur in mobile,");
            EditorGUILayout.LabelField("use 'Camera Radial Blur Mask' instead.]");
            
            ctarget.RadialBlurShader = (Shader)EditorGUILayout.ObjectField("radial blur shader:",ctarget.RadialBlurShader, typeof(Shader),true);

			ctarget.RBStrengthType = (MAGTYPE)EditorGUILayout.EnumPopup("sample strength type:", ctarget.RBStrengthType);
			
			if (ctarget.RBStrengthType == MAGTYPE.Fixed)
				ctarget.RBSampleStrength = EditorGUILayout.Slider("sample strength:",ctarget.RBSampleStrength,0f,1f);
			else
				ctarget.RBSampleStrengthCurve = EditorGUILayout.CurveField("strength curve:", ctarget.RBSampleStrengthCurve,
                    Color.green, new Rect(0f,0f,99f,1f));
            EditorGUILayout.EndVertical();
        }
	}
	
    
    protected void RadialBlurTexAddConfig(XftEventComponent ctarget)
    {
        DisplayRadialBlurTexAddConfig = EditorGUILayout.Foldout(DisplayRadialBlurTexAddConfig, "RadialBlur Mask Configuration");
     
     //if (DisplayRadialBlurTexAddConfig)
        {
            EditorGUILayout.BeginVertical();
            
            if (ctarget.RadialBlurTexAddShader == null)
            {
                ctarget.RadialBlurTexAddShader = Shader.Find ("Xffect/PP/radial_blur_mask");
            }
            
            ctarget.RadialBlurTexAddShader = (Shader)EditorGUILayout.ObjectField("radial blur mask shader:",ctarget.RadialBlurTexAddShader, typeof(Shader),true);
            string assetPath = AssetDatabase.GetAssetPath(ctarget.RadialBlurTexAddShader);
            int index = assetPath.LastIndexOf("Xffect");
            string basePath = assetPath.Substring (0,index+7);
            string texPath = basePath + "Resources/PostProcess/Textures/radial_mask1.psd";
            
            
            if (ctarget.RadialBlurMask == null)
                ctarget.RadialBlurMask = (Texture2D)AssetDatabase.LoadAssetAtPath(texPath,typeof(Texture2D));
            
            ctarget.RadialBlurMask = (Texture2D)EditorGUILayout.ObjectField("radial blur mask:",ctarget.RadialBlurMask, typeof(Texture2D),false);
            
            ctarget.RBMaskSampleDist = EditorGUILayout.FloatField("sample dist:",ctarget.RBMaskSampleDist);
           
            
            ctarget.RBMaskStrengthType = (MAGTYPE)EditorGUILayout.EnumPopup("sample strength type:", ctarget.RBMaskStrengthType);
         
            if (ctarget.RBMaskStrengthType == MAGTYPE.Fixed)
             ctarget.RBMaskSampleStrength = EditorGUILayout.Slider("sample strength:",ctarget.RBMaskSampleStrength,0f,1f);
            else
             ctarget.RBMaskSampleStrengthCurve = EditorGUILayout.CurveField("strength curve:", ctarget.RBMaskSampleStrengthCurve,
                    Color.green, new Rect(0f,0f,99f,1f));
            
            EditorGUILayout.EndVertical();
        }
    }
    
	protected void GlowConfig(XftEventComponent ctarget)
	{
		DisplayGlowConfig = EditorGUILayout.Foldout(DisplayGlowConfig, "Glow Configuration");
		
		//if (DisplayGlowConfig)
        {
            EditorGUILayout.BeginVertical();
            
            ctarget.GlowCompositeShader = (Shader)EditorGUILayout.ObjectField("composite shader:",ctarget.GlowCompositeShader, typeof(Shader),true);
            ctarget.GlowDownSampleShader = (Shader)EditorGUILayout.ObjectField("down sample shader:",ctarget.GlowDownSampleShader, typeof(Shader),true);
            ctarget.GlowBlurShader = (Shader)EditorGUILayout.ObjectField("blur shader:",ctarget.GlowBlurShader, typeof(Shader),true);
            
			ctarget.GlowIntensity = EditorGUILayout.Slider("glow intensity:", ctarget.GlowIntensity,0f,10f);
			ctarget.GlowBlurIterations = EditorGUILayout.IntSlider("blur iterations:",ctarget.GlowBlurIterations,0,30);
			ctarget.GlowBlurSpread = EditorGUILayout.Slider("blur spread:", ctarget.GlowBlurSpread,0.5f,1f);
			
			EditorGUILayout.Space();
			ctarget.GlowColorGradualType = (COLOR_GRADUAL_TYPE)EditorGUILayout.EnumPopup("gradual type:", ctarget.GlowColorGradualType);
            if (ctarget.GlowColorGradualType == COLOR_GRADUAL_TYPE.CURVE)
            {
                ctarget.ColorCurve = EditorGUILayout.CurveField("color curve:", ctarget.ColorCurve,
                Color.green, new Rect(0f,0f,99f,1f));
            }
            else
            {
                ctarget.GlowColorGradualTime = EditorGUILayout.FloatField("color gradual time:", ctarget.GlowColorGradualTime);
            }
            ctarget.GlowColorStart = EditorGUILayout.ColorField("glow color start:", ctarget.GlowColorStart);
			ctarget.GlowColorEnd = EditorGUILayout.ColorField("glow color end:", ctarget.GlowColorEnd);
			
			
			
			EditorGUILayout.EndVertical();
        }
	}
	
	protected void SoundConfig(XftEventComponent ctarget)
	{
		DisplaySoundConfig = EditorGUILayout.Foldout(DisplaySoundConfig, "Sound Configuration");
		//if (DisplaySoundConfig)
		{
			EditorGUILayout.BeginVertical();
			ctarget.Clip = (AudioClip)EditorGUILayout.ObjectField("audio clip:",ctarget.Clip,typeof(AudioClip),true);
			ctarget.Volume = EditorGUILayout.FloatField("volume:", ctarget.Volume);
			ctarget.Pitch = EditorGUILayout.FloatField("pitch:",ctarget.Pitch);
			EditorGUILayout.EndVertical();
		}
	}
	
	protected void LightConfig(XftEventComponent ctarget)
	{
		DisplayLightConfig = EditorGUILayout.Foldout(DisplayLightConfig, "Light Configuration");
		//if (DisplayLightConfig)
		{
			EditorGUILayout.BeginVertical();
			
			ctarget.LightComp = (Light)EditorGUILayout.ObjectField("light obj:",ctarget.LightComp,typeof(Light),true);
			
			ctarget.LightIntensityType = (MAGTYPE)EditorGUILayout.EnumPopup("intensity type:", ctarget.LightIntensityType);
			if (ctarget.LightIntensityType == MAGTYPE.Curve_OBSOLETE)
				ctarget.LightIntensityCurve = EditorGUILayout.CurveField("intensity curve:", ctarget.LightIntensityCurve);
			else
				ctarget.LightIntensity = EditorGUILayout.FloatField("intensity:", ctarget.LightIntensity);
			EditorGUILayout.Space();
			
			ctarget.LightRangeType = (MAGTYPE)EditorGUILayout.EnumPopup("range type:", ctarget.LightRangeType);
			if (ctarget.LightRangeType == MAGTYPE.Curve_OBSOLETE)
				ctarget.LightRangeCurve = EditorGUILayout.CurveField("range curve:", ctarget.LightRangeCurve);
			else
				ctarget.LightRange = EditorGUILayout.FloatField("range:", ctarget.LightRange);
			EditorGUILayout.EndVertical();
		}
	}
	
	protected void CameraShakeConfig(XftEventComponent ctarget)
	{
		DisplayCameraShakeConfig = EditorGUILayout.Foldout(DisplayCameraShakeConfig, "CameraShake Configuration");
		//if (DisplayCameraShakeConfig)
		{
			EditorGUILayout.BeginVertical();
			EditorGUILayout.LabelField("[note camera shake will change");
            EditorGUILayout.LabelField("the camera's local position directly.]");
			ctarget.PositionStifness = EditorGUILayout.FloatField("position stiffness", ctarget.PositionStifness);
			ctarget.PositionDamping = EditorGUILayout.FloatField("position damping:", ctarget.PositionDamping);
			EditorGUILayout.Space();
			ctarget.RotationStiffness = EditorGUILayout.FloatField("rotation stiffness:", ctarget.RotationStiffness);
			ctarget.RotationDamping = EditorGUILayout.FloatField("rotation damping:", ctarget.RotationDamping);
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("spring force:");
			ctarget.PositionForce = EditorGUILayout.Vector3Field("position force:", ctarget.PositionForce);
			ctarget.RotationForce = EditorGUILayout.Vector3Field("rotation force:", ctarget.RotationForce);
			EditorGUILayout.Space();
			ctarget.UseEarthQuake = EditorGUILayout.Toggle("use earthquake?", ctarget.UseEarthQuake);
			if (ctarget.UseEarthQuake)
			{
				ctarget.EarthQuakeTime = EditorGUILayout.FloatField("earthquake time:", ctarget.EarthQuakeTime);
				ctarget.EarthQuakeCameraRollFactor = EditorGUILayout.FloatField("roll factor:", ctarget.EarthQuakeCameraRollFactor);
				ctarget.EarthQuakeMagTye = (MAGTYPE)EditorGUILayout.EnumPopup("magnitude type:", ctarget.EarthQuakeMagTye);
				if (ctarget.EarthQuakeMagTye == MAGTYPE.Curve_OBSOLETE)
				{
					ctarget.EarthQuakeMagCurve = EditorGUILayout.CurveField("magnitude curve:", ctarget.EarthQuakeMagCurve);
				}
				else
				{
					ctarget.EarthQuakeMagnitude = EditorGUILayout.FloatField("earthquake magnitude:", ctarget.EarthQuakeMagnitude);
				}
			}
			
			EditorGUILayout.EndVertical();
		}
	}*/
}
