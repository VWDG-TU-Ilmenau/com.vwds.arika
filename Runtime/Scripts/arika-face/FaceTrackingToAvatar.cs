#if UNITY_IOS && UNITY_EDITOR
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARKit;
using Unity.Collections; // For NativeArray

namespace com.vwds.arika.face
{
    public class FaceTrackingToAvatar : MonoBehaviour
    {
        public SkinnedMeshRenderer avatarRenderer; // Assign Ready Player Me avatar's SkinnedMeshRenderer in the Inspector
        public ARCameraManager phoneCamera;
        public GameObject FaceObject;
        public ARFaceManager faceManager; // Assign ARFaceManager in the Inspector
        private Transform headBoneTransform;
        private bool isAvatarRendererSet;
        private Quaternion phoneWorldRotation;
        private Quaternion faceWorldRotation;
        public void SetAvatarRenderer(SkinnedMeshRenderer avatar)
        {
            avatarRenderer = avatar;
            isAvatarRendererSet = true;
            headBoneTransform = GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head);
        }

        void LateUpdate()
        {
            if (!isAvatarRendererSet)
                return;

            // Ensure there's at least one face being tracked
            foreach (ARFace face in faceManager.trackables)
            {
                if (face == null) continue;

                // Access ARKit blend shapes through the ARKitFaceSubsystem
                ARKitFaceSubsystem faceSubsystem = (ARKitFaceSubsystem)faceManager.subsystem;
                if (faceSubsystem == null) continue;

                // Get the blend shape coefficients
                NativeArray<ARKitBlendShapeCoefficient> blendShapes = faceSubsystem.GetBlendShapeCoefficients(face.trackableId, Allocator.Temp);

                // Map ARKit blend shapes to RPM avatar blend shapes
                foreach (var blendShape in blendShapes)
                {
                    string blendShapeName = GetBlendShapeName(blendShape.blendShapeLocation);
                    if (!string.IsNullOrEmpty(blendShapeName))
                    {
                        int index = avatarRenderer.sharedMesh.GetBlendShapeIndex(blendShapeName);
                        if (index >= 0)
                        {
                            avatarRenderer.SetBlendShapeWeight(index, blendShape.coefficient * 100f);
                        }
                    }
                }

                blendShapes.Dispose(); // Dispose the NativeArray after use
            }
            SetFaceOrientation();
        }

        private void SetFaceOrientation()
        {
            phoneWorldRotation = phoneCamera.transform.rotation;
            faceWorldRotation = phoneWorldRotation * FaceObject.transform.rotation;
            headBoneTransform.rotation = faceWorldRotation;
        }

        // Map ARKit blend shape locations to RPM avatar blend shape names
        private string GetBlendShapeName(ARKitBlendShapeLocation location)
        {
            switch (location)
            {
                case ARKitBlendShapeLocation.JawOpen: return "jawOpen";
                case ARKitBlendShapeLocation.MouthSmileLeft: return "smileLeft";
                case ARKitBlendShapeLocation.MouthSmileRight: return "smileRight";
                case ARKitBlendShapeLocation.BrowDownLeft: return "browDownLeft";
                case ARKitBlendShapeLocation.BrowDownRight: return "browDownRight";
                case ARKitBlendShapeLocation.EyeBlinkLeft: return "blinkLeft";
                case ARKitBlendShapeLocation.EyeBlinkRight: return "blinkRight";
                default: return null;
            }
        }
    }
}
#endif