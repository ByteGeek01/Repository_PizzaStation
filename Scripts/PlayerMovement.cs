using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    PlayerInput input;
    InputAction action;
    InputAction jump;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpHeight = 8f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float rot = 5f;

    private CharacterController cc;
    private Vector3 velocity;
    private bool isGrounded;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    private Vector3 lastMoveDirection = Vector3.zero;

    [SerializeField] private Transform cameraTransform;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        input = GetComponent<PlayerInput>();
        action = input.actions.FindAction("Move");
        jump = input.actions.FindAction("Jump");
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        ApplyGravityAndJump();
    }

    public void Move()
    {
        Vector2 inputDirection = action.ReadValue<Vector2>();

        // Obtener dirección en base a la cámara
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camRight * inputDirection.x + camForward * inputDirection.y;

        if (move.magnitude > 0.1f)
        {
            // Rotar hacia dirección de movimiento
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rot);

            // Actualizar dirección
            lastMoveDirection = move.normalized;

            // Mover al personaje
            cc.Move(move * speed * Time.deltaTime);
        }
    }

    private void ApplyGravityAndJump()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (jump.triggered && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        cc.Move(velocity * Time.deltaTime);
    }
}
