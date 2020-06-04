using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Xft
{
    [ExecuteInEditMode]
    public class XftCameraShakeComp : MonoBehaviour
    {

        protected Spring mPositionSpring;
        protected Spring mRotationSpring;

        protected XftEventComponent m_client;

        public bool EarthQuakeToggled = false;

        protected float m_earthQuakeTimeTemp = 0f;

        protected Vector3 mLastPosition;
        protected Vector3 mLastRotation;

        protected Vector3 mOriPosition;
        protected Vector3 mOriRotation;


        public Spring PositionSpring
        {
            set { mPositionSpring = value; }
            get { return mPositionSpring; }
        }

        public Spring RotationSpring
        {
            set { mRotationSpring = value; }
            get { return mRotationSpring; }
        }

        public XftEventComponent Client
        {
            get { return m_client; }
        }

        void Awake()
        {
            this.enabled = false;
        }


        public void Reset(XftEventComponent client)
        {
            m_client = client;
            if (m_client.CameraShakeType == XCameraShakeType.Spring)
            {
                if (PositionSpring != null && !CheckDone())
                {
                    //last event shake has not finished, so set the client pos to original pos.
                    transform.localPosition = mOriPosition;
                    transform.localRotation = Quaternion.Euler(mOriRotation);
                }

                //reset spring
                PositionSpring = new Spring(client.transform, Spring.TransformType.Position);
                PositionSpring.MinVelocity = 0.00001f;
                RotationSpring = new Spring(client.transform, Spring.TransformType.Rotation);
                RotationSpring.MinVelocity = 0.00001f;
                PositionSpring.Stiffness = new Vector3(m_client.PositionStifness, m_client.PositionStifness, m_client.PositionStifness);
                PositionSpring.Damping = Vector3.one - new Vector3(m_client.PositionDamping, m_client.PositionDamping, m_client.PositionDamping);
                RotationSpring.Stiffness = new Vector3(m_client.RotationStiffness, m_client.RotationStiffness, m_client.RotationStiffness);
                RotationSpring.Damping = Vector3.one - new Vector3(m_client.RotationDamping, m_client.RotationDamping, m_client.RotationDamping);

                m_client.transform.localPosition = transform.localPosition;
                m_client.transform.localRotation = transform.localRotation;
                PositionSpring.RefreshTransformType();
                RotationSpring.RefreshTransformType();
                m_earthQuakeTimeTemp = m_client.EarthQuakeTime;


                mLastPosition = transform.localPosition;
                mLastRotation = transform.localRotation.eulerAngles;
                mOriPosition = transform.localPosition;
                mOriRotation = transform.localRotation.eulerAngles;
            }


            //NOTE: NEED TO APPLY THE M_CLIENT'S elapsedTime = 0 first.
            Update();

        }

        //constant spring force.
        void UpdateEarthQuake()
        {

            if (m_client == null || !m_client.UseEarthQuake || m_earthQuakeTimeTemp <= 0.0f 
                || !EarthQuakeToggled || m_client.ElapsedTime > m_client.EarthQuakeTime)
                return;

            m_earthQuakeTimeTemp -= 0.0166f * (60 * Time.deltaTime);

            float magnitude = 0f;
            if (m_client.EarthQuakeMagTye == MAGTYPE.Fixed)
                magnitude = m_client.EarthQuakeMagnitude;
            else if (m_client.EarthQuakeMagTye == MAGTYPE.Curve_OBSOLETE)
                magnitude = m_client.EarthQuakeMagCurve.Evaluate(m_client.ElapsedTime);
            else
                magnitude = m_client.EarthQuakeMagCurveX.Evaluate(m_client.ElapsedTime);

            Vector3 horizMove = Vector3.Scale(XftSmoothRandom.GetVector3Centered(1), new Vector3(magnitude,
                                                        0, magnitude)) * Mathf.Min(m_earthQuakeTimeTemp, 1.0f);

            float vertMove = 0;
            if (UnityEngine.Random.value < 0.3f)
            {
                vertMove = UnityEngine.Random.Range(0, (magnitude * 0.35f)) * Mathf.Min(m_earthQuakeTimeTemp, 1.0f);
                if (PositionSpring.State.y >= PositionSpring.RestState.y)
                    vertMove = -vertMove;
            }

            PositionSpring.AddForce(horizMove);

            RotationSpring.AddForce(new Vector3(0, 0, -horizMove.x * 2) * m_client.EarthQuakeCameraRollFactor);
            PositionSpring.AddForce(new Vector3(0, vertMove, 0));
        }

        public bool CheckDone()
        {
            if (m_client.CameraShakeType == XCameraShakeType.Spring)
            {
                if (PositionSpring == null || RotationSpring == null)
                {
                    //not yet inited.
                    return true;
                }
                if (PositionSpring.Done && RotationSpring.Done)
                    return true;
            }
            else
            {
                if (m_client.ElapsedTime > m_client.ShakeCurveTime)
                    return true;
            }
            return false;
        }

        void UpdateCurve()
        {
            float t = m_client.ElapsedTime / m_client.ShakeCurveTime;

            Vector3 posV = mOriPosition + m_client.PositionForce * (m_client.PositionCurve.Evaluate(t) * 2f -1f);
            Vector3 rotV = mOriRotation + m_client.RotationForce * (m_client.RotationCurve.Evaluate(t) * 2f -1f);

            Vector3 posPhase = posV - mLastPosition;
            Vector3 rotPhase = rotV - mLastRotation;

            transform.localPosition += posPhase;
            Vector3 curRot = transform.localRotation.eulerAngles;
            transform.localRotation = Quaternion.Euler(curRot + rotPhase);

            mLastPosition = posV;
            mLastRotation = rotV;
        }

        void UpdateSpring()
        {
            if (PositionSpring == null || RotationSpring == null)
            {
                //not inited
                return;
            }

            UpdateEarthQuake();

            PositionSpring.FixedUpdate();
            RotationSpring.FixedUpdate();

            Vector3 posPhase = m_client.transform.localPosition - mLastPosition;
            Vector3 rotPhase = m_client.transform.localEulerAngles - mLastRotation;

            transform.localPosition += posPhase;
            Vector3 curRot = transform.localRotation.eulerAngles;
            transform.localRotation = Quaternion.Euler(curRot + rotPhase);

            mLastPosition = m_client.transform.localPosition;
            mLastRotation = m_client.transform.localEulerAngles;


        }

        void Update()
        {

            if (m_client == null)
                return;

            if (m_client.CameraShakeType == XCameraShakeType.Curve)
                UpdateCurve();
            else
                UpdateSpring();

            if (CheckDone())
            {
                this.enabled = false;
                return;
            }
        }
    }
}