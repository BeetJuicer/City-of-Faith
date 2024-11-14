using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateGameObject : MonoBehaviour
{
    // Drag the GameObjects you want to activate in the Inspector
    public List<GameObject> objectsToActivate;

    // Drag the GameObjects you want to deactivate in the Inspector
    public List<GameObject> objectsToDeactivate;

    // This method will be called when the button is clicked
    public void ActivateObjects()
    {
        foreach (GameObject obj in objectsToActivate)
        {
            obj.SetActive(true); // Activate each GameObject
        }
    }

    // This method will be called when the button is clicked
    public void DeactivateObjects()
    {
        foreach (GameObject obj in objectsToDeactivate)
        {
            obj.SetActive(false); // Deactivate each GameObject
        }
    }
}

