using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterrogationBlockController : MonoBehaviour
{
    // Reference to the gameplay manager and the item to spawn
    public GameObject gameplayManager;
    public GameObject item;

    // Components and variables
    private Animator _animator;
    private bool _isTaken;
    private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize components
        _animator = GetComponent<Animator>();
        _isTaken = false;
        _audioSource = GetComponent<AudioSource>();
    }

    // Function to obtain an item from the block
    public void obtainItem()
    {
        // Check if the item hasn't been taken yet
        if (!_isTaken)
        {
            // Set the block as pressed and mark as taken
            _animator.SetBool("isPressed", true);
            _isTaken = true;

            // Check if there's an item to spawn
            if (item != null)
            {
                // Play sound effect for item appearance
                AudioClip powerupAppearsSound = Resources.Load<AudioClip>(
                    "Sounds/smb_powerup_appears"
                );
                _audioSource.PlayOneShot(powerupAppearsSound);

                // Get the current position of the object
                Vector3 spawnPosition = transform.position;

                // Adjust the position in the Y-axis to be just above the current object
                spawnPosition.y += 1.0f; // You can adjust this value for the desired height

                // Instantiate the item with the new position
                Instantiate(item, spawnPosition, transform.rotation);
            }
            else
            {
                // If there's no item, inform the gameplay manager about obtaining a coin
                gameplayManager.GetComponent<GameplayManagerController>().coinObtained(null);
            }
        }
    }
}
