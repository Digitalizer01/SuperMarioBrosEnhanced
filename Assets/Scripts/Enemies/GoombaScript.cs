using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoombaScript : MonoBehaviour
{
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
            // Move the Goomba horizontally based on MoveRight flag
            _rb.velocity = new Vector2(MoveRight ? MoveSpeed : -MoveSpeed, _rb.velocity.y);
        }
    }

    // Detect collision while staying on a collider
    private void OnCollisionStay2D(Collision2D col)
    {
        // Check collision with certain tagged objects to change the direction of the Goomba
        if (
            col.gameObject.CompareTag("Ground")
            || col.gameObject.CompareTag("Block_Breakable")
            || col.gameObject.CompareTag("Block_Interrogation")
            || col.gameObject.CompareTag("Goomba")
        )
        {
            Vector2 leftNormal = new Vector2(-1, 0);
            Vector2 rightNormal = new Vector2(1, 0);

            bool isTouchingLeft = false;
            bool isTouchingRight = false;

            // Check contact points to determine collision direction
            foreach (ContactPoint2D contact in col.contacts)
            {
                if (Vector2.Dot(contact.normal, leftNormal) > 0.5f)
                {
                    isTouchingLeft = true;
                }
                else if (Vector2.Dot(contact.normal, rightNormal) > 0.5f)
                {
                    isTouchingRight = true;
                }
            }

            // Change movement direction based on collision side
            if (isTouchingLeft && !isTouchingRight)
            {
                MoveRight = false;
            }
            else if (!isTouchingLeft && isTouchingRight)
            {
                MoveRight = true;
            }
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
}
