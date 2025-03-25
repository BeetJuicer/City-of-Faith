using UnityEngine;

public class DestroyWolves : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wolf"))
        {
            Destroy(other.gameObject);
        }
    }
}
