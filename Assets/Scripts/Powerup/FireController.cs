using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FireController : MonoBehaviour
{
    private Rigidbody2D _rb;

    void Start()
    {
        // Accessing the Rigidbody2D component attached to the GameObject
        _rb = GetComponent<Rigidbody2D>();

        // Destroy the GameObject after 3 seconds (lifespan of the fire object)
        Destroy(gameObject, 3f);
    }

    // Detect collision with 2D colliders
    private void OnCollisionEnter2D(Collision2D collision)
    {
        int enemyLayer = LayerMask.NameToLayer("Enemies");

        // Check if the collided object is on the "Enemies" layer
        if (collision.gameObject.layer == enemyLayer)
        {
            // Access the GoombaScript component of the collided enemy to eliminate it
            GameObject
                .Find("GameplayManager")
                .gameObject.GetComponent<GameplayManagerController>()
                .defeatedEnemy(collision.gameObject);

            // Destroy the fire object after hitting an enemy
            Destroy(gameObject);
        }
    }
}
