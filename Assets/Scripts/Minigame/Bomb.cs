using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameController gameController;
    Transform tr;

    void Start()
    {
        tr = GetComponent<Transform>();
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    void FixedUpdate()
    {
        // Move the bomb downward
        tr.position -= new Vector3(0f, 0.12f, 0f);

        // Destroy the bomb if it falls below a certain point
        if (tr.position.y < -7f)
            Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the egg collides with the basket
        if (collision.gameObject.name == "Basket")
        {
            Destroy(this.gameObject);  // Destroy the bomb
            gameController.AddScore(-100); // Deduct score on bomb collision
        }
    }
}