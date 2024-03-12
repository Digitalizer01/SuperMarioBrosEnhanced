using TMPro;
using UnityEngine;

public class InfoUI : MonoBehaviour
{
    // Reference to the GameplayManagerController object
    public GameObject gameplayManagerController;

    // Text objects to display time, coins, and points
    private TMP_Text _infoTimeText;
    private TMP_Text _infoCoinsText;
    private TMP_Text _infoPointsText;

    // Variables to hold time, coins, and points
    [HideInInspector]
    public float _infoTime;

    [HideInInspector]
    public int _infoCoins;

    [HideInInspector]
    public int _infoPoints;

    void Start()
    {
        // Get references to the text objects within the UI
        _infoTimeText = transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        _infoCoinsText = transform.GetChild(1).gameObject.GetComponent<TMP_Text>();
        _infoPointsText = transform.GetChild(2).gameObject.GetComponent<TMP_Text>();

        // Initialize variables for time, coins, and points
        _infoTime = 0f;
        _infoCoins = 0;
        _infoPoints = 0;
    }

    void Update()
    {
        // Update the UI text with the current time, coins, and points
        _infoTimeText.text = "Time: \n" + Mathf.FloorToInt(_infoTime).ToString();
        _infoCoinsText.text = "Coins: \n" + _infoCoins;
        _infoPointsText.text = "Points: \n" + _infoPoints;
    }
}
