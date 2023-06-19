using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace com.vwds.arika
{
    public class EmoteController : MonoBehaviour
    {
        public UnityEvent Wave;
        public UnityEvent Beckon;
        public UnityEvent Confirm;
        public UnityEvent Decline;
        public UnityEvent PointStart;
        public UnityEvent Pointing;
        public UnityEvent PointEnd;
        private IKControl ikControl;
        private Animator animatorController;
        // Start is called before the first frame update
        void Start()
        {
            animatorController = GetComponent<Animator>();
            ikControl = GetComponent<IKControl>();
        }

        public void OnWave()
        {
            triggerAnimation("Wave");
            Wave.Invoke();
        }
        // Update is called once per frame
        public void OnBeckon()
        {
            triggerAnimation("Beckoning");
            Beckon.Invoke();
        }
        public void OnConfirm()
        {
            triggerAnimation("ThumbsUp");
            Confirm.Invoke();
        }
        public void OnDecline()
        {
            triggerAnimation("ThumbsDown");
            Decline.Invoke();
        }
        public void OnPointStart()
        {
            ikControl.IsPointing = true;
            ikControl.PointHand();
            PointStart.Invoke();
        }

        public void OnPointing()
        {
            Pointing.Invoke();
        }

        public void OnPointEnd()
        {
            ikControl.IsPointing = false;
            ikControl.PointHand();
            PointEnd.Invoke();
        }

        private void triggerAnimation(string parameterName)
        {
            animatorController.SetTrigger(parameterName);
        }
    }
}