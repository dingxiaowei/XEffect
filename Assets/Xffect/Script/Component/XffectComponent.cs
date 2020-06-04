//----------------------------------------------
//            Xffect Editor
// Copyright © 2012- Shallway Studio
// http://shallway.net
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Xft
{
    [AddComponentMenu("Xffect/XffectComponent")]
    [ExecuteInEditMode]
    public class XffectComponent : MonoBehaviour
    {
        #region Member
        static public string CurVersion = "4.4.1";
        public string MyVersion = "4.4.1";

        Dictionary<string, VertexPool> MatDic = new Dictionary<string, VertexPool>();
        List<EffectLayer> EflList = new List<EffectLayer>();
        List<XftEventComponent> EventList = new List<XftEventComponent>();

        List<VertexPool> MatList = new List<VertexPool>();

        //Editable
        public float LifeTime = -1f;

        public bool IgnoreTimeScale = false;

        protected float ElapsedTime = 0f;

        protected bool Initialized = false;
        protected List<GameObject> MeshList = new List<GameObject>();

        protected double LastTime = 0f;
        protected double CurTime = 0f;

        public bool EditView = false;


        public float Scale = 1f;

        public int MaxFps = 60;


        public bool AutoDestroy = false;


        public bool MergeSameMaterialMesh = true;


        public bool Paused = false;

        public bool UpdateWhenOffScreen = true;

        public bool UseWith2DSprite = false;
        public string SortingLayerName = "Default";
        public int SortingOrder = 0;


        public float PlaybackTime = 1f;

        protected bool mIsActive = false;

        #endregion

        #region edit mode functions
#if UNITY_EDITOR
        public void EnableEditView()
        {
            if (!EditorApplication.isPlaying)
            {
                EditView = true;
                enabled = true;
            }

        }

        public void DisableEditView()
        {
            if (!EditorApplication.isPlaying)
            {
                EditView = false;

                //need to reset each event to do some disable task.
                foreach (XftEventComponent e in EventList)
                {
                    e.ResetCustom();
                }

                ClearEditModeMeshs();

                //A trick to let the next update to re-inited.
                EflList.Clear();
                EventList.Clear();
            }
        }


        public void ResetEditScene()
        {
            if (!IsActive(gameObject))
                Active();
            ClearEditModeMeshs();

            //A trick to let the next update to re-inited.
            EflList.Clear();
            EventList.Clear();

            Initialize();
            Reset();
            Start();
            ElapsedTime = 0f;

        }

        public bool CheckEditModeInited()
        {
            if (EflList.Count == 0)
                return false;
            foreach (EffectLayer el in EflList)
            {
                //ugly trick....
                if (el.emitter == null)
                    return false;
            }
            return true;
        }




        public void ClearEditModeMeshs()
        {

            //IMPORTANT, first need to clear other mesh reference before clear the meshes.
            MatDic.Clear();
            MatList.Clear();
            MeshList.Clear();

            ArrayList tobeDelete = new ArrayList();
            foreach (Transform child in transform)
            {

                if (!child.name.Contains("xftmesh"))
                    continue;

                MeshFilter mf = (MeshFilter)child.GetComponent(typeof(MeshFilter));
                if (mf != null)
                {
                    tobeDelete.Add(mf.gameObject);
                }
            }

            //dangerious code, do not put your own mesh assets to the Xffect Object's children.
            foreach (GameObject obj in tobeDelete)
            {
                GameObject.DestroyImmediate(obj);
            }
        }

#endif
        #endregion


        #region camera
        protected Camera mCamera;
        public Camera MyCamera
        {
            get
            {
#if UNITY_EDITOR
                if (!EditorApplication.isPlaying && SceneView.lastActiveSceneView != null)
                {
                    //just get the editor camera
                    return SceneView.lastActiveSceneView.camera;
                }
                else
                {
                    if (mCamera == null || !mCamera.gameObject.activeInHierarchy || !mCamera.enabled)
                        FindMyCamera();
                    return mCamera;
                }
#else
                if (mCamera == null || !mCamera.gameObject.activeInHierarchy || !mCamera.enabled)
                    FindMyCamera();
                return mCamera;
#endif
            }
            set
            {
                mCamera = value;
            }
        }
        public void FindMyCamera()
        {
            int layerMask = 1 << gameObject.layer;
            Camera[] cameras = GameObject.FindObjectsOfType(typeof(Camera)) as Camera[];
            for (int i = 0, imax = cameras.Length; i < imax; ++i)
            {
                Camera cam = cameras[i];

                if ((cam.cullingMask & layerMask) != 0)
                {
                    mCamera = cam;
                    return;
                }
            }
        }
        #endregion


        void Awake()
        {
            //if (!Initialized)
            //Initialize();
            FindMyCamera();
        }
        void DestoryMeshObject(GameObject obj)
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                DestroyImmediate(obj);
            }
            else
            {
                Destroy(obj);
            }
#else
        Destroy(obj);
#endif
        }

        //note this function also modified MeshList.
        MeshFilter CreateMeshObj(Material mat)
        {
            GameObject obj = new GameObject("xftmesh " + mat.name);
            obj.layer = gameObject.layer;
            MeshList.Add(obj);
            obj.AddComponent<MeshFilter>();
            obj.AddComponent<MeshRenderer>();
            SetActive(obj, true);



#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                XffectComponent.SetActive(obj, true);
                //obj.hideFlags = HideFlags.HideAndDontSave;
            }
