using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEngine;

public static class DataManager
{
    static string LOG_PATH = Application.streamingAssetsPath + "/Logs/";

    /// <summary>
    /// Searches the folder containing all XML LogData for the game and returns them as the LogData type.
    /// </summary>
    /// <returns>An array of all LogData in the log folder (path defined in DataManager).</returns>
    public static LogData[] GetLogDataFromFolder()
    {
        List<LogData> locatedLogs = new List<LogData>();

        string logDirectoryPath = LOG_PATH;

        if (!Directory.Exists(logDirectoryPath))
        {
            Directory.CreateDirectory(logDirectoryPath);
        }

        string[] filePaths = Directory.GetFiles(logDirectoryPath, "*.xml", SearchOption.AllDirectories);

        foreach (string filePath in filePaths)
        {
            LogData logData = Deserialize<LogData>(filePath);
            locatedLogs.Add(logData);
        }

        if (locatedLogs.Count == 0)
        {
            Debug.Log("No logs could be located.");
            return null;
        }

        Debug.Log("Loaded " + locatedLogs.Count + " logs from folder.");
        return locatedLogs.ToArray();
    }

    /// <summary>
    /// Serializes the given LogData and creates an XML file with the given fileName in the Logs folder.
    /// </summary>
    public static void CreateLogXMLFile(LogData log, string fileName)
    {
        string logDirectoryPath = LOG_PATH;

        if (!Directory.Exists(logDirectoryPath))
        {
            Directory.CreateDirectory(logDirectoryPath);
        }

        string path = logDirectoryPath + fileName + ".xml";

        XMLSerialize(log, path);        
    }

    /// <summary>
    /// Creates a test log xml file.
    /// </summary>
    public static void CreateTestLogData()
    {
        LogData logData = new LogData();
        logData.logName = "#255 Log Menu Name";
        logData.header = "NAME";
        logData.id = 255;
        logData.text = "Here is some text that will be displayed in the log.";

        CreateLogXMLFile(logData, "Log_255_TestLog");
        Debug.Log("Created test LogData XML File.");
    }

    /// <summary>
    /// Serializes any object into a new XML file at the given path.
    /// </summary>
    private static void XMLSerialize(object item, string path)
    {
        XmlSerializer serializer = new XmlSerializer(item.GetType());
        StreamWriter writer = new StreamWriter(path);
        serializer.Serialize(writer.BaseStream, item);
        writer.Close();
    }

    /// <summary>
    /// Deserializes any XML file at the given path.
    /// </summary>
    private static T Deserialize<T>(string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        StreamReader reader = new StreamReader(path);
        T deserialized = (T)serializer.Deserialize(reader.BaseStream);
        reader.Close();
        return deserialized;
    }
}
