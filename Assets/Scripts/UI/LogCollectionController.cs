using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogCollectionController : MonoBehaviour
{
    [SerializeField] LogOption logOptionPrefab;
    [SerializeField] Transform logOptionLayoutGroup;

    [Space]
    [SerializeField] Text logDisplayText;
    [SerializeField] Text logDisplayTitleText;

    [Space]
    [SerializeField] string currentFilter;

    [Header("Development Options")]
    [SerializeField] bool unlockAllLogsOnStart;

    List<LogOption> spawnedLogOptions = new List<LogOption>();

    MainMenuController mainMenuController;


    private void Start()
    {
        mainMenuController = FindObjectOfType<MainMenuController>();

        if (unlockAllLogsOnStart)
        {
            UnlockAllLogs();
        }

        UpdateLogCollection();
    }

    public void UpdateLogCollection()
    {
        if (GameStateHandler.unlockedLogs == null)
        {
            return;
        }

        ClearLogList();
        ClearLogText();

        if (GameStateHandler.logData != null && GameStateHandler.logData.Length > 0)
        {
            if (GameStateHandler.unlockedLogs != null)
            {
                for (int i = 0; i < GameStateHandler.logData.Length; i++)
                {
                    foreach (int logIndex in GameStateHandler.unlockedLogs)
                    {
                        if (logIndex == i && GameStateHandler.logData[i].header.ToLower().Contains(currentFilter))
                        {
                            LogOption spawnedLogOption = Instantiate(logOptionPrefab, logOptionLayoutGroup);
                            spawnedLogOption.authorText.text = GameStateHandler.logData[i].header;
                            spawnedLogOption.logTitleText.text = GameStateHandler.logData[i].logName;
                            spawnedLogOption.logIconText.text = "#" + GameStateHandler.logData[i].id;

                            CharacterData cd = mainMenuController.GetCharacterByFirstName(GameStateHandler.logData[i].header);
                            if (cd != null)
                            {
                                spawnedLogOption.logIcon.color = cd.CharacterColor;
                            }              

                            int matchingLogIndex = logIndex;
                            spawnedLogOption.GetComponent<Button>().onClick.AddListener(delegate { SelectLog(matchingLogIndex); });

                            spawnedLogOptions.Add(spawnedLogOption);
                            break;
                        }
                    }
                }
            }
        }
    }

    private void ClearLogList()
    {
        foreach (LogOption lo in spawnedLogOptions)
        {
            Destroy(lo.gameObject);
        }

        spawnedLogOptions.Clear();
    }

    private void ClearLogText()
    {
        logDisplayTitleText.text = "";
        logDisplayText.text = "";
    }

    private void UnlockAllLogs()
    {
        if (GameStateHandler.unlockedLogs != null && GameStateHandler.logData != null)
        {
            Debug.Log("Unlocking all logs...");
            GameStateHandler.unlockedLogs.Clear();
            for (int i = 0; i < GameStateHandler.logData.Length; i++)
            {
                GameStateHandler.unlockedLogs.Add(GameStateHandler.logData[i].id);
            }
        }
    }

    private void SelectLog(int index)
    {
        if (GameStateHandler.logData != null && GameStateHandler.logData.Length > index)
        {
            Color color = Color.white;
            CharacterData cd = mainMenuController.GetCharacterByFirstName(GameStateHandler.logData[index].header);
            if (cd != null)
            {
                color = cd.CharacterColor;
            }

            logDisplayTitleText.text = GameStateHandler.logData[index].logName + " <color=#" + ColorUtility.ToHtmlStringRGBA(color) + "><size=28>" + GameStateHandler.logData[index].header + "</size></color>";
            logDisplayText.text = GameStateHandler.logData[index].text;
        }

        StartCoroutine(UpdateUIAfterTime(0.05f));
    }

    IEnumerator UpdateUIAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        UpdateLogScreen();
    }

    private void UpdateLogScreen()
    {
        logDisplayTitleText.gameObject.SetActive(false);
        logDisplayText.gameObject.SetActive(false);

        logDisplayTitleText.gameObject.SetActive(true);
        logDisplayText.gameObject.SetActive(true);
    }

    public void SetLogFilter(string filter)
    {
        currentFilter = filter.ToLower();
        UpdateLogCollection();
    }
}
