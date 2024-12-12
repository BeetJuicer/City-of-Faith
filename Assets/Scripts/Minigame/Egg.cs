using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour
{
    Transform tr;

    void Start()
    {
        tr = GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        // Move the egg downward
        tr.position -= new Vector3(0f, 0.12f, 0f);

        // Destroy the egg if it falls below a certain point
        if (tr.position.y < -7f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the egg collides with the basket
        if (collision.gameObject.name == "Basket")
        {
            Destroy(gameObject);

            // Access the GameController instance and call AddScore
            GameController.instance.AddScore(5);
        }
    }
}