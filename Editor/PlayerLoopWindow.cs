using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PlayerLoopInjector.Editor
{
    internal class PlayerLoopWindow : EditorWindow
    {
        [MenuItem("Window/Injected PlayerLoop")]
        public static PlayerLoopWindow Open()
        {
            var window = EditorWindow.GetWindow<PlayerLoopWindow>();
            window.name = "Injected PlayerLoop";
            window.titleContent = new GUIContent(window.name);

            window.minSize = new UnityEngine.Vector2(300, 500);

            window.Show();
            return window;
        }

        void OnEnable()
        {
            rootVisualElement.Add(GetGUI());
            Injector.Global.OnInitialization += () =>
            {
                rootVisualElement.Clear();
                rootVisualElement.Add(GetGUI());
            };
        }

        VisualElement GetGUI()
        {
            var container = new VisualElement();
            var header = new Label("Injected PlayerLoop");
            header.style.fontSize = 24;
            header.style.unityTextAlign = TextAnchor.MiddleCenter;
            header.style.backgroundColor = Color.black;
            container.Add(header);

            if (!Application.isPlaying)
            {
                return container;
            }

            var systems = GetSystems();

            container.Add(GetDelegateInfo("On Initialization", systems.Select(e => e.OnInitialization)));
            container.Add(GetDelegateInfo("On EarlyUpdate", systems.Select(e => e.OnEarlyUpdate)));
            container.Add(GetDelegateInfo("On PreUpdate", systems.Select(e => e.OnPreUpdate)));
            container.Add(GetDelegateInfo("On Update", systems.Select(e => e.OnUpdate)));
            container.Add(GetDelegateInfo("On PostUpdate", systems.Select(e => e.OnPostUpdate)));
            container.Add(GetDelegateInfo("On EndOfFrame", systems.Select(e => e.OnEndOfFrame)));

            return container;
        }

        static VisualElement GetDelegateInfo(string name, IEnumerable<System.Action> delegates)
        {
            var container = new VisualElement();
            var header = new Label(name);
            header.style.fontSize = 18;
            container.Add(header);

            foreach (var del in delegates)
            {
                foreach (var target in del.GetInvocationList())
                {
                    var delName = new Label("- " + target.Target.ToString());
                    delName.style.marginLeft = 20;
                    container.Add(delName);
                }
            }

            return container;
        }

        static List<InjectedSystemCallbacks> GetSystems()
        {
            return (List<InjectedSystemCallbacks>)typeof(Injector).GetField("injected", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        }
    }
}