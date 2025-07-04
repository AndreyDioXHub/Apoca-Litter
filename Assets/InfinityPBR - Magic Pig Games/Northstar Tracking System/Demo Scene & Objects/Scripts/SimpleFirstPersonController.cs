using UnityEngine;
using UnityEngine.InputSystem;

namespace MagicPigGames {
    [RequireComponent(typeof(CharacterController))]
    public class SimpleFirstPersonController : MonoBehaviour {
        public float moveSpeed = 3.0f;
        public float turnSpeed = 300.0f;
        public float tiltSpeed = 200.0f;
        public float maxTilt = 40.0f;

        private CharacterController characterController;
        private float tilt = 0.0f;
        private Vector2 moveInput;
        private Vector2 lookInput;

        void Start() {
            characterController = GetComponent<CharacterController>();
            //Cursor.lockState = CursorLockMode.Locked;
        }

        void Update() {
            // Mouse Look Control
            float turn = lookInput.x * turnSpeed * Time.deltaTime;
            transform.Rotate(0, turn, 0);

            tilt -= lookInput.y * tiltSpeed * Time.deltaTime;
            tilt = Mathf.Clamp(tilt, -maxTilt, maxTilt);
            Camera.main.transform.localRotation = Quaternion.Euler(tilt, 0, 0);

            // Movement Control
            float moveDirectionY = characterController.velocity.y;
            Vector3 forwardMovement = transform.TransformDirection(Vector3.forward) * moveInput.y * moveSpeed;
            Vector3 rightMovement = transform.TransformDirection(Vector3.right) * moveInput.x * moveSpeed;
            Vector3 move = forwardMovement + rightMovement;

            if (!characterController.isGrounded) {
                move.y = moveDirectionY;
            }

            characterController.SimpleMove(move);
        }

        public void OnMove(InputAction.CallbackContext context) {
            moveInput = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context) {
            lookInput = context.ReadValue<Vector2>();
        }
    }
}
