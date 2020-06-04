using UnityEngine;
using UnityEditor;
using Xft;
 
[CustomEditor(typeof(XffectComponent))]
public class XffectComponentCustom : Editor
{
	public static string DefaultMatPath = "EffectLibrary/Textures/Glow/glow01.mat";
	
	string LayerName = "EffectLayer";
	
	#region check update


	public string latestVersion;
	public static WWW updateCheckObject; 
	public static string updateURL = "http://shallway.net/products/xffect/version.php";
	protected static System.DateTime _lastUpdateCheck;
	public static System.DateTime lastUpdateCheck {
		get {
			try {
				_lastUpdateCheck = System.DateTime.Parse (EditorPrefs.GetString ("XffectLastUpdateCheck",System.DateTime.UtcNow.ToString ()));
			}
			catch (System.FormatException) {
				_lastUpdateCheck = System.DateTime.UtcNow;
				Debug.LogWarning ("Invalid DateTime string encountered when loading from preferences");
			}
			return _lastUpdateCheck;
		}
		set {
			_lastUpdateCheck = value;
			EditorPrefs.SetString ("XffectLastUpdateCheck", _lastUpdateCheck.ToString ());
		}
	}
	public static bool ParseServerMessage (string result)
	{
		if (string.IsNullOrEmpty (result)) {
			return false;
		}
		string[] sarr = result.Split('\n');
		
		if (sarr == null || sarr.Length == 0)
		{
			//Debug.LogWarning("parse server message error:" + result +"\nXffect Pro");
			return false;
		}
		
		for (int i = 0; i < sarr.Length; i++)
		{
			string str = sarr[i];
			
			if (string.IsNullOrEmpty(str) || str.Contains("*"))
				continue;
			string[] item = str.Split('|');
			if (item.Length != 3)
			{
				//Debug.LogWarning("parse server message error:" + result +"\nXffect Pro");
				continue;
			}
			item[2].Trim('\r');
			
			string version = item[0];
			string version_url = item[1];
			string version_desc = item[2];

			EditorPrefs.SetString ("XffectLatestVersion",version.ToString ());
			EditorPrefs.SetString ("XffectLatestVersionDesc",version_desc);
			EditorPrefs.SetString ("XffectLatestVersionURL",version_url);
			return true;
		}
		
		return true;
	}
	public static void CheckForUpdates () {
		if (updateCheckObject != null && updateCheckObject.isDone) {
			
			if (!string.IsNullOrEmpty (updateCheckObject.error)) {
				//Debug.LogWarning ("There was an error while checking for updates\n" +
				//"The error might dissapear if you switch build target from Webplayer to Standalone: " +
				//updateCheckObject.error);
				updateCheckObject = null;
				return;
			}
			ParseServerMessage (updateCheckObject.text);
			updateCheckObject = null;
		}
		
		//if (updateCheckObject == null)
			//updateCheckObject = new WWW (updateURL);
		
		if (System.DateTime.Compare (lastUpdateCheck.AddDays (1f),System.DateTime.UtcNow) < 0) {
			//Debug.Log ("Checking For Updates... " + System.DateTime.UtcNow.ToString ()+"\nXffect Pro");
			updateCheckObject = new WWW (updateURL);
			lastUpdateCheck = System.DateTime.UtcNow;
		}
	}
	#endregion
	
    public static string website = "http://shallway.net/xffect";
    public static string forum = "http://shallway.net/xffect/forum/";
	
	public XffectComponent Script;


	protected XEditorTool mXEditor;
	
	
	

	protected SerializedProperty LifeTime;
    protected SerializedProperty IgnoreTimeScale;
	protected SerializedProperty EditView;
    protected SerializedProperty Scale;
	protected SerializedProperty AutoDestroy;

    protected SerializedProperty MergeSameMaterialMesh;

    protected SerializedProperty Paused;

    protected SerializedProperty UpdateWhenOffScreen;

    protected SerializedProperty UseWith2DSprite;
    protected SerializedProperty SortingLayerName;
    protected SerializedProperty SortingOrder;

    protected SerializedProperty MaxFps;

    protected SerializedProperty PlaybackTime;

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
	
	void LoadStyle()
	{
		if (EffectLayerCustom.IsSkinLoaded)
			return;
		EffectLayerCustom.LoadStyle();
	}
	
	void OnEnable()
	{
		Script = target as XffectComponent;
		InitSerializedProperty();
		LoadStyle();
		
		if (Application.isEditor && !EditorApplication.isPlaying) {
			EditorApplication.update = Script.Update;
		}
	}
	
