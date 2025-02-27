using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    // public vars
    public Camera playerCamera;
    public Animator animator;
    public GameObject SitPosition1;
    public float gravity = 10f;
    public float lookXLimit = 100f;
    public float defaultHeight = 2f;
    [SerializeField] public float walkSpeed = 0.9f;
    [SerializeField] public float lookSpeed = 2f;

    [SerializeField] public float defaultFOV = 60f;
    [SerializeField] public float walkFOV = 61f;
    [SerializeField] public float runFOV = 66f;
    [SerializeField] public float fovLerpSpeed = 5f;
    [SerializeField] public float speed = 5f;

    // private vars

    private Vector3 moveDirection = Vector3.zero;
    private float cameraRotationX = 100;
    private CharacterController characterController;
    private bool canMove = true;
    private bool canLook = true;
    private bool IsSitting = false;

    // player inputs
    private float mouseX = 0;
    private float mouseY = 0;
    private float joystickX = 0;
    private float joystickY = 0;

    void Start()
    {
        animator = GetComponent<Animator>(); // Cambiado a Animator
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        characterController.height = defaultHeight;
        Cursor.visible = false;
        SitPosition1 = GameObject.Find("SitPosition1");
    }

    void Update()
    {
        GetInputs();
        Move();
        UpdateAnimations();
        RotateCamera();
        canSeat();
    }

    void GetInputs()
    {
        joystickX = Input.GetAxis("Horizontal");
        joystickY = Input.GetAxis("Vertical");
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
    }

    void UpdateAnimations()
    {
        bool isWalking = Mathf.Abs(joystickY) > 0.1f;
        animator.SetBool("IsWalking", isWalking);

        if (isWalking)
        {
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }
    }

    void Move()
    {
        if (canMove) 
        {
            // movimiento del jugador x y
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            float curSpeedX = walkSpeed * joystickY;
            float curSpeedY = walkSpeed * joystickX;

            moveDirection = (forward * curSpeedX) + (right * curSpeedY);
            
            // efecto gravedad

            if (!characterController.isGrounded)
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }

            characterController.Move(moveDirection * Time.deltaTime);

            AdjustFOV(curSpeedX, curSpeedY);        
        }
    }

    void RotateCamera()
    {
        cameraRotationX += -mouseY * lookSpeed;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(cameraRotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, mouseX * lookSpeed, 0);
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
   
    void AdjustFOV(float speedX, float speedY)
    {
        float targetFOV = defaultFOV; 

        if (Math.Abs(speedX) > 0 || Math.Abs(speedY) > 0)
        {
            targetFOV = walkFOV;
        }

        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, fovLerpSpeed * Time.deltaTime);
    }
}
