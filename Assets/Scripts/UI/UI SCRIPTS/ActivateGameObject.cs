using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateGameObject : MonoBehaviour
{
    // Drag the GameObject you want to activate in the Inspector
    public GameObject objectToActivate;

    // This method will be called when the button is clicked
    public void ActivateObject()
    {
        objectToActivate.SetActive(true); // Activate the GameObject
    }


    // Drag the GameObject you want to Deactivate in the Inspector
    public GameObject objectToDeactivate;

    // This method will be called when the button is clicked
    public void DeactivateObject()
    {
        objectToDeactivate.SetActive(false); // Deactivate the GameObject
    }

}