#endif

            MeshFilter Meshfilter;
            MeshRenderer Meshrenderer;
            Meshfilter = (MeshFilter)obj.GetComponent(typeof(MeshFilter));
            Meshrenderer = (MeshRenderer)obj.GetComponent(typeof(MeshRenderer));
            Meshrenderer.castShadows = false;
            Meshrenderer.receiveShadows = false;
            Meshrenderer.GetComponent<Renderer>().sharedMaterial = mat;

            if (UseWith2DSprite)
            {
                Meshrenderer.sortingLayerName = SortingLayerName;
                Meshrenderer.sortingOrder = SortingOrder;
            }

            Meshfilter.sharedMesh = new Mesh();
            return Meshfilter;
        }

        public void Initialize()
        {
            if (EflList.Count > 0)
            {//already inited.
                return;
            }

            List<GameObject> tobeDeleted = new List<GameObject>();


            //if it's instantiated, delete all the old meshes.
            foreach (Transform child in transform)
            {
                if (child.gameObject.name.Contains("xftmesh"))
                    tobeDeleted.Add(child.gameObject);
            }


            foreach (Transform child in transform)
            {
                EffectLayer el = (EffectLayer)child.GetComponent(typeof(EffectLayer));
                if (el == null)
                    continue;

                if (el.Material == null)
                {
                    Debug.LogWarning("effect layer: " + el.gameObject.name + " has no material, please assign a material first!");
                    continue;
                }

                Material mat = el.Material;
                //ver 1.2.1
                mat.renderQueue = mat.shader.renderQueue;
                mat.renderQueue += el.Depth;
                EflList.Add(el);

                if (MergeSameMaterialMesh)
                {
                    if (!MatDic.ContainsKey(mat.name))
                    {
                        MeshFilter mf = CreateMeshObj(mat);
                        MatDic[mat.name] = new VertexPool(mf.sharedMesh, mat);
                    }
                }
                else
                {
                    MeshFilter mf = CreateMeshObj(mat);
                    MatList.Add(new VertexPool(mf.sharedMesh, mat));
                }
            }


            foreach (GameObject obj in tobeDeleted)
            {
                DestoryMeshObject(obj);
            }

            //now set each gameobject's parent.
            foreach (GameObject obj in MeshList)
            {
                obj.transform.parent = this.transform;

                //fixed 2012.6.25, 
                obj.transform.position = Vector3.zero;
                obj.transform.rotation = Quaternion.identity;

                //fixed 2012.7.11, avoid the lossy scale influence the mesh object.
                Vector3 realLocalScale = Vector3.zero;
                realLocalScale.x = 1 / obj.transform.parent.lossyScale.x;
                realLocalScale.y = 1 / obj.transform.parent.lossyScale.y;
                realLocalScale.z = 1 / obj.transform.parent.lossyScale.z;

                obj.transform.localScale = realLocalScale * Scale;
            }


            //assign vertex pool.
            for (int i = 0; i < EflList.Count; i++)
            {
                EffectLayer efl = EflList[i];

                if (MergeSameMaterialMesh)
                {
                    efl.Vertexpool = MatDic[efl.Material.name];
                }
                else
                {
                    if (EflList.Count != MatList.Count)
                    {
                        Debug.LogError("something wrong with the no merge mesh mat list!");
                        efl.Vertexpool = MatList[0];
                        continue;
                    }
                    efl.Vertexpool = MatList[i];
                }

            }


            //start each effect layer.


            this.transform.localScale = Vector3.one;

            foreach (EffectLayer el in EflList)
            {
                el.StartCustom();
            }

            //add events
            EventList.Clear();
            foreach (Transform child in transform)
            {
                XftEventComponent xftevent = child.GetComponent<XftEventComponent>();
                if (xftevent == null)
                    continue;
                EventList.Add(xftevent);
                xftevent.Initialize(this);
            }
            Initialized = true;
        }
        void Start()
        {

            if (!Initialized)
                Initialize();

            LastTime = Time.realtimeSinceStartup;
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                LastTime = (float)EditorApplication.timeSinceStartup;
            }
