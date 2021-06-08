using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PlayerLoopInjector.Tests
{
    class EarlyUpdateSystem : IPlayerLoopEarlyUpdate
    {
        public float Count = 0;

        public void PlayerLoopEarlyUpdate()
        {
            Count++;
        }
    }

    class PreUpdateSystem : IPlayerLoopPreUpdate
    {
        public float Count;

        public void PlayerLoopPreUpdate()
        {
            Count++;
        }
    }

    class UpdateSystem : IPlayerLoopUpdate
    {
        public float Count;

        public void PlayerLoopUpdate()
        {
            Count++;
        }
    }

    class PostUpdateSystem : IPlayerLoopPostUpdate
    {
        public float Count;

        public void PlayerLoopPostUpdate()
        {
            Count++;
        }
    }

    class EndOfFrameSystem : IPlayerLoopEndOfFrame
    {
        public float Count;

        public void PlayerLoopEndOfFrame()
        {
            Count++;
        }
    }

    class FixedUpdateSystem : IPlayerLoopFixedUpdate
    {
        public float Count;

        public void PlayerLoopFixedUpdate()
        {
            Count++;
        }
    }

    class InjectedSystem : IPlayerLoop
    {
        public Queue<int> Order = new Queue<int>();

        public void PlayerLoopEarlyUpdate()
        {
            Order.Enqueue(1);
        }

        public void PlayerLoopEndOfFrame()
        {
            Order.Enqueue(5);
        }

        public void PlayerLoopFixedUpdate()
        {

        }

        public void PlayerLoopInitialization()
        {
            Order.Clear();
            Order.Enqueue(0);
        }

        public void PlayerLoopPostUpdate()
        {
            Order.Enqueue(4);
        }

        public void PlayerLoopPreUpdate()
        {
            Order.Enqueue(2);
        }

        public void PlayerLoopUpdate()
        {
            Order.Enqueue(3);
        }
    }

    public class InjectorTests
    {
        [UnityTest]
        public IEnumerator InjectEarlyUpdate()
        {
            var earlyUpdateSystem = new EarlyUpdateSystem();
            Injector.Inject(earlyUpdateSystem);
            yield return null;

            Assert.AreEqual(1, earlyUpdateSystem.Count);

            for (int i = 0; i < 10; i++)
            {
                yield return null;
            }

            Assert.AreEqual(11, earlyUpdateSystem.Count);
        }

        [UnityTest]
        public IEnumerator InjectPreUpdate()
        {
            var preUpdateSystem = new PreUpdateSystem();
            Injector.Inject(preUpdateSystem);
            yield return null;

            Assert.AreEqual(1, preUpdateSystem.Count);

            for (int i = 0; i < 10; i++)
            {
                yield return null;
            }

            Assert.AreEqual(11, preUpdateSystem.Count);
        }

        [UnityTest]
        public IEnumerator InjectUpdate()
        {
            var updateSystem = new UpdateSystem();
            Injector.Inject(updateSystem);
            yield return null;

            Assert.AreEqual(1, updateSystem.Count);

            for (int i = 0; i < 10; i++)
            {
                yield return null;
            }

            Assert.AreEqual(11, updateSystem.Count);
        }

        [UnityTest]
        public IEnumerator InjectPostUpdate()
        {
            var postUpdateSystem = new PostUpdateSystem();
            Injector.Inject(postUpdateSystem);
            yield return null;

            Assert.AreEqual(1, postUpdateSystem.Count);

            for (int i = 0; i < 10; i++)
            {
                yield return null;
            }

            Assert.AreEqual(11, postUpdateSystem.Count);
        }

        [UnityTest]
        public IEnumerator InjectEndOfFrame()
        {
            var endOfFrameSystem = new EndOfFrameSystem();
            Injector.Inject(endOfFrameSystem);
            yield return null;

            Assert.AreEqual(1, endOfFrameSystem.Count);

            for (int i = 0; i < 10; i++)
            {
                yield return null;
            }

            Assert.AreEqual(11, endOfFrameSystem.Count);
        }

        [UnityTest]
        public IEnumerator InjectFixedUpdate()
        {
            var fixedUpdateSystem = new FixedUpdateSystem();
            Injector.Inject(fixedUpdateSystem);
            yield return new WaitForFixedUpdate();

            Assert.AreEqual(1, fixedUpdateSystem.Count);

            for (int i = 0; i < 10; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            Assert.AreEqual(11, fixedUpdateSystem.Count);
        }

        [UnityTest]
        public IEnumerator OrderOfInjectedSystem()
        {
            var injectedSystem = new InjectedSystem();
            Injector.Inject(injectedSystem);

            yield return null;
            yield return new WaitForEndOfFrame();

            string order = injectedSystem.Order.Aggregate("", (acc, val) => acc + ", " + val);
            Assert.AreEqual(6, injectedSystem.Order.Count);

            for (int i = 0; i < injectedSystem.Order.Count; i++)
            {
                Assert.AreEqual(i, injectedSystem.Order.Dequeue(), order);
            }
        }
    }
}
