using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character_DisplayCard : MonoBehaviour
{
    bool isLocked;

    public Image background;
    public CanvasGroup cardGroup;

    [Space]
    public CanvasGroup frontGroup;
    public CanvasGroup backGroup;

    [Header("Front")]
    public Image portraitBackground;
    public Image portrait;

    [Space]
    public Text characterClass;
    public Text characterName;

    [Header("Back")]
    public Image back_background;

    public Text healthText;
    public Text speedText;
    public Text resistanceText;

    public Text suitModsText1;
    public Text suitModsText2;
    public Text weaponModsText;
    public Text meleeWeaponsText;

    public Text back_characterClass;

    public CharacterData LinkedCharacterData { get; private set; }
    public RectTransform RectTransform { get; private set; }

    private void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
    }

    public void SetCardValues(CharacterData characterData)
    {
        LinkedCharacterData = characterData;

        // Front
        characterName.text = characterData.CharacterNameOnly;
        characterClass.text = characterData.CharacterClassOnly;

        characterName.color = Color.white; //characterData.CharacterColor;
        characterClass.color = Color.white; //characterData.CharacterColor;

        portraitBackground.color = characterData.CharacterColor;
        portrait.sprite = characterData.Portrait;

        // Back
        back_background.color = characterData.CharacterColor;

        if (!characterData.RandomStats)
        {
            healthText.text = characterData.MaxHealth.ToString();
            speedText.text = characterData.Speed.ToString();
            resistanceText.text = characterData.Resistance.ToString();

            suitModsText1.text = "";
            for (int i = 0; i < 3; i++)
            {
                if (i >= characterData.SuitMods.Length)
                {
                    break;
                }

                suitModsText1.text += characterData.SuitMods[i].SuitModOption.OptionText;
                if (i < characterData.SuitMods.Length - 1)
                {
                    suitModsText1.text += "\n";
                }
            }

            suitModsText2.text = "";
            for (int i = 3; i < 6; i++)
            {
                if (i >= characterData.SuitMods.Length)
                {
                    break;
                }

                suitModsText2.text += characterData.SuitMods[i].SuitModOption.OptionText;
                if (i < characterData.SuitMods.Length - 1)
                {
                    suitModsText2.text += "\n";
                }
            }

            weaponModsText.text = "";
            if (characterData.StartWeaponMod != null)
            {
                weaponModsText.text = characterData.StartWeaponMod.WeaponModName;
            }

            meleeWeaponsText.text = "";
            if (characterData.StartMeleeWeapon != null)
            {
                meleeWeaponsText.text = characterData.StartMeleeWeapon.WeaponName;
            }
        }
        else
        {
            healthText.text = "Random";
            speedText.text = "Random";
            resistanceText.text = "Random";

            suitModsText1.text = "Random";
            suitModsText2.text = "";
            weaponModsText.text = "Random";
            meleeWeaponsText.text = "Random";
        }

        back_characterClass.text = characterData.CharacterClassOnly;
    }
}
