using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.Profiling;

namespace PlayerLoopInjector
{
    public static class Injector
    {
        static InjectedSystemCallbacks global;
        static List<InjectedSystemCallbacks> injected;
        static Dictionary<object, InjectedSystemCallbacks> indirectInjectedLookup;

        public static InjectedSystemCallbacks Global => global;

        static Injector()
        {
            indirectInjectedLookup = new Dictionary<object, InjectedSystemCallbacks>();
            injected = new List<InjectedSystemCallbacks>();
            global = new InjectedSystemCallbacks();

            var playerLoop = PlayerLoop.GetDefaultPlayerLoop();

            var injectedInitializationSystem =
                MakeLoopSystem<InjectedSystems.InjectedInitializationSystem>(InjectedInitialization);
            var injectedEarlyUpdateSystem =
                MakeLoopSystem<InjectedSystems.InjectedEarlyUpdateSystem>(InjectedEarlyUpdate);
            var injectedFixedUpdateSystem =
                MakeLoopSystem<InjectedSystems.InjectedFixedUpdateSystem>(InjectedFixedUpdate);
            var injectedPreUpdateSystem =
                MakeLoopSystem<InjectedSystems.InjectedPreUpdateSystem>(InjectedPreUpdate);
            var injectedUpdateSystem =
                MakeLoopSystem<InjectedSystems.InjectedUpdateSystem>(InjectedUpdate);
            var injectedPostUpdateSystem =
                MakeLoopSystem<InjectedSystems.InjectedPostUpdateSystem>(InjectedPostUpdate);
            var injectedEndOfFrameSystem =
                MakeLoopSystem<InjectedSystems.InjectedEndOfFrameSystem>(InjectedEndOfFrameUpdate);

            if (!InjectSystem<UnityEngine.PlayerLoop.Initialization>(ref playerLoop, ref injectedInitializationSystem))
            {
                Debug.LogError("Failed to inject InjectedSystems.InjectedInitializationSystem into PlayerLoop");
            }

            if (!InjectSystem<UnityEngine.PlayerLoop.EarlyUpdate>(ref playerLoop, ref injectedEarlyUpdateSystem))
            {
                Debug.LogError("Failed to inject InjectedSystems.InjectedEarlyUpdateSystem into PlayerLoop");
            }

            if (!InjectSystem<UnityEngine.PlayerLoop.FixedUpdate>(ref playerLoop, ref injectedFixedUpdateSystem))
            {
                Debug.LogError("Failed to inject InjectedSystems.InjectedFixedUpdateSystem into PlayerLoop");
            }

            if (!InjectSystem<UnityEngine.PlayerLoop.PreUpdate>(ref playerLoop, ref injectedPreUpdateSystem))
            {
                Debug.LogError("Failed to inject InjectedSystems.InjectedPreUpdateSystem into PlayerLoop");
            }

            if (!InjectSystem<UnityEngine.PlayerLoop.Update>(ref playerLoop, ref injectedUpdateSystem))
            {
                Debug.LogError("Failed to inject InjectedSystems.InjectedUpdateSystem into PlayerLoop");
            }

            if (!InjectSystem<UnityEngine.PlayerLoop.PreLateUpdate>(ref playerLoop, ref injectedPostUpdateSystem))
            {
                Debug.LogError("Failed to inject InjectedSystems.InjectedPostUpdateSystem into PlayerLoop");
            }

            if (!InjectSystem<UnityEngine.PlayerLoop.PostLateUpdate>(ref playerLoop, ref injectedEndOfFrameSystem))
            {
                Debug.LogError("Failed to inject InjectedSystems.InjectedEndOfFrameSystem into PlayerLoop");
            }

            PlayerLoop.SetPlayerLoop(playerLoop);
        }

