using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using NaughtyAttributes;

public class CharacterScreen : MonoBehaviour
{
    [Header("General")]
    [SerializeField] Text characterName;
    [SerializeField] Image characterPortrait;
    [SerializeField] Button textButtonPrefab;
    [SerializeField] GameObject selectionBox;
    [SerializeField] LayoutGroup optionsLayoutGroup;
    [SerializeField] Text infoText;

    [Space]
    [SerializeField] CanvasGroup leftCanvasGroup;
    [SerializeField] CanvasGroup rightCanvasGroup;
    [SerializeField] CanvasGroup righTextBoxCanvasGroup;

    [Header("Options")]
    [SerializeField] List<CharacterScreenOption> suitOptions;       // 0
    [SerializeField] List<CharacterScreenOption> weaponOptions;     // 1
    [SerializeField] List<CharacterScreenOption> inventoryOptions;  // 2
    [SerializeField] List<CharacterScreenOption> logsOptions;       // 3

    [Header("Stats")]
    [SerializeField] Text SpeedText;
    [SerializeField] Text ResistanceText;
    [SerializeField] Text HealthPointsText;

    [Header("Text Box Screen")]
    [SerializeField] Text textBoxBody;
    [SerializeField] Text textBoxHeader;
    bool textBoxEnabled;

    [Header("StatDescriptions")]
    [SerializeField] bool showDescriptions;
    [ShowIf("showDescriptions")] [TextArea] [SerializeField] string damageDescription;
    [ShowIf("showDescriptions")] [TextArea] [SerializeField] string accuracyDescription;
    [ShowIf("showDescriptions")] [TextArea] [SerializeField] string penetrationDescription;
    [ShowIf("showDescriptions")] [TextArea] [SerializeField] string magSizeDescription;
    [ShowIf("showDescriptions")] [TextArea] [SerializeField] string projectilesDescription;
    [ShowIf("showDescriptions")] [TextArea] [SerializeField] string projectileSpeedDescription;
    [ShowIf("showDescriptions")] [TextArea] [SerializeField] string fireRangeDescription;
    [ShowIf("showDescriptions")] [TextArea] [SerializeField] string aoeDescription;
    [ShowIf("showDescriptions")] [TextArea] [SerializeField] string rpmDescription;
    [ShowIf("showDescriptions")] [TextArea] [SerializeField] string reloadSpeedDescription;
    [ShowIf("showDescriptions")] [TextArea] [SerializeField] string failureChanceDescription;
    [ShowIf("showDescriptions")] [TextArea] [SerializeField] string RestartTimeDescription;

    List<Button> spawnedButtons = new List<Button>();

    PlayerController playerController;
    WorldGenerator worldGenerator;

    public void InitCharacterScreen()
    {
        playerController = FindObjectOfType<PlayerController>();
        worldGenerator = FindObjectOfType<WorldGenerator>();
        SetMenuPage(1);
    }

