using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameStateHandler
{
    public static CharacterData characterData;

    public static List<int> unlockedLocalLogs;
    public static List<int> unlockedLogs;

    public static LogData[] logData;

    static bool hasBeenInitialized = false;

    /// <summary>
    /// Loads all logs from log folder.
    /// </summary>
    public static void InitLogs()
    {
        if (!hasBeenInitialized)
        {
            // Narrative.
            LogData[] loadedLogData = DataManager.GetLogDataFromFolder();
            if (loadedLogData != null && loadedLogData.Length > 0)
            {
                logData = loadedLogData.OrderBy(l => l.id).ToArray();
            }

            Debug.Log("Loaded " + (loadedLogData != null ? loadedLogData.Length.ToString() : "NULL") + " log data from StreamingAssets.");

            LoadUnlockedLogs();
            hasBeenInitialized = true;
        }        
    }

    public static void LocalToGlobalLogsTransfer()
    {
        if (unlockedLogs == null)
        {
            unlockedLogs = new List<int>();
        }

        if (unlockedLocalLogs != null)
        {
            for (int i = 0; i < unlockedLocalLogs.Count; i++)
            {
                if (!unlockedLogs.Contains(unlockedLocalLogs[i]))
                {
                    unlockedLogs.Add(unlockedLocalLogs[i]);
                }         
            }
        }
    }

    public static void LoadUnlockedLogs()
    {
        unlockedLogs = new List<int>();
        for (int i = 0; i < logData.Length; i++)
        {
            int unlockedLog = PlayerPrefs.GetInt("Log_" + i.ToString(), 0);
            if (unlockedLog > 0)
            {
                unlockedLogs.Add(i);
            }
        }
    }

    public static void SaveUnlockedLogs()
    {
        if (unlockedLogs != null && unlockedLogs.Count > 0 && logData != null)
        {
            for (int i = 0; i < logData.Length; i++)
            {
                bool hasLog = false;
                for (int k = 0; k < unlockedLogs.Count; k++)
                {
                    if (i == unlockedLogs[k])
                    {
                        PlayerPrefs.SetInt("Log_" + i.ToString(), 1);
                        hasLog = true;
                        break;
                    }
                }

                if (!hasLog)
                {
                    PlayerPrefs.SetInt("Log_" + i.ToString(), 0);
                }
            }
        }
    }
}
