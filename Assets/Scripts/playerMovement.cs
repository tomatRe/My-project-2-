using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{

    public Camera playerCamera;
    public Animator animator;
    public float walkSpeed = 3f;
    public float runSpeed = 3f;
    public float jumpPower = 7f;
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 100f;
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    public float defaultFOV = 60f;
    public float walkFOV = 61f;
    public float runFOV = 66f;
    public float fovLerpSpeed = 5f;
    public float speed = 5f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 100;
    private CharacterController characterController;

    private bool canMove = true;
    private bool IsSitting = false;

    void Start()
    {
        animator = GetComponent<Animator>(); // Cambiado a Animator
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Move();
        canSeat();
    }


    void Move()
    {
        
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);


        float moveInput = Input.GetAxis("Vertical"); // Use "W" or "S" (or arrow keys) for movement
        bool isWalking = Mathf.Abs(moveInput) > 0.1f; // Detect movement
         if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.R) && canMove)
        {
            characterController.height = crouchHeight;
            walkSpeed = crouchSpeed;
            runSpeed = crouchSpeed;
        }
        else
        {
            characterController.height = defaultHeight;
            walkSpeed = 1.8f;
            runSpeed = 1.8f;
        }
        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
        AdjustFOV(curSpeedX, curSpeedY, isRunning);

                // Update Animator parameter
        animator.SetBool("IsWalking", isWalking);
        // Move the player if walking
        if (isWalking)
        {
            animator.SetBool("IsWalking", true);

        }
        else {
            animator.SetBool("IsWalking", false);
        }

    }

    void canSeat()
    {
        if (Input.GetKey(KeyCode.E))
        {
            Vector3 fwd = playerCamera.transform.TransformDirection(Vector3.forward);
            
             if (Physics.Raycast(playerCamera.transform.position, fwd, 1))
             {           
                 IsSitting = true;
                 sitChair();
             } 
    }
        if (Input.GetKey(KeyCode.Q))
        {
            animator.SetBool("IsSitting", false);
            canMove = true;
        }
    }
    void sitChair()
    {
         animator.SetBool("IsSitting", true);
         canMove = false;
    }


    void AdjustFOV(float speedX, float speedY, bool isRunning)
    {
        float targetFOV = defaultFOV; 

        if (Math.Abs(speedX) > 0 || Math.Abs(speedY) > 0)
        {
            targetFOV = isRunning ? runFOV : walkFOV;
        }

        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, fovLerpSpeed * Time.deltaTime);
    }
}
