using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cargar escenas

public class CreditsManager : MonoBehaviour
{
    public GameObject creditsArea;
    public float scrollSpeed = 1.0f;
    public AudioSource audioSource;

    private bool musicFinished = false;

    void Start()
    {
        StartCoroutine(ScrollCredits());
    }

    // Method to control the scrolling of the credits

    IEnumerator ScrollCredits()
    {
        while (true)
        {
            Vector3 currentPosition = creditsArea.transform.position;
            Vector3 newPosition = new Vector3(
                currentPosition.x,
                currentPosition.y + (scrollSpeed * Time.deltaTime),
                currentPosition.z
            );

            creditsArea.transform.position = newPosition;

            if (!audioSource.isPlaying && !musicFinished)
            {
                musicFinished = true;
                OnMusicComplete();
            }

            yield return null;
        }
    }

    // Method called when the music has finished playing
    void OnMusicComplete()
    {
        SceneManager.LoadScene("GameOver");
    }
}
