using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using NaughtyAttributes;

public enum Rarity { Common, Rare, Epic, Legendary, Unique }

[System.Serializable]
public struct RoomRarityChances
{
    [Range(0, 1)] public float commonChance;
    [Range(0, 1)] public float rareChance;
    [Range(0, 1)] public float epicChance;
    [Range(0, 1)] public float legendaryChance;
}

[System.Serializable]
public struct PropRarityChances
{
    [Range(0, 1)] public float commonChance;
    [Range(0, 1)] public float rareChance;
    [Range(0, 1)] public float epicChance;
    [Range(0, 1)] public float legendaryChance;
}

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] [ReorderableList] List<Room> allRooms;
    [SerializeField] [ReorderableList] List<Prop> allProps;
    public PropRarityChances propRarityChances;

    [Header("Room Settings")]
    [SerializeField] Room startRoom;
    [SerializeField] Room winRoom;
    [SerializeField] Vector2Int minMaxRoomCount = new Vector2Int(5, 10);
    int roomCount;
    int maxRoomCount;

    [SerializeField] [Range(0f, 1f)] float doorClosedChance = 0.5f;
    [SerializeField] [Range(0f, 1f)] float ventSpawnChance = 1;

    [SerializeField] bool destroyInvalidRooms;
    [SerializeField] bool generateOnStart;
    [SerializeField] RoomRarityChances roomRarityChances;

    [Header("Sound")]
    [FMODUnity.EventRef]
    [SerializeField] string roomSpawnSound;

    [FMODUnity.EventRef]
    [SerializeField] string roomSuccessSound;

    [FMODUnity.EventRef]
    public string loadingMusicEvent;
    FMOD.Studio.EventInstance loadingMusic;

    [FMODUnity.EventRef]
    public string audioExclamationEvent;
    FMOD.Studio.EventInstance audioExclamation;

    bool audioOnce = false;

    [Header("Other")]
    [SerializeField] Transform loadingScreenCamera;
    [SerializeField] [ReorderableList] List<Material> batchingMaterials = new List<Material>();

    Queue<Room> openSet;
    List<Room> allSpawnedRooms;
    List<Room> sortedCommonRooms = new List<Room>();
    List<Room> sortedEndRooms = new List<Room>();

    List<Room> allTESTSpawnedRooms;

    HashSet<Room> allRoomsSet = new HashSet<Room>();
    HashSet<Room> commonRooms = new HashSet<Room>();
    HashSet<Room> rareRooms = new HashSet<Room>();
    HashSet<Room> epicRooms = new HashSet<Room>();
    HashSet<Room> legendaryRooms = new HashSet<Room>();
    HashSet<Room> uniqueRooms = new HashSet<Room>();

    bool isBuildingWorld;
    float worldBuildTimer;

    Dictionary<ConnectionPoint, ConnectionPoint> connectionDictionary = new Dictionary<ConnectionPoint, ConnectionPoint>();

    EnemySpawnManager enemySpawnManager;
    UIController uiController;
    PlayerController player;
    GameObject playerContainer;

    // Other.
    List<GameObject> batchList = new List<GameObject>();
    public UnityEvent OnWorldStart;

    public Room StartRoom { get; private set; }
    public Room EndRoom { get; private set; }

    public Vector3 PlayerSpawnPos { get; private set; }
    public int CurrentLevel { get; private set; }

    public void OnApplicationQuit()
    {
        GameStateHandler.LocalToGlobalLogsTransfer();
        GameStateHandler.SaveUnlockedLogs();
    }

    private void Awake()
    {
        // Secondary log init (for testing).
        GameStateHandler.InitLogs();

        // Initializing.
        playerContainer = GameObject.FindGameObjectWithTag("Player_Container");
        player = FindObjectOfType<PlayerController>();
        uiController = FindObjectOfType<UIController>();
        enemySpawnManager = FindObjectOfType<EnemySpawnManager>();

        // Local to global logs on player death.
        player.GetComponent<Destructible>().OnDeath.AddListener(delegate { GameStateHandler.LocalToGlobalLogsTransfer(); });

        PlayerSpawnPos = player.transform.position;

        //Music
        loadingMusic = FMODUnity.RuntimeManager.CreateInstance(loadingMusicEvent);
        loadingMusic.start();

        // Generator.
        foreach (Room r in allRooms)
        {
            switch (r.Rarity)
            {
                case Rarity.Common:
                    commonRooms.Add(r);
                    break;
                case Rarity.Rare:
                    rareRooms.Add(r);
                    break;
                case Rarity.Epic:
                    epicRooms.Add(r);
                    break;
                case Rarity.Legendary:
                    legendaryRooms.Add(r);
                    break;
                case Rarity.Unique:
                    uniqueRooms.Add(r);
                    break;
            }

            allRoomsSet.Add(r);

            if (r.ConnectionPoints != null && r.ConnectionPoints.Length > 0)
            {
                if (r.ConnectionPoints.Length == 1)
                {
                    sortedEndRooms.Add(r);
                }
            }
            else
            {
                Debug.LogError("Room: " + r.gameObject.name + " does not have any connection points!");
            }
        }

        // Order common rooms after Size.
        sortedCommonRooms = commonRooms.OrderBy(r => r.Volume).ToList();

        if (generateOnStart)
        {
            StopAllCoroutines();
            //StartCoroutine(DestroyWorld());
            StartCoroutine(GenerateWorld(true));
        }

        GameStateHandler.unlockedLocalLogs = new List<int>();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.G) && !isBuildingWorld)
        //{
        //    StopAllCoroutines();
        //    StartCoroutine(DestroyWorld());
        //    //StartCoroutine(GenerateWorld());
        //}

        if (isBuildingWorld)
        {
            worldBuildTimer += Time.deltaTime;
        }

        uiController.loadingScreenEnabled = isBuildingWorld;


    }

    /// <summary>
    /// Generates a new world from the last EndRoom.
    /// </summary>
    public void GenerateWorldFromEndRoom()
    {
        if (EndRoom == null)
        {
            Debug.LogError("EndRoom is null! Can't generate world from it.");
            return;
        }

        StopAllCoroutines();
        StartCoroutine(DestroyWorld(true));
        //StartCoroutine(GenerateWorld(true));

        CurrentLevel++;
    }

    private IEnumerator DestroyWorld(bool originRoom = false)
    {
        //Remove all sounds first.
        StopAllFMODEmitters();

        enemySpawnManager.ClearSpawnPoints();
        enemySpawnManager.RemoveAllEnemies();

        GameObject[] ragdolls = GameObject.FindGameObjectsWithTag("Ragdoll");
        if (ragdolls != null && ragdolls.Length > 0)
        {
            foreach (GameObject r in ragdolls)
            {
                Destroy(r);
            }
        }

        if (player != null)
        {
            player.GetComponent<OcclusionCulling>().ClearRenderReferences();
        }

        if (allSpawnedRooms == null)
        {
            yield break; // return;
        }

        foreach (Room room in allSpawnedRooms)
        {
            if (originRoom && room == EndRoom)
            {
                room.ResetConnectionPoints();
                continue;
            }

            Destroy(room.gameObject);
        }

        yield return new WaitForSeconds(2);
        //yield return new WaitUntil(() => RoomsAreDestroyed());

        int temp = 0;
        foreach (Room room in allSpawnedRooms)
        {
            if (room != null)
            {
                temp++;
            }
        }
        Debug.Log("ROOMS LEFT: " + temp);

        allSpawnedRooms.Clear();

        if (allTESTSpawnedRooms == null)
        {
            yield break; // return;
        }

        foreach (Room room in allTESTSpawnedRooms)
        {
            Destroy(room.gameObject);
        }

        allTESTSpawnedRooms.Clear();

        StartCoroutine(GenerateWorld(true));
    }

    private IEnumerator GenerateWorld(bool originRoom = false)
    {
        //yield return new WaitUntil(() => (transform.childCount == 0 || transform.childCount == 1));

        if (!originRoom)
        {
            player.transform.position = PlayerSpawnPos;
            player.transform.localRotation = Quaternion.identity;
        }

        connectionDictionary.Clear();

        //playerContainer.SetActive(false);
        //player.gameObject.SetActive(false);
        player.IsFrozen = true;
        player.GetComponent<FPSController>().IsFrozen = true;

        isBuildingWorld = true;
        Debug.ClearDeveloperConsole();

        maxRoomCount = Random.Range(minMaxRoomCount.x, minMaxRoomCount.y);
        roomCount = 0;

        openSet = new Queue<Room>();
        allSpawnedRooms = new List<Room>();
        allTESTSpawnedRooms = new List<Room>();

        /*--START ROOM--*/

        if (CurrentLevel == 0 && startRoom != null)
        {
            List<Room> startRooms = new List<Room>();
            startRooms.Add(startRoom);
            EndRoom = GenerateRoom(startRooms, 0);
        }

        /*--------------*/

        if (!originRoom)
        {
            StartRoom = GenerateRoom(sortedCommonRooms, allRooms.Count);
            StartRoom.gameObject.name = "Room_Start";
        }
        else
        {
            EndRoom.ResetConnectionPoints();
            //EndRoom.GetComponentInChildren<WorldButton>().onPressEvents = null;

            StartRoom = EndRoom;

            if (loadingScreenCamera)
            {
                loadingScreenCamera.position = StartRoom.transform.position;
            }
        }

        allSpawnedRooms.Add(StartRoom);
        openSet.Enqueue(StartRoom);

        //Debug.Log("<b> =======| Generating World |======= </b>");
        //Debug.Log("<b>WorldGenerator:</b> Building Primary Rooms... <b>ROOM TARGET: </b>" + maxRoomCount);
        //////////////////////////////
        // -- Primary Room Spawn -- //
        //////////////////////////////
        while (openSet.Count > 0)
        {
            // Find active connection point.
            Room activeRoom = openSet.Dequeue();
            ConnectionPoint activeConnectionPoint = null;

            // Find a connection point that has no connection.
            for (int i = 0; i < activeRoom.ConnectionPoints.Length; i++)
            {
                if (activeRoom.ConnectionPoints[i].Connected == false)
                {
                    activeConnectionPoint = activeRoom.ConnectionPoints[i];
                    break;
                }
            }

            // Spawn new room.
            Room spawnedRoom = null;
            bool hasOpenConnectionPoints = false;
            for (int i = sortedCommonRooms.Count - 1; i >= 0; i--)
            {
                FMODUnity.RuntimeManager.PlayOneShot(roomSpawnSound, transform.position);

                spawnedRoom = GenerateRoom(sortedCommonRooms, i + 1);
                ConnectionPoint spawnedConnectionPoint = null;
                if (spawnedRoom.ConnectionPoints.Length > 0)
                {
                    spawnedConnectionPoint = spawnedRoom.ConnectionPoints[Random.Range(0, spawnedRoom.ConnectionPoints.Length)];
                }

                AlignConnections(spawnedRoom, activeConnectionPoint, spawnedConnectionPoint);

                //CheckRoomCollision(spawnedRoom, true);
                yield return new WaitForSeconds(0.1f);
                //yield return new WaitUntil(() => BoundsHaveUpdated(spawnedRoom.BoundsArray[0]));

                bool connectionSuccess = !CheckRoomCollision(spawnedRoom);

                if (connectionSuccess)
                {
                    activeConnectionPoint.Connected = true;
                    spawnedConnectionPoint.Connected = true;

                    //activeConnectionPoint.NavMeshLink.area = spawnedConnectionPoint.NavMeshLink.area = 1;
                    //activeConnectionPoint.NavMeshLink.UpdateLink();

                    //Debug.Log("Align Success");
                    for (int j = 0; j < spawnedRoom.ConnectionPoints.Length; j++)
                    {
                        if (spawnedRoom.ConnectionPoints[j].Connected == false)
                        {
                            hasOpenConnectionPoints = true;
                            break;
                        }
                    }

                    if (hasOpenConnectionPoints)
                    {
                        openSet.Enqueue(spawnedRoom);
                    }

                    allSpawnedRooms.Add(spawnedRoom);
                    spawnedRoom.gameObject.name = "Room_" + roomCount;

                    // Manage doors.
                    if (spawnedConnectionPoint.Door != null)
                    {
                        Destroy(spawnedConnectionPoint.Door.gameObject);
                    }
                    if (activeConnectionPoint.Door != null)
                    {
                        if (Random.Range(0f, 1f) < (1 - doorClosedChance))
                        {
                            //activeConnectionPoint.Door.Toggle();
                            activeConnectionPoint.NavMeshLink.area = NavMesh.GetAreaFromName("Walkable");
                            activeConnectionPoint.NavMeshLink.UpdateLink();
                            Destroy(activeConnectionPoint.Door.gameObject);
                        }
                    }

                    // Remove NavMeshLink
                    if (spawnedConnectionPoint.NavMeshLink != null)
                    {
                        //Destroy(spawnedConnectionPoint.NavMeshLink);
                       // spawnedConnectionPoint.NavMeshLink.area = 1; //i.e. not walkable.
                    }

                    connectionDictionary.Add(activeConnectionPoint, spawnedConnectionPoint);
                    connectionDictionary.Add(spawnedConnectionPoint, activeConnectionPoint);

                    FMODUnity.RuntimeManager.PlayOneShot(roomSuccessSound, transform.position);

                    break;
                }
                else
                {
                    activeConnectionPoint.Connected = false;
                    spawnedConnectionPoint.Connected = false;

                    if (destroyInvalidRooms)
                    {
                        Destroy(spawnedRoom.gameObject);
                    }
                    else
                    {
                        //Debug.Log("Align Failure");
                        //Debug.Log("<b><color=red>DESTROYED</color></b> " + spawnedRoom.gameObject.name);
                        spawnedRoom.GetComponentInChildren<Renderer>().material.color = Color.red;
                        allTESTSpawnedRooms.Add(spawnedRoom);
                    }

                    // Try a smaller room.
                    spawnedRoom = null;

                    continue;
                }
            }

            if (spawnedRoom == null)
            {
                continue;
            }

            // Active room check.
            hasOpenConnectionPoints = false;
            for (int i = 0; i < activeRoom.ConnectionPoints.Length; i++)
            {
                if (activeRoom.ConnectionPoints[i].Connected == false)
                {
                    hasOpenConnectionPoints = true;
                    break;
                }
            }

            if (hasOpenConnectionPoints)
            {
                openSet.Enqueue(activeRoom);
            }

            roomCount++;
            uiController.SetLoadingValue("Generating World...", Mathf.Clamp(Mathf.RoundToInt(((float)roomCount / (float)maxRoomCount) * 100), 0, 100));

            if (roomCount >= maxRoomCount)
            {
                // Close all connections.
                break;
            }
        }

        //////////////////////////
        // -- Win Room Spawn -- //
        //////////////////////////
        if (winRoom)
        {
            //Debug.Log("<b>WorldGenerator:</b> Building Win Room. <color=teal>[" + worldBuildTimer.ToString("0.0") + " sec]</color>");

            Room endWinRoomReplacement = StartRoom;
            ConnectionPoint endRoomCP = null;

            // Look for a room that has both an open and closed connection point (and is of the right volume) as far away from the player as possible.
            foreach (Room activeRoom in allSpawnedRooms)
            {
                if (activeRoom.Volume <= winRoom.Volume)
                {
                    continue;
                }

                float newDistance = Vector3.Distance(player.transform.position, activeRoom.transform.position);
                float oldDistance = Vector3.Distance(player.transform.position, endWinRoomReplacement.transform.position);

                if (newDistance > oldDistance)
                {
                    ConnectionPoint connectionPoint = null;
                    int connectionPointCount = 0;
                    // Active room check, has open connection points (is end roon)?.
                    for (int i = 0; i < activeRoom.ConnectionPoints.Length; i++)
                    {
                        if (activeRoom.ConnectionPoints[i].Connected == true)
                        {
                            connectionPoint = activeRoom.ConnectionPoints[i];
                            connectionPointCount++;
                        }
                    }

                    if (connectionPoint != null && connectionPointCount == 1)
                    {
                        endRoomCP = GetConnectedPoint(connectionPoint);
                        if (endRoomCP != null)
                        {
                            endWinRoomReplacement = activeRoom;
                        }
                    }
                }
            }

            // Remove
            if (endRoomCP != null && endWinRoomReplacement != null && endWinRoomReplacement != StartRoom)
            {
                // Destroy old room.
                allSpawnedRooms.Remove(endWinRoomReplacement);
                Destroy(endWinRoomReplacement.gameObject);

                connectionDictionary.Remove(endRoomCP);
                endRoomCP.Connected = false;

                //Debug.Log(endRoomCP + " " + endRoomCP.gameObject.name + " " + endRoomCP.transform.position + " " + endRoomCP.GetComponentInParent<Room>().gameObject.name);

                // Spawn win room.
                EndRoom = Instantiate(winRoom, transform);

                yield return new WaitForSeconds(0.1f);

                AlignConnections(EndRoom, endRoomCP, EndRoom.ConnectionPoints[0]);
                EndRoom.ConnectionPoints[0].Connected = true;
                endRoomCP.Connected = true;

                connectionDictionary.Add(EndRoom.ConnectionPoints[0], endRoomCP);
                connectionDictionary.Add(endRoomCP, EndRoom.ConnectionPoints[0]);

                if (endRoomCP.Door != null)
                {
                    Destroy(endRoomCP.Door.gameObject);
                }

                EndRoom.GetComponentInChildren<WorldButton>().onPressEvents[1].uEvent.AddListener(delegate { GenerateWorldFromEndRoom(); });
                allSpawnedRooms.Add(EndRoom);

                //Debug.Log("<b>WorldGenerator:</b> Win Room SUCCESS!");
            }
            else
            {
                Debug.Log("<b>WorldGenerator:</b> Win Room FAILURE!");
            }
        }
        else
        {
            Debug.Log("<b>WorldGenerator:</b> No Win-Room Prefab Assigned.");
        }

        //Debug.Log("<b>WorldGenerator:</b> Building Secondary Rooms (end knots)... <color=teal>[" + worldBuildTimer.ToString("0.0") + " sec]</color>");
        ////////////////////////////////////////////
        // -- Secondary Room Spawn (end knots) -- //
        ////////////////////////////////////////////
        List<Room> allPrimaryRooms = new List<Room>(allSpawnedRooms);
        int checkedRoomCount = 0;
        int endRoomCount = 0;
        foreach (Room activeRoom in allPrimaryRooms)
        {
            ConnectionPoint activeConnectionPoint = null;

            for (int i = 0; i < activeRoom.ConnectionPoints.Length; i++)
            {
                if (activeRoom.ConnectionPoints[i].Connected == false)
                {
                    activeConnectionPoint = activeRoom.ConnectionPoints[i];

                    for (int j = sortedEndRooms.Count - 1; j >= 0; j--)
                    {
                        FMODUnity.RuntimeManager.PlayOneShot(roomSpawnSound, transform.position);

                        Room spawnedRoom = GenerateRoom(sortedEndRooms, j);
                        ConnectionPoint spawnedConnectionPoint = spawnedRoom.ConnectionPoints[0];

                        AlignConnections(spawnedRoom, activeConnectionPoint, spawnedConnectionPoint);

                        yield return new WaitUntil(() => BoundsHaveUpdated(spawnedRoom.BoundsArray[0]));
                        bool connectionSuccess = !CheckRoomCollision(spawnedRoom);

                        if (connectionSuccess)
                        {
                            endRoomCount++;
                            activeConnectionPoint.Connected = true;
                            spawnedConnectionPoint.Connected = true;

                            allSpawnedRooms.Add(spawnedRoom);
                            spawnedRoom.gameObject.name = "Room_" + roomCount + endRoomCount;

                            // Manage doors.
                            if (spawnedConnectionPoint.Door != null)
                            {
                                Destroy(spawnedConnectionPoint.Door.gameObject);
                            }
                            if (activeConnectionPoint.Door != null)
                            {
                                if (Random.Range(0f, 1f) < (1 - doorClosedChance))
                                {
                                    //activeConnectionPoint.NavMeshLink.area = NavMesh.GetAreaFromName("Walkable");
                                    //activeConnectionPoint.NavMeshLink.UpdateLink();
                                    Destroy(activeConnectionPoint.Door.gameObject);
                                }
                            }

                            connectionDictionary.Add(activeConnectionPoint, spawnedConnectionPoint);
                            connectionDictionary.Add(spawnedConnectionPoint, activeConnectionPoint);

                            FMODUnity.RuntimeManager.PlayOneShot(roomSuccessSound, transform.position);

                            break;
                        }
                        else
                        {
                            activeConnectionPoint.Connected = false;
                            spawnedConnectionPoint.Connected = false;

                            if (destroyInvalidRooms)
                            {
                                Destroy(spawnedRoom.gameObject);
                            }
                            else
                            {
                                //Debug.Log("Align Failure");
                                //Debug.Log("<b><color=red>DESTROYED</color></b> " + spawnedRoom.gameObject.name);
                                spawnedRoom.GetComponentInChildren<Renderer>().material.color = Color.red;
                                allTESTSpawnedRooms.Add(spawnedRoom);
                            }

                            // Try a smaller room.
                            spawnedRoom = null;
                            continue;
                        }
                    }

                    if (activeConnectionPoint.Connected == false && activeConnectionPoint.Door != null)
                    {
                        activeConnectionPoint.Door.Lock();
                    }
                }
            }

            checkedRoomCount++;
            uiController.SetLoadingValue("Closing Gaps...", Mathf.Clamp(Mathf.RoundToInt(((float)checkedRoomCount / (float)maxRoomCount) * 100), 0, 100));
        }

        //Spawn the Props.
        //Debug.Log("<b>WorldGenerator:</b> Spawning Props. <color=teal>[" + worldBuildTimer.ToString("0.0") + " sec]</color>");
        SpawnProps();

        //Assign the logs.
        //Debug.Log("<b>WorldGenerator:</b> Assigning Logs. <color=teal>[" + worldBuildTimer.ToString("0.0") + " sec]</color>");
        AssignLogs();

        //Spawn the Vents.
        //Debug.Log("<b>WorldGenerator:</b> Spawning Vents. <color=teal>[" + worldBuildTimer.ToString("0.0") + " sec]</color>");
        SpawnVents();

        //Build the NavMesh.
        //Debug.Log("<b>WorldGenerator:</b> Building NavMesh. <color=teal>[" + worldBuildTimer.ToString("0.0") + " sec]</color>");
        BuildNavMeshLinks();

        //Batching.
        //Debug.Log("<b>WorldGenerator:</b> Batching rooms. <color=teal>[" + worldBuildTimer.ToString("0.0") + " sec]</color>");
        batchList = new List<GameObject>();
        //batchList.AddRange(GameObject.FindGameObjectsWithTag("Wall"));
        //batchList.AddRange(GameObject.FindGameObjectsWithTag("Floor"));
        //batchList.AddRange(GameObject.FindGameObjectsWithTag("Ceiling"));
        //batchList.AddRange(GameObject.FindGameObjectsWithTag("Corner"));
        //batchList.AddRange(GameObject.FindGameObjectsWithTag("DoorFrame"));

        List<GameObject> environments = new List<GameObject>();
        foreach (Room room in allSpawnedRooms)
        {
            GameObject environment = room.transform.Find("Environment").gameObject;
            Renderer[] envRenderers = environment.transform.GetComponentsInChildren<Renderer>();
            foreach (Renderer env in envRenderers)
            {
                if (env.CompareTag("DoorFrame") == false && batchingMaterials.Contains(env.sharedMaterial))
                {
                    batchList.Add(env.gameObject);
                }
            }
        }


        StaticBatchingUtility.Combine(batchList.ToArray(), gameObject);

        //Remove Unused textures.
        Resources.UnloadUnusedAssets();

        // Player Spawn.
        //Debug.Log("<b>WorldGenerator: <color=teal>Spawning Player!</color></b>");
        //playerContainer.SetActive(true);
        //player.gameObject.SetActive(true);
        player.transform.position = StartRoom.transform.position;

        player.IsFrozen = false;
        player.GetComponent<FPSController>().IsFrozen = false;
        player.InitPlayer();

        // World Generation Complete.
        Debug.Log("<b>WorldGenerator:</b> <color=green>World Complete.</color> <color=teal>[" + worldBuildTimer.ToString("0.0") + " sec]</color>" + " <b>TOTAL ROOMS:</b> " + allSpawnedRooms.Count);
        isBuildingWorld = false;
        worldBuildTimer = 0f;

        //STOP MUSIC HERE
        loadingMusic.setParameterByName("Music", 0f);
        loadingMusic.release();

        if (audioOnce == false)
        {
            FMODUnity.RuntimeManager.PlayOneShot(audioExclamationEvent);
            audioExclamation.release();
            audioOnce = !false;
        }

        if (enemySpawnManager)
        {
            enemySpawnManager.InitSpawnManager();
        }
        OnWorldStart.Invoke();
    }

    private bool BoundsHaveUpdated(Bounds bounds)
    {
        if (bounds.center != Vector3.zero)
        {
            return true;
        }

        return false;
    }

    private bool RoomsAreDestroyed()
    {
        foreach (Room room in allSpawnedRooms)
        {
            if (room != null && !room.name.Contains("Win"))
            {
                return false;
            }
        }
        return true;

    }

    private Room GenerateRoom(List<Room> roomList, int topRoomID)
    {
        Room randomRoomPrefab = null;
        if (topRoomID >= roomList.Count)
        {
            randomRoomPrefab = GetRoomBasedOnRarity(allRoomsSet);
        }
        else
        {
            randomRoomPrefab = roomList[Random.Range(0, topRoomID)];
        }

        Room spawnedRoom = Instantiate(randomRoomPrefab, transform);
        return spawnedRoom;
    }

    /// <summary>
    /// Gets a random room from the given set based on rarity.
    /// </summary>
    private Room GetRoomBasedOnRarity(HashSet<Room> rooms)
    {
        float chance = Random.Range(0f, 1f);

        HashSet<Room> pickableRooms = new HashSet<Room>();

        if (chance <= roomRarityChances.legendaryChance) // Legendary event chance.
        {
            pickableRooms.UnionWith(legendaryRooms);
        }
        if (chance <= roomRarityChances.epicChance) // Epic event chance.
        {
            pickableRooms.UnionWith(epicRooms);
        }
        if (chance <= roomRarityChances.rareChance) // Rare event chance.
        {
            pickableRooms.UnionWith(rareRooms);
        }
        if (chance <= roomRarityChances.commonChance) // Common event chance.
        {
            pickableRooms.UnionWith(commonRooms);
        }

        pickableRooms.IntersectWith(rooms);
        List<Room> roomList = new List<Room>(pickableRooms);

        if (roomList.Count == 0)
        {
            Debug.LogError("No rooms with weight could be found. Make sure you've set the 'Room Rarity Chances' in the World Generator.", this);
        }

        return roomList[Random.Range(0, roomList.Count)];
    }

    private void AlignConnections(Room alignRoom, ConnectionPoint staticPoint, ConnectionPoint alignPoint)
    {
        float rotationAngle = Vector3.SignedAngle(alignPoint.transform.forward, -staticPoint.transform.forward, Vector3.up);
        alignRoom.transform.Rotate(new Vector3(0, rotationAngle, 0), Space.World);

        Vector3 connectionDis = staticPoint.transform.position - alignPoint.transform.position;
        Vector3 newPos = alignRoom.transform.position + connectionDis;

        //Debug.Log("New Center: " + newPos);

        alignRoom.transform.Translate(connectionDis, Space.World);
    }

    private void AlignConnectionToVector(Room alignRoom, ConnectionPoint alignPoint, Vector3 connectPoint, Vector3 connectForward)
    {
        float rotationAngle = Vector3.SignedAngle(alignPoint.transform.forward, connectForward, Vector3.up);
        alignRoom.transform.Rotate(new Vector3(0, rotationAngle, 0), Space.World);

        Vector3 connectionDis = connectPoint - alignPoint.transform.position;
        Vector3 newPos = alignRoom.transform.position + connectionDis;

        //Debug.Log("New Center: " + newPos);

        alignRoom.transform.Translate(connectionDis, Space.World);
    }

    private bool CheckRoomCollision(Room room, bool before = false)
    {
        if (room.Colliders == null || room.Colliders.Length == 0)
        {
            // No colliders, no collisions.
            return false;
        }

        for (int i = 0; i < room.Colliders.Length; i++)
        {
            //room.Colliders[i].bounds = center;
            Bounds roomBounds = room.Colliders[i].bounds;
            //roomBounds.center = center;

            for (int j = 0; j < allSpawnedRooms.Count; j++)
            {
                if (room == allSpawnedRooms[j])
                {
                    continue;
                }

                for (int k = 0; k < allSpawnedRooms[j].Colliders.Length; k++)
                {
                    Bounds otherRoomBounds = allSpawnedRooms[j].Colliders[k].bounds;
                    //otherRoomBounds.center = allSpawnedRooms[j].transform.position;

                    if (roomBounds.Intersects(otherRoomBounds))
                    {
                        //Debug.Log((before ? "BEFORE: " : "AFTER: ") + "<b>Bounds:</b>" + roomBounds);
                        //Debug.Log(room.gameObject.name + " intersected with " + allSpawnedRooms[j].gameObject.name + "!");
                        //Debug.Log("<b>BoundsA:</b> " + roomBounds.ToString() + " | <b>BoundsB:</b> " + otherRoomBounds.ToString());
                        return true;
                    }
                    else
                    {
                        //Debug.Log(room.gameObject.name + " <b>DID NOT</b> intersec with " + allSpawnedRooms[j].gameObject.name + "!");
                        //Debug.Log("<b>BoundsA:</b> " + roomBounds.ToString() + " | <b>BoundsB:</b> " + otherRoomBounds.ToString());
                    }
                }
            }
        }

        return false;
    }

    private ConnectionPoint GetConnectedPoint(ConnectionPoint connectionPoint)
    {
        ConnectionPoint connectedPoint = null;
        if (connectionDictionary.TryGetValue(connectionPoint, out connectedPoint))
        {
            return connectedPoint;
        }
        else
        {
            return null;
        }
    }

    private void BuildNavMesh()
    {
        NavMeshSurface[] surfaces = FindObjectsOfType<NavMeshSurface>();
        for (int i = 0; i < surfaces.Length; i++)
        {
            if (surfaces[i].navMeshData != null)
            {
                surfaces[i].BuildNavMesh();
            }
        }
    }

    private void BuildNavMeshLinks()
    {
        NavMeshLink[] surfaces = FindObjectsOfType<NavMeshLink>();
        for (int i = 0; i < surfaces.Length; i++)
        {
            surfaces[i].UpdateLink();
        }
    }

    public void StopAllFMODEmitters()
    {
        FMODUnity.StudioEventEmitter[] instances = FindObjectsOfType<FMODUnity.StudioEventEmitter>();
        foreach (FMODUnity.StudioEventEmitter emitter in instances)
        {
            emitter.Stop();
            emitter.EventInstance.release();
        }

        FindObjectOfType<MusicManager>().StopMusic();

        //Stop all events inside the master bus.
        FMODUnity.RuntimeManager.GetBus("Bus:/").stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        
    }

    private void SpawnProps()
    {
        PropSpawner[] propSpawners = FindObjectsOfType<PropSpawner>();

        for (int i = 0; i < propSpawners.Length; i++)
        {
            propSpawners[i].BuildPropList(allProps);
            propSpawners[i].SpawnProp();
        }
    }

    private void AssignLogs()
    {
        if (GameStateHandler.unlockedLogs == null)
        {
            GameStateHandler.LoadUnlockedLogs();
        }

        Pickup_Log[] pickupLogs = FindObjectsOfType<Pickup_Log>();

        List<LogData> assignedLogs = new List<LogData>();

        foreach (Pickup_Log pickup_Log in pickupLogs)
        {
            if (GameStateHandler.logData == null || GameStateHandler.logData.Length <= 0)
            {
                Debug.Log("Log Database is Empty!");
                break;
            }

            List<LogData> logData = new List<LogData>();
            logData.AddRange(GameStateHandler.logData);

            LogData log = null;
            do
            {
                if (log != null)
                {
                    logData.Remove(log);

                    if (logData.Count <= 0)
                    {
                        //No could be assigned.
                        //Debug.Log("No log could be assigned");
                        break;
                    }
                }
                log = logData[Random.Range(0, logData.Count)];
            }
            while (GameStateHandler.unlockedLocalLogs.Contains(log.id) || GameStateHandler.unlockedLogs.Contains(log.id));

            if (logData.Count <= 0)
            {
                //No could be assigned.
                Debug.Log("No log could be assigned");
                continue;
            }
            else if (!assignedLogs.Contains(log) && log != null)
            {
                Debug.Log("Assigning log with ID: " + log.id);
                assignedLogs.Add(log);
                pickup_Log.InitLog(log.id);
            }
            else
            {
                //There are no more logs, spawn other pickup.
                Debug.Log("We're out of logs, create another pickup instead?");
                Destroy(pickup_Log);
                //pickup_Log.GetComponentInParent<PropSpawner>().RemovePropFromPropList(pickup_Log);
                //pickup_Log.GetComponentInParent<PropSpawner>().SpawnProp(false);
            }
        }
    }

    private void SpawnVents()
    {
        float maxRoomDist = 0;
        foreach (Room r in allSpawnedRooms)
        {
            float dist = Vector3.Distance(player.transform.position, r.transform.position);
            if (dist > maxRoomDist)
            {
                maxRoomDist = dist;
            }
        }

        List<Vent> allVents = FindObjectsOfType<Vent>().ToList<Vent>();
        List<Vent> ventsToRemove = new List<Vent>();
        foreach (Vent v in allVents)
        {
            if (Vector3.Distance(player.transform.position, v.transform.position) < maxRoomDist - v.SpawnDistancePerimiter || Random.Range(0f, 1f) > ventSpawnChance)
            {
                ventsToRemove.Add(v);
            }
        }

        foreach (Vent v in ventsToRemove)
        {
            allVents.Remove(v);
            Destroy(v.gameObject);
        }

        Vent connectedVent = new Vent();
        foreach (Vent v in allVents)
        {
            if (v.Connected)
            {
                continue;
            }

            float ventDistance = Mathf.Infinity;
            foreach (Vent otherVent in allVents)
            {
                if (otherVent == v || otherVent.Connected)
                {
                    continue;
                }

                float currentDistance = Vector3.Distance(v.transform.position, otherVent.transform.position);
                if (currentDistance < ventDistance)
                {
                    ventDistance = currentDistance;
                    connectedVent = otherVent;
                }
            }
            if (connectedVent == null || connectedVent.Connected == true)
            {
                continue;
            }

            v.ConnectedVent = connectedVent;
            v.Connected = true;

            connectedVent.ConnectedVent = v;
            connectedVent.Connected = true;
        }

        //Remove vents without connections (if there is an oddnumber of vents for example).
        allVents = FindObjectsOfType<Vent>().ToList<Vent>();
        foreach (Vent v in allVents)
        {
            if (v.ConnectedVent == null)
            {
                Destroy(v.gameObject);
            }
        }
    }
}
