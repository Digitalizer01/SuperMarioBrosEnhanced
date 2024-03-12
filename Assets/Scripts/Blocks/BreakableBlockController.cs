using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableBlockController : MonoBehaviour
{
    public GameObject item; // Item that the block holds

    private Animator _animator; // Reference to the Animator component
    private bool _isTaken; // Flag to check if the item is already taken
    private AudioSource _audioSource; // Reference to the AudioSource component

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>(); // Access the Animator component
        _isTaken = false; // Set the item as not taken initially
        _audioSource = GetComponent<AudioSource>(); // Access the AudioSource component
    }

    // Function to break the block
    public void breakBlock()
    {
        _animator.SetBool("isBroken", true); // Set the "isBroken" parameter in the animator

        // Play a sound effect for breaking the block
        AudioClip jumpSound = Resources.Load<AudioClip>("Sounds/smb_breakblock");
        _audioSource.PlayOneShot(jumpSound);

        Rigidbody2D rb = GetComponent<Rigidbody2D>(); // Access the Rigidbody2D component

        rb.velocity = Vector2.zero; // Reset velocity
        rb.isKinematic = false; // Disable kinematic property for physics

        Collider2D blockObjectCollider = GetComponent<Collider2D>();
        if (blockObjectCollider != null)
        {
            blockObjectCollider.enabled = false; // Disable collider to prevent further interactions
        }

        rb.velocity = new Vector2(rb.velocity.x, 6f); // Apply vertical force
        rb.gravityScale = 4f; // Increase gravity

        Destroy(this.gameObject, 4f); // Destroy the block after a delay
    }

    // Function to obtain the item from the block
    public void obtainItem()
    {
        if (item != null)
        {
            if (!_isTaken)
            {
                _animator.SetBool("isPressed", true); // Set the "isPressed" parameter in the animator

                // Play a sound effect for the item appearance
                AudioClip powerupAppearsSound = Resources.Load<AudioClip>(
                    "Sounds/smb_powerup_appears"
                );
                _audioSource.PlayOneShot(powerupAppearsSound);

                // Get the current position of the object
                Vector3 spawnPosition = transform.position;

                // Adjust the position in the Y-axis to be just above the current object
                spawnPosition.y += 1.0f; // Adjust this value based on desired height

                // Instantiate the item with the new position
                Instantiate(item, spawnPosition, transform.rotation);

                _isTaken = true; // Set the item as taken
            }
        }
        else
        {
            breakBlock(); // If there's no item, break the block directly
        }
    }
}
