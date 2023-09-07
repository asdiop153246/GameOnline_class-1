using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerBasicScript : MonoBehaviour
{
    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    public Transform cam;
    private Vector3 currentRotation;
    public float mouseSensitivity = 2.0f;
    public float speed = 5.0f;
    public float rotationSpeed = 10.0f;
    public float jumpCooldown;
    public float jumpForce;
    public float RunSpeed = 8.0f;

    [Header("Stamina")]
    public float staminaConsumptionRate = 10f;
    public float staminaRegenRate = 5f;
    [SerializeField] public float stamina = 100f;
    [SerializeField] private float maxStamina = 100f;

    private Animator animator;
    private Rigidbody rb;
    private bool walking;
    private bool running;
    private bool readyToJump = true;
    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;
    Vector3 moveDirection;
    Vector3 movement;
    float cameraverticalRotation = 0f;
    public bool isCursorLocked;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        stamina = maxStamina;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        running = false;
        isCursorLocked = true;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        //float verticalInput = Input.GetAxis("Vertical");
        //walking = Mathf.Abs(verticalInput) > 0.01f;
        //running = Input.GetKey(sprintKey) && !walking;
    }

    private void FixedUpdate()
    {
        checkMovement();
        MoveForward();
        JumpInput();
        LockCursor();
        UpdateStamina();
    }
    private void MoveForward()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        Debug.Log("walking =" + walking);
        movement = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.MovePosition(transform.position + movement * speed * Time.deltaTime);

        if (movement.x != 0 || movement.y != 0 || movement.z != 0)
        {
            animator.SetBool("Walk", true);
        }
        else
        {
            animator.SetBool("Walk", false);
        }

        if (Input.GetKey(sprintKey) && stamina > 0)
        {
            rb.MovePosition(transform.position + movement * RunSpeed * Time.deltaTime);
        }
    }
    private void checkMovement()
    {

        if (movement.x != 0 || movement.y != 0 || movement.z != 0)
        {
            walking = true;
        }
        else
        {
            walking = false;
        }
    }
    private void JumpInput()
    {
        if (Input.GetKey(jumpKey) && readyToJump && stamina > 0)
        {
            readyToJump = false;
            stamina -= 20;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void UpdateStamina()
    {
        if (Input.GetKey(sprintKey) && stamina > 0)
        {
            stamina -= staminaConsumptionRate * Time.deltaTime;
        }
        else if (stamina < maxStamina)
        {
            stamina += staminaRegenRate * Time.deltaTime;
        }

        stamina = Mathf.Clamp(stamina, 0, maxStamina);
    }

    private void LockCursor()
    {
        if (Input.GetKeyDown(KeyCode.P) && isCursorLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isCursorLocked = false;
        }
        else if (Input.GetKeyDown(KeyCode.P) && !isCursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isCursorLocked = true;
        }
    }
    public void UseStamina(float amount)
    {
        stamina -= amount;
        stamina = Mathf.Clamp(stamina, 0, maxStamina);
    }
}
