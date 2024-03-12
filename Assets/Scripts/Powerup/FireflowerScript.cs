using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireflowerScript : MonoBehaviour
{
    // Flag to track if the fireflower has been taken
    private bool _isTaken;

    // Rigidbody component reference
    private Rigidbody2D _rb;

    void Start()
    {
        // Initialize references and variables
        _rb = GetComponent<Rigidbody2D>();
        _isTaken = false;
    }

    void Update()
    {
        // If the fireflower is taken, destroy it after a short delay
        if (_isTaken)
        {
            Destroy(this.gameObject, 0.1f);
        }
    }

    // Function called when the fireflower is taken
    public void fireflowerTaken()
    {
        // Set the fireflower as taken and stop its velocity
        _isTaken = true;
        _rb.velocity = Vector2.zero;
    }
}
