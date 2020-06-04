using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Xft;

public class XGradientEditor : EditorWindow 
{
	
	protected List<Xft.ColorKey> mColorKeys;
	
	protected ColorParameter mColorParam;
	
	protected Editor mEditor;
	
	protected Texture2D mEditorTex;
	
	
	public static int ColorTexWidth = 512;
	public static int ColorTexHeight = 32;
	
	protected bool mDragging;
	
	protected int mDragKey;
	
	protected float mDragOffset;
	protected Rect InfoBox;
	protected Rect ColorBox;
	
	public static float ColorFieldWidth = 40f;
	public static float ColorBoxHeight = 24f;
	public static float ColorBoxRightPadding = 40f;

	
    static public void Show(ColorParameter param, Texture2D etex, Editor editor) {
        // Get existing open window or if none, make a new one:
        XGradientEditor window = GetWindow<XGradientEditor>(true, "Xffect Color", false);
		
		window.mColorKeys = param.Colors;
		window.mColorParam = param;
		window.mEditor = editor;
		window.mEditorTex = etex;
		
		window.maxSize = new Vector2(1000, 100);
		window.minSize = new Vector2(500, 100);
    }
    
	
	#region editor gradient tex process
	public void RefreshEditorGradientTex()
	{
		if (mEditorTex == null)
		{
			mEditorTex =new Texture2D(ColorTexWidth,ColorTexHeight, TextureFormat.RGBA32, false);
			mEditorTex.wrapMode = TextureWrapMode.Clamp;
		}
		
		RefreshEditorGradient(mEditorTex, mColorParam);
		mEditor.Repaint();
	}
	
	public void RefreshEditorGradient ()
    {
		if (mEditorTex == null)
			return;
		
		RefreshEditorGradient(mEditorTex,mColorParam);
    }
	
	public void RefreshEditorGradient (Texture2D tex, ColorParameter p)
    {
        Color col;
        for (int x = 0; x < tex.width; x++) {
            col = p.GetGradientColor(x / (float)tex.width);
            for (int y=0;y<tex.height;y++)
                tex.SetPixel(x,y,col);
        }
        tex.Apply();
    }
	
	#endregion
    void OnGUI () 
	{
        
        if (mColorKeys == null || mEditor == null)
            return;
        
		bool leftMouseDown = Event.current.type == EventType.MouseDown && Event.current.button == 0;
        bool leftMouseUp = Event.current.type == EventType.MouseUp && Event.current.button == 0;
		bool doubleClick = leftMouseDown && Event.current.clickCount > 1;
		bool rightMouseDown = Event.current.type == EventType.MouseDown && Event.current.button == 1;
		bool isDrag = Event.current.type==EventType.MouseDrag;
		
		GUILayout.Label("Usage: double click to add new color; drag to move color; right click to delete color.", new GUILayoutOption[] { GUILayout.MinHeight(20), GUILayout.ExpandWidth(true) });
		
		if (Event.current.type == EventType.Repaint )
		{
			InfoBox = GUILayoutUtility.GetLastRect();
			
			
		}
		
		GUILayout.Box("", new GUILayoutOption[] { GUILayout.MinHeight(ColorBoxHeight), GUILayout.Width(InfoBox.width - ColorBoxRightPadding) });
		Rect r = GUILayoutUtility.GetLastRect();
		r = new Rect(r.x + 2, r.y + 2, r.width - 4, r.height - 4);
		
		if (!mEditorTex && Event.current.type == EventType.Repaint){
			XEditorTool.RefreshGradientTex(ref mEditorTex,mColorParam,mEditor);
        }
		
		if (mEditorTex)
		{
			EditorGUI.DrawPreviewTexture(r, mEditorTex,EffectLayerCustom.ColorBkgMat);
		}

		Vector2 mp = Event.current.mousePosition;
		
		bool isOutSideWindow = (mp.x > InfoBox.width);
		
		for (int k = 0; k < mColorKeys.Count; k++)
		{
			Rect HandleRect = new Rect(r.x + mColorKeys[k].t * r.width, r.y, ColorFieldWidth, r.height);
			
			// Delete Key?
			if (rightMouseDown && HandleRect.Contains(mp) && mColorKeys.Count > 1) {
				mColorKeys.RemoveAt(k);
				XEditorTool.RefreshGradientTex(ref mEditorTex,mColorParam,mEditor);
				Event.current.Use();
				Repaint();
				break;
			}
			
			// Begin Drag?
			if (isDrag && !mDragging && HandleRect.Contains(mp)) {
				mDragging = true;
				mDragKey = k;
				mDragOffset = mp.x - HandleRect.x;
				Event.current.Use();
			}
			// Drag?
			if (Event.current.type==EventType.Repaint && mDragging && mDragKey==k) {
				
				float pos = mp.x - r.x-mDragOffset;
				
				pos = Mathf.Clamp(pos,0,r.width);
				
				float t = pos / (r.width);
				mColorKeys[mDragKey].t = t;
				
				HandleRect = new Rect(r.x + t * r.width, r.y, ColorFieldWidth, r.height);
				Repaint();
			}
			
			if (leftMouseDown && HandleRect.Contains(mp))
                    Event.current.Use();
			
			if (leftMouseUp || mp.x < 0 || isOutSideWindow) {
				//Debug.LogWarning(mp.x + ":" + r.width);
				if (mDragging && mDragKey==k) {
					mDragging = false;
					mDragKey = -1;
					XEditorTool.RefreshGradientTex(ref mEditorTex,mColorParam,mEditor);
					Event.current.Use();
				}
				else if (HandleRect.Contains(mp) && !isOutSideWindow) {
					Event.current.type = EventType.MouseDown;
				}
			}
			
			Color c = mColorKeys[k].Color;
			mColorKeys[k].Color = EditorGUI.ColorField(HandleRect, mColorKeys[k].Color);
			if (c != mColorKeys[k].Color)
				XEditorTool.RefreshGradientTex(ref mEditorTex,mColorParam,mEditor);
		}
		
		if (doubleClick && r.Contains(mp)) {
			mColorParam.AddColorKey((mp.x-r.x)/r.width,Color.white);
			XEditorTool.RefreshGradientTex(ref mEditorTex,mColorParam,mEditor);
			Repaint();
		}
		
		if (mDragKey >= 0)
		{
			string location = "Location:" + mColorKeys[mDragKey].t * 100f + "%";
			GUILayout.Label(location, new GUILayoutOption[] { GUILayout.MinHeight(20), GUILayout.ExpandWidth(true) });
		}
		
    }
}
