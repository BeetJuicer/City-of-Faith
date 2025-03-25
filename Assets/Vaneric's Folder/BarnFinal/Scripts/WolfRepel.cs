using UnityEngine;

public class WolfRepel : MonoBehaviour
{
    public float repelForce = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wolf"))
        {
            Rigidbody2D wolfRb = other.GetComponent<Rigidbody2D>();
            if (wolfRb != null)
            {
                Vector2 repelDirection = other.transform.position - transform.position;
                repelDirection.Normalize();
                wolfRb.AddForce(repelDirection * repelForce, ForceMode2D.Impulse);

                AudioSourceBarn.Instance.PlayWolfRepelSound();
            }
        }
    }
}
