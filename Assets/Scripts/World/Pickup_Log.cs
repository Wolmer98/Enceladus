using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_Log : Pickup
{
    [SerializeField] int logID;

    public int LogID { get { return logID; } }

    void Start()
    {
        base.Start();
    }

    public void InitLog(int logID)
    {
        this.logID = logID;
    }

    public override bool PickUp(PlayerController pc, bool pickupIsPrefab = false)
    {
        GameStateHandler.unlockedLocalLogs.Add(logID);

        base.PickUp(pc, pickupIsPrefab);
        return true;
    }
}
