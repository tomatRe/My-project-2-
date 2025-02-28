using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    // public vars
    public Camera playerCamera;
    public Animator animator;
    public GameObject sitPosition1;
    public GameObject standPosition;
    public GameObject circle;
    public bool isSitting = false;
    public float gravity = 10f;
    public float lookXLimit = 100f;
    public float lookYLimit = 0f;
    public float defaultHeight = 2f;
    [SerializeField] public float walkSpeed = 0.9f;
    [SerializeField] public float lookSpeed = 2f;

    [SerializeField] public float defaultFOV = 60f;
    [SerializeField] public float walkFOV = 61f;
    [SerializeField] public float runFOV = 66f;
    [SerializeField] public float fovLerpSpeed = 5f;
    [SerializeField] public float speed = 5f;

    //aim training game

    float x = 0;
    float y = 0;
   public float playerScore = 0;
    // private vars

    private Vector3 moveDirection = Vector3.zero;
    private float cameraRotationX = 0;
    private float cameraRotationY = 0;
    private CharacterController characterController;
    private bool canMove = true;
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
        sitPosition1 = GameObject.Find("SitPosition1");
        standPosition = GameObject.Find("StandPosition");
        circle.SetActive(false);
    }

    void Update()
    {
        GetInputs();
        Move();
        UpdateAnimations();
        RotateCamera();
        canSeat();
        StartText();
        AimTrainingGame();
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

        if (!isSitting)
        {
            cameraRotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            cameraRotationX += -mouseY * lookSpeed;
            cameraRotationX = Mathf.Clamp(cameraRotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(cameraRotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, mouseX * lookSpeed, 0);
        }
        else
        {
            cameraRotationX += mouseX * lookSpeed;
            cameraRotationY += mouseY * lookSpeed;
            cameraRotationX = Mathf.Clamp(cameraRotationX, -lookXLimit, lookXLimit);
            cameraRotationY = Mathf.Clamp(cameraRotationY, -lookYLimit, lookYLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(-cameraRotationY, cameraRotationX, 0);
        }
    }

    public async Task canSeat()
    {
        if (Input.GetKey(KeyCode.E))
        {
            RaycastHit hit;
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 1))
            {
                if (hit.collider.tag == "ChairG")
                {
                    animator.SetBool("SittingAnimation", true);
                    canMove = false;
                    isSitting = true;
                    SitPosition();
                    lookXLimit = 30f;
                    lookYLimit = 30f;

                }
            }
        }

        if (Input.GetKey(KeyCode.Q))
        {
            animator.SetBool("SittingAnimation", false);
            await Task.Delay(1000);
            StandPosition();
            isSitting = false;
            canMove = true;
            lookXLimit = 100f;
        }
    }


void StartText()
    {
        if (isSitting)
       {
      
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 5))
            {
                if (hit.collider.tag == "startButton")
                {
                    Destroy(GameObject.Find("startButton"));
                    StartAim();

                }
            }
        }
       }
    }
    void StartAim()
    {
     circle.SetActive(true);
    }

    public void SystemRandom()
    {
        x = UnityEngine.Random.Range(-2.2724f, -1.8286f);
        y = UnityEngine.Random.Range(1.02f, 1.2027f);
    }

    void AimTrainingGame() {
        playerScore = 0;

        SystemRandom();
        if (isSitting)
        {

       
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 5))
            {
                if (hit.collider.tag == "circle")
                {
                    circle.transform.position = new Vector3(x, y, -3.4994f);
                    playerScore++;
                    }
            }
        }
        }
    }
    void SitPosition()
    {
        // Desactiva el CharacterController para mover al jugador directamente
        characterController.enabled = false;

        // Mueve al jugador a la posición y rotación del GameObject SitPosition1
        transform.position = sitPosition1.transform.position;
        transform.rotation = sitPosition1.transform.rotation;
    }

    void StandPosition()
    {
        // Activa el CharacterController para que el jugador pueda moverse
        characterController.enabled = true;
        // Mueve al jugador a la posición y rotación del GameObject SitToStand
        transform.position = standPosition.transform.position;
        transform.rotation = standPosition.transform.rotation;
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