#endif
        }


        public void Update()
        {
            CurTime = Time.realtimeSinceStartup;

            float deltaTime = (float)(CurTime - LastTime);

#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                if (!EditView || EditorUtility.IsPersistent(gameObject))
                    return;
                CurTime = EditorApplication.timeSinceStartup;
                if (!CheckEditModeInited())
                    ResetEditScene();
                deltaTime = (float)(CurTime - LastTime);
                deltaTime *= PlaybackTime;
            }
#endif
            //simple method to check game delay: the game must run above 10 FPS.
            if (deltaTime > 0.1f)
            {
                deltaTime = 0.0333f;
            }

            if (Paused)
            {
                LastTime = CurTime;
                return;
            }

            if (!UpdateWhenOffScreen && !IsInCameraView() && !EditView)
            {
                LastTime = CurTime;
                return;
            }



            if (!IgnoreTimeScale)
                deltaTime *= Time.timeScale;

            ElapsedTime += deltaTime;



            for (int i = 0; i < EflList.Count; i++)
            {
                if (EflList[i] == null)//be destroyed?
                    return;
                EffectLayer el = EflList[i];
                if (ElapsedTime > el.StartTime && IsActive(el.gameObject))
                {
                    el.FixedUpdateCustom(deltaTime);
                }
            }


            for (int i = 0; i < EventList.Count; i++)
            {
                XftEventComponent e = EventList[i];
                if (IsActive(e.gameObject))
                {
                    e.UpdateCustom(deltaTime);
                }
            }

            LastTime = CurTime;

#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                LateUpdate();
            }
#endif
        }

        public void ResetEditorEvents()
        {
            for (int i = 0; i < EventList.Count; i++)
            {
                XftEventComponent e = EventList[i];
                e.ResetCustom();
            }
        }

        void DoFinish()
        {

            mIsActive = false;

#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                foreach (EffectLayer el in EflList)
                {
                    el.Reset();
                }

                foreach (XftEventComponent e in EventList)
                {
                    e.ResetCustom();
                }

                ClearEditModeMeshs();
                ElapsedTime = 0f;
                return;
            }
