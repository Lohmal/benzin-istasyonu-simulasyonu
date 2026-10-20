using UnityEngine;
using UnityEngine.InputSystem;

namespace BenzinIstasyonu
{
    /// <summary>
    /// Oyuncuyu WASD / ok tuşları / gamepad sol çubuğu ile yürütür.
    /// Hareket kameraya göre yapılır: W her zaman ekranda "yukarı" demektir.
    /// Space / gamepad güney tuşu (A / Çarpı) ile zıplar.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Tooltip("Yürüme hızı (metre/saniye)")]
        [SerializeField] float moveSpeed = 6f;

        [Tooltip("Karakterin yürüdüğü yöne dönme hızı (derece/saniye)")]
        [SerializeField] float turnSpeed = 720f;

        [Tooltip("Yerçekimi (negatif olmalı)")]
        [SerializeField] float gravity = -20f;

        // YENİ
        [Tooltip("Zıplama yüksekliği (metre)")]
        [SerializeField] float jumpHeight = 1.5f;

        // YENİ
        [Tooltip("Yerden ayrıldıktan sonra hâlâ zıplanabilen kısa süre (saniye). Kenarlarda daha rahat his verir.")]
        [SerializeField] float coyoteTime = 0.1f;

        // YENİ
        [Tooltip("Yere inmeden hemen önce basılan zıplama tuşunun hatırlanma süresi (saniye)")]
        [SerializeField] float jumpBufferTime = 0.1f;

        CharacterController controller;
        Transform cameraTransform;
        float verticalVelocity;
        float coyoteTimer;      // YENİ
        float jumpBufferTimer;  // YENİ

        /// <summary>
        /// Başka bir scriptin (ör. ClickToMove) verdiği yürüme yönü.
        /// Klavye / gamepad kullanılırken yok sayılır.
        /// </summary>
        public Vector3 AutoMove { get; set; }

        /// <summary>Bu karede klavye ya da gamepad ile yön verildi mi?</summary>
        public bool HasDirectInput { get; private set; }

        void Awake()
        {
            controller = GetComponent<CharacterController>();
            if (Camera.main != null) cameraTransform = Camera.main.transform;
        }

        void Update()
        {
            Vector2 input = ReadInput();
            HasDirectInput = input.sqrMagnitude > 0.01f;
            Vector3 move = HasDirectInput ? ToWorldDirection(input) : AutoMove;

            if (move.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(move);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }

            bool grounded = controller.isGrounded;

            // Yerdeyken aşağı hızı sıfırlanır.
            if (grounded && verticalVelocity < 0f) verticalVelocity = -2f;

            // YENİ: zamanlayıcılar
            coyoteTimer = grounded ? coyoteTime : coyoteTimer - Time.deltaTime;
            jumpBufferTimer = JumpPressedThisFrame() ? jumpBufferTime : jumpBufferTimer - Time.deltaTime;

            // YENİ: zıplama
            if (jumpBufferTimer > 0f && coyoteTimer > 0f)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpBufferTimer = 0f;
                coyoteTimer = 0f;
            }

            // Havadayken yerçekimi birikir.
            verticalVelocity += gravity * Time.deltaTime;

            Vector3 velocity = move * moveSpeed + Vector3.up * verticalVelocity;
            controller.Move(velocity * Time.deltaTime);
        }

        // YENİ
        static bool JumpPressedThisFrame()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame) return true;

            Gamepad gamepad = Gamepad.current;
            if (gamepad != null && gamepad.buttonSouth.wasPressedThisFrame) return true;

            return false;
        }

        // Ekrandaki yönü (yukarı/sağ) dünyadaki yöne çevirir.
        Vector3 ToWorldDirection(Vector2 input)
        {
            Vector3 forward = Vector3.forward;
            Vector3 right = Vector3.right;
            if (cameraTransform != null)
            {
                forward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
                right = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;
            }
            Vector3 move = forward * input.y + right * input.x;
            return move.sqrMagnitude > 1f ? move.normalized : move;
        }

        static Vector2 ReadInput()
        {
            Vector2 value = Vector2.zero;

            Keyboard keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) value.y += 1f;
                if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) value.y -= 1f;
                if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) value.x += 1f;
                if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) value.x -= 1f;
            }

            Gamepad gamepad = Gamepad.current;
            if (gamepad != null) value += gamepad.leftStick.ReadValue();

            return Vector2.ClampMagnitude(value, 1f);
        }
    }
}