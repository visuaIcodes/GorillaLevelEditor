using BepInEx;
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

        private float moveSpeed = 10;
        private float pitch = 0;
        private float yaw = 0;
        private float mouseSensitivity = 13;

        private float lerp = 35;

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

            Vector2 delta = Mouse.current.delta.value;

            yaw += delta.x * mouseSensitivity * dt;
            pitch -= delta.y * mouseSensitivity * dt;
            pitch = Mathf.Clamp(pitch, -89, 89);

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
            camera.transform.rotation = Quaternion.Lerp(camera.transform.rotation, rotation, lerp * Time.deltaTime);
            player.transform.rotation = Quaternion.Lerp(player.transform.rotation, Quaternion.Euler(0, yaw, 0), lerp * Time.deltaTime);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void UpdateMovement(float dt)
        {
            if (EditorManager.InsidePlaymode)
                return;

            Vector3 moveDelta = Vector3.zero;
            float currentMoveSpeed = moveSpeed;

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
                currentMoveSpeed *= 3;

            moveDelta.Normalize();
            SetPosition(position + moveDelta * currentMoveSpeed * dt);
        }

        public void SetPosition(Vector3 newPosition)
        {
            float t = lerp * Time.deltaTime;

            position = newPosition;
            player.transform.position = Vector3.Lerp(player.transform.position, newPosition, t);
            camera.transform.position = Vector3.Lerp(camera.transform.position, newPosition, t);
        }
    }
}