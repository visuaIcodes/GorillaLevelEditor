using ImGuiNET;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace GorillaLevelEditor.Core.Rendering
{
    internal class UI
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetDllDirectory(string lpPathName);

        [DllImport("kernel32", SetLastError = true)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        public static void Load()
        {
            LoadDll("Dependencies/cimgui.dll");
            LoadDll("Dependencies/ImGui.NET.dll");

            RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
        }

        public static void Cleanup()
        {
            RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
            GameObject.Destroy(GUIRenderer.Instance.gameObject);
        }

        private static void LoadDll(string path)
        {
            string filePath = Path.Combine(BepInEx.Paths.PluginPath, "GorillaLevelEditor", path);
            if (!File.Exists(filePath))
                throw new Exception("Invalid file path!");

            IntPtr res = LoadLibrary(filePath);
        }

        private static void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if (camera != Camera.main)
                return;

            UIInput.Update();
            ImGui.NewFrame();

            RenderUIContent();

            ImGui.Render();

            ImDrawDataPtr drawData = ImGui.GetDrawData();
            if (drawData.CmdListsCount == 0)
                return;

            GUIRenderer.Instance.RenderDrawDataSRP(context, drawData);
        }

        private static void RenderUIContent()
        {
            ImGui.ShowDemoWindow();
        }
    }
}