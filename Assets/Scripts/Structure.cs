using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Structure : MonoBehaviour, IClickableObject
{
    [SerializeField] private Structure_SO structure_so;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DisplayUI()
    {
        //UIManager.HUD.DisplayStructureUI(structure_so);
        //TODO: Use UI manager.
        print("Display UI\n" + structure_so.structureName + ": " + structure_so.description);
    }


    //temporary naughtyattributes button
    [Button]
    public void OnStructureClicked()
    {
        DisplayUI();
    }

    [Button]
    public void OnDestroy()
    {
        //print("Destroyed " + structure_so.structureName + ". Returning " + structure_so.resellValue + " gold coins.");
    }

    public void OnObjectClicked()
    {
        DisplayUI();
    }
}
