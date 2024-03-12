using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomScript : MonoBehaviour
{
    public float MoveSpeed; // Speed of horizontal movement
    public bool MoveRight; // Direction of initial movement

    private bool _isTaken; // Flag indicating if the mushroom has been taken
    private Rigidbody2D _rb; // Reference to the Rigidbody2D component

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>(); // Accessing the Rigidbody2D component
        _isTaken = false; // Mushroom is initially not taken
    }

    void Update()
    {
        if (!_isTaken)
        {
            // Move the mushroom horizontally based on MoveRight flag
            _rb.velocity = new Vector2(MoveRight ? MoveSpeed : -MoveSpeed, _rb.velocity.y);
        }
        else
        {
            // Destroy the mushroom object after being taken
            Destroy(this.gameObject, 0.1f);
        }
    }

    // Detect collision while staying on a collider
    private void OnCollisionStay2D(Collision2D col)
    {
        // Check collision with certain tagged objects to change the direction of the mushroom
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

    public void mushroomTaken()
    {
        _isTaken = true; // Set the mushroom as taken
        _rb.velocity = Vector2.zero; // Stop the mushroom's movement
    }
}
