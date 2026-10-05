using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Oyuncuyu WASD / ok tuşları / gamepad sol çubuğu ile yürütür.
/// Hareket kameraya göre yapılır: W her zaman ekranda "yukarı" demektir.
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

    CharacterController controller;
    Transform cameraTransform;
    float verticalVelocity;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (Camera.main != null) cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        Vector2 input = ReadInput();
        Vector3 move = ToWorldDirection(input);

        if (move.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        // Yerdeyken aşağı hızı sıfırlanır, havadayken yerçekimi birikir.
        if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = move * moveSpeed + Vector3.up * verticalVelocity;
        controller.Move(velocity * Time.deltaTime);
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
