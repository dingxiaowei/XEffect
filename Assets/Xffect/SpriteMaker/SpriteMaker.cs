#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;
namespace Xft
{
    public class SpriteMaker : MonoBehaviour
    {
        #region enums

        public enum EStep
        {
            None,
            Prepare,
            Processing,
            Finished,
        }

        #endregion

        #region member
        public XffectComponent Effect;
        public CameraController MainCamera;
        [HideInInspector]
        public Texture2D RectTex;
        [HideInInspector]
        public Texture2D BkgTex;


        public int StartWidth = 256;
        public int StartHeight = 256;

        protected int mOutputWidth;
        protected int mOutputHeight;
        protected string mOutputPath = "SpriteMaker/Output/";
        protected GUIStyle mStyle = new GUIStyle();
        protected Rect mOutputRect;
        protected float mSliderW;
        protected float mSliderH;


        protected bool mShowGUI = true;
        protected float mElapsedTime = 0f;
        protected int mCurTexIndex = 0;
        protected float mCaptureInterval = 0.4f;
        protected float mTimeLen = 2f;

        protected EStep mStep = EStep.None;


        protected bool mIsCapturing = false;

        protected float mTotalTime = 0f;

        #endregion

        void RefreshRect()
        {
            int screenWidth = (int)Screen.width;
            int screenHeight = (int)Screen.height;

            mOutputWidth = (int)(screenWidth * mSliderW);
            mOutputHeight = (int)(screenHeight * mSliderH);

            if (Mathf.Abs(mOutputWidth - StartWidth) < 2)
                mOutputWidth = StartWidth;

            if (Mathf.Abs(mOutputHeight - StartHeight) < 2)
                mOutputHeight = StartHeight;



            int left = screenWidth / 2 - mOutputWidth / 2;
            int top = screenHeight / 2 - mOutputHeight / 2;

            mOutputRect = new Rect(left, top, mOutputWidth, mOutputHeight);
        }


        string GetXffectPath()
        {
            Shader temp = Shader.Find("Xffect/PP/radial_blur_mask");
            string assetPath = AssetDatabase.GetAssetPath(temp);
            int index = assetPath.LastIndexOf("Xffect");
            string basePath = assetPath.Substring(0, index + 7);

            return basePath;
        }

        IEnumerator CaptureRect(string path)
        {
            Effect.Paused = true;
            mIsCapturing = true;


            yield return new WaitForEndOfFrame();

            Texture2D tex2D = new Texture2D(mOutputWidth, mOutputHeight, TextureFormat.RGB24, false);

            tex2D.ReadPixels(mOutputRect, 0, 0);
            tex2D.Apply();

#if UNITY_WEBPLAYER
            Debug.LogError("Sprite Maker can't run at web player mode!");
#else
            byte[] bytes = tex2D.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, bytes);
#endif
            Destroy(tex2D);

            yield return new WaitForSeconds(0.5f);

            mTotalTime -= 0.5f;

            mIsCapturing = false;
            mCurTexIndex++;
            Effect.Paused = false;
        }

        void Awake()
        {
            mSliderW = (float)StartWidth / Screen.width;
            mSliderH = (float)StartHeight / Screen.height;

            RefreshRect();
            //MainCamera.Target = Effect.transform;
        }

        void OnDestroy()
        {
            EditorUtility.ClearProgressBar();
        }

        void PrepareCapture()
        {
            mShowGUI = false;

            MainCamera.GetComponent<Camera>().backgroundColor = Color.black;
            mElapsedTime += Time.deltaTime;

            if (mElapsedTime > 0.5f)
            {
                Effect.Active();
                mElapsedTime = 0f;
                mStep = EStep.Processing;
                mTotalTime = 0f;
                mIsCapturing = false;
            }
        }

