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

    // Implementing the ApplyBuff method from IBuffable
    public void ApplyBuff()
    {
        Debug.Log("Buff applied to NPC building!");
    }
}