	void OnDisable ()
	{
		if (Application.isEditor && !EditorApplication.isPlaying) {
			EditorApplication.update = null;
		}
	}
	
	void InitSerializedProperty()
	{
		LifeTime = serializedObject.FindProperty("LifeTime");
		IgnoreTimeScale = serializedObject.FindProperty("IgnoreTimeScale");
		EditView = serializedObject.FindProperty("EditView");
		Scale = serializedObject.FindProperty("Scale");
        AutoDestroy = serializedObject.FindProperty("AutoDestroy");
        MergeSameMaterialMesh = serializedObject.FindProperty("MergeSameMaterialMesh");
        Paused = serializedObject.FindProperty("Paused");
        UpdateWhenOffScreen = serializedObject.FindProperty("UpdateWhenOffScreen");

        UseWith2DSprite = serializedObject.FindProperty("UseWith2DSprite");
        SortingLayerName = serializedObject.FindProperty("SortingLayerName");
        SortingOrder = serializedObject.FindProperty("SortingOrder");

        MaxFps = serializedObject.FindProperty("MaxFps");
        PlaybackTime = serializedObject.FindProperty("PlaybackTime");
	}


    public void OnSceneGUI()
    {

        serializedObject.Update();

        Rect r = new Rect(0, Screen.height - 120, 160, 80);


        Vector2 mouse = Event.current.mousePosition;

        Rect r2 = r;
        r2.yMin -= 30;
        r2.xMin -= 10;
        r2.xMax += 10;
        r2.yMax += 10;

        if (r2.Contains(mouse) && Event.current.type == EventType.Layout)
        {
            int controlID = GUIUtility.GetControlID(1024, FocusType.Passive);
            HandleUtility.AddControl(controlID, 0F);
        }

        Handles.BeginGUI();
        GUILayout.BeginArea(r, Script.gameObject.name, "Window");


        EditView.boolValue = GUILayout.Toggle(EditView.boolValue, "update in editor?");

        PlaybackTime.floatValue = EditorGUILayout.FloatField("Playback time: ", PlaybackTime.floatValue);

        if (PlaybackTime.floatValue < 0f)
            PlaybackTime.floatValue = 0f;

        if (EditView.boolValue)
        {
            Script.EnableEditView();
        }
        else
        {
            Paused.boolValue = false;
            Script.DisableEditView();
        }

        if (EditView.boolValue)
        {

            string disp = "Pause";
            if (Paused.boolValue)
                disp = "Play";
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(disp))
            {
                Paused.boolValue = !Paused.boolValue;
            }

            if (GUILayout.Button("Reset"))
            {
                Paused.boolValue = false;
                Script.ResetEditScene();
            }
            EditorGUILayout.EndHorizontal();
        }
        GUILayout.EndArea();

        Handles.EndGUI();

