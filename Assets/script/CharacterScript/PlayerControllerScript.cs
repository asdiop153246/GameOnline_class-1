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

    [Header("Controls")]
    public Transform cam;
    public Transform target;
    public Transform orientation;

    [Header("Movement Parameters")]
    public float speed = 3.5f;
    public float RunSpeed = 7.0f;
    public float swimSpeed = 4.0f;
    public float rotationSpeed = 10.0f;
    public float mouseSensitivity = 2.0f;
    public float playerHeight;
    public LayerMask whatIsGround;

    [Header("Jump Parameters")]
    public float jumpCooldown;
    public float jumpForce;

    [Header("Stamina")]
    public float staminaConsumptionRate = 10f;
    public float swimConsumptionRate = 7f;
    public float staminaRegenRate = 5f;
    public float stamina = 100f;
    private float maxStamina = 100f;
    public Image StaminaBar;
    private float lerptimer;
    public float chipSpeed = 2f;
    public Image BackStaminaBar;

    private Animator animator;
    private Rigidbody rb;
    private PlayerHealth health;
    private bool walking;
    private bool readyToJump = true;
    private bool grounded;
    private bool isCursorLocked = true;
    public bool canMove = true;
    public bool isSwimming;
    public bool canJump = true;
    public GameObject PlayerUI;
    private Vector3 movement;

    private void Start()
    {
        if (!IsOwner)
        {
            Destroy(PlayerUI);
            Destroy(StaminaBar);
            Destroy(BackStaminaBar);
            return;
        };

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        stamina = maxStamina;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        health = GetComponent<PlayerHealth>();
        //running = false;
        isCursorLocked = true;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        if (!canMove) return;
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
        if (isSwimming != true)
        {
            if (rb.useGravity != true)
            {
                rb.useGravity = true;
            }
            float verticalInput = Input.GetAxis("Vertical");
            float horizontalInput = Input.GetAxis("Horizontal");
            movement = orientation.forward * verticalInput + orientation.right * horizontalInput;
            rb.MovePosition(transform.position + movement * speed * Time.deltaTime);
            canJump = true;
            //animator.SetBool("Swim", false);
            animator.SetBool("TreadingSwim", false);
            if (walking == true)
            {
                animator.SetBool("Walk", true);
            }
            else
            {
                animator.SetBool("Walk", false);
                animator.SetBool("Run", false);
            }


            if (Input.GetKey(sprintKey) && stamina > 5)
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
        else
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);
            canJump = false;
            if(Input.GetAxisRaw("Vertical") > 0)
            {
                //animator.SetBool("Swim", true);
                animator.SetBool("TreadingSwim", true);
                transform.position += target.forward * swimSpeed * Time.deltaTime;
                stamina -= swimConsumptionRate * Time.deltaTime;
                stamina = Mathf.Clamp(stamina, 0, maxStamina);
                UpdateStaminaUI();
            }
            if (Input.GetAxisRaw("Vertical") < 0)
            {
                //animator.SetBool("Swim", true);
                animator.SetBool("TreadingSwim", true);
                transform.position -= target.forward * swimSpeed * Time.deltaTime;
                stamina -= swimConsumptionRate * Time.deltaTime;
                stamina = Mathf.Clamp(stamina, 0, maxStamina);
                UpdateStaminaUI();
            }
            if (Input.GetAxisRaw("Vertical") == 0)
            {
                animator.SetBool("Swim", false);
                animator.SetBool("TreadingSwim", true);
                stamina -= swimConsumptionRate * Time.deltaTime;
                stamina = Mathf.Clamp(stamina, 0, maxStamina);
                UpdateStaminaUI();
            }
            if (stamina <= 0)
            {
                health.RequestTakeDamageServerRpc(0.01f);  
            }
        }
    }

    private void JumpInput()
    {
        if (Input.GetKey(jumpKey) && readyToJump && stamina > 20 && canJump == true)
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
        animator.SetTrigger("Jump");
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
        float hFraction = stamina / maxStamina;
        float fillDifference = Mathf.Abs(StaminaBar.fillAmount - hFraction);

        if (fillDifference > 0.01f)
        {
            lerptimer += Time.deltaTime;
            float percentComplete = lerptimer / chipSpeed;
            StaminaBar.fillAmount = hFraction;
            BackStaminaBar.fillAmount = Mathf.Lerp(StaminaBar.fillAmount, hFraction, percentComplete);
            BackStaminaBar.color = StaminaBar.fillAmount > hFraction ? Color.red : Color.blue;
        }
    }
    public void UseStamina(float amount)
    {
        stamina -= amount;
        stamina = Mathf.Clamp(stamina, 0, maxStamina);
        UpdateStaminaUI();
    }
    public void ResetVector()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
