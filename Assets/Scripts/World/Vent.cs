using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vent : MonoBehaviour
{
    [Tooltip("The distance from the farthest room that the vent can spawn.")]
    [SerializeField] float spawnDistancePerimiter = 0;

    public float SpawnDistancePerimiter { get { return spawnDistancePerimiter; } }
    public Vent ConnectedVent { get; set; }
    public bool Connected { get; set; }
    public bool Used { get; set; }

    private void Start()
    {
        GetComponent<FMODUnity.StudioEventEmitter>().EventInstance.setParameterByName("Ventilation", 2);
    }

    public void Use()
    {
        //Used = true;
        //ConnectedVent.Used = true;

        //GetComponent<GlowObject>().GlowColor = Color.black;
        //ConnectedVent.GetComponent<GlowObject>().GlowColor = Color.black;

        GetComponent<FMODUnity.StudioEventEmitter>().EventInstance.setParameterByName("Ventilation", 0);
        ConnectedVent.GetComponent<FMODUnity.StudioEventEmitter>().EventInstance.setParameterByName("Ventilation", 0);
        ConnectedVent.GetComponent<FMODUnity.StudioEventEmitter>().EventInstance.release();
        FindObjectOfType<PlayerController>().transform.position = ConnectedVent.transform.position + Vector3.up;
    }
}
