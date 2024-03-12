using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CompanySceneManager : MonoBehaviour
{
    // Duration in seconds of the idle screen before changing the scene
    public float screenDelay = 3.0f;

    void Start()
    {
        StartCoroutine(CompanySceneFlow());
    }

    IEnumerator CompanySceneFlow()
    {
        // Wait for the idle screen time
        yield return new WaitForSeconds(screenDelay);

        SceneManager.LoadScene("Title");
    }
}
