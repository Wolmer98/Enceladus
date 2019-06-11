using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using NaughtyAttributes;

[System.Serializable]
public class MainMenuPage
{
    public RectTransform rectTransform;
    public Vector3 enterExitDirection = new Vector3(-1, 0, 0);
}

public class MainMenuController : MonoBehaviour
{
    [SerializeField] MainMenuPage[] menuGroups;
    [SerializeField] int currentMenu;

    [SerializeField] CanvasGroup areYouSureScreen;

    [SerializeField] float lerpSpeed = 1f;

    [Header("Data")]
    [SerializeField] Character_DisplayCard cardPrefab;
    [SerializeField] [ReorderableList] CharacterData[] allCharacters;

    Dictionary<string, CharacterData> nameToCharacterDictionary = new Dictionary<string, CharacterData>();

    CardScroller cardScroller;

    [Header("Music")]
    [FMODUnity.EventRef]
    [SerializeField] string mainThemePath;

    FMOD.Studio.EventInstance mainThemeInstance;

    private void Awake()
    {
        GameStateHandler.InitLogs();

        foreach (CharacterData cd in allCharacters)
        {
            nameToCharacterDictionary.Add(cd.CharacterNameOnly.ToLower(), cd);
        }
    }

    private void Start()
    {
        foreach (MainMenuPage menuGroup in menuGroups)
        {
            menuGroup.rectTransform.gameObject.SetActive(true);
        }

        cardScroller = FindObjectOfType<CardScroller>();
        SpawnAllCards();
        cardScroller.InitCardScroller();

        SetAreYouSureScreen(false);

        mainThemeInstance = FMODUnity.RuntimeManager.CreateInstance(mainThemePath);
        mainThemeInstance.start();
    }

    private void Update()
    {
        for (int i = 0; i < menuGroups.Length; i++)
        {
            if (i == currentMenu)
            {
                menuGroups[i].rectTransform.anchoredPosition = Vector3.Lerp(menuGroups[i].rectTransform.anchoredPosition, Vector3.zero, Time.deltaTime * lerpSpeed);
            }
            else
            {
                menuGroups[i].rectTransform.anchoredPosition = Vector3.Lerp(menuGroups[i].rectTransform.anchoredPosition, menuGroups[i].enterExitDirection * 5000, Time.deltaTime * lerpSpeed);
            }
        }    
    }

    private void SpawnAllCards()
    {
        for (int i = 0; i < allCharacters.Length; i++)
        {
            Character_DisplayCard spawnedCard = Instantiate(cardPrefab, cardScroller.cardParent);
            spawnedCard.SetCardValues(allCharacters[i]);
            spawnedCard.GetComponent<Button>().onClick.AddListener(delegate { cardScroller.FlipCard(); });

            cardScroller.cards.Add(spawnedCard);
        }
    }

    public void SetAreYouSureScreen(bool enabled)
    {
        areYouSureScreen.gameObject.SetActive(enabled);
    }

    public void SetCurrentMenu(int i)
    {
        currentMenu = i;
    }

    public CharacterData GetCharacterByName(string name)
    {
        CharacterData returnData;
        if (nameToCharacterDictionary.TryGetValue(name.ToLower(), out returnData))
        {
            return returnData;
        }

        return null;
    }

    public CharacterData GetCharacterByFirstName(string name)
    {
        for (int i = 0; i < allCharacters.Length; i++)
        {
            if (allCharacters[i].CharacterNameOnly.ToLower().Contains(name.ToLower()))
            {
                return allCharacters[i];
            }
        }

        return null;
    }

    public void StartGame()
    {
        mainThemeInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

        GameStateHandler.characterData = cardScroller.SelectedCard().LinkedCharacterData;
        SceneManager.LoadScene(1);
    }

    public void OnApplicationQuit()
    {
        GameStateHandler.SaveUnlockedLogs();
    }

    public void ExitGame()
    {       
        Application.Quit();
    }
}
