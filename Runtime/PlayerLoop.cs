using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.LowLevel;

namespace PlayerLoopInjector
{
    public enum LoopInjectionPoint
    {
        Initialization = 0,
        EarlyUpdate,
        FixedUpdate,
        PreUpdate,
        Update,
        PostUpdate,
        EndOfFrame,
    }

    public interface IPlayerLoop :
        IPlayerLoopInitialization, IPlayerLoopEarlyUpdate, IPlayerLoopPreUpdate,
        IPlayerLoopUpdate, IPlayerLoopPostUpdate, IPlayerLoopFixedUpdate,
        IPlayerLoopEndOfFrame
    { }

    public interface IPlayerLoopInitialization
    {
        void PlayerLoopInitialization();
    }

    public interface IPlayerLoopEarlyUpdate
    {
        void PlayerLoopEarlyUpdate();
    }

    public interface IPlayerLoopPreUpdate
    {
        void PlayerLoopPreUpdate();
    }

    public interface IPlayerLoopUpdate
    {
        void PlayerLoopUpdate();
    }

    public interface IPlayerLoopPostUpdate
    {
        void PlayerLoopPostUpdate();
    }

    public interface IPlayerLoopFixedUpdate
    {
        void PlayerLoopFixedUpdate();
    }

    public interface IPlayerLoopEndOfFrame
    {
        void PlayerLoopEndOfFrame();
    }
}