#endif

            for (int i = 0; i < EflList.Count; i++)
            {
                EffectLayer el = EflList[i];
                el.Reset();
            }
            DeActive();
            ElapsedTime = 0f;

            if (AutoDestroy)
            {
                GameObject.Destroy(gameObject);
            }

        }


        bool IsInCameraView()
        {
            for (int i = 0; i < MeshList.Count; i++)
            {
                GameObject obj = MeshList[i];
                if (obj != null)
                {
                    MeshRenderer mr = obj.GetComponent<MeshRenderer>();
                    if (mr.isVisible)
                        return true;
                }
            }


            for (int i = 0; i < EflList.Count; i++)
            {
                EffectLayer el = EflList[i];

                Vector3 cpoint = el.ClientTransform.position;

                Vector3 vpoint = MyCamera.WorldToViewportPoint(cpoint);

                if (vpoint.x >= 0 && vpoint.x <= 1 && vpoint.y >= 0 && vpoint.y <= 1 && vpoint.z > 0)
                    return true;
            }


            return false;
        }


        void UpdateMeshObj()
        {

            //make sure each mesh position is always zero.
            for (int i = 0; i < MeshList.Count; i++)
            {
                GameObject obj = MeshList[i];
                if (obj != null)
                {
                    obj.transform.position = Vector3.zero;
                    obj.transform.rotation = Quaternion.identity;
                }
            }

            if (MergeSameMaterialMesh)
            {

                //ElementAt() STILL CAUSE GC ALLOC!! SO WE CAN'T AVOID GC ALLOC IN DICTIONARY?
                //for (int i = 0; i < MatDic.Count; i++)
                //{
                //    KeyValuePair<string,VertexPool> pair = MatDic.ElementAt(i);
                //    pair.Value.LateUpdate();
                //}

                foreach (KeyValuePair<string, VertexPool> pair in MatDic)
                {
                    pair.Value.LateUpdate();
                }
            }
            else
            {

                for (int i = 0; i < MatList.Count; i++)
                {
                    VertexPool vp = MatList[i];
                    vp.LateUpdate();
                }
            }
        }


        void LateUpdate()
        {
            if (!UpdateWhenOffScreen && !IsInCameraView() && !EditView)
            {
                return;
            }


            UpdateMeshObj();

            if (ElapsedTime > LifeTime && LifeTime >= 0)
            {
                DoFinish();
            }
            else if (LifeTime < 0 && EflList.Count > 0)
            {
                //Xffect LifeTime < 0， 且又是EmitByRate的话，会自动判断是否已经没有活动节点，没有则自动Deactive()。
                float deltaTime = (float)(CurTime - LastTime);
                bool allEfLFinished = true;

                for (int i = 0; i < EflList.Count; i++)
                {
                    EffectLayer el = EflList[i];
                    if (!el.EmitOver(deltaTime))
                    {
                        allEfLFinished = false;
                    }
                }

                if (allEfLFinished)
                {
                    DoFinish();
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            //foreach (KeyValuePair<string, VertexPool> pair in MatDic)
            {
                //Mesh mesh = pair.Value.Mesh;
                //Gizmos.DrawWireCube(tempre.bounds.center, tempre.bounds.size);
                //Debug.Log(mesh.bounds.size);
            }
        }

        #region API
        //[System.Obsolete("Active() is deprecated, please use Activate() instead.")]
        public void Active()
        {
            if (!Initialized)
            {//may active by out caller, and not inited yet.
                Initialize();
            }

            //no longer support unity 3.x now.
            gameObject.SetActive(true);

            //foreach (Transform child in transform)
            //{
            //    SetActive(child.gameObject, true);
            //}
            //SetActive(gameObject, true);

            Reset();
        }
        //[System.Obsolete("DeActive() is deprecated, please use Stop() instead.")]
        public void DeActive()
        {

            for (int i = 0; i < EventList.Count; i++)
            {
                XftEventComponent e = EventList[i];
                e.ResetCustom();
            }


            //foreach (Transform child in transform)
            //{
            //    SetActive(child.gameObject, false);
            //}
            //SetActive(gameObject, false);


            gameObject.SetActive(false);

        }
        public bool IsPlaying
        {
            get { return mIsActive; }
        }

        public void Reset()
        {
            mIsActive = true;
            ElapsedTime = 0f;

            //reset times
            Start();

            for (int i = 0; i < EflList.Count; i++)
            {
                EffectLayer el = EflList[i];
                el.Reset();
            }


            for (int i = 0; i < EventList.Count; i++)
            {
                XftEventComponent e = EventList[i];
                e.ResetCustom();
            }
        }
        public void StopSmoothly(float fadeTime)
        {
            for (int i = 0; i < EflList.Count; i++)
            {
                EffectLayer el = EflList[i];
                el.StopSmoothly(fadeTime);
            }

        }
        public void ActiveNoInterrupt()
        {
            if (IsActive(gameObject))
                return;
            Active();
        }

        public void SetScale(Vector2 scale, string eflName)
        {
            foreach (EffectLayer el in EflList)
            {
                if (el.gameObject.name == eflName)
                    el.SetScale(scale);
            }
        }

        public void SetColor(Color color, string eflName)
        {
            foreach (EffectLayer el in EflList)
            {
                if (el.gameObject.name == eflName)
                    el.SetColor(color);
            }
        }

        public void SetRotation(float angle, string eflName)
        {
            foreach (EffectLayer el in EflList)
            {
                if (el.gameObject.name == eflName)
                    el.SetRotation(angle);
            }
        }

        public void SetGravityGoal(Transform goal, string eflName)
        {
            foreach (EffectLayer el in EflList)
            {
                if (el.gameObject.name == eflName && el.GravityAffectorEnable)
                    el.SetArractionAffectorGoal(goal);
            }
        }

        public void SetCollisionGoalPos(Transform pos, string eflName)
        {
            foreach (EffectLayer el in EflList)
            {
                if (el.gameObject.name == eflName && el.UseCollisionDetection)
                    el.SetCollisionGoalPos(pos);
            }
        }

        #endregion

        #region API deprecated



        //obsolete
        public void ResetCamera(Camera cam)
        {

        }

        //set the client transform
        public void SetClient(Transform client)
        {
            foreach (EffectLayer el in EflList)
            {
                el.SetClient(client);
            }
        }

        //set Direction,  Not recommended, you can change the client's rotation to affect the direction.
        public void SetDirectionAxis(Vector3 axis)
        {
            foreach (EffectLayer el in EflList)
            {
                el.OriVelocityAxis = axis;
            }
        }

        public void SetEmitPosition(Vector3 pos)
        {
            foreach (EffectLayer el in EflList)
            {
                el.EmitPoint = pos;
            }
        }

        public void SetCollisionGoalPos(Transform pos)
        {
            foreach (EffectLayer el in EflList)
            {
                if (el.UseCollisionDetection)
                    el.SetCollisionGoalPos(pos);
            }
        }

        public void SetArractionAffectorGoal(Transform goal)
        {
            foreach (EffectLayer el in EflList)
            {
                if (el.GravityAffectorEnable)
                    el.SetArractionAffectorGoal(goal);
            }
        }

        public void SetArractionAffectorGoal(Transform goal, string eflName)
        {
            foreach (EffectLayer el in EflList)
            {
                if (el.gameObject.name == eflName && el.GravityAffectorEnable)
                    el.SetArractionAffectorGoal(goal);
            }
        }

        //only support one effect layer. for my own game shero use.
        public EffectNode EmitByPos(Vector3 pos)
        {
            if (EflList.Count > 1)
                Debug.LogWarning("EmitByPos only support one effect layer!");
            EffectLayer el = EflList[0];
            return el.EmitByPos(pos);
        }

        public void StopEmit()
        {
            for (int i = 0; i < EflList.Count; i++)
            {
                EffectLayer el = EflList[i];
                el.StopEmit();
            }
        }

        #endregion

        #region for_unity4_compatible

        public static bool IsActive(GameObject obj)
        {
            //#if UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4
            //            return obj.activeSelf;
            //#else
            //        return obj.active;
            //#endif


            //NO LONGER SUPPORT UNITY 3.X
            return obj.activeSelf;
        }

        public static void SetActive(GameObject obj, bool flag)
        {
            //#if UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4
            //            obj.SetActive(flag);
            //#else
            //        obj.active = flag;
            //#endif

            //NO LONGER SUPPORT 3.X
            obj.SetActive(flag);
        }

        #endregion




        //water mark, used for free version
        //void OnGUI()
        //{
        //    GUI.Label(new Rect(Screen.width-140,Screen.height - 30, 140, 30), "Xffect Editor Free");
        //}

    }
}