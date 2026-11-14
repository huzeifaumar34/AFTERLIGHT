using UnityEngine;

namespace Afterlight
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerMotor : MonoBehaviour
    {
        [SerializeField] private float walkSpeed = 4.0f;
        [SerializeField] private float gravity = -22f;
        [SerializeField] private float turnSpeed = 11f;
        [SerializeField] private float interactRadius = 2.35f;

        private CharacterController controller;
        private HumanoidRigAnimator animator;
        private OnScreenJoystick joystick;
        private bool inputLocked;
        private bool controlsEnabled;
        private float verticalVelocity;
        private Camera mainCamera;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            animator = GetComponent<HumanoidRigAnimator>();
            mainCamera = Camera.main;
        }

        public void Configure(OnScreenJoystick touchJoystick)
        {
            joystick = touchJoystick;
        }

        public void SetControlsEnabled(bool enabled)
        {
            controlsEnabled = enabled;
            if (!enabled && animator) animator.SetMoving(false);
        }

        public void SetInputLocked(bool locked)
        {
            inputLocked = locked;
            if (locked && animator) animator.SetMoving(false);
        }

        private void Update()
        {
            if (!controlsEnabled || inputLocked) return;

            Vector2 stick = joystick != null ? joystick.Direction : Vector2.zero;
            float keyboardX = (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) ? 1f : 0f)
                - (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ? 1f : 0f);
            float keyboardY = (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) ? 1f : 0f)
                - (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) ? 1f : 0f);
            float horizontal = Mathf.Clamp(keyboardX + stick.x, -1f, 1f);
            float vertical = Mathf.Clamp(keyboardY + stick.y, -1f, 1f);
            Vector3 input = new Vector3(horizontal, 0f, vertical);

            if (input.sqrMagnitude > 1f) input.Normalize();

            Vector3 cameraForward = mainCamera ? mainCamera.transform.forward : Vector3.forward;
            Vector3 cameraRight = mainCamera ? mainCamera.transform.right : Vector3.right;
            cameraForward.y = 0f;
            cameraRight.y = 0f;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 move = cameraForward * input.z + cameraRight * input.x;
            if (move.sqrMagnitude > 0.001f)
            {
                Quaternion target = Quaternion.LookRotation(move.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, turnSpeed * Time.deltaTime);
            }

            if (controller.isGrounded && verticalVelocity < 0f)
                verticalVelocity = -2f;
            verticalVelocity += gravity * Time.deltaTime;

            Vector3 velocity = move * walkSpeed;
            velocity.y = verticalVelocity;
            controller.Move(velocity * Time.deltaTime);

            if (animator) animator.SetMoving(move.sqrMagnitude > 0.02f);

            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))
                TryInteract();
        }

        public void TryInteract()
        {
            if (!controlsEnabled || inputLocked) return;

            Vector3 origin = transform.position + Vector3.up * 0.9f + transform.forward * 0.55f;
            Collider[] hits = Physics.OverlapSphere(origin, interactRadius);
            WorldInteractable best = null;
            float bestDistance = float.MaxValue;

            for (int i = 0; i < hits.Length; i++)
            {
                WorldInteractable candidate = hits[i].GetComponentInParent<WorldInteractable>();
                if (candidate == null) continue;
                float distance = (candidate.transform.position - transform.position).sqrMagnitude;
                if (distance < bestDistance)
                {
                    best = candidate;
                    bestDistance = distance;
                }
            }

            if (best != null)
            {
                var ui = FindFirstObjectByType<GameUI>();
                if (ui != null) best.Interact(this, ui);
            }
        }
    }
}
