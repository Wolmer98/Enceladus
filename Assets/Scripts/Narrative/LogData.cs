using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

[System.Serializable]
public class LogData
{
    public string logName;
    public string header;

    public int id;

    [TextArea]
    public string text;

    public LogData()
    {

    }
}
