using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPartsHolder;
    private List<GameObject> tutorialParts = new List<GameObject>();

    [SerializeField] private GameObject tutorialUI;

    private int index = 0;
    public static bool doContinue;
    private void Start()
    {
        for (int i = 0; i < tutorialPartsHolder.transform.childCount; i++)
        {
            tutorialParts.Add(tutorialPartsHolder.transform.GetChild(i).gameObject);
            tutorialParts[i].SetActive(false);
        }

        tutorialParts[0].SetActive(true);
        StartCoroutine("PlayTutorial");
    }
    private void Update()
    {
        if (Input.GetButtonDown("Continue Tutorial") && !Input.GetButtonDown("Cancel"))
        {
            doContinue = true;
        }
    }
    private IEnumerator PlayTutorial()
    {
        doContinue = false;
        while (!doContinue)
        {
            yield return null;
        }
        tutorialParts[index].SetActive(false);
        index++;
        if (index < tutorialParts.Count)
        {
            tutorialParts[index].SetActive(true);
            UAP_AccessibilityManager.StopSpeaking();
            UAP_AccessibilityManager.ResetCurrentContainerFocus();
            UAP_AccessibilityManager.Say(tutorialParts[index].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
            StartCoroutine("PlayTutorial");
        }
        else
        {
            GameManager.Instance.ExitToMainMenu("Tutorial");
        }
        yield break;
    }
}
