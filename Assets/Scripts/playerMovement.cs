using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{

    public Camera playerCamera;
    public Animator animator;
    public GameObject SitPosition1;
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
    private bool canLook = true;
    private bool IsSitting = false;

    void Start()
    {
        animator = GetComponent<Animator>(); // Cambiado a Animator
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SitPosition1 = GameObject.Find("SitPosition1");
    }

    void Update()
    {
        Move();
        canSeat();
    }


    void Move()
    {
        if (canMove) { 
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);


        float moveInput = Input.GetAxis("Vertical"); 
        bool isWalking = Mathf.Abs(moveInput) > 0.1f; 
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
            walkSpeed = 0.9f;
            runSpeed = 0.8f;
        }
        characterController.Move(moveDirection * Time.deltaTime);


        AdjustFOV(curSpeedX, curSpeedY, isRunning);

        animator.SetBool("IsWalking", isWalking);
        if (isWalking)
        {
            animator.SetBool("IsWalking", true);

        }
        else {
            animator.SetBool("IsWalking", false);
        }
        
    }
        RotateCamera();
    }

    void RotateCamera()
    {
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }
    void canSeat()
    {
        if (Input.GetKey(KeyCode.E))
        {
            RaycastHit hit;
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1))
            {
                if (hit.collider.tag == "ChairG")
                {   
                    IsSitting = true;
                    sitChair();
                    SitPosition();                    
                }
             } 
    }
        if (Input.GetKey(KeyCode.Q))
        {
            animator.SetBool("IsSitting", false);
            canMove = true;
        }
    }
    void SitPosition()
    {
        // Desactiva el CharacterController para mover al jugador directamente
        characterController.enabled = false;

        // Mueve al jugador a la posición y rotación del GameObject SitPosition1
        transform.position = SitPosition1.transform.position;
        transform.rotation = SitPosition1.transform.rotation;
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