    private void Update()
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
            return;
        }

        UpdateStats();
    }

    public void SetMenuPage(int index)
    {
        SetMenuPage(index, -1);
    }

    /// <summary>
    /// Spawns all the options from the given menu page to the CharacterScreen. 
    /// </summary>
    public void SetMenuPage(int index, int optionIndex)
    {
        textBoxEnabled = index == 3; // Menu page is log page.

        // Update all UI.
        UpdateAllCanvases();

        // Set correct alpha.
        rightCanvasGroup.alpha = textBoxEnabled ? 0 : 1;
        righTextBoxCanvasGroup.alpha = textBoxEnabled ? 1 : 0;

        // Update all lists.
        UpdateWeaponStats();
        UpdateModsList();
        UpdateLogsList();
        UpdateSuitList();
        DisableSelector();

        SetInfoText("");
        SetHeaderText("");

        if (spawnedButtons != null && spawnedButtons.Count > 0)
        {
            foreach (Button b in spawnedButtons)
            {
                Destroy(b.gameObject);
            }
        }
        spawnedButtons.Clear();

        CharacterScreenOption[] options = GetOptionsFromIndex(index);
        if (options == null || options.Length == 0)
        {
            Debug.LogWarning("The menu page that was opened has no options!");
            return;
        }

        for (int i = 0; i < options.Length; i++)
        {
            Button spawnedButton = Instantiate(textButtonPrefab, optionsLayoutGroup.transform);
            spawnedButton.GetComponentInChildren<Text>().text = options[i].OptionText;

            string infoText = options[i].OptionInfo;
            string headerText = options[i].OptionHeader != "" ? options[i].OptionHeader : "Log";
            spawnedButton.onClick.AddListener(delegate { SetInfoText(infoText); });
            spawnedButton.onClick.AddListener(delegate { SetHeaderText(headerText); });
            spawnedButton.onClick.AddListener(delegate { MoveSelector(spawnedButton); });
            spawnedButton.onClick.AddListener(delegate { DelayedUIUpdate(); });

            // Preselect this option.
            if (optionIndex >= 0 && optionIndex == i)
            {
                spawnedButton.onClick.Invoke();
            }

            spawnedButtons.Add(spawnedButton);
        }

        StartCoroutine(UpdateUIAfterTime(0.05f));
    }

    IEnumerator UpdateUIAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        UpdateAllCanvases();
    }

    private void UpdateAllCanvases()
    {
        leftCanvasGroup.gameObject.SetActive(false);
        rightCanvasGroup.gameObject.SetActive(false);
        righTextBoxCanvasGroup.gameObject.SetActive(false);

        leftCanvasGroup.gameObject.SetActive(true);
        rightCanvasGroup.gameObject.SetActive(true);
        righTextBoxCanvasGroup.gameObject.SetActive(true);
    }

    private void UpdateSuitList()
    {
        suitOptions.Clear();

        if (playerController != null && playerController.SuitMods != null)
        {
            CharacterScreenOption[] suitMods = playerController.SuitMods.ToArray();
            for (int i = 0; i < suitMods.Length; i++)
            {
                suitOptions.Add(suitMods[i]);
            }
        }
    }

    private void UpdateWeaponStats()
    {
        weaponOptions.Clear();

        if (playerController != null && playerController.MainWeapon != null)
        {     
            weaponOptions.Add(new CharacterScreenOption("Damage: " + playerController.MainWeapon.WeaponStats.damage, damageDescription));
            weaponOptions.Add(new CharacterScreenOption("Accuracy: " + Mathf.FloorToInt(playerController.MainWeapon.WeaponStats.accuracy * 100) + "%", accuracyDescription));
            weaponOptions.Add(new CharacterScreenOption("Penetration: " + playerController.MainWeapon.WeaponStats.penetration, penetrationDescription));

            weaponOptions.Add(new CharacterScreenOption("Mag Size: " + playerController.MainWeapon.WeaponStats.magSize, magSizeDescription));

            weaponOptions.Add(new CharacterScreenOption("Projectiles: " + playerController.MainWeapon.WeaponStats.projectileAmount, projectilesDescription));
            weaponOptions.Add(new CharacterScreenOption("Projectile Speed: " + playerController.MainWeapon.WeaponStats.projectileSpeed + " m/s", projectileSpeedDescription));

            weaponOptions.Add(new CharacterScreenOption("Fire Range: " + playerController.MainWeapon.WeaponStats.fireRange + " m", fireRangeDescription));
            weaponOptions.Add(new CharacterScreenOption("AOE: " + playerController.MainWeapon.WeaponStats.areaOfEffect + " m", aoeDescription));
            weaponOptions.Add(new CharacterScreenOption("RPM: " + playerController.MainWeapon.WeaponStats.roundsPerMinute, rpmDescription));
            weaponOptions.Add(new CharacterScreenOption("Reload Speed: " + playerController.MainWeapon.WeaponStats.reloadSpeed, reloadSpeedDescription));

            weaponOptions.Add(new CharacterScreenOption("Failure Chance: " + Mathf.FloorToInt(playerController.MainWeapon.WeaponStats.failureRate * 100) + "%", failureChanceDescription));
            weaponOptions.Add(new CharacterScreenOption("Restart Time: " + playerController.MainWeapon.WeaponStats.restartTime + " sec", RestartTimeDescription));
            //...
        }
    }

    private void UpdateModsList()
    {
        inventoryOptions.Clear();

        if (playerController != null && playerController.MainWeapon != null)
        {
            WeaponMod[] weaponMods = playerController.MainWeapon.WeaponMods;
            inventoryOptions.Add(new CharacterScreenOption("<b>Main Mod:</b> " + weaponMods[0].WeaponModName, weaponMods[0].WeaponModDescription));

            for (int i = weaponMods.Length - 1; i > 0; i--)
            {
                inventoryOptions.Add(new CharacterScreenOption(weaponMods[i].WeaponModName, weaponMods[i].WeaponModDescription));
            }
        }
    }

    private void UpdateLogsList()
    {
        logsOptions.Clear();

        if (worldGenerator != null && GameStateHandler.logData != null && GameStateHandler.logData.Length > 0)
        {
            if (GameStateHandler.unlockedLocalLogs != null && GameStateHandler.unlockedLocalLogs.Count > 0)
            {
                for (int i = 0; i < GameStateHandler.logData.Length; i++)
                {
                    foreach (int logIndex in GameStateHandler.unlockedLocalLogs)
                    {
                        if (logIndex == i)
                        {
                            logsOptions.Add(new CharacterScreenOption(GameStateHandler.logData[i].logName, GameStateHandler.logData[i].text, GameStateHandler.logData[i].header));
                            break;
                        }
                    }            
                }
            } 
        }
    }

    private void UpdateStats()
    {
        if (playerController != null && SpeedText != null && ResistanceText != null && HealthPointsText != null && playerController.FPSController != null && playerController.Destructible != null)
        {
            SpeedText.text = Mathf.FloorToInt(playerController.FPSController.Speed).ToString();
            ResistanceText.text = Mathf.FloorToInt(playerController.Destructible.Resistance).ToString();
            HealthPointsText.text = Mathf.FloorToInt(playerController.Destructible.Health) + "/" + Mathf.FloorToInt(playerController.Destructible.MaxHealth);
        }
    }

    private void DelayedUIUpdate()
    {
        StartCoroutine(UpdateUIAfterTime(0.05f));
    }

    private void MoveSelector(Button button)
    {
        selectionBox.SetActive(true);
        selectionBox.transform.parent = button.transform;
        selectionBox.transform.localPosition = Vector3.zero;
        //selectionBox.transform.parent = transform;
    }

    private void DisableSelector()
    {
        selectionBox.transform.parent = transform;
        selectionBox.SetActive(false);
    }

    public void SetCharacterNameText(string text)
    {
        characterName.text = text;
    }

    public void SetCharacterPortraitImage(Sprite portrait)
    {
        characterPortrait.sprite = portrait;
    }

    private void SetInfoText(string text)
    {
        if (textBoxEnabled)
        {
            textBoxBody.text = text;
        }
        else
        {
            infoText.text = text;
        }       
    }

    private void SetHeaderText(string text)
    {
        if (textBoxEnabled)
        {
            textBoxHeader.text = text;
        }
        else
        {

        }
    }

    private CharacterScreenOption[] GetOptionsFromIndex(int index)
    {
        switch (index)
        {
            case 0:
                return suitOptions.ToArray();
            case 1:
                return weaponOptions.ToArray();
            case 2:
                return inventoryOptions.ToArray();
            case 3:
                return logsOptions.ToArray();
        }

        return null;
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
