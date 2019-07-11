using UnityEngine;
using UnityEngine.SceneManagement;

public class CheatController : MonoBehaviour
{
    [Header("Ctrl + Alt + [Key]")]
    [SerializeField] KeyCode toEndKey = KeyCode.C;
    [SerializeField] KeyCode noClipKey = KeyCode.V;
    [SerializeField] KeyCode toggleDoorsKey = KeyCode.H;
    [SerializeField] KeyCode killEnemiesKey = KeyCode.K;
    [SerializeField] KeyCode deletePlayerPrefsKey = KeyCode.P;
    [SerializeField] KeyCode savePlayerPrefsKey = KeyCode.L;
    [SerializeField] KeyCode reloadSceneKey = KeyCode.X;
    [SerializeField] KeyCode hurtPlayerKey = KeyCode.Z;
    [SerializeField] KeyCode spawnEnemiesKey = KeyCode.B;
    [SerializeField] KeyCode increaseSpeedKey = KeyCode.U;
    [SerializeField] KeyCode disableWater = KeyCode.I;
    [SerializeField] KeyCode addPlayerResist = KeyCode.R;

    WorldGenerator worldGenerator;
    PlayerController playerController;

    private void Awake()
    {
        worldGenerator = FindObjectOfType<WorldGenerator>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt))
        {
            if (Input.GetKeyDown(toEndKey))
            {
                TeleportToEnd();
            }
            else if (Input.GetKeyDown(noClipKey))
            {
                ToggleNoClip();
            }
            else if (Input.GetKey(toggleDoorsKey))
            {
                ToggleAllDoors();
            }
            else if (Input.GetKeyDown(killEnemiesKey))
            {
                KillAllEnemies();
            }
            else if (Input.GetKeyDown(deletePlayerPrefsKey))
            {
                PlayerPrefs.DeleteAll();
            }
            else if (Input.GetKeyDown(savePlayerPrefsKey))
            {
                GameStateHandler.SaveUnlockedLogs();
            }
            else if (Input.GetKeyDown(reloadSceneKey))
            {
                FindObjectOfType<EnemySpawnManager>().RemoveAllEnemies();
                SceneManager.LoadScene(1);
            }
            else if (Input.GetKeyDown(hurtPlayerKey))
            {
                playerController = FindObjectOfType<PlayerController>();
                if (playerController != null)
                {
                    playerController.Destructible.Hurt(50);
                }
            }
            else if (Input.GetKeyDown(spawnEnemiesKey))
            {
                EnemySpawnManager esm = FindObjectOfType<EnemySpawnManager>();

                if (esm != null && playerController != null)
                {
                    esm.SpawnWaveEnemies(playerController.transform.position, 20f, 1);
                }
            }
            else if (Input.GetKeyDown(increaseSpeedKey))
            {
                FPSController playerC = FindObjectOfType<FPSController>();
                if (playerC != null)
                    playerC.AddSpeed(10);
            }
            else if(Input.GetKeyDown(disableWater))
            {
                FindObjectOfType<WaterManager>().gameObject.SetActive(false);
                Debug.Log("Water gameobject has been disabled");
            }
            else if (Input.GetKeyDown(addPlayerResist))
            {
                FindObjectOfType<PlayerController>().gameObject.GetComponent<Destructible>().AddResistance(1000);
                Debug.Log("Player has gained 1000 resistance");
            }
        }

        // No Clip movement.
        if (noClipToggled)
        {
            if (Input.GetKey(KeyCode.Q))
            {
                playerController.transform.position = new Vector3(
                    playerController.transform.position.x,
                    playerController.transform.position.y + Time.deltaTime * 2,
                    playerController.transform.position.z
                );
            }
            else if (Input.GetKey(KeyCode.E))
            {
                playerController.transform.position = new Vector3(
                    playerController.transform.position.x,
                    playerController.transform.position.y - Time.deltaTime * 2,
                    playerController.transform.position.z
                );
            }
        }

    }

    private void TeleportToEnd()
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        playerController.GetComponent<CharacterController>().enabled = false;
        playerController.transform.position = worldGenerator.EndRoom.ConnectionPoints[0].transform.position;
        playerController.GetComponent<CharacterController>().enabled = true;

        Debug.Log("<b>Cheat: </b> Teleport to end.");
    }

    float savedGravity = 9.81f;
    bool noClipToggled;
    private void ToggleNoClip()
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        if (noClipToggled)
        {
            playerController.GetComponent<FPSController>().Gravity = savedGravity;
            playerController.GetComponent<Collider>().enabled = true;
            Debug.Log("<b>Cheat: </b> NoClip Dectivated");
        }
        else
        {
            savedGravity = playerController.GetComponent<FPSController>().Gravity;
            playerController.GetComponent<FPSController>().Gravity = 0;
            playerController.GetComponent<Collider>().enabled = false;
            Debug.Log("<b>Cheat: </b> NoClip Activated");
        }

        noClipToggled = !noClipToggled;
    }

    private void ToggleAllDoors()
    {
        Door[] doors = FindObjectsOfType<Door>();
        if (doors != null && doors.Length > 0)
        {
            foreach (Door door in doors)
            {
                door.Toggle();
            }
        }
    }

    private void KillAllEnemies()
    {
        FindObjectOfType<EnemySpawnManager>().RemoveAllEnemies();
    }

}