        serializedObject.ApplyModifiedProperties();
    }

	public override void OnInspectorGUI ()
	{
		
		serializedObject.Update();


        //check if need to upgrade.
        System.Version myVer = new System.Version(Script.MyVersion);
        if (myVer <= new System.Version("4.4.0"))
        {
            DoPatch440(Script);
            EditorUtility.SetDirty(target);
        }

		XEditor.BeginCommonArea("xffect main config",Script.gameObject.name,this,true);

        EditorGUILayout.Space();


        if (EditorUtility.IsPersistent(target))
        {
            if (GUILayout.Button("Put To Scene"))
            {
                GameObject obj = PrefabUtility.InstantiatePrefab(Selection.activeObject as GameObject) as GameObject; 
                Selection.activeGameObject = obj;
                if (obj != null)
                {
                    XffectComponent xobj = obj.GetComponent<XffectComponent>();
                    if (xobj != null)
                    {
                        xobj.EditView = true;
                        xobj.EnableEditView();
                    }
                }
                
            }
        }

		
		XEditor.DrawToggle("update in editor?","",EditView);
		
		//EditView.boolValue = EditorGUILayout.Toggle("update in editor:", EditView.boolValue,GUILayout.Height(40f));
		
		
		//if (EditView.boolValue == true) {
			//if (!XffectComponent.IsActive(Script.gameObject)) {
				//EditView.boolValue = false;
				//Debug.Log ("you need to activate the xffect object: " + Script.gameObject.name + " before updating it in editor.");
			//}
		//}

		if (EditView.boolValue) {
			Script.EnableEditView ();
		} else {
            Paused.boolValue = false;
			Script.DisableEditView ();
		}

		if (EditView.boolValue)
        {

            PlaybackTime.floatValue = EditorGUILayout.FloatField("Playback time: ", PlaybackTime.floatValue);

            if (PlaybackTime.floatValue < 0f)
                PlaybackTime.floatValue = 0f;

            string disp = "Pause";
            if (Paused.boolValue)
                disp = "Play";
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(disp))
            {
                Paused.boolValue = !Paused.boolValue;
            }

			if (GUILayout.Button ("Reset")) {
                Paused.boolValue = false;
				Script.ResetEditScene ();
			}
            EditorGUILayout.EndHorizontal();
		}
		
		XEditor.DrawSeparator();
		
		XEditor.DrawFloat("life(-1 means infinite):","",LifeTime);
		XEditor.DrawToggle("ignore time scale?","",IgnoreTimeScale);
		
        XEditor.DrawToggle("auto destroy?","check on this option if you want this obj to be destroyed when finished, note this option only works in play mode.",AutoDestroy);

        XEditor.DrawToggle("merge same mesh?", "check on this option to merge the meshes with same material, can reduce drawcalls.", MergeSameMaterialMesh);

        XEditor.DrawToggle("update when offscreen?", "", UpdateWhenOffScreen);

        if (XEditor.DrawToggle("use with 2d sprite?", "", UseWith2DSprite))
        {
            XEditor.DrawText("sorting layer name:", "", SortingLayerName);
            XEditor.DrawInt("sorting order:", "", SortingOrder);
        }

		EditorGUILayout.Space();



        XEditor.DrawInt("Max Fps:", "", MaxFps);

		XEditor.DrawFloat("scale:","change this Xffect's scale",Scale);

        

        if (!Mathf.Approximately(1f, Scale.floatValue))
        {
            XEditor.DrawInfo("note it's not recommended to use this function to change xffect's scale. if you encounter strange behavious, please change it back to 1.");
        }
		
		if (GUILayout.Button ("Add Layer")) {
			GameObject layer = new GameObject (LayerName);
			EffectLayer efl = (EffectLayer)layer.AddComponent <EffectLayer>();
			layer.transform.parent = Selection.activeTransform;
			efl.transform.localPosition = Vector3.zero;
			//default to effect layer object.
			efl.ClientTransform = efl.transform;
			efl.GravityObject = efl.transform;
			efl.BombObject = efl.transform;
			efl.TurbulenceObject = efl.transform;
			efl.AirObject = efl.transform;
			efl.VortexObj = efl.transform;
			//efl.DirCenter = efl.transform;
			efl.Material = AssetDatabase.LoadAssetAtPath(XEditorTool.GetXffectPath() + DefaultMatPath, typeof(Material)) as Material;
			
            efl.gameObject.layer = Script.gameObject.layer;

            efl.LineStartObj = efl.transform;
            
			Selection.activeGameObject = layer;
		}
		
		
		if (GUILayout.Button ("Add Event")) {
			GameObject obj = new GameObject ("_Event");
			XftEventComponent xevent = (XftEventComponent)obj.AddComponent <XftEventComponent>();
			xevent.transform.parent = Selection.activeTransform;
			xevent.transform.localPosition = Vector3.zero;
			xevent.RadialBlurShader = Shader.Find ("Xffect/PP/radial_blur");
			xevent.GlowCompositeShader = Shader.Find ("Xffect/PP/glow_compose");
			xevent.GlowDownSampleShader = Shader.Find ("Xffect/PP/glow_downsample");
			xevent.GlowBlurShader = Shader.Find ("Xffect/PP/glow_conetap");
			xevent.RadialBlurObj = xevent.transform;
            xevent.ColorInverseShader = Shader.Find ("Xffect/PP/color_inverse");

            xevent.GlowPerObjBlendShader = Shader.Find("Hidden/PP/Xffect/glow_per_obj/blend");
            xevent.GlowPerObjReplacementShader = Shader.Find("Hidden/PP/Xffect/glow_per_obj/replacement");
            xevent.gameObject.layer = Script.gameObject.layer;
            
			Selection.activeGameObject = obj;
		}
		XEditor.EndXArea();

		DrawInfos();
		serializedObject.ApplyModifiedProperties();
	}
	
	
	void DrawInfos()
	{
		XEditor.BeginCommonArea("xffect infos","Support",this,false);
		
		///////////check for update////////////////////////////
		CheckForUpdates();
		string latestVersion = EditorPrefs.GetString("XffectLatestVersion",XffectComponent.CurVersion);
		System.Version lv = new System.Version(latestVersion);
		string url = EditorPrefs.GetString("XffectLatestVersionURL","http://shallway.net");
        if (lv > new System.Version(XffectComponent.CurVersion))
		{
			string desc = EditorPrefs.GetString("XffectLatestVersionDesc","");
			string info = "There is a new version available, the latest version is:" +  latestVersion + "\nInfo:" + desc;
			XEditor.DrawInfo(info);
			
			if (GUILayout.Button ("Download")) {
				Application.OpenURL (url);
			}
			
		}
		//////////////////////////////////////////////////
        string curversion = "Version:" + XffectComponent.CurVersion;
        XEditor.DrawInfo(curversion);
        XEditor.DrawInfo("email: shallwaycn@gmail.com");
        XEditor.DrawInfo("weibo: http://weibo.com/shallwaycn");
        
		//EditorGUILayout.LabelField(curversion);
		//EditorGUILayout.LabelField("Arthor: shallway");
		//EditorGUILayout.LabelField("Contact: shallwaycn@gmail.com");
		
		if (GUILayout.Button ("Website")) {
			Application.OpenURL (website);
		}

        if (GUILayout.Button("Forum"))
        {
            Application.OpenURL(forum);
        }

		EditorGUILayout.Space();
		
		XEditor.EndXArea();
	}

    [MenuItem("Window/Xffect/Website")]
    static void DoVisitWebsite()
    {
        Application.OpenURL(website);
    }
	
	[MenuItem("GameObject/Create Other/Xffect Object")]
	[MenuItem("Window/Xffect/Create Xffect Object")]
	static void DoCreateXffectObject ()
	{
        
        Transform parent = Selection.activeTransform;
        
		GameObject go = new GameObject ("XffectObj");
		go.transform.localScale = Vector3.one;
		go.transform.rotation = Quaternion.identity;
		go.AddComponent<XffectComponent> ();

		Selection.activeGameObject = go;

		GameObject layer = new GameObject ("EffectLayer");
		EffectLayer efl = (EffectLayer)layer.AddComponent <EffectLayer>();
		layer.transform.parent = go.transform;

		efl.transform.localPosition = Vector3.zero;
		//fixed 2012.6.25. default to effect layer object.
		efl.ClientTransform = efl.transform;
		efl.GravityObject = efl.transform;
		efl.BombObject = efl.transform;
		efl.TurbulenceObject = efl.transform;
		efl.AirObject = efl.transform;
		efl.VortexObj = efl.transform;
		//efl.DirCenter = efl.transform;
        efl.DragObj = efl.transform;
		
		efl.Material = AssetDatabase.LoadAssetAtPath(XEditorTool.GetXffectPath() + DefaultMatPath, typeof(Material)) as Material;

        efl.LineStartObj = efl.transform;

        if (parent != null)
        {
            go.transform.parent = parent;
        }
    }



    #region patch

    public string GetPath(Transform current)
    {
        if (current.parent == null)
            return "/" + current.name;

        return GetPath(current.parent) + "/" + current.name;
    }


    void DoPatch415(XffectComponent xffect)
    {

        xffect.MyVersion = "4.1.5";

        Object[] deps = EditorUtility.CollectDeepHierarchy(new Object[] { xffect.gameObject as Object });
        foreach (Object obj in deps)
        {
            if (obj == null || obj.GetType() != typeof(GameObject))
                continue;
            GameObject go = obj as GameObject;
            EffectLayer efl = go.GetComponent<EffectLayer>();
            if (efl != null)
            {
                DoPatch415(efl);
            }
        }
    }

    void DoPatch415(EffectLayer layer)
    {
        if (layer.EmitLoop < 0)
        {
            layer.EmitDuration = -1f;
        }
        
        layer.EmitLoop = 1;

    }


    void DoPatch440(XffectComponent xffect)
    {

        xffect.MyVersion = XffectComponent.CurVersion;

        Object[] deps = EditorUtility.CollectDeepHierarchy(new Object[] { xffect.gameObject as Object });
        foreach (Object obj in deps)
        {
            if (obj == null || obj.GetType() != typeof(GameObject))
                continue;
            GameObject go = obj as GameObject;
            EffectLayer efl = go.GetComponent<EffectLayer>();
            if (efl != null)
            {
                DoPatch440(efl);
            }
        }
    }

    void DoPatch440(EffectLayer layer)
    {
        if (layer.IsRandomStartColor)
        {
            XEditorTool.PatchColorGradient(layer.RandomColorParam, layer.RandomColorGradient);
        }

        if (layer.ColorChangeType == COLOR_CHANGE_TYPE.Gradient)
        {
            XEditorTool.PatchColorGradient(layer.ColorParam, layer.ColorGradient);
        }

    }


    #endregion

}
