using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Structure : MonoBehaviour
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

    [Button]
    private void OnStructureClicked()
    {
        print("Display UI\n" + structure_so.structureName + ": " + structure_so.description);
    }

    [Button]
    private void OnDestroy()
    {
        print("Destroyed " + structure_so.structureName + ". Returning " + structure_so.resellValue + " gold coins.");
    }
}