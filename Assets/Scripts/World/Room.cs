using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Room : MonoBehaviour
{
    [SerializeField] string roomName = "Default";
    [SerializeField] Rarity rarity;
    [SerializeField] ConnectionPoint[] connectionPoints;
    [SerializeField] PatrolWaypoint[] patrolWaypoints;
    [SerializeField] EnemySpawnPoint[] enemySpawnPoints;
    [SerializeField] int volume;

    public Collider[] Colliders { get; private set; }

    public string RoomName { get { return roomName; } } 
    public Rarity Rarity { get { return rarity; } }
    public ConnectionPoint[] ConnectionPoints { get { return connectionPoints; } }
    public PatrolWaypoint[] PatrolWaypoints { get { return patrolWaypoints; } }
    public EnemySpawnPoint[] EnemySpawnPoints { get { return enemySpawnPoints; } }
    public int Volume { get { return volume; } }

    public Bounds[] BoundsArray
    {
        get
        {
            List<Bounds> roomBounds = new List<Bounds>();
            foreach (Collider col in Colliders)
            {
                roomBounds.Add(col.bounds);
            }

            return roomBounds.ToArray();
        }
    }

    private void Awake()
    {
        connectionPoints = GetComponentsInChildren<ConnectionPoint>();
        patrolWaypoints = GetComponentsInChildren<PatrolWaypoint>();
        enemySpawnPoints = GetComponentsInChildren<EnemySpawnPoint>();
        Colliders = GetComponents<Collider>();
    }

    /// <summary>
    /// Calculates the volume of all the colliders attached to the room parent object.
    /// </summary>
    private int CalculateBoundsSize()
    {
        if (Colliders == null || Colliders.Length == 0)
        {
            Colliders = GetComponents<Collider>();
        }

        if (Colliders == null || Colliders.Length == 0)
        {
            return 0;
        }

        int volume = 0;
        Bounds[] bounds = BoundsArray;
        foreach (Bounds b in bounds)
        {
            volume += Mathf.FloorToInt(b.size.x * b.size.y * b.size.z);
            Debug.Log("Volume calc: " + volume);
        }

        this.volume = volume;
        return volume;
    }

    /// <summary>
    /// Sets the connection- and patrolpoints of the room, as well as its colliders and name.
    /// </summary>
    public string UpdateRoomStructure()
    {
        connectionPoints = GetComponentsInChildren<ConnectionPoint>();
        patrolWaypoints = GetComponentsInChildren<PatrolWaypoint>();
        enemySpawnPoints = GetComponentsInChildren<EnemySpawnPoint>();
        Colliders = GetComponents<Collider>();
        gameObject.name = "Room_" + roomName + "_" + Mathf.FloorToInt(CalculateBoundsSize()) + "_" + connectionPoints.Length + "x";
        return gameObject.name;
    }

    /// <summary>
    /// Sets all connection points on this room to open.
    /// </summary>
    public void ResetConnectionPoints()
    {
        foreach (ConnectionPoint cp in connectionPoints)
        {
            cp.Connected = false;
        }
    }

    /// <summary>
    /// [Depracted] Bakes this rooms nav mesh.
    /// </summary>
    public void BakeNavMesh()
    {
        NavMeshSurface surface = GetComponentInChildren<NavMeshSurface>();
        surface.BuildNavMesh();
    }
}
