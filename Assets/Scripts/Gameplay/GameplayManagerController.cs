using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManagerController : MonoBehaviour
{
    // References to game objects and variables
    public GameObject player;
    public GameObject fireworks;
    public GameObject infoUI;
    private AudioSource _audioSource;
    private Vector2 _respawnPosition;

    // Game variables
    [HideInInspector]
    public float _time;

    [HideInInspector]
    public int _coins;

    [HideInInspector]
    public int _points;

    void Start()
    {
        // Initialize variables and set default respawn position if not previously set
        _time = 0f;
        _coins = 0;
        _points = 0;

        if (!PlayerPrefs.HasKey("RespawnX") && !PlayerPrefs.HasKey("RespawnY"))
        {
            _respawnPosition = new Vector2(-3.599f, 3.62f);
        }
        else
        {
            _respawnPosition = new Vector2(
                PlayerPrefs.GetFloat("RespawnX"),
                PlayerPrefs.GetFloat("RespawnY")
            );
        }

        player.GetComponent<Transform>().position = _respawnPosition;

        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Update game time and UI information
        _time += Time.deltaTime;

        infoUI.GetComponent<InfoUI>()._infoTime = _time;
        infoUI.GetComponent<InfoUI>()._infoCoins = _coins;
        infoUI.GetComponent<InfoUI>()._infoPoints = _points;
    }

    // Function called when a coin is obtained
    public void coinObtained(GameObject coinObject)
    {
        _coins++;
        _points += 100;

        AudioClip _coinsound = Resources.Load<AudioClip>("Sounds/smb_coin");
        _audioSource.PlayOneShot(_coinsound);
        if (coinObject != null)
        {
            Destroy(coinObject);
        }
    }

    // Function called when an enemy is defeated
    public void defeatedEnemy(GameObject enemyObject)
    {
        GoombaScript goombaScript = enemyObject.GetComponent<GoombaScript>();

        if (goombaScript != null)
        {
            // Normal goomba
            goombaScript.goombaDead();
        }
        else
        {
            // Follower goomba
            enemyObject.GetComponent<GoombaFollowerScript>().goombaDead();
        }
    }

    // Function called when a mushroom is taken
    public void mushroomTaken(GameObject mushroomObject)
    {
        _points += 200;

        mushroomObject.GetComponent<MushroomScript>().mushroomTaken();
    }

    // Function called when a fireflower is taken
    public void fireflowerTaken(GameObject fireflowerObject)
    {
        _points += 200;

        fireflowerObject.GetComponent<FireflowerScript>().fireflowerTaken();
    }

    // Function called when the player dies
    public void playerDies()
    {
        AudioClip playerDiesSound = Resources.Load<AudioClip>("Sounds/smb_mariodie");
        _audioSource.PlayOneShot(playerDiesSound);

        AudioSource overworldSong = GameObject.Find("OverworldSong").GetComponent<AudioSource>();
        overworldSong.Stop();

        StartCoroutine(playerRespawn());
    }

    // Function called when the player wins
    public void playerWins()
    {
        fireworksAnimation();

        AudioClip playerWinsSound = Resources.Load<AudioClip>("Sounds/smb_stage_clear");
        _audioSource.PlayOneShot(playerWinsSound);

        AudioSource overworldSong = GameObject.Find("OverworldSong").GetComponent<AudioSource>();
        overworldSong.Stop();

        PlayerPrefs.DeleteAll();

        if (SceneManager.GetActiveScene().name == "World11")
        {
            StartCoroutine(gameOverScene("World12", 6f));
        }
        else if (SceneManager.GetActiveScene().name == "World12")
        {
            StartCoroutine(gameOverScene("Credits", 6f));
        }
    }

    // Function to change respawn position based on checkpoints
    public void changeRespawnPosition(GameObject checkpointObject)
    {
        switch (checkpointObject.name)
        {
            case "Checkpoint1":
                _respawnPosition = new Vector2(-3.599f, 3.62f);
                break;

            case "Checkpoint2":
                _respawnPosition = new Vector2(14.37f, 3.62f);
                break;

            case "Checkpoint3":
                _respawnPosition = new Vector2(50.48f, 3.62f);
                break;

            case "Checkpoint4":
                _respawnPosition = new Vector2(65.8f, 3.62f);
                break;

            case "Checkpoint5":
                _respawnPosition = new Vector2(132f, 3.62f);
                break;
        }
    }

    // Coroutine for player respawn
    public IEnumerator playerRespawn()
    {
        yield return new WaitForSeconds(3f);

        string currentSceneName = SceneManager.GetActiveScene().name;

        // Save each component of the Vector2 separately
        PlayerPrefs.SetFloat("RespawnX", _respawnPosition.x);
        PlayerPrefs.SetFloat("RespawnY", _respawnPosition.y);

        SceneManager.LoadScene(currentSceneName);
    }

    // Coroutine to load a game over scene with delay
    private IEnumerator gameOverScene(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

    // Function for fireworks animation
    private void fireworksAnimation()
    {
        var particlesGo = (GameObject)Instantiate(fireworks);
        particlesGo.GetComponent<FireworksController>().Explode();
    }
}
