using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMov : MonoBehaviour
{
    [Header("Camera")]
    private float MouseMov = 2f;
    private float VertRotation = 0f;
    private Transform cameraTransform;

    [Header("Move")]
    private Rigidbody rb;
    public float speed = 5f;
    private float moveHorizontal;
    private float moveForward;

    public float jump = 8f;
    public float fall = 2.5f;
    public float ascending = 2f;
    private bool isGrounded = true;

    [Header("Ground")]
    public LayerMask ground;

    private float groundCheckTimer = 0f;
    private float groundCheckDelay = 0.3f;
    private float playerHeight;
    private float raycastDistance;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        cameraTransform = Camera.main.transform;

        playerHeight = GetComponent<CapsuleCollider>().height * transform.localScale.y;
        raycastDistance = (playerHeight / 2) * 0.2f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Update()
    {
        moveHorizontal = Input.GetAxisRaw("Horizontal");
        moveForward = Input.GetAxisRaw("Vertical");

        RotateCamera();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        if(!isGrounded && groundCheckTimer <= 0f)
        {
            Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
            isGrounded = Physics.Raycast(rayOrigin, Vector3.down, raycastDistance, ground);
        }
        else
        {
            groundCheckTimer -= Time.deltaTime;
        }
    }

    public void FixedUpdate()
    {
        MovePlayer();
        ApplyJumpPhysics();
    }

    public void MovePlayer()
    {
        Vector3 move = (transform.right * moveHorizontal + transform.forward * moveForward).normalized;
        Vector3 targetVelocity = move * speed;

        Vector3 velocity = rb.linearVelocity;
        velocity.x = targetVelocity.x;
        velocity.z = targetVelocity.z;
        rb.linearVelocity = velocity;

        if (isGrounded && moveHorizontal == 0 && moveForward == 0) 
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    public void RotateCamera()
    {
        float horizontalRot = Input.GetAxis("Mouse X") * MouseMov;
        transform.Rotate(0, horizontalRot, 0);

        VertRotation -= Input.GetAxis("Mouse Y") * MouseMov;
        VertRotation = Mathf.Clamp(VertRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(VertRotation, 0, 0);
    }

    public void Jump()
    {
        isGrounded = false;
        groundCheckTimer = groundCheckDelay;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jump, rb.linearVelocity.z);
    }

    public void ApplyJumpPhysics()
    {
        if (rb.linearVelocity.y < 0)
        {
            // Falling: Apply fall multiplier to make descent faster
            rb.linearVelocity += Vector3.up * Physics.gravity.y * fall * Time.fixedDeltaTime;
        } // Rising
        else if (rb.linearVelocity.y > 0)
        {
            // Rising: Change multiplier to make player reach peak of jump faster
            rb.linearVelocity += Vector3.up * Physics.gravity.y * ascending * Time.fixedDeltaTime;
        }
    }
}
