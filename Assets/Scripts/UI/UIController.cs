using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UIController : MonoBehaviour
{
    [Header("Character Screen Screen")]
    [SerializeField] CanvasGroup characterUI;
    [SerializeField] Image SciFiEffectRenderer;
    [SerializeField] Text levelDisplay;

    [Space]
    [FMODUnity.EventRef]
    [SerializeField] string openScreenSound;
    [FMODUnity.EventRef]
    [SerializeField] string closeScreenSound;

    [Header("Pickup UI")]
    [SerializeField] CanvasGroup pickUpGroup;
    [SerializeField] Text pickupText;
    [Space]
    [SerializeField] CanvasGroup notifyGroup;
    [SerializeField] Text notifyText;
    [SerializeField] float notifyTime = 3f;
    string notificationText;
    bool showNotification;
    bool interactionNotification;
    Vector2Int notificationCSPos;

    [Header("Other UI")]
    [SerializeField] Text magInfoText;
    [SerializeField] Text storageAmmo;
    [SerializeField] CanvasGroup hurtFadeScreen;
    [SerializeField] Image chargeUpImage;
    [SerializeField] Image minChargeUpImage;
    [SerializeField] GameObject miniMap;

    [Header("Loading Screen")]
    [SerializeField] CanvasGroup loadingScreen;
    [SerializeField] Camera loadingScreenCamera;
    [SerializeField] Text loadingText;
    public bool loadingScreenEnabled;

    [Header("Death Screen")]
    [SerializeField] CanvasGroup deathScreen;

    [Header("Screen Blur")]
    [SerializeField] Image screenBlurRenderer;
    float screenBlurTimer = 0;
    float currentScreenBlurSize = 0;

    [SerializeField] PlayerController player;
    WorldGenerator worldGenerator;

    CharacterScreen characterScreen;
    bool characterScreenEnabled;
    float scifiEffectTimer = 0;

    private Queue<Notification> notificationQueue = new Queue<Notification>();

    //public float ScreenBlurPercentage { get { return currentScreenBlurSize / screenBlurCap; } }

    bool hasShownObjective;

    private void Awake()
    {
        characterScreen = FindObjectOfType<CharacterScreen>();
        player = FindObjectOfType<PlayerController>();
        worldGenerator = FindObjectOfType<WorldGenerator>();
        //player.InitPlayer();

        if (characterScreen != null)
        {
            characterScreen.InitCharacterScreen();
        }
        SetCharacterScreen(characterScreenEnabled);

        hasShownObjective = false;

        if (worldGenerator)
        {
            worldGenerator.OnWorldStart.AddListener(delegate { SetElevatorFloor(); });
            worldGenerator.OnWorldStart.AddListener(delegate { ShowObjective(); });
            worldGenerator.OnWorldStart.AddListener(delegate { SetMinimap(); });
        }

        SetCursorLock(true);
    }

    private void Start()
    {
        player.OnInteract.AddListener(delegate { ShowPickupNotification(); });

        SetMinimap(false);
    }

    public void ShowObjective()
    {
        if (hasShownObjective)
        {
            return;
        }

        StartCoroutine(ShowObjectiveCoroutine());
    }

    private IEnumerator ShowObjectiveCoroutine()
    {
        yield return new WaitForSeconds(2f);

        ShowNotification("Facility has been compromised...", 3f);

        yield return new WaitForSeconds(4.5f);

        ShowNotification("Please evacuate through closest <color=orange><b>ELEVATOR</b></color>.", 8f);

        hasShownObjective = true;
    }

    private void Update()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerController>();
            return;
        }

        if (magInfoText && storageAmmo && player != null && player.MainWeapon != null)
        {
            DisplayMagAmmo(player.MainWeapon.CurrentMagAmmo, player.MainWeapon.WeaponStats.magSize, player.MainWeapon.FireBehavior.IsContinuosLaser);
            DisplayStorageAmmo(player.MainWeapon.StorageAmmo, player.MainWeapon.FireBehavior.IsContinuosLaser);

            if (player.MainWeapon.WeaponStats.chargeUp)
            {
                DisplayChargeUpValue(player.MainWeapon.ChargeValue);
            }
            else
            {
                DisplayChargeUpValue(0);
            }
        }

        // Pickup text.
        if (player != null && player.MainCamera != null && pickUpGroup != null)
        {
            RaycastHit hit;

            Pickup pickup = null;
            InstallWeaponMod imw = null;
            if (Physics.Raycast(player.MainCamera.transform.position, player.MainCamera.transform.forward, out hit, player.Reach, player.InteractionMask))
            {
                pickup = hit.transform.GetComponentInParent<Pickup>();
                imw = hit.transform.GetComponentInParent<InstallWeaponMod>();
            }

            bool showPickupText = pickup != null || imw != null;

            pickUpGroup.alpha = Mathf.Lerp(pickUpGroup.alpha, showPickupText ? 1f : 0f, Time.deltaTime * 10);
            if (pickup != null)
            {
                pickupText.text = "<color=orange>[E] </color>" + pickup.PickupName;
            }
            else if (imw != null)
            {
                pickupText.text = "<color=orange>[E] </color>" + "Mod Pack";
            }
        }

        // Notification text
        if (player != null && notifyGroup != null && notifyText != null)
        {
            if (showNotification)
            {
                notifyText.text = notificationText;
                notifyGroup.alpha = Mathf.Lerp(notifyGroup.alpha, 1f, Time.deltaTime * 10);
                notifyText.rectTransform.anchoredPosition = new Vector2(
                    Mathf.Lerp(notifyText.rectTransform.anchoredPosition.x, 20f, Time.deltaTime * 10),
                    0f
                );
            }
            else
            {
                notifyGroup.alpha = Mathf.Lerp(notifyGroup.alpha, 0f, Time.deltaTime * 6);
                notifyText.rectTransform.anchoredPosition = new Vector2(
                    Mathf.Lerp(notifyText.rectTransform.anchoredPosition.x, -1000f, Time.deltaTime * 10),
                    0f
                );
            }
        }

        // Loading screen
        if (loadingScreen)
        {
            if (loadingScreenEnabled)
            {
                loadingScreen.alpha = 1;
                characterUI.alpha = 0;
                loadingScreenCamera.enabled = true;
            }
            else
            {
                if (loadingScreen.alpha < 0.1f)
                {
                    loadingScreen.alpha = 0;
                }
                //loadingScreen.alpha = Mathf.Lerp(loadingScreen.alpha, 0, Time.deltaTime * 5);
                loadingScreen.alpha = 0;

                characterUI.alpha = Mathf.Lerp(characterUI.alpha, 1, Time.deltaTime * 5);

                if (loadingScreenCamera)
                {
                    loadingScreenCamera.enabled = false;
                }
            }
        }

        // Lerp hurt screen to 0 alpha.
        if (hurtFadeScreen)
        {
            hurtFadeScreen.alpha = Mathf.Lerp(hurtFadeScreen.alpha, 0, Time.deltaTime * 7);
        }

        // Display level.
        if (levelDisplay && worldGenerator)
        {
            levelDisplay.text = "Level: " + worldGenerator.CurrentLevel;
        }

        // Character Screen Open.
        if (player != null && Input.GetKeyDown(player.InputData.GetKeyCode("Inventory")))
        {
            scifiEffectTimer = 0;
            characterScreenEnabled = !characterScreenEnabled;

            if (showNotification && interactionNotification)
            {
                Debug.Log("OPENED: " + notificationCSPos.ToString());
                SetCharacterScreen(characterScreenEnabled, notificationCSPos.x, notificationCSPos.y);
            }
            else
            {
                SetCharacterScreen(characterScreenEnabled);
            }
        }

        //if (characterScreenEnabled)
        //{
        //    SciFiEffectRenderer.material.SetFloat("_CurrentTime", scifiEffectTimer);
        //    scifiEffectTimer += Time.deltaTime * 0.3f;
        //}

        if (screenBlurRenderer != null && screenBlurTimer <= 1 && player.MainWeapon != null)
        {
            screenBlurTimer += Time.deltaTime / player.MainWeapon.MainWeaponMod.ScreenBlurTime;
            screenBlurRenderer.material.SetFloat("_BlurSize", Mathf.Lerp(player.MainWeapon.MainWeaponMod.ScreenBlurSize.Evaluate(currentScreenBlurSize / player.MainWeapon.MainWeaponMod.ScreenBlurCap) * 0.01f, 0, screenBlurTimer));
        }
        else if (screenBlurTimer > 1)
        {
            currentScreenBlurSize = 0;
        }
    }

    private Vector2Int CalculateMenuPathFromInteractionType(InteractionType interactionType)
    {
        switch (interactionType)
        {
            case InteractionType.Pickup:
                return new Vector2Int(1, 0);
            case InteractionType.MajorMod:
                return new Vector2Int(2, 0);
            case InteractionType.Mod:
                return new Vector2Int(2, 1);
            case InteractionType.Log:
                return new Vector2Int(3, 0);
        }

        return new Vector2Int(1, 0);
    }

    public void ShowPickupNotification()
    {
        if (player != null && notifyGroup != null && notifyText != null)
        {
            string text = player.LastPickedUpText;
            InteractionType interactionType = player.LastInteractionType;
            // Log text.

            Vector2Int menuPos = CalculateMenuPathFromInteractionType(player.LastInteractionType);
            if (player.LastInteractionType == InteractionType.Log)
            {
                menuPos.y = GetMenuPositionFromLog(player.LastPickedUpLog);
            }

            notificationQueue.Enqueue(new Notification(
                text,
                interactionType,
                menuPos
                )
            );

            StartCoroutine(ShowPickupNotificationText(notifyTime));
        }
    }

    private IEnumerator ShowPickupNotificationText(float time)
    {
        if (showNotification)
        {
            yield break;
        }

        while (notificationQueue.Count > 0)
        {
            showNotification = true;
            Notification currentNotification = notificationQueue.Dequeue();
            interactionNotification = currentNotification.interactionType != InteractionType.Pickup;
            notificationCSPos = currentNotification.menuPosition;

            // Notifications need to hold all the above information in the queue in order to work properly.

            notificationText = currentNotification.text + (interactionNotification ? " <color=orange>[" + player.InputData.GetKeyCode("Inventory").ToString().ToUpper() + "]</color>" : "");
            yield return new WaitForSeconds(time);
            showNotification = false;
            yield return new WaitForSeconds(1f);
        }

        showNotification = false;
    }

    public void ShowNotification(string text, float time)
    {
        StartCoroutine(ShowNotificationText(text, time));
    }

    private IEnumerator ShowNotificationText(string text, float time)
    {
        showNotification = true;

        interactionNotification = false;
        notificationText = text;
        yield return new WaitForSeconds(time);

        showNotification = false;
    }

    private int GetMenuPositionFromLog(int log)
    {
        if (GameStateHandler.unlockedLocalLogs != null)
        {
            List<int> sortedLogs = GameStateHandler.unlockedLocalLogs.OrderBy(l => l).ToList();
            for (int i = 0; i < sortedLogs.Count; i++)
            {
                if (sortedLogs[i] == log)
                {
                    return i;
                }
            }
        }

        return 0;
    }

    private void SetMinimap(bool active = true)
    {
        if (miniMap != null)
        {
            miniMap.SetActive(active);
        }
    }

    private void SetCharacterScreen(bool value, int screenIndex = 1, int optionIndex = -1)
    {
        if (characterScreen == null)
        {
            return;
        }

        FMODUnity.RuntimeManager.PlayOneShot(value ? openScreenSound : closeScreenSound, transform.position);

        characterScreen.GetComponent<CanvasGroup>().alpha = value ? 1 : 0;

        player.IsFrozen = value;

        if (player.FPSController != null)
        {
            player.FPSController.IsFrozen = value;
        }

        SetCursorLock(!value);

        characterScreen.SetMenuPage(screenIndex, optionIndex);
    }

    private void SetElevatorFloor()
    {
        GameObject[] floorTexts = GameObject.FindGameObjectsWithTag("FloorText");
        for (int i = 0; i < floorTexts.Length; i++)
        {
            floorTexts[i].GetComponent<TextMeshPro>().text = worldGenerator.CurrentLevel.ToString();
        }
    }

    private void DisplayMagAmmo(float ammo, float magSize, bool continousAmmo)
    {
        string format = continousAmmo ? "F1" : "F0";
        magInfoText.text = ammo.ToString("F0" /*format*/); // + " / " + magSize;
    }

    private void DisplayStorageAmmo(float ammo, bool continousAmmo)
    {
        string format = continousAmmo ? "F1" : "F0";
        storageAmmo.text = ammo.ToString("F0" /*format*/);
    }

    private void DisplayChargeUpValue(float chargeValue)
    {
        if (chargeUpImage != null && minChargeUpImage != null)
        {

            if (player.MainWeapon.WeaponStats.chargeUp == false)
            {
                minChargeUpImage.enabled = false;
                chargeUpImage.fillAmount = 0;
            }
            else
            {
                chargeUpImage.fillAmount = chargeValue;

                minChargeUpImage.enabled = true;
                minChargeUpImage.rectTransform.anchoredPosition = new Vector2(player.MainWeapon.WeaponStats.minimumChargeUp * 100, minChargeUpImage.rectTransform.anchoredPosition.y);
            }
        }
    }

    /// <summary>
    /// Sets the fade screen to be opaque.
    /// </summary>
    public void TriggerHurtScreen(float value = 0.8f)
    {
        hurtFadeScreen.alpha = value;
    }

    public void SetLoadingValue(string text, float value)
    {
        if (loadingText == null)
        {
            return;
        }

        loadingText.text = "[" + value + "%] " + text;
    }

    public void SetCursorLock(bool value)
    {
        Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !value;
    }

    public void EnableDeathScreen()
    {
        deathScreen.gameObject.SetActive(true);
    }

    public void ScreenBlur()
    {
        screenBlurTimer = 0;
        currentScreenBlurSize += player.MainWeapon.MainWeaponMod.ScreenBlurSizeIncrement;
        currentScreenBlurSize = Mathf.Min(currentScreenBlurSize, player.MainWeapon.MainWeaponMod.ScreenBlurCap);
    }
}

public class Notification
{
    public string text;
    public InteractionType interactionType;
    public Vector2Int menuPosition;

    public Notification(string text, InteractionType interactionType, Vector2Int menuPosition)
    {
        this.text = text;
        this.interactionType = interactionType;
        this.menuPosition = menuPosition;
    }
}
