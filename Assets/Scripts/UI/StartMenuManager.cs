using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour
{
    public GameObject menuOptionButtonPrefab;
    public GameObject menuOptionSelector;
    public Transform menuArea;
    public GameObject screen;

    private List<string> _menuOptionsList;
    private int _menuIndex;

    void Start()
    {
        _menuOptionsList = new List<string>();
        _menuOptionsList.Add("Press any key to start!");
        _menuIndex = 0;

        generateButtons();
        adjustScreenSize();

        // Delete all PlayerPrefs to start a clean game
        PlayerPrefs.DeleteAll();
    }

    void Update()
    {
        // Check if any key or controller button has been pressed
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene("World11");
        }
    }

    private void adjustScreenSize()
    {
        float screenHeight = Screen.height;
        float screenWidth = Screen.width;

        RectTransform screenRectTransform = screen.GetComponent<RectTransform>();

        screenRectTransform.sizeDelta = new Vector2(screenWidth, screenHeight);
    }

    private void generateButtons()
    {
        float buttonHeight = menuOptionButtonPrefab.GetComponent<RectTransform>().rect.height;
        Vector2 position = menuOptionButtonPrefab.GetComponent<RectTransform>().anchoredPosition;

        foreach (string option in _menuOptionsList)
        {
            var buttonObject = Instantiate(menuOptionButtonPrefab, menuArea);

            TMP_Text buttonText = buttonObject.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = option;
            }

            buttonObject.GetComponent<RectTransform>().anchoredPosition = position;

            position.y -= buttonHeight;
        }
    }
}
