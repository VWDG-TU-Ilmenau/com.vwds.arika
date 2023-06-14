using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Events;

namespace com.vwds.arika
{
    public class AvatarCalibrationManager : MonoBehaviour
    {
        public static AvatarCalibrationManager Instance;
        public GameObject GizmoVisualization;
        private AvatarCalibrationState currentState = AvatarCalibrationState.GetPhoneHeightFromGround;

        private float phoneToFloorDistance = 0f;
        private float faceToPhoneDistance = 0f;
        private float userHeight = 0f;
        private ARPlane floorPlane;
        private ARSessionOrigin aRSessionOrigin;
        private ARPlaneManager planeManager;
        private ARFaceManager faceManager;
        private ARCameraManager cameraManager;
        private Camera aRCamera;
        private bool isCalibrated;
        private bool isFloorCalibrated;
        private bool isFaceCalibrated;
        private bool isUpdatingAvatar;
        private Transform faceTransform;
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }
        }
        // Start is called before the first frame update
        void Start()
        {
            aRSessionOrigin = FindObjectOfType<ARSessionOrigin>();
            aRCamera = aRSessionOrigin.GetComponentInChildren<Camera>();

            planeManager = aRSessionOrigin.GetComponent<ARPlaneManager>();
            planeManager.planesChanged += OnPlaneChanged;

            faceManager = aRSessionOrigin.GetComponent<ARFaceManager>();
            //faceManager.facesChanged += OnFaceChanged;

            cameraManager = aRCamera.GetComponent<ARCameraManager>();
        }

        public void OnPlaneChanged(ARPlanesChangedEventArgs planeList)
        {
            // if (isFloorCalibrated)
            //     return;

            floorPlane = FindFloorPlane(planeList.updated);
            aRSessionOrigin.transform.position = new Vector3(aRSessionOrigin.transform.position.x, floorPlane.transform.position.y, aRSessionOrigin.transform.position.z);
            aRSessionOrigin.transform.rotation = floorPlane.transform.rotation;
            // phoneToFloorDistance = Mathf.Abs(aRCamera.transform.position.y - floorPlane.transform.position.y);
            phoneToFloorDistance = Vector3.Distance(aRCamera.transform.position, floorPlane.transform.position);

        }

        public void OnFaceChanged(ARFacesChangedEventArgs faceList)
        {
            if (isUpdatingAvatar)
            {
                GizmoVisualization.transform.rotation = faceList.updated[0].transform.rotation;
                StartCoroutine(EndUpdate());
            }

            if (isFaceCalibrated)
                return;

            faceTransform = faceList.added[0].transform;
            // faceToPhoneDistance = Mathf.Abs(aRCamera.transform.position.y - faceTransform.position.y);
            faceToPhoneDistance = Vector3.Distance(aRCamera.transform.position, faceTransform.position);
            currentState = AvatarCalibrationState.GetUserHeight;
            //UpdateCalibrationState();


        }
        // Update is called once per frame
        void Update()
        {

        }

        public void UpdateUserFace()
        {
            ChangeCameraFacingDirection(CameraFacingDirection.User);
            isUpdatingAvatar = true;
        }

        IEnumerator EndUpdate()
        {
            yield return new WaitForEndOfFrame();
            ChangeCameraFacingDirection(CameraFacingDirection.World);
        }

        void UpdateCalibrationState()
        {
            switch (currentState)
            {
                case AvatarCalibrationState.GetPhoneHeightFromGround:
                    break;
                case AvatarCalibrationState.GetUserFaceDistance:
                    isFloorCalibrated = true;
                    ChangeCameraFacingDirection(CameraFacingDirection.User);
                    break;
                case AvatarCalibrationState.GetUserHeight:
                    isFaceCalibrated = true;
                    //ChangeCameraFacingDirection(CameraFacingDirection.World);
                    EstimateUserHeight();
                    EndCalibration();
                    break;
                default:
                    break;
            }
        }
        public void ChangeCameraFacingDirection(CameraFacingDirection dir)
        {
            cameraManager.requestedFacingDirection = dir;
        }

        public void EndCalibration()
        {
            isCalibrated = true;
            cameraManager.requestedFacingDirection = CameraFacingDirection.World;
            //InvokeRepeating(nameof(UpdateUserFace), 0f, 5f);
        }

        public ARPlane FindFloorPlane(List<ARPlane> aRPlanes)
        {
            ARPlane plane = aRPlanes[0];

            foreach (var otherPlane in aRPlanes)
            {
                if (otherPlane.transform.position.y <= plane.transform.position.y)
                {
                    plane = otherPlane;
                    currentState = AvatarCalibrationState.GetUserFaceDistance;
                    //UpdateCalibrationState();
                }
            }
            return plane;
        }

        void EstimateUserHeight()
        {
            userHeight = phoneToFloorDistance + faceToPhoneDistance;
        }

        public float GetFaceToPhoneDistance()
        {
            return faceToPhoneDistance;
        }
        public float GetPhoneToFloorDistance()
        {
            return phoneToFloorDistance;
        }
        public CameraFacingDirection GetFacingDirection()
        {
            return cameraManager.currentFacingDirection;
        }

        public float GetUserHeight()
        {
            return userHeight;
        }
    }

    enum AvatarCalibrationState
    {
        GetPhoneHeightFromGround = 0,
        GetUserFaceDistance,
        GetUserHeight
    }
}

