using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Xft;
using UnityEditor;

//patch for version below 4.0.0
public class XffectPatch400 : Editor
{

    [MenuItem("Window/Xffect/Patch/Ver4.0.0/Patch Selected Object")]
    public static void PatchForSelectObject()
    {
        GameObject curSelect = Selection.activeGameObject;

        EditorUtility.DisplayProgressBar("Xffect Patch", "patching:" + curSelect.name + ", please wait...", 0.95f);

        DoPatch(curSelect.transform);

        EditorUtility.ClearProgressBar();
    }


    [MenuItem("Window/Xffect/Patch/Ver4.0.0/Patch Current Project")]
    public static void PatchForPrefabs()
    {
        // check all prefabs to see if we can find any objects we are interested in
        List<string> allPrefabPaths = new List<string>();
        Stack<string> paths = new Stack<string>();
        paths.Push(Application.dataPath);
        while (paths.Count != 0)
        {
            string path = paths.Pop();
            string[] files = Directory.GetFiles(path, "*.prefab");
            foreach (var file in files)
            {
                allPrefabPaths.Add(file.Substring(Application.dataPath.Length - 6));
            }

            foreach (string subdirs in Directory.GetDirectories(path))
                paths.Push(subdirs);
        }

        // Check all prefabs
        int currPrefabCount = 1;
        foreach (string prefabPath in allPrefabPaths)
        {
            EditorUtility.DisplayProgressBar("Xffect Patch", "Searching and patching xffect prefabs in project folder, please wait...", (float)currPrefabCount / (float)(allPrefabPaths.Count));

            GameObject iterGo = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
            if (!iterGo) continue;

            if (DoPatch(iterGo.transform))
            {
                //IMPORTANT: MAKE THIS PREFAB TO BE RE-IMPORTED!
                EditorUtility.SetDirty(iterGo);
            }


            ++currPrefabCount;

            //if (currPrefabCount % 100 == 0)
            {
                iterGo = null;
                EditorUtility.UnloadUnusedAssets();
                System.GC.Collect();
            }
        }


        // unload all unused assets
        EditorUtility.UnloadUnusedAssets();
        System.GC.Collect();

        EditorUtility.DisplayProgressBar("Xffect Patch", "Saving...", 1f);
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();

        EditorUtility.ClearProgressBar();
    }

    public static bool DoPatch(Transform xffect)
    {
        bool needToReimport = false;
        Object[] deps = EditorUtility.CollectDeepHierarchy(new Object[] { xffect.gameObject as Object });

        foreach (Object obj in deps)
        {
            if (obj == null || obj.GetType() != typeof(GameObject))
                continue;
            GameObject go = obj as GameObject;
            EffectLayer efl = go.GetComponent<EffectLayer>();
            if (efl != null)
            {
                DoPatch(efl);
                needToReimport = true;
            }

            XftEventComponent xevent = go.GetComponent<XftEventComponent>();
            if (xevent != null)
            {
                DoPatch(xevent);
                needToReimport = true;
            }
        }

        return needToReimport;
    }

    public static string GetPath(Transform current)
    {
        if (current.parent == null)
            return "/" + current.name;

        return GetPath(current.parent) + "/" + current.name;
    }


    public static void DoPatch(XftEventComponent xevent)
    {
    }

    public static void DoPatch(EffectLayer layer)
    {
        Debug.Log("patching for effect layer:" + GetPath(layer.transform));


        if (layer.ColorChangeType != COLOR_CHANGE_TYPE.Constant)
        {
            layer.Color1 = Color.white;
        }


        if (layer.EmitType == 4 && layer.LineStartObj == null)
        {
            layer.LineStartObj = layer.transform;
        }

        if (!layer.UseSameScaleCurve)
        {
            layer.MaxScaleValueY = layer.MaxScaleCalue;
        }


        if (layer.RenderType == 2 || layer.RenderType == 3)
        {
            float y = layer.OriTopLeftUV.y - layer.OriUVDimensions.y;
            if (y >= 0f)
                layer.OriTopLeftUV.y = layer.OriTopLeftUV.y - layer.OriUVDimensions.y;
        }

    }


}

