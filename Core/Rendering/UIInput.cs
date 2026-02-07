using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace GorillaLevelEditor.Core.Rendering
{
    internal class UIInput
    {
        private static List<char> textInput = new();

        public static void Initialize()
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.KeyMap[(int)ImGuiKey.Tab] = (int)KeyCode.Tab;
            io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)KeyCode.LeftArrow;
            io.KeyMap[(int)ImGuiKey.RightArrow] = (int)KeyCode.RightArrow;
            io.KeyMap[(int)ImGuiKey.UpArrow] = (int)KeyCode.UpArrow;
            io.KeyMap[(int)ImGuiKey.DownArrow] = (int)KeyCode.DownArrow;
            io.KeyMap[(int)ImGuiKey.PageUp] = (int)KeyCode.PageUp;
            io.KeyMap[(int)ImGuiKey.PageDown] = (int)KeyCode.PageDown;
            io.KeyMap[(int)ImGuiKey.Home] = (int)KeyCode.Home;
            io.KeyMap[(int)ImGuiKey.End] = (int)KeyCode.End;
            io.KeyMap[(int)ImGuiKey.Insert] = (int)KeyCode.Insert;
            io.KeyMap[(int)ImGuiKey.Delete] = (int)KeyCode.Delete;
            io.KeyMap[(int)ImGuiKey.Backspace] = (int)KeyCode.Backspace;
            io.KeyMap[(int)ImGuiKey.Space] = (int)KeyCode.Space;
            io.KeyMap[(int)ImGuiKey.Enter] = (int)KeyCode.Return;
            io.KeyMap[(int)ImGuiKey.Escape] = (int)KeyCode.Escape;
            io.KeyMap[(int)ImGuiKey.A] = (int)KeyCode.A;
            io.KeyMap[(int)ImGuiKey.C] = (int)KeyCode.C;
            io.KeyMap[(int)ImGuiKey.V] = (int)KeyCode.V;
            io.KeyMap[(int)ImGuiKey.X] = (int)KeyCode.X;
            io.KeyMap[(int)ImGuiKey.Y] = (int)KeyCode.Y;
            io.KeyMap[(int)ImGuiKey.Z] = (int)KeyCode.Z;

            Keyboard.current.onTextInput += OnTextInput;
        }

        public static void Update()
        {
            var io = ImGui.GetIO();
            io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;
            io.ConfigFlags |= ImGuiConfigFlags.NavEnableGamepad;
            io.DeltaTime = Time.deltaTime;
            io.WantCaptureMouse = true;
            io.WantCaptureKeyboard = true;
            io.DisplaySize = new Vector2(Screen.width, Screen.height);
            io.DisplayFramebufferScale = Vector2.one;

            if (Mouse.current != null)
            {
                var mouse = Mouse.current;
                io.MouseDown[0] = mouse.leftButton.isPressed;
                io.MouseDown[1] = mouse.rightButton.isPressed;
                io.MouseDown[2] = mouse.middleButton.isPressed;

                var pos = mouse.position.ReadValue();
                io.MousePos = new Vector2(pos.x, Screen.height - pos.y);
                io.MouseWheel = mouse.scroll.ReadValue().y;
            }

            if (Keyboard.current != null)
            {
                io.KeyCtrl = Keyboard.current.leftCtrlKey.isPressed || Keyboard.current.rightCtrlKey.isPressed;
                io.KeyAlt = Keyboard.current.leftAltKey.isPressed || Keyboard.current.rightAltKey.isPressed;
                io.KeyShift = Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;
                io.KeySuper = Keyboard.current.leftMetaKey.isPressed || Keyboard.current.rightMetaKey.isPressed;

                foreach (var c in textInput)
                    io.AddInputCharacter(c);

                textInput.Clear();
            }
        }

        private static void OnTextInput(char character)
            => textInput.Add(character);
    }
}