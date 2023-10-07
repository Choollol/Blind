using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        MainMenu, InGame, Paused, GameOver
    }

    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    public static bool isGameActive;

    public static int wins { get; private set; }
    public static int losses { get; private set; }

    public static bool hasReadInstructions { get; private set; }

    private GameState state;
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
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (!PlayerPrefs.HasKey("wins"))
        {
            VolumeManager.SetVolume(0.8f);
            Save();
        }
        Load();
        state = GameState.MainMenu;
        //SceneManager.LoadSceneAsync("Menu");
    }

    private void Update()
    {
        switch (state)
        {
            case GameState.MainMenu:
                {
                    MainMenuUpdate();
                    break;
                }
            case GameState.InGame:
                {
                    InGameUpdate();
                    break;
                }
            case GameState.Paused:
                {
                    PausedUpdate();
                    break;
                }
            case GameState.GameOver:
                {
                    GameOverUpdate();
                    break;
                }
        }
    }
    private void MainMenuUpdate()
    {
        if (UIManager.Instance.currentUI == "Main UI")
        {
            if (Input.GetButtonDown("1"))
            {
                StartGame("Menu");
                state = GameState.InGame;
                Enemy.canMove = true;
            }
            else if (Input.GetButtonDown("2"))
            {
                UIManager.Instance.SwitchUI("Controls UI");
            }
            else if (Input.GetButtonDown("3"))
            {
                UIManager.Instance.SwitchUI("Record UI");
                MainMenuManager.Instance.UpdateRecord();
            }
            else if (Input.GetButtonDown("Cancel"))
            {
                Application.Quit();
            }
        }
        else if (Input.GetButtonDown("Cancel"))
        {
            hasReadInstructions = true;
            MainMenuManager.Instance.UpdateInstructions();
            UIManager.Instance.SwitchUI("Main UI");
        }
    }
    private void StartGame(string sceneToUnload)
    {
        Time.timeScale = 1;
        SceneManager.UnloadSceneAsync(sceneToUnload);
        SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
        UAP_AccessibilityManager.StopSpeaking();
        AudioListener.volume = 1;
        StartCoroutine(BeginBattle());
    }
    private IEnumerator BeginBattle()
    {
        yield return new WaitForSeconds(3);
        isGameActive = true;
    }
    private void InGameUpdate()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            PauseGame();
        }
    }
    private void PauseGame()
    {
        isGameActive = false;
        Time.timeScale = 0;
        state = GameState.Paused;
        AudioManager.StopSound("Cheer Sound");
    }
    private void PausedUpdate()
    {
        if (Input.GetButtonDown("Cancel") || Input.GetButtonDown("1"))
        {
            UnpauseGame();
        }
        else if (Input.GetButtonDown("2"))
        {
            ExitToMainMenu();
        }
    }
    private void UnpauseGame()
    {
        isGameActive = true;
        Time.timeScale = 1;
        state = GameState.InGame;
        AudioManager.PlaySound("Cheer Sound");
    }
    private void ExitToMainMenu()
    {
        state = GameState.MainMenu;
        SceneManager.UnloadSceneAsync("Paused");
        SceneManager.UnloadSceneAsync("Game");
        SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Additive);
    }
    public void GameOver()
    {
        state = GameState.GameOver;
        SceneManager.UnloadSceneAsync("Game");
        SceneManager.LoadSceneAsync("Game_Over");
    }
    private void GameOverUpdate()
    {
        if (Input.GetButtonDown("Cancel") || Input.GetButtonDown("1"))
        {
            ExitToMainMenu();
        }
        else if (Input.GetButtonDown("2"))
        {
            StartGame("Game_Over");
        }
    }
    public void ExitToMenu()
    {
        isGameActive = false;
    }
    public void Cheer(float duration, float volume)
    {
        StartCoroutine(RaiseCheerVolume(duration, volume));
    }
    private IEnumerator RaiseCheerVolume(float duration, float volume)
    {
        StartCoroutine(AudioManager.FadeAudio("Cheer Sound", 1, volume));
        yield return new WaitForSeconds(duration);
        StartCoroutine(AudioManager.FadeAudio("Cheer Sound", 1.5f, 0.05f));
    }
    public void PlayerDeath()
    {
        isGameActive = false;
        GameOverManager.Instance.SetText(false);
        Cheer(5, 1f);
        StartCoroutine(PlayerLose());
    }
    private IEnumerator PlayerLose()
    {
        losses++;
        Save();
        yield return new WaitForSeconds(1.5f);
        AudioManager.PlaySound("Battle End Sound");
        yield return new WaitForSeconds(1);
        StartCoroutine(FadeAudioListener(1, 0));
        yield return new WaitForSeconds(2);
        isGameActive = false;
        GameOver();
        yield break;
    }
    public void EnemyDeath()
    {
        GameOverManager.Instance.SetText(true);
        Cheer(7, 1f);
        StartCoroutine(PlayerWin());
        //hurtSound.transform.position = playerController.transform.position + playerController.transform.forward * 2;
    }

    private IEnumerator PlayerWin()
    {
        wins++;
        Save();
        yield return new WaitForSeconds(1.5f);
        AudioManager.PlaySound("Battle End Sound");
        yield return new WaitForSeconds(3.5f);
        StartCoroutine(FadeAudioListener(1, 0));
        yield return new WaitForSeconds(2);
        isGameActive = false;
        GameOver();
        yield break;
    }
    private IEnumerator FadeAudioListener(float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = AudioListener.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            AudioListener.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }
    private void Save()
    {
        PlayerPrefs.SetInt("wins", wins);
        PlayerPrefs.SetInt("losses", losses);
        PlayerPrefs.SetFloat("volume", VolumeManager.volume);
        PlayerPrefs.SetInt("hasReadInstructions", hasReadInstructions ? 1 : 0);
        PlayerPrefs.Save();
    }
    private void Load()
    {
        wins = PlayerPrefs.GetInt("wins");
        losses = PlayerPrefs.GetInt("losses");
        //hasReadInstructions = PlayerPrefs.GetInt("hasReadInstructions") == 1;
        VolumeManager.SetVolume(PlayerPrefs.GetFloat("volume"));
    }
    private void OnApplicationQuit()
    {
        Save();
    }
}
