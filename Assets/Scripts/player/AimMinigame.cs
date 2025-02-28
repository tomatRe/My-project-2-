using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimMinigame : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public GameObject circle;
    public float playerScore = 0;
    public bool isSitting = false;
    float x = 0;
    float y = 0;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        StartText();
    }


    void StartText()
    {
        if (isSitting)
        {

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = playerMovement.playerCamera.ScreenPointToRay(Input.mousePosition);
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

    void AimTrainingGame()
    {
        playerScore = 0;

        SystemRandom();
        if (isSitting)
        {


            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = playerMovement.playerCamera.ScreenPointToRay(Input.mousePosition);
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
}
