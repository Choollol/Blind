using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();

    private Vector3 enemySpawnPos = new Vector3(0, 0, 20);
    void Start()
    {
        Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], enemySpawnPos, Quaternion.identity);
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        AudioManager.PlaySound("Cheer Sound", 0);
        StartCoroutine(AudioManager.FadeAudio("Cheer Sound", 1, 0.05f));
        yield return new WaitForSeconds(1.5f);
        AudioManager.PlaySound("Battle Start Sound");
        yield break;
    }

}
