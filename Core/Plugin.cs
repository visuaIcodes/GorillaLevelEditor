using BepInEx;
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
        private ModuleManager ModuleManager;
        private EditorCamera Camera;

        public void Awake()
        {
            GorillaTagger.OnPlayerSpawned(() =>
            {
                ModuleManager = new ModuleManager();
                ModuleManager.InitializeBaseModules();

                Camera = new EditorCamera();
                Camera.Initialize();
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