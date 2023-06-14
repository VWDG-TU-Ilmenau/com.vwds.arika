using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace com.vwds.arika
{
    public class Move : MonoBehaviour
    {
        // Start is called before the first frame update
        public Transform EndMarker;
        public PhoneModelBehaviour PhoneObject;
        public GameObject AvatarFace;
        public Vector3 offset;
        public float VelocityMultiplyer;
        public bool isVR;
        public int FramesWindow = 10;
        public float MinimumHeadRotation = 30;
        public float MaximumHeadRotation = 55;
        public float MinimumPhoneDistanceFromUser = 0.3195f;
        public float MaximumPhoneDistanceFromUser = 0.35f;
        private Animator animController;
        private float currentAngleOfPhone = 0f;
        private Vector2 AvatarToPhoneVector = new Vector2(0f, 0f);
        private Vector2 AvatarToForwardVector = new Vector2(0f, 0f);
        private float distanceFromPhone;
        private Vector2 velocity;
        private Vector3 prevPosition;
        private float velocityX;
        private float velocityZ;
        private float[] velocityXArray;
        private float[] velocityZArray;
        private float averageVelocityX;
        private float averageVelocityZ;
        private int currentFrame;
        private float speed;
        void Start()
        {
            currentFrame = 0;

            animController = GetComponent<Animator>();
            prevPosition = transform.position;

            velocityXArray = new float[FramesWindow];
            velocityZArray = new float[FramesWindow];
        }
        public void ChangeAvatarHeight(float height)
        {

        }

        void Update()
        {
            var rotY = EndMarker.rotation.eulerAngles.y;
            CalculateVelocity();
            //If in VR
            if (isVR)
            {
                if (Mathf.Abs(transform.rotation.eulerAngles.y - rotY) > 40)
                    transform.rotation = Quaternion.Lerp(transform.rotation,
                        Quaternion.Euler(0f, rotY, 0f), 0.06f);

                var endPos = EndMarker.position;
                endPos.y = 0f;
                transform.position = Vector3.Lerp(transform.position, endPos, 0.12f);

            }
            else
            {
                CalculateAngleAvatarAndPhone();

                if (CalculateAvatarPhoneAngle() >= MaximumHeadRotation)
                {
                    //Debug.Log("In PhoneObject Rotation");
                    EndMarker.GetComponent<FollowObject>().UpdateTransform();
                    transform.rotation = Quaternion.Euler(0f, PhoneObject.transform.rotation.eulerAngles.y, 0f);
                    var endPos = EndMarker.position;
                    endPos.y = 0f;
                    transform.position = endPos;
                }

                distanceFromPhone = Vector3.Distance(AvatarFace.transform.position, PhoneObject.transform.position);

                if (distanceFromPhone < MinimumPhoneDistanceFromUser || distanceFromPhone > MaximumPhoneDistanceFromUser)
                {
                    //Debug.Log("In PhoneObject Position");
                    EndMarker.GetComponent<FollowObject>().UpdateTransform();
                    var endPos = EndMarker.position;
                    endPos.y = 0f;
                    transform.position = endPos;
                }
            }

            animController.SetFloat("Side Direction", averageVelocityX);
            animController.SetFloat("Forward Direction", averageVelocityZ);
        }

        public void CalculateVelocity()
        {
            if (PhoneObject == null)
                return;

            velocityX = ((transform.localPosition.x - prevPosition.x) / Time.deltaTime) * VelocityMultiplyer;
            velocityZ = ((transform.localPosition.z - prevPosition.z) / Time.deltaTime) * VelocityMultiplyer;

            speed = (transform.localPosition - prevPosition).magnitude / Time.deltaTime;

            prevPosition = transform.localPosition;

            velocityXArray[currentFrame] = velocityX;
            velocityZArray[currentFrame] = velocityZ;

            currentFrame++;

            if (currentFrame >= FramesWindow)
            {

                float tempAverageVelocityX = GetMedian(RemoveOutliers(velocityXArray));
                float tempAverageVelocityZ = GetMedian(RemoveOutliers(velocityZArray));

                if (tempAverageVelocityX == 0)
                {
                    averageVelocityX = Mathf.Lerp(averageVelocityX, 0, 0.5f);
                }
                else
                {
                    averageVelocityX = tempAverageVelocityX;
                }

                if (tempAverageVelocityZ == 0)
                {
                    averageVelocityZ = Mathf.Lerp(averageVelocityZ, 0, 0.5f);
                }
                else
                {
                    averageVelocityZ = tempAverageVelocityZ;
                }

                currentFrame = 0;
            }
        }

        public float GetMedian(List<float> rawData)
        {
            float median = 0f;
            rawData.Sort();
            median = rawData[rawData.Count / 2];
            return median;
        }

        public List<float> RemoveOutliers(float[] rawData)
        {
            List<float> filteredData = rawData.ToList();

            float average = rawData.Average();
            float sumOfSquaresOfDifferences = rawData.Select(val => (val - average) * (val - average)).Sum();
            double sd = Math.Sqrt(sumOfSquaresOfDifferences / rawData.Length);

            if (sd != 0)
            {
                for (int i = 0; i < filteredData.Count; i++)
                {
                    if (filteredData[i] >= 3 * sd)
                    {
                        filteredData.RemoveAt(i);
                    }
                }
            }
            else
            {
                filteredData.Clear();
                filteredData.Add(0f);
            }

            return filteredData;
        }

        public void CalculateAngleAvatarAndPhone()
        {
            AvatarToPhoneVector = new Vector2(PhoneObject.transform.position.x, PhoneObject.transform.position.z) - new Vector2(transform.position.x, transform.position.z);
            AvatarToForwardVector = (new Vector2(transform.forward.x * 10f, transform.forward.z * 10f) - new Vector2(transform.position.x, transform.position.z));
            currentAngleOfPhone = Mathf.Acos((Vector2.Dot(AvatarToPhoneVector, AvatarToForwardVector)) / ((AvatarToPhoneVector).magnitude * (AvatarToForwardVector).magnitude));
        }

        public float CalculateAvatarPhoneAngle()
        {
            float angle = Vector3.SignedAngle(PhoneObject.transform.forward, transform.forward, transform.up);
            return Mathf.Abs(angle);
        }
    }
}