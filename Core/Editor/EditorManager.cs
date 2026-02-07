using BepInEx.Logging;

namespace GorillaLevelEditor.Core.Editor
{
    internal static class EditorManager
    {
        public static bool InsidePlaymode => EditorState == EditorState.Playmode;

        public static EditorState EditorState { get; private set; }

        public static void SetEditorState(EditorState state)
        {
            if (EditorState == state)
                return;

            EditorState = state;
            Plugin.LogSource.LogDebug($"Set editor state to {state}");
        }
    }
}