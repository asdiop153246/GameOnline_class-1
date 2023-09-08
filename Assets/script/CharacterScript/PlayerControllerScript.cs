using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerScript : NetworkBehaviour
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
    public Image StaminaBar;
    private float lerptimer;
    public float chipSpeed = 2f;
    public Image BackStaminaBar;
    private bool isInitialized = false;

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

    private bool isSprinting = false;

    private void Start()
    {
        if (!IsOwner)
        {
            Destroy(StaminaBar);
            Destroy(BackStaminaBar);
            return;
        };

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
        if (!IsOwner) return;
        checkMovement();
        MoveForward();
        JumpInput();
        LockCursor();
        UpdateStamina();
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
    private void MoveForward()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        movement = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.MovePosition(transform.position + movement * speed * Time.deltaTime);
        Debug.Log("walking =" + walking);
         if (walking == true)
        {
            animator.SetBool("Walk", true);
        }
        else
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);
        }


        if (Input.GetKey(sprintKey) && stamina > 0)
        {
            rb.MovePosition(transform.position + movement * RunSpeed * Time.deltaTime);
            if (walking == true)
            {
                animator.SetBool("Run", true);
                //animator.SetBool("Walk", false);
            }
            else
            {
                animator.SetBool("Run", false);
            }
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
        UpdateStaminaUI();
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

    private void UpdateStaminaUI()
    {
        float fillF = StaminaBar.fillAmount;
        float fillB = BackStaminaBar.fillAmount;
        float hFraction = stamina / maxStamina;

        if (fillB > hFraction)
        {
            StaminaBar.fillAmount = hFraction;
            BackStaminaBar.color = Color.red;
            lerptimer += Time.deltaTime;
            float percentComplete = lerptimer / chipSpeed;
            BackStaminaBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }

        if (fillF < hFraction)
        {
            StaminaBar.fillAmount = hFraction;
            BackStaminaBar.color = Color.blue;
            lerptimer += Time.deltaTime;
            float percentComplete = lerptimer / chipSpeed;
            BackStaminaBar.fillAmount = Mathf.Lerp(fillF, hFraction, percentComplete);
        }
    }
    public void UseStamina(float amount)
    {
        stamina -= amount;
        stamina = Mathf.Clamp(stamina, 0, maxStamina);
        UpdateStaminaUI();
    }
}
