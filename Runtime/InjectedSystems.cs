using System;

namespace PlayerLoopInjector
{
    public class InjectedSystemCallbacks
    {
        public object Owner;

        public Action OnInitialization;
        public Action OnEarlyUpdate;
        public Action OnFixedUpdate;
        public Action OnPreUpdate;
        public Action OnUpdate;
        public Action OnPostUpdate;
        public Action OnEndOfFrame;
    }

    struct InjectedSystems
    {
        public struct InjectedInitializationSystem
        {
        }

        public struct InjectedEarlyUpdateSystem
        {
        }

        public struct InjectedFixedUpdateSystem
        {
        }

        public struct InjectedPreUpdateSystem
        {
        }

        public struct InjectedUpdateSystem
        {
        }

        public struct InjectedPostUpdateSystem
        {
        }

        public struct InjectedEndOfFrameSystem
        {
        }
    }
}