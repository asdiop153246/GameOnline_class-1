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
    public NetworkVariable<float> Stamina = new NetworkVariable<float>(100f);
    public NetworkVariable<float> maxStaminas = new NetworkVariable<float>(100f);
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
    void Start()
    {
        if (!IsOwner)
        {
            Destroy(StaminaBar);
            Destroy(BackStaminaBar);
            return;
        };

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Stamina.Value = maxStaminas.Value;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        running = false;
        isCursorLocked = true;
    }
    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
    }
    void moveForward()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        movement = orientation.forward * verticalInput + orientation.right * horizontalInput;
        //moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.MovePosition(transform.position + movement * speed * Time.deltaTime);
        if (Mathf.Abs(verticalInput) > 0.01f)
        {
            // move forward only
            if (verticalInput > 0.01f)
            {
                //float translation = verticalInput * speed;
                //translation *= Time.fixedDeltaTime;
                //rb.MovePosition(rb.position + this.transform.forward * translation);

                if (!walking)
                {
                    running = true;
                    animator.SetBool("Walk", true);
                }
            }
        }
        else if (walking)
        {
            running = false;
            animator.SetBool("Walk", false);
        }
        if (Input.GetKey(sprintKey) && Stamina.Value > 0)
        {
            rb.MovePosition(transform.position + movement * RunSpeed * Time.deltaTime);
        }
    }
    //private void CamMovement() 
    //{
    //    // New: Get the mouse movement input
    //    float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
    //    float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

    //    // New: Rotate the player and the camera based on the mouse input
    //    cameraverticalRotation -= mouseY;
    //    cameraverticalRotation = Mathf.Clamp(cameraverticalRotation, -90f, 90f);
    //    transform.localEulerAngles = Vector3.right * cameraverticalRotation;

    //    // New: Apply the rotation to the player and the camera
    //    transform.rotation = Quaternion.Euler(0f, currentRotation.y, 0f);
    //    orientation.localRotation = Quaternion.Euler(currentRotation.x, 0f, 0f);
    //}
    private void JumpInput() 
    {
        if (Input.GetKey(jumpKey) && readyToJump == true)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }

    private void Staminas()
    {

        if (Input.GetKey(sprintKey) && Stamina.Value > 0)
        {
            Stamina.Value -= staminaConsumptionRate * Time.deltaTime;
        }
        else if (Stamina.Value < maxStaminas.Value)
        {
            Stamina.Value += staminaRegenRate * Time.deltaTime;
        }

        Stamina.Value = Mathf.Clamp(Stamina.Value, 0, maxStaminas.Value);
        Debug.Log(Stamina.Value);
    }
    private void LockCursor()
    {
        if (Input.GetKeyDown(KeyCode.P) && isCursorLocked == true)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isCursorLocked = false;
        }
        else if (Input.GetKeyDown(KeyCode.P) && isCursorLocked == false)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isCursorLocked = true;
        }

    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        Staminas();
        moveForward();
        JumpInput();
        LockCursor();
        UpdateStaminaUI();
    }

    public void UpdateStaminaUI()
    {
        float fillF = StaminaBar.fillAmount;
        float fillB = BackStaminaBar.fillAmount;
        float hFraction = Stamina.Value / maxStaminas.Value;
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
}
