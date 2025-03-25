using UnityEngine;

public class Sheep : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Flower"))
        {
            Destroy(other.gameObject);
            Debug.Log("Sheep ate the flower!");
        }
    }
}
