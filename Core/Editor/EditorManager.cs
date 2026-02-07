using BepInEx.Logging;
using GorillaLevelEditor.Constants;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GorillaLevelEditor.Core.Editor
{
    internal static class EditorManager
    {
        public static bool InsidePlaymode => EditorState == EditorState.Playmode;

        public static EditorState EditorState { get; private set; }

        public static GameObject CurrentSelectedGameObject { get; private set; }

        public static EditorCamera Camera { get; private set; }

        public static void Initialize(EditorCamera camera)
        {
            Camera = camera;
        }

        public static void SetEditorState(EditorState state)
        {
            if (EditorState == state)
                return;

            GTPlayer.Instance.enabled = EditorState == EditorState.Playmode;

            EditorState = state;
            Plugin.LogSource.LogDebug($"Set editor state to {state}");
        }

        public static void Update()
        {
            Camera.Update();
            if (EditorState == EditorState.Editing)
            {
                HandleEditingState();
            }
        }


        private static void HandleEditingState()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                int layerMask =
                    (1 << LayerMask.NameToLayer("Default")) |
                    (1 << LayerMask.NameToLayer("GorillaObject")) |
                    (1 << LayerMask.NameToLayer("RopeSwing")) |
                    (1 << LayerMask.NameToLayer("Prop")) |
                    (1 << LayerMask.NameToLayer("Bake"));

                if (Physics.Raycast(Camera.GetPosition(), Camera.GetForward(), out RaycastHit hit, EditorConstants.MAX_SELECT_GAMEOBJECT_DISTANCE, layerMask))
                {
                    CurrentSelectedGameObject = hit.collider.gameObject;
                    if (CurrentSelectedGameObject.TryGetComponent<Renderer>(out Renderer ren))
                    {
                        ren.material.color = Color.red;
                    }
                    Debug.Log("Selected: " + CurrentSelectedGameObject.name);
                }
            }
        }
    }
}