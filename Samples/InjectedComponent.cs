using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerLoopInjector.Samples
{
    public class InjectedComponent : MonoBehaviour, IPlayerLoop
    {
        void OnEnable()
        {
            Injector.Inject(this);
        }

        void OnDisable()
        {
            Injector.Remove(this);
        }

        public void PlayerLoopInitialization()
        {
            Debug.Log("PlayerLoopInitialization");
        }

        public void PlayerLoopEarlyUpdate()
        {
            Debug.Log("PlayerLoopEarlyUpdate");
        }

        public void PlayerLoopPreUpdate()
        {
            Debug.Log("PlayerLoopPreUpdate");
        }

        public void PlayerLoopUpdate()
        {
            Debug.Log("PlayerLoopUpdate");
        }

        public void PlayerLoopPostUpdate()
        {
            Debug.Log("PlayerLoopPostUpdate");
        }

        public void PlayerLoopFixedUpdate()
        {
            Debug.Log("PlayerLoopFixedUpdate");
        }

        public void PlayerLoopEndOfFrame()
        {
            Debug.Log("PlayerLoopEndOfFrame");
        }
    }
}