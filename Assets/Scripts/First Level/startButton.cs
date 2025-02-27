using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public GameObject startButton;
public GameObject playerCharacter;

public class startButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if raycast from camera to startButton
        {
            if (startButton is clicked)
            {
                startButton.SetActive(false);
                playerCharacter.SetActive(true);
            }
        }
    }
}
