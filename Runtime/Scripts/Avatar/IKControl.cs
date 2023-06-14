using UnityEngine;

namespace com.vwds.arika
{
    [RequireComponent(typeof(Animator))]

    public class IKControl : MonoBehaviour
    {

        protected Animator animator;

        public bool ikActive = false;
        public bool isCrouch = false;
        public Transform rightHandObj = null;
        public Transform leftHandObj = null;
        public Transform lookObj = null;
        public Handedness Handedness = Handedness.RightHanded;
        public bool IsPointing;
        private  PhoneModelBehaviour PhoneObject;

        void Start()
        {
            animator = GetComponent<Animator>();
            Invoke(nameof(InitializeIK), 3f);
        }

        public void InitializeIK()
        {
            PhoneObject = FindObjectOfType<PhoneModelBehaviour>();
        }

        //a callback for calculating IK
        void OnAnimatorIK()
        {
            if (animator)
            {

                //if the IK is active, set the position and rotation directly to the goal. 
                if (ikActive)
                {

                    // Set the look target position, if one has been assigned
                    if (lookObj != null)
                    {
                        animator.SetLookAtWeight(0.5f);
                        animator.SetLookAtPosition(lookObj.position);
                    }

                    // Set the right hand target position and rotation, if one has been assigned
                    if (rightHandObj != null && (Handedness == Handedness.RightHanded || Handedness == Handedness.BothHands))
                    {
                        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
                    }

                    // Set the right hand target position and rotation, if one has been assigned
                    if (leftHandObj != null && (Handedness == Handedness.LeftHanded || Handedness == Handedness.BothHands))
                    {
                        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                        animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
                    }

                }

                //if the IK is not active, set the position and rotation of the hand and head back to the original position
                else
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                    animator.SetLookAtWeight(0);
                }
            }
        }

        public void PointHand()
        {
            animator.SetBool("Point", IsPointing);
        }

        public Animator GetAnimator()
        {
            return animator;
        }
    }

    public enum Handedness
    {
        LeftHanded,
        RightHanded,
        BothHands
    }
}