using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameController gameController; // Reference to the GameController for managing game logic
    Transform tr;                         // Transform component of the bomb for position updates

    void Start()
    {
        // Cache the Transform component of the bomb
        tr = GetComponent<Transform>();

        // Find and assign the GameController script from the "GameController" GameObject
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    void FixedUpdate()
    {
        // Move the bomb downward at a constant speed
        tr.position -= new Vector3(0f, 0.12f, 0f);

        // Destroy the bomb if it goes below the screen (y-position less than -7)
        if (tr.position.y < -7f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the bomb collides with the "Basket" object
        if (collision.gameObject.name == "Basket")
        {
            Destroy(this.gameObject); // Destroy the bomb upon collision

            // Play the bomb explosion sound effect
            AudioSourceMiniGame.instance.PlaySoundEffect(AudioSourceMiniGame.instance.bombExplosionSound);

            // Deduct 100 points from the score for hitting a bomb
            gameController.AddScore(-100);
        }
    }
}