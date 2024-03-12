using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Movement Variables
    public float linearSpeed = 1;
    public float stopThreshold = 0.1f;
    public float jumpVelocity = 1f;
    public float maxVelocity = 1f;

    // Components and References
    private Rigidbody2D _rb;
    private Animator _animator = null;
    private SpriteRenderer _spriteRenderer;
    private AudioSource _audioSource;
    private BoxCollider2D _boxCollider;

    // Gameplay Objects and Flags
    public GameObject gameplayManager;
    public GameObject fire;
    private bool _onGround;
    private bool _hasJumped;
    private bool _moveRight;
    private bool _stopMoving;
    private bool _hasDead;
    private bool _hasWonEventActivated;
    private bool _stateBig;
    private bool _stateFireflower;
    private bool _isCooldown = false; // Variable to track if the player is in cooldown
    private float _cooldownDuration = 3f; // Cooldown duration in seconds
    private float _cooldownTimer = 0f; // Timer for the cooldown
    private bool _isVisible = true; // Variable to control player visibility during cooldown

    // Collision Normal Vectors
    private Vector2 _bottomNormal = new Vector2(0, -1);
    private Vector2 _topNormal = new Vector2(0, 1);
    private Vector2 _leftNormal = new Vector2(-1, 0);
    private Vector2 _rightNormal = new Vector2(1, 0);

    private void Awake()
    {
        // Getting required components
        _rb = GetComponent<Rigidbody2D>(); // Rigidbody2D for physics
        _animator = GetComponent<Animator>(); // Animator for animations
        _spriteRenderer = GetComponent<SpriteRenderer>(); // SpriteRenderer for visual properties
        _audioSource = GetComponent<AudioSource>(); // AudioSource for playing sounds
        _boxCollider = GetComponent<BoxCollider2D>(); // BoxCollider2D for collisions

        // Initializing state flags and variables
        _stateBig = false; // Player state: not big by default
        _stateFireflower = false; // Player state: not powered by fireflower by default
        _hasJumped = false;
        _stopMoving = false;
        _hasDead = false;
        _hasWonEventActivated = false;
        _moveRight = true; // Player is facing right by default

        _onGround = false;
        _animator.SetBool("isDead", false); // Setting initial animation state

        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemies"), false);
    }

    private void Update()
    {
        // Movement and action handling
        if (!_stopMoving)
        {
            // Detecting key presses for movement and actions
            bool keyPressed = false;

            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetAxis("Horizontal") < 0)
            {
                MoveBackward();
                keyPressed = true;
            }
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetAxis("Horizontal") > 0)
            {
                MoveForward();
                keyPressed = true;
            }
            if (
                (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump"))
                && _onGround
                && !_hasJumped
            )
            {
                // Jump action handling
                _onGround = false;
                _hasJumped = true;

                AudioClip jumpSound;

                if (_stateBig)
                {
                    jumpSound = Resources.Load<AudioClip>("Sounds/smb_jump-super");
                }
                else
                {
                    jumpSound = Resources.Load<AudioClip>("Sounds/smb_jump-small");
                }

                _audioSource.PlayOneShot(jumpSound);

                _rb.AddForce(Vector2.up * jumpVelocity, ForceMode2D.Impulse);
            }
            if (Input.GetKeyDown(KeyCode.F) || Input.GetButtonDown("Fire1"))
            {
                // Handling firing action if powered by fireflower
                if (_stateFireflower)
                {
                    int direction;

                    if (_moveRight)
                    {
                        direction = 1;
                    }
                    else
                    {
                        direction = -1;
                    }

                    // Obtaining position in front of the player
                    Vector2 spawnPosition = transform.position + transform.right * 1.5f * direction;

                    // Instantiating the fire object
                    GameObject shot = Instantiate(fire, spawnPosition, Quaternion.identity);

                    Rigidbody2D rbShot = shot.GetComponent<Rigidbody2D>();

                    if (rbShot != null)
                    {
                        // Applying force to the instantiated object
                        rbShot.AddForce(transform.right * 10f * direction, ForceMode2D.Impulse);
                    }

                    AudioClip fireSound = Resources.Load<AudioClip>("Sounds/smb_fireball");
                    _audioSource.PlayOneShot(fireSound);
                }
            }

            if (!keyPressed && Mathf.Abs(Input.GetAxis("Horizontal")) < 0.1f)
            {
                SetIdleIfStopped();
            }

            // Setting animation states based on movement and jumping
            if (_rb.velocity.y != 0 && !_onGround)
            {
                _animator.SetBool("isJumping", true);
            }
            else if (_rb.velocity.y == 0 && _onGround)
            {
                _animator.SetBool("isJumping", false);
            }

            if (_isCooldown)
            {
                HandleCooldown();
            }
        }
        else if (_hasDead)
        {
            // Respawn player if dead
            gameplayManager.GetComponent<GameplayManagerController>().playerRespawn();
        }
        else if (!_hasWonEventActivated)
        {
            // Player wins if not dead and the game stops moving
            gameplayManager.GetComponent<GameplayManagerController>().playerWins();
            _hasWonEventActivated = true;
        }
    }

    // Movement methods
    private void MoveForward()
    {
        _moveRight = true;
        var right = transform.right;

        if (_rb.velocity.magnitude < maxVelocity)
        {
            _rb.velocity +=
                new Vector2(right.x * linearSpeed, right.y * linearSpeed) * Time.deltaTime;
        }

        _animator.SetBool("isJumping", false);
        _animator.SetBool("isRunning", true);
        _spriteRenderer.flipX = false;
    }

    private void MoveBackward()
    {
        _moveRight = false;
        var right = transform.right;

        if (_rb.velocity.magnitude < maxVelocity)
        {
            _rb.velocity -=
                new Vector2(right.x * linearSpeed, right.y * linearSpeed) * Time.deltaTime;
        }

        _animator.SetBool("isJumping", false);
        _animator.SetBool("isRunning", true);
        _spriteRenderer.flipX = true;
    }

    private void SetIdleIfStopped()
    {
        if (Mathf.Abs(_rb.velocity.x) < stopThreshold)
        {
            _animator.SetBool("isRunning", false);
        }
    }

    // Collision handling methods
    private void OnCollisionStay2D(Collision2D col)
    {
        _animator.SetBool("isJumping", false);

        _onGround = false;

        List<Vector2> list = new List<Vector2>();

        // Checking collision with various types of objects
        if (
            col.gameObject.CompareTag("Ground")
            || col.gameObject.CompareTag("Block_Breakable")
            || col.gameObject.CompareTag("Block_Interrogation")
        )
        {
            foreach (ContactPoint2D contact in col.contacts)
            {
                if (Vector2.Dot(contact.normal, _topNormal) > 0.5f)
                {
                    _onGround = true;
                    _hasJumped = false;
                }
            }
        }

        // Check collision with Goomba enemy
        if (col.gameObject.CompareTag("Goomba") && !_isCooldown)
        {
            Vector2 topNormal = new Vector2(0, 1);

            list.Clear();

            foreach (ContactPoint2D contact in col.contacts)
            {
                list.Add(contact.normal);
            }

            if (list.Contains(topNormal))
            {
                // Jump on Goomba to defeat it
                _rb.AddForce(Vector2.up * (jumpVelocity / 2), ForceMode2D.Impulse);
                gameplayManager
                    .gameObject.GetComponent<GameplayManagerController>()
                    .defeatedEnemy(col.gameObject);
            }
            else
            {
                // Player collided with Goomba from the side or bottom
                if (_stateBig && !_isCooldown)
                {
                    // Player loses power-up when hit by Goomba
                    PlayerLosePowerUp();

                    // Activar cooldown después de ser golpeado
                    StartCooldown();
                }
                else if (!_isCooldown)
                {
                    // Player dies when small and hit by Goomba
                    PlayerDies();
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player touches the ground after winning
        if (collision.gameObject.CompareTag("Ground") && _stopMoving)
        {
            // Reset animation states after winning
            _animator.SetBool("isJumping", false);
            _animator.SetBool("isRunning", false);
            _animator.SetBool("isDead", false);
            _animator.SetBool("isPole", false);
        }

        // Check collision with the pole to finish the level
        if (collision.gameObject.CompareTag("Pole"))
        {
            // Set animation for touching the pole
            _animator.SetBool("isPole", true);
            _rb.velocity = new Vector2(0, 0);
            _rb.gravityScale = 1f;
            _stopMoving = true;

            Physics2D.IgnoreLayerCollision(
                gameObject.layer,
                LayerMask.NameToLayer("Enemies"),
                true
            );

            _spriteRenderer.flipX = false;
        }

        // Collision with Goomba enemy
        if (collision.gameObject.CompareTag("Goomba") && !_isCooldown)
        {
            print("Goomba Collision Out: " + gameObject.name);

            Vector2 topNormal = new Vector2(0, 1);

            List<Vector2> list = new List<Vector2>();

            foreach (ContactPoint2D contact in collision.contacts)
            {
                list.Add(contact.normal);
            }

            if (list.Contains(topNormal))
            {
                // Jump on Goomba to defeat it
                _rb.AddForce(Vector2.up * (jumpVelocity / 2), ForceMode2D.Impulse);
                gameplayManager
                    .gameObject.GetComponent<GameplayManagerController>()
                    .defeatedEnemy(collision.gameObject);
            }
            else
            {
                // Player collided with Goomba from the side or bottom
                if (_stateBig)
                {
                    // Player loses power-up when hit by Goomba
                    PlayerLosePowerUp();
                    StartCooldown();
                }
                else
                {
                    // Player dies when small and hit by Goomba
                    PlayerDies();
                }
            }
        }

        // Collision with Mushroom or Fireflower power-ups
        if (
            collision.gameObject.CompareTag("Mushroom")
            || collision.gameObject.CompareTag("Fireflower")
        )
        {
            Vector2 topNormal = new Vector2(0, 1);

            List<Vector2> list = new List<Vector2>();

            foreach (ContactPoint2D contact in collision.contacts)
            {
                list.Add(contact.normal);
            }

            if (list.Count != 0)
            {
                if (collision.gameObject.CompareTag("Mushroom"))
                {
                    // Collect Mushroom power-up
                    gameplayManager
                        .GetComponent<GameplayManagerController>()
                        .mushroomTaken(collision.gameObject);

                    PlayerPowerUp();
                }
                else
                {
                    // Collect Fireflower power-up
                    gameplayManager
                        .GetComponent<GameplayManagerController>()
                        .fireflowerTaken(collision.gameObject);

                    if (_stateBig)
                    {
                        PlayerPowerUpFireflower();
                    }
                    else
                    {
                        PlayerPowerUp();
                    }
                }

                collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
        }

        // Collision with Ground or Breakable/Interrogation Blocks
        if (
            collision.gameObject.CompareTag("Ground")
            || collision.gameObject.CompareTag("Block_Breakable")
            || collision.gameObject.CompareTag("Block_Interrogation")
        )
        {
            List<Vector2> list = new List<Vector2>();

            foreach (ContactPoint2D contact in collision.contacts)
            {
                list.Add(contact.normal);
            }

            if (list.Contains(_bottomNormal))
            {
                if (collision.gameObject.CompareTag("Block_Interrogation"))
                {
                    // Activate item from interrogation block
                    collision.gameObject.GetComponent<InterrogationBlockController>().obtainItem();
                }
                if (collision.gameObject.CompareTag("Block_Breakable"))
                {
                    if (_stateBig)
                    {
                        // Break breakable block if powered up
                        collision.gameObject.GetComponent<BreakableBlockController>().obtainItem();
                    }
                }
                // Play bump sound effect
                AudioClip bumpSound = Resources.Load<AudioClip>("Sounds/smb_bump");
                _audioSource.PlayOneShot(bumpSound);
            }
        }
    }

    // Handling exit from collision with different objects
    private void OnCollisionExit2D(Collision2D collision)
    {
        _animator.SetBool("isRunning", false);

        if (
            collision.gameObject.CompareTag("Ground")
            || collision.gameObject.CompareTag("Pipe")
            || collision.gameObject.CompareTag("Block_Breakable")
            || collision.gameObject.CompareTag("Block_Interrogation")
        )
        {
            _onGround = false;
        }
    }

    // Player dies and handles actions related to death
    void PlayerDies()
    {
        _stopMoving = true;
        _hasDead = true;

        _animator.SetBool("isDead", true);

        _rb.velocity = Vector2.zero;

        Collider2D playerCollider = GetComponent<Collider2D>();
        if (playerCollider != null)
        {
            playerCollider.enabled = false;
        }

        _rb.velocity = new Vector2(_rb.velocity.x, jumpVelocity);

        gameplayManager.GetComponent<GameplayManagerController>().playerDies();
    }

    // Player powers up after collecting Mushroom
    private void PlayerPowerUp()
    {
        _animator.SetBool("isPowerup", true);

        AudioClip powerupSound = Resources.Load<AudioClip>("Sounds/smb_powerup");
        _audioSource.PlayOneShot(powerupSound);

        if (!_stateBig)
        {
            Vector2 newSize = _boxCollider.size;
            newSize.y *= 2; // Double the height (or adjust as needed)
            _boxCollider.size = newSize;
        }

        _stateBig = true;
    }

    // Player powers up with Fireflower after already being big
    private void PlayerPowerUpFireflower()
    {
        AudioClip powerupSound = Resources.Load<AudioClip>("Sounds/smb_powerup");
        _audioSource.PlayOneShot(powerupSound);

        _stateFireflower = true;
    }

    // Player loses power-up (shrinks or loses Fireflower)
    private void PlayerLosePowerUp()
    {
        _animator.SetBool("isPowerup", false);

        AudioClip losePowerupSound = Resources.Load<AudioClip>("Sounds/smb_pipe_lose-powerup");
        _audioSource.PlayOneShot(losePowerupSound);

        Vector2 newSize = _boxCollider.size;
        newSize.y /= 2; // Halve the height (or adjust as needed)
        _boxCollider.size = newSize;

        _stateBig = false;
        _stateFireflower = false;
    }

    // Trigger event when colliding with specific objects
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DeathCollider"))
        {
            // Player dies when colliding with death collider
            PlayerDies();
        }
        if (other.CompareTag("Coin"))
        {
            // Coin obtained when colliding with it
            gameplayManager
                .GetComponent<GameplayManagerController>()
                .coinObtained(other.gameObject);
        }
        if (other.CompareTag("Checkpoint"))
        {
            // Change respawn position when reaching a checkpoint
            gameplayManager
                .GetComponent<GameplayManagerController>()
                .changeRespawnPosition(other.gameObject);
        }
    }

    private void HandleCooldown()
    {
        // Update the cooldown timer
        _cooldownTimer += Time.deltaTime;

        // Change player visibility during cooldown
        if (_cooldownTimer % 0.5f < 0.25f)
        {
            _isVisible = false;
        }
        else
        {
            _isVisible = true;
        }

        // Turn off visibility if the cooldown duration has passed
        if (_cooldownTimer >= _cooldownDuration)
        {
            _isCooldown = false;
            _isVisible = true;
            _cooldownTimer = 0f;

            // Stop ignoring collisions with the "Enemies" layer at the end of the cooldown
            Physics2D.IgnoreLayerCollision(
                gameObject.layer,
                LayerMask.NameToLayer("Enemies"),
                false
            );
        }

        // Apply visibility to the player sprite renderer
        _spriteRenderer.enabled = _isVisible;
    }

    private void StartCooldown()
    {
        _isCooldown = true;
        _isVisible = true;
        _cooldownTimer = 0f;

        // Ignore collisions with objects on the "Enemies" layer during cooldown
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemies"), true);
    }
}
