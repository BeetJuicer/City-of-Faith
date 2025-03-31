using System.Collections.Generic;
using UnityEngine;

public class StructureMonitor : MonoBehaviour
{
    private List<GameObject> trackedStructures = new List<GameObject>();

    [SerializeField] private string structureTag = "Building";  // Change if needed

    private void Update()
    {
        // Find all GameObjects with the "Building" tag
        GameObject[] allStructures = GameObject.FindGameObjectsWithTag(structureTag);

        foreach (GameObject structure in allStructures)
        {
            if (!trackedStructures.Contains(structure))
            {
                // Try to get PrefabImageController
                PrefabImageController controller = structure.GetComponent<PrefabImageController>();
                if (controller != null)
                {
                    controller.EnableImage();  // Enable the image automatically
                    trackedStructures.Add(structure);
                }
            }
        }

        // Cleanup: Remove destroyed objects from the list
        trackedStructures.RemoveAll(item => item == null);
    }
}
