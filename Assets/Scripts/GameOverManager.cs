using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    private static GameOverManager instance;
    public static GameOverManager Instance { get { return instance; } }

    [SerializeField] private TextMeshProUGUI gameOverText;
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
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    public void SetText(bool didWin)
    {
        if (didWin)
        {
            gameOverText.text = "You Win";
        }
        else
        {
            gameOverText.text = "You Lose";
        }
    }
}
