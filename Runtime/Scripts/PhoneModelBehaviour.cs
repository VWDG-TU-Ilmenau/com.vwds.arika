using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace MetaReal.Avatars
{
    public class PhoneModelBehaviour : MonoBehaviour
    {
        private ARCameraManager cameraManager;
        // Start is called before the first frame update
        void Start()
        {
            cameraManager = FindObjectOfType<ARCameraManager>();
        }

        // Update is called once per frame
        void Update()
        {

            if (cameraManager == null)
                return;

            UpdateTransform();
        }

        public void UpdateTransform()
        {

            transform.position = cameraManager.transform.position;
            transform.rotation = cameraManager.transform.rotation;
        }
    }
}

