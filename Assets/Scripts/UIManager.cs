using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance
    {
        get { return instance; }
    }

    [SerializeField] private List<GameObject> uiList;

    public static bool doStayMain;
    public string currentUI { get; private set; }

    private Dictionary<string, GameObject> uiDict = new Dictionary<string, GameObject>();
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        foreach (GameObject ui in uiList)
        {
            uiDict.Add(ui.name, ui);
        }
    }
    void Start()
    {
        if (uiDict.TryGetValue("Main UI", out _))
        {
            SwitchUI("Main UI");
        }
    }
    public void ClearUI()
    {
        foreach (GameObject ui in uiList)
        {
            ui.SetActive(false);
        }
    }
    public void SwitchUI(string newUI)
    {
        ClearUI();
        currentUI = newUI;
        uiDict[currentUI].SetActive(true);
        UAP_AccessibilityManager.StopSpeaking();
        UAP_AccessibilityManager.ResetCurrentContainerFocus();
        for (int i = 0; i < uiDict[currentUI].transform.childCount; i++)
        {
            if (uiDict[currentUI].transform.GetChild(i).gameObject.activeSelf)
            {
                UAP_AccessibilityManager.Say(uiDict[currentUI].transform.GetChild(i).GetComponent<TextMeshProUGUI>().text, true);
                break;
            }
        }
    }
    private void OpenMain()
    {
        SwitchUI("Main UI");
    }
}
