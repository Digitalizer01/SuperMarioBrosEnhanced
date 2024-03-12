using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoombaFollowerScript : MonoBehaviour
{
    public GameObject player;
    public float MoveSpeed; // Speed of horizontal movement
    public bool MoveRight; // Direction of initial movement

    private bool _isDead; // Flag indicating if the Goomba is dead
    private Rigidbody2D _rb; // Reference to the Rigidbody2D component

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>(); // Accessing the Rigidbody2D component
        _isDead = false; // Goomba is initially alive
    }

    void Update()
    {
        if (!_isDead)
        {
            followPlayer();
        }
    }

    public void goombaDead()
    {
        _isDead = true; // Set the Goomba as dead

        _rb.velocity = Vector2.zero; // Stop the Goomba's movement

        // Trigger the "isDead" animation parameter
        GetComponent<Animator>()
            .SetBool("isDead", true);

        // Play a sound effect for stomping the Goomba
        AudioClip stompGoombaSound = Resources.Load<AudioClip>("Sounds/smb_stomp");
        GetComponent<AudioSource>().PlayOneShot(stompGoombaSound);

        int enemyLayer = LayerMask.NameToLayer("Enemies");

        // Ignore collisions between Goomba and other enemies
        Physics2D.IgnoreLayerCollision(enemyLayer, gameObject.layer);

        // Ignore collision between Goomba and player
        Physics2D.IgnoreCollision(
            GameObject.Find("Player").GetComponent<Collider2D>(),
            GetComponent<Collider2D>(),
            true
        );

        // Destroy the Goomba object after a delay
        Destroy(this.gameObject, 2f);
    }

    private void followPlayer()
    {
        Vector2 targetPosition = player.transform.position;
        Vector2 currentPosition = _rb.position;

        // Calculate the direction to move towards the player
        Vector2 direction = (targetPosition - currentPosition).normalized;

        // Update the velocity to move towards the player
        _rb.velocity = direction * 4f;
    }
}
