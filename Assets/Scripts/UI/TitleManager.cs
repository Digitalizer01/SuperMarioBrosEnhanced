using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class TitleManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject screen;
    public float delayBeforeLoad = 2.0f;

    void Start()
    {
        adjustScreenSize();

        // Start playing the video and load the scene afterward
        StartCoroutine(PlayVideoAndLoadScene());
    }

    private void adjustScreenSize()
    {
        float screenHeight = Screen.height;
        float screenWidth = Screen.width;

        RectTransform screenRectTransform = screen.GetComponent<RectTransform>();

        screenRectTransform.sizeDelta = new Vector2(screenWidth, screenHeight);
    }

    // Coroutine to play the video and load the scene afterward
    IEnumerator PlayVideoAndLoadScene()
    {
        // Wait for the video to finish playing
        yield return new WaitForSeconds((float)videoPlayer.clip.length);

        // Wait for additional time if needed
        yield return new WaitForSeconds(delayBeforeLoad);

        SceneManager.LoadScene("TitleMenu");
    }
}
