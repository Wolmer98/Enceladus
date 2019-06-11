using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class CharacterScreenOption
{
    [SerializeField] string optionText;

    [SerializeField] string optionHeader;
    [TextArea]
    [SerializeField] string optionInfo;

    public string OptionText { get { return optionText; } }

    public string OptionHeader { get { return optionHeader; } }
    public string OptionInfo { get { return optionInfo; } }

    public CharacterScreenOption(string optionText, string optionInfo, string optionHeader = "")
    {
        this.optionText = optionText;
        this.optionHeader = optionHeader;
        this.optionInfo = optionInfo;
    }
}
