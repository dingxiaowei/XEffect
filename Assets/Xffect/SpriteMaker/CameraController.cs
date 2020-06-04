using UnityEngine;
using System.Collections;

namespace Xft
{
    public class CameraController : MonoBehaviour
    {

        public Transform Target;

        public int ZoomRate = 40;
        public float ZoomDampening = 5.0f;

        public float RotateSpeed = 200f;

        public int RotateYMin = -80;
        public int RotateYMax = 80;

        public float panSpeed = 0.3f;

        protected float mDistance = 0f;
        protected float mCurDistance = 0f;
        protected float mDesiredDistance = 0f;

        protected Quaternion mCurrentRotation;
        protected Quaternion mDesiredRotation;

        protected float mDegX = 0f;
        protected float mDegY = 0f;


        void Start()
        {
            mDistance = Vector3.Distance(transform.position, Target.position);
            mCurDistance = mDistance;
            mDesiredDistance = mDistance;

            mCurrentRotation = transform.rotation;
            mDesiredRotation = transform.rotation;

            mDegX = transform.rotation.eulerAngles.y;
            mDegY = transform.rotation.eulerAngles.x;
        }
        
        float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }

        void LateUpdate()
        {

            transform.LookAt(Target);

            if (Input.GetMouseButton(0))
            {
                mDegX += Input.GetAxis("Mouse X") * RotateSpeed * Time.deltaTime;
                mDegY -= Input.GetAxis("Mouse Y") * RotateSpeed * Time.deltaTime;

                //Clamp the vertical axis for the orbit
                mDegY = ClampAngle(mDegY, RotateYMin, RotateYMax);
                // set camera rotation 
                mDesiredRotation = Quaternion.Euler(mDegY, mDegX, 0);
                mCurrentRotation = transform.rotation;

                Quaternion rotation = Quaternion.Lerp(mCurrentRotation, mDesiredRotation, Time.deltaTime * ZoomDampening);
                transform.rotation = rotation;
            }
            else if (Input.GetMouseButton(1))
            {
                //grab the rotation of the camera so we can move in a psuedo local XY space
                Target.rotation = transform.rotation;
                Target.Translate(Vector3.right * -Input.GetAxis("Mouse X") * panSpeed);
                Target.Translate(transform.up * -Input.GetAxis("Mouse Y") * panSpeed, Space.World);
            }

            // affect the desired Zoom distance if we roll the scrollwheel
            mDesiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * ZoomRate * Mathf.Abs(mDesiredDistance);
            // For smoothing of the zoom, lerp distance
            mCurDistance = Mathf.Lerp(mCurDistance, mDesiredDistance, Time.deltaTime * ZoomDampening);

            // calculate position based on the new currentDistance 
            Vector3 newPos = Target.position - (transform.rotation * Vector3.forward * mCurDistance);
            transform.position = newPos;
        }

    }
}