        void ProcessCapture()
        {
            mTotalTime += Time.deltaTime;
            if (mIsCapturing)
                return;

            mElapsedTime += Time.deltaTime;

            if (mElapsedTime < mCaptureInterval)
                return;

            mElapsedTime -= mCaptureInterval;

            string path = GetXffectPath() + mOutputPath + Effect.gameObject.name + mCurTexIndex + ".png";

            EditorUtility.DisplayProgressBar("Processing", path, mTotalTime / mTimeLen);

            StartCoroutine(CaptureRect(path));

            if (mCurTexIndex * mCaptureInterval > mTimeLen || !Effect.gameObject.activeSelf)
            {
                mElapsedTime = 0f;
                mCurTexIndex = 0;
                mStep = EStep.Finished;
            }
        }


        void CaptureFinished()
        {
            mShowGUI = true;
            mStep = EStep.None;
            Effect.DeActive();
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
            return;
        }

        void LateUpdate()
        {
            switch (mStep)
            {
                case EStep.None:
                    return;
                case EStep.Prepare:
                    PrepareCapture();
                    break;
                case EStep.Processing:
                    ProcessCapture();
                    break;
                case EStep.Finished:
                    CaptureFinished();
                    break;
                default:
                    break;
            }
        }

        void OnGUI()
        {

            if (!mShowGUI)
                return;

            mStyle.normal.background = RectTex;

            GUI.Box(mOutputRect, "", mStyle);


            GUI.Label(new Rect(240, 0, 300, 70), "left mouse button to rotate, scroll to zoom, right mouse button to move.");

            #region View Controller
            GUI.BeginGroup(new Rect(0, 0, 240, 120));

            GUI.Label(new Rect(60, 0, 100, 30), "View Controller");

            GUI.BeginGroup(new Rect(0, 30, 240, 30));
            GUI.Label(new Rect(0, 0, 80, 30), "OutputWidth:");
            mSliderW = GUI.HorizontalSlider(new Rect(80, 5, 100, 30), mSliderW, 0.0f, 1.0f);
            GUI.Label(new Rect(180, 0, 60, 30), mOutputWidth.ToString());
            GUI.EndGroup();

            GUI.BeginGroup(new Rect(0, 60, 240, 30));
            GUI.Label(new Rect(0, 0, 80, 30), "OutputHeight:");
            mSliderH = GUI.HorizontalSlider(new Rect(80, 5, 100, 30), mSliderH, 0.0f, 1.0f);
            GUI.Label(new Rect(180, 0, 60, 30), mOutputHeight.ToString());
            GUI.EndGroup();


            if (GUI.Button(new Rect(0,90,200,30),"Preview"))
            {
                Effect.Active();
            }


            GUI.EndGroup();
            #endregion



            #region Parameter
            GUI.BeginGroup(new Rect(0, 140, 240, 130));
            GUI.Label(new Rect(60, 0, 100, 30), "Parameter");

            GUI.BeginGroup(new Rect(0, 30, 240, 30));
            GUI.Label(new Rect(0, 0, 80, 30), "Interval:");
            mCaptureInterval = GUI.HorizontalSlider(new Rect(80, 5, 100, 30), mCaptureInterval, 0.05f, 2.0f);
            GUI.Label(new Rect(180, 0, 60, 30), mCaptureInterval.ToString("0.00") + " s");
            GUI.EndGroup();

            GUI.BeginGroup(new Rect(0, 60, 240, 30));
            GUI.Label(new Rect(0, 0, 80, 30), "Time Len:");
            mTimeLen = GUI.HorizontalSlider(new Rect(80, 5, 100, 30), mTimeLen, 0.1f, 8f);
            GUI.Label(new Rect(180, 0, 60, 30), mTimeLen.ToString("0.0") + " s");
            GUI.EndGroup();


            if (GUI.Button(new Rect(0, 90, 200, 30), "Capture"))
            {
                mStep = EStep.Prepare;
            }

            GUI.EndGroup();

            #endregion


            //if (GUI.Button(new Rect(0, 0, 30f, 20f), RectTex))
            //{
            //    mStep = EStep.Prepare;
            //}

            RefreshRect();
        }
    }
}
#endif


