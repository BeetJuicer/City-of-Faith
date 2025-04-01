using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBuilding : MonoBehaviour, IBuffable
{
    private NPC_SO npcData;

    private void OnStructureBuilt(Structure s)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, npcData.detectionRadius);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.TryGetComponent<IBuffable>(out IBuffable buffable))
            {
                buffable.ApplyBuff();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (npcData == null) return;

        Gizmos.color = Color.green; // Set the color of the Gizmo
        Gizmos.DrawWireSphere(transform.position, npcData.detectionRadius); // Draw a wire sphere
    }

    // Implementing the ApplyBuff method from IBuffable
    public void ApplyBuff()
    {
        Debug.Log("Buff applied to NPC building!");
    }
}
