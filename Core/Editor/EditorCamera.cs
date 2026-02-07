using BepInEx;
using GorillaLevelEditor.Constants;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GorillaLevelEditor.Core.Editor
{
    internal class EditorCamera
    {
        private Camera camera = null;
        private Vector3 position = Vector3.zero;

        // mouse smoothing variables
        private Vector2 targetLook;
        private Vector2 smoothLook;
        private Vector2 lookVelocity;

        GTPlayer player => GTPlayer.Instance;

        public void Initialize()
        {
            GameObject.Destroy(GorillaTagger.Instance.thirdPersonCamera);
            camera = GTPlayer.Instance.headCollider.GetComponent<Camera>();
        }

        public void Update()
        {
            GTPlayer.Instance.enabled = EditorManager.InsidePlaymode;

            UpdateMouse(Time.deltaTime);
            UpdateMovement(Time.deltaTime);
        }

        public void UpdateMouse(float dt)
        {
            if (!UnityInput.Current.GetMouseButton(1))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                return;
            }

            float sensitivity = EditorConstants.DEFAULT_MOUSE_SENSITIVTY;

            Vector2 mouseDelta = Mouse.current.delta.value;
            targetLook.x += mouseDelta.x * sensitivity * dt;
            targetLook.y -= mouseDelta.y * sensitivity * dt;
            targetLook.y = Mathf.Clamp(targetLook.y, -89, 89);

            smoothLook = Vector2.SmoothDamp(smoothLook, targetLook, ref lookVelocity, EditorConstants.MOUSE_LOOK_SMOOTHNESS,
                Mathf.Infinity, dt);

            Quaternion cameraRotation = Quaternion.Euler(smoothLook.y, smoothLook.x, 0);
            Quaternion playerRotation = Quaternion.Euler(0, smoothLook.x, 0);

            camera.transform.rotation = cameraRotation;
            player.transform.rotation = playerRotation;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void UpdateMovement(float dt)
        {
            if (EditorManager.InsidePlaymode)
                return;

            Vector3 moveDelta = Vector3.zero;
            float currentMoveSpeed = EditorConstants.DEFAULT_MOVE_SPEED;

            if (UnityInput.Current.GetKey(KeyCode.W))
                moveDelta += camera.transform.forward;
            if (UnityInput.Current.GetKey(KeyCode.S))
                moveDelta -= camera.transform.forward;
            if (UnityInput.Current.GetKey(KeyCode.A))
                moveDelta -= camera.transform.right;
            if (UnityInput.Current.GetKey(KeyCode.D))
                moveDelta += camera.transform.right;
            if (UnityInput.Current.GetKey(KeyCode.Space))
                moveDelta += camera.transform.up;
            if (UnityInput.Current.GetKey(KeyCode.LeftControl) || UnityInput.Current.GetKey(KeyCode.RightControl))
                moveDelta += -camera.transform.up;

            if (UnityInput.Current.GetKey(KeyCode.LeftShift) || UnityInput.Current.GetKey(KeyCode.RightShift))
                currentMoveSpeed *= EditorConstants.SHIFT_MOVE_MULTIPLIER;

            moveDelta.Normalize();
            SetPosition(position + moveDelta * currentMoveSpeed * dt);
        }

        public void SetPosition(Vector3 newPosition)
        {
            float t = EditorConstants.LERP * Time.deltaTime;

            position = newPosition;
            player.transform.position = Vector3.Lerp(player.transform.position, newPosition, t);
            camera.transform.position = Vector3.Lerp(camera.transform.position, newPosition, t);
        }
    }
}