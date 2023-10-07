using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    private static MainMenuManager instance;
    public static MainMenuManager Instance { get { return instance; } }

    [SerializeField] private TextMeshProUGUI winsText;
    [SerializeField] private TextMeshProUGUI lossesText;
    [SerializeField] private TextMeshProUGUI instructionsText;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        UpdateInstructions();
    }
    private void Start()
    {
    }

    private void Update()
    {
        
    }
    public void UpdateRecord()
    {
        winsText.text = "Wins: " + GameManager.wins;
        lossesText.text = "Losses: " + GameManager.losses;
    }
    public void UpdateInstructions()
    {
        if (GameManager.hasReadInstructions)
        {
            instructionsText.gameObject.SetActive(false);
        }
    }
}
