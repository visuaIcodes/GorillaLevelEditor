using BepInEx;
using BepInEx.Logging;
using GorillaLevelEditor.Constants;
using GorillaLevelEditor.Core.Editor;
using GorillaLevelEditor.Core.Rendering;
using System.ComponentModel;
using UnityEngine;

namespace GorillaLevelEditor.Core
{
    [BepInPlugin(PluginData.PLUGIN_GUID, PluginData.PLUGIN_NAME, PluginData.PLUGIN_VERSION)]
    [Description(PluginData.PLUGIN_DESCRIPTION)]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource LogSource { get; private set; }

        private EditorCamera Camera;
        private CoroutineManager CoroutineManager;
        private GUIRenderer GUIRenderer;

        private GameObject ManagerGo;

        public void Awake()
        {
            LogSource = Logger;
            Init();
        }

        public void Init()
        {
            GorillaTagger.OnPlayerSpawned(() =>
            {
                ManagerGo = new GameObject("GorillaLevelEditor_manager");
                CoroutineManager = ManagerGo.AddComponent<CoroutineManager>();
                GUIRenderer = ManagerGo.AddComponent<GUIRenderer>();
                GUIRenderer.Init();

                Camera = new EditorCamera();
                Camera.Initialize();

                UI.Load();

                EditorManager.Initialize(Camera);
                EditorManager.SetEditorState(EditorState.Editing);
            });
        }

        public void Update()
        {
            EditorManager.Update();
        }

        public void OnApplicationQuit()
        {
            UI.Cleanup();
        }
    }
}