using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour
{
    Transform tr; // Transform component for controlling the egg's position

    void Start()
    {
        // Cache the Transform component for efficient position updates
        tr = GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        // Move the egg downward at a constant speed
        tr.position -= new Vector3(0f, 0.12f, 0f);

        // Destroy the egg if it goes off the screen (y-position less than -7)
        if (tr.position.y < -7f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the egg collides with the "Basket" object
        if (collision.gameObject.name == "Basket")
        {
            Destroy(gameObject); // Remove the egg after it's caught

            // Play the egg-catching sound effect
            AudioSourceMiniGame.instance.PlaySoundEffect(AudioSourceMiniGame.instance.eggCatchSound);

            // Add 5 points to the score via the GameController
            GameController.instance.AddScore(5);
        }
    }
}
