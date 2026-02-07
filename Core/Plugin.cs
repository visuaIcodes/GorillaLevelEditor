using BepInEx;
using BepInEx.Logging;
using GorillaLevelEditor.Constants;
using GorillaLevelEditor.Core.Editor;
using GorillaLevelEditor.Core.Modules;
using System.ComponentModel;

namespace GorillaLevelEditor.Core
{
    [BepInPlugin(PluginData.PLUGIN_GUID, PluginData.PLUGIN_NAME, PluginData.PLUGIN_VERSION)]
    [Description(PluginData.PLUGIN_DESCRIPTION)]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource LogSource { get; private set; }

        private ModuleManager ModuleManager;
        private EditorCamera Camera;

        public void Awake()
        {
            LogSource = Logger;
            GorillaTagger.OnPlayerSpawned(() =>
            {
                ModuleManager = new ModuleManager();
                ModuleManager.InitializeBaseModules();

                Camera = new EditorCamera();
                Camera.Initialize();

                EditorManager.SetEditorState(EditorState.Editing);
            });
        }

        public void Update()
        {
            ModuleManager.OnUpdate();
            Camera.Update();
        }

        public void OnGUI()
        {
            ModuleManager.OnRenderUI();
        }
    }
}