        static bool InjectSystem<T>(ref PlayerLoopSystem current, ref PlayerLoopSystem injected, bool first = true)
        {
            if (current.type == typeof(T))
            {
                var newList = new PlayerLoopSystem[current.subSystemList.Length + 1];
                int offset = 0;

                if (first)
                {
                    newList[0] = injected;
                    offset = 1;
                }
                else
                {
                    newList[current.subSystemList.Length] = injected;
                }

                for (int i = 0; i < current.subSystemList.Length; i++)
                {
                    newList[i + offset] = current.subSystemList[i];
                }

                current.subSystemList = newList;

                return true;
            }

            if (current.subSystemList != null)
            {
                for (int i = 0; i < current.subSystemList.Length; i++)
                {
                    if (InjectSystem<T>(ref current.subSystemList[i], ref injected, first))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        static PlayerLoopSystem MakeLoopSystem<T>(PlayerLoopSystem.UpdateFunction callback)
        {
            return new PlayerLoopSystem
            {
                updateDelegate = callback,
                type = typeof(T),
            };
        }

        static void InjectedInitialization()
        {
            injected.RemoveAll(e => e.Owner == null || e.Owner.Equals(null));

            Profiler.BeginSample("PlayerLoopInjector::Initialization");

            global.OnInitialization?.Invoke();

            for (int i = 0; i < injected.Count; i++)
            {
                injected[i].OnInitialization?.Invoke();
            }

            Profiler.EndSample();
        }

        static void InjectedEarlyUpdate()
        {
            Profiler.BeginSample("PlayerLoopInjector::EarlyUpdate");

            global.OnEarlyUpdate?.Invoke();

            for (int i = 0; i < injected.Count; i++)
            {
                injected[i].OnEarlyUpdate?.Invoke();
            }

            Profiler.EndSample();
        }

        static void InjectedFixedUpdate()
        {
            Profiler.BeginSample("PlayerLoopInjector::FixedUpdate");

            global.OnFixedUpdate?.Invoke();

            for (int i = 0; i < injected.Count; i++)
            {
                injected[i].OnFixedUpdate?.Invoke();
            }

            Profiler.EndSample();
        }

        static void InjectedPreUpdate()
        {
            Profiler.BeginSample("PlayerLoopInjector::PreUpdate");

            global.OnPreUpdate?.Invoke();

            for (int i = 0; i < injected.Count; i++)
            {
                injected[i].OnPreUpdate?.Invoke();
            }

            Profiler.EndSample();
        }

        static void InjectedUpdate()
        {
            Profiler.BeginSample("PlayerLoopInjector::Update");

            global.OnUpdate?.Invoke();

            for (int i = 0; i < injected.Count; i++)
            {
                injected[i].OnUpdate?.Invoke();
            }

            Profiler.EndSample();
        }

        static void InjectedPostUpdate()
        {
            Profiler.BeginSample("PlayerLoopInjector::PostUpdate");

            global.OnPostUpdate?.Invoke();

            for (int i = 0; i < injected.Count; i++)
            {
                injected[i].OnPostUpdate?.Invoke();
            }

            Profiler.EndSample();
        }

        static void InjectedEndOfFrameUpdate()
        {
            Profiler.BeginSample("PlayerLoopInjector::EndOfFrame");

            global.OnEndOfFrame?.Invoke();

            for (int i = 0; i < injected.Count; i++)
            {
                injected[i].OnEndOfFrame?.Invoke();
            }

            Profiler.EndSample();
        }

        /// <summary>
        /// Removes the injected system from running
        /// </summary>
        /// <param name="target">target to remove</param>
        public static void Remove(object target)
        {
            int index = injected.FindIndex(e => e.Owner == target);

            if (index != -1)
            {
                injected.RemoveAt(index);
            }

            indirectInjectedLookup.Remove(target);
        }

        /// <summary>
        /// Injects a target inheriting from IPlayerLoop into the PlayerLoop
        /// </summary>
        /// <param name="target">target to inject</param>
        public static void Inject(IPlayerLoop target)
        {
            var callbacks = new InjectedSystemCallbacks
            {
                Owner = (object)target,
                OnInitialization = target.PlayerLoopInitialization,
                OnEarlyUpdate = target.PlayerLoopEarlyUpdate,
                OnFixedUpdate = target.PlayerLoopFixedUpdate,
                OnPreUpdate = target.PlayerLoopPreUpdate,
                OnUpdate = target.PlayerLoopUpdate,
                OnPostUpdate = target.PlayerLoopPostUpdate,
                OnEndOfFrame = target.PlayerLoopEndOfFrame,
            };

            injected.Add(callbacks);
            indirectInjectedLookup.Add(target, callbacks);
        }

        /// <summary>
        /// Injects the given callback into the given LoopInjectionPoint,
        /// target is the owner for this injection
        /// </summary>
        /// <param name="target">owner of the injection</param>
        /// <param name="injectionPoint">point in PlayerLoop to inject at</param>
        /// <param name="callback">delegate to inject</param>
        public static void Inject(object target, LoopInjectionPoint injectionPoint, Action callback)
        {
            if (indirectInjectedLookup.TryGetValue(target, out var callbacks))
            {
                SetUpdateCallback(callbacks, injectionPoint, callback);
                return;
            }

            callbacks = new InjectedSystemCallbacks
            {
                Owner = target,
            };
            SetUpdateCallback(callbacks, injectionPoint, callback);

            injected.Add(callbacks);
            indirectInjectedLookup.Add(target, callbacks);
        }

        /// <summary>
        /// Checks which IPlayerLoop interfaces the target is inheriting from and injects those into the PlayerLoop
        /// </summary>
        /// <param name="target">target object to inject</param>
        public static void Inject(object target)
        {
            if (target is IPlayerLoop playerLoop)
            {
                Inject(playerLoop);
                return;
            }

            if (target is IPlayerLoopInitialization initialization)
            {
                Inject(target, LoopInjectionPoint.Initialization, initialization.PlayerLoopInitialization);
            }

            if (target is IPlayerLoopEarlyUpdate earlyUpdate)
            {
                Inject(target, LoopInjectionPoint.EarlyUpdate, earlyUpdate.PlayerLoopEarlyUpdate);
            }

            if (target is IPlayerLoopPreUpdate preUpdate)
            {
                Inject(target, LoopInjectionPoint.PreUpdate, preUpdate.PlayerLoopPreUpdate);
            }

            if (target is IPlayerLoopUpdate update)
            {
                Inject(target, LoopInjectionPoint.Update, update.PlayerLoopUpdate);
            }

            if (target is IPlayerLoopPostUpdate postUpdate)
            {
                Inject(target, LoopInjectionPoint.PostUpdate, postUpdate.PlayerLoopPostUpdate);
            }

            if (target is IPlayerLoopFixedUpdate fixedUpdate)
            {
                Inject(target, LoopInjectionPoint.FixedUpdate, fixedUpdate.PlayerLoopFixedUpdate);
            }

            if (target is IPlayerLoopEndOfFrame endOfFrame)
            {
                Inject(target, LoopInjectionPoint.EndOfFrame, endOfFrame.PlayerLoopEndOfFrame);
            }
        }

        /// <summary>
        /// Checks if the given target is injected
        /// </summary>
        /// <param name="target">target object to check</param>
        /// <returns>true if injected</returns>
        public static bool IsInjected(object target)
        {
            return indirectInjectedLookup.ContainsKey(target);
        }

        /// <summary>
        /// Clears all injected delegates in Injector::Global
        /// </summary>
        public static void ClearGlobal()
        {
            global.OnInitialization = null;
            global.OnEarlyUpdate = null;
            global.OnFixedUpdate = null;
            global.OnPreUpdate = null;
            global.OnUpdate = null;
            global.OnPostUpdate = null;
            global.OnEndOfFrame = null;
        }

        /// <summary>
        /// Clears all injected systems
        /// </summary>
        public static void ClearInjected()
        {
            injected.Clear();
            indirectInjectedLookup.Clear();
        }

        /// <summary>
        /// Clears everything, both Global and Systems
        /// </summary>
        public static void ClearAll()
        {
            ClearGlobal();
            ClearInjected();
        }

        /// <summary>
        /// Gives the amount of injected systems
        /// </summary>
        /// <returns>count of injected systems</returns>
        public static int Count()
        {
            return injected.Count;
        }

        public static void InjectGlobal(Action callback, LoopInjectionPoint injectionPoint)
        {
            switch(injectionPoint)
            {
                case LoopInjectionPoint.Initialization:
                    global.OnInitialization += callback;
                    break;
                case LoopInjectionPoint.EarlyUpdate:
                    global.OnEarlyUpdate += callback;
                    break;
                case LoopInjectionPoint.FixedUpdate:
                    global.OnFixedUpdate += callback;
                    break;
                case LoopInjectionPoint.PreUpdate:
                    global.OnPreUpdate += callback;
                    break;
                case LoopInjectionPoint.Update:
                    global.OnUpdate += callback;
                    break;
                case LoopInjectionPoint.PostUpdate:
                    global.OnPostUpdate += callback;
                    break;
                case LoopInjectionPoint.EndOfFrame:
                    global.OnEndOfFrame += callback;
                    break;
            }
        }

        public static void UnInjectGlobal(Action callback, LoopInjectionPoint injectionPoint)
        {
            switch(injectionPoint)
            {
                case LoopInjectionPoint.Initialization:
                    global.OnInitialization -= callback;
                    break;
                case LoopInjectionPoint.EarlyUpdate:
                    global.OnEarlyUpdate -= callback;
                    break;
                case LoopInjectionPoint.FixedUpdate:
                    global.OnFixedUpdate -= callback;
                    break;
                case LoopInjectionPoint.PreUpdate:
                    global.OnPreUpdate -= callback;
                    break;
                case LoopInjectionPoint.Update:
                    global.OnUpdate -= callback;
                    break;
                case LoopInjectionPoint.PostUpdate:
                    global.OnPostUpdate -= callback;
                    break;
                case LoopInjectionPoint.EndOfFrame:
                    global.OnEndOfFrame -= callback;
                    break;
            }
        }

        static void SetUpdateCallback(InjectedSystemCallbacks callbacks, LoopInjectionPoint injectionPoint, Action callback)
        {
            switch (injectionPoint)
            {
                case LoopInjectionPoint.Initialization:
                    callbacks.OnInitialization = callback;
                    break;
                case LoopInjectionPoint.EarlyUpdate:
                    callbacks.OnEarlyUpdate = callback;
                    break;
                case LoopInjectionPoint.FixedUpdate:
                    callbacks.OnFixedUpdate = callback;
                    break;
                case LoopInjectionPoint.PreUpdate:
                    callbacks.OnPreUpdate = callback;
                    break;
                case LoopInjectionPoint.Update:
                    callbacks.OnUpdate = callback;
                    break;
                case LoopInjectionPoint.PostUpdate:
                    callbacks.OnPostUpdate = callback;
                    break;
                case LoopInjectionPoint.EndOfFrame:
                    callbacks.OnEndOfFrame = callback;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(injectionPoint), injectionPoint, null);
            }
        }

        #region DEBUG
        /// <summary>
        /// Helper to log out the PlayerLoop into a text file.
        /// File is stored under "Assets/player_loop.txt"
        /// </summary>
        public static void LogPlayerLoop()
        {
            var playerLoop = PlayerLoop.GetCurrentPlayerLoop();

            var value = PrintPlayerLoopTypes(ref playerLoop);
            System.IO.File.WriteAllText(Application.dataPath + "/player_loop.txt", value);
        }

        static string PrintPlayerLoopTypes(ref PlayerLoopSystem current, int indent = 0)
        {
            string msg = "";
            for (int i = 0; i < indent; i++) msg += "\t";
            msg += current.type + "\n";

            if (current.subSystemList != null)
            {
                for (int i = 0; i < current.subSystemList.Length; i++)
                {
                    msg += PrintPlayerLoopTypes(ref current.subSystemList[i], indent + 1);
                }
            }

            return msg;
        }
        #endregion
    }
}
