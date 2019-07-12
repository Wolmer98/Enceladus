using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class OcclusionCulling : MonoBehaviour
{
    [SerializeField] float occlusionRange = 20;

    List<Renderer> renderers = new List<Renderer>();
    List<GameObject> visibleRooms = new List<GameObject>();

    [SerializeField] LayerMask occlusionLayer;
    [SerializeField] string fogOfWarLayerName = "FogOfWarVisible";

    bool canUpdate = true;
    bool initialized = true;

    WorldGenerator wg;

    private void Start()
    {
        wg = FindObjectOfType<WorldGenerator>();
        wg.OnWorldStart.AddListener(Init);
    }

    public void Init()
    {
        renderers.AddRange(wg.GetComponentsInChildren<Renderer>());

        foreach (Renderer r in renderers)
        {
            if (r != null)
            {
                r.enabled = false;
            }
        }

        renderers.Clear();
        initialized = true;

        UpdateCulling();
    }

    private void Update()
    {
        if (canUpdate == true)
        {
            StartCoroutine(UpdateCullingTimer());
        }
    }

    IEnumerator UpdateCullingTimer()
    {
        canUpdate = false;
        yield return new WaitForSeconds(0.5f);

        UpdateCulling();
        canUpdate = true;
    }

    public void ClearRenderReferences()
    {
        renderers.Clear();
        visibleRooms.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("RoomBounds"))
        {
            UpdateCulling(true);
        }
    }

    private void UpdateCulling(bool enteredRoom = false)
    {
        if (!initialized)
        {
            return;
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, enteredRoom ? 1 : occlusionRange, occlusionLayer);

        foreach (Collider room in colliders)
        {
            if (enteredRoom && room.gameObject.layer != LayerMask.NameToLayer(fogOfWarLayerName) && room.name.Contains("Win") == false)
            {
                SetEnvironmentLayer(room.gameObject);
            }

            if (!visibleRooms.Contains(room.gameObject))
            {
                visibleRooms.Add(room.gameObject);
            }
        }
        List<Collider> collidersList = new List<Collider>(colliders);
        List<GameObject> roomsToBeRemoved = new List<GameObject>();
        for (int i = 0; i < visibleRooms.Count; i++)
        {
            if (visibleRooms[i] == null)
            {
                continue;
            }

            Collider roomCol = visibleRooms[i].GetComponent<Collider>();
            if (roomCol != null && collidersList.Contains(roomCol))
            {
                RenderRoom(visibleRooms[i], true);
            }
            else if (!enteredRoom)
            {
                RenderRoom(visibleRooms[i], false);
                roomsToBeRemoved.Add(visibleRooms[i]);
            }
        }

        foreach (GameObject room in roomsToBeRemoved)
        {
            visibleRooms.Remove(room);
        }
    }

    private void SetEnvironmentLayer(GameObject room)
    {
        room.transform.Find("Environment").gameObject.layer = LayerMask.NameToLayer(fogOfWarLayerName);

        renderers.Clear();
        renderers.AddRange(room.GetComponentsInChildren<Renderer>());

        foreach (Renderer r in renderers)
        {
            Transform parent = r.transform.parent;
            bool skip = false;
            while (parent != null && parent.gameObject != room)
            {
                if (parent.gameObject.layer == LayerMask.NameToLayer("Interaction"))
                {
                    skip = true;
                    Debug.Log("SKIP");
                    break;
                }
                else
                    parent = parent.parent;
            }

            if (skip)
                continue;

            if (r.gameObject.layer != LayerMask.NameToLayer("Interaction"))
            {
                r.gameObject.layer = LayerMask.NameToLayer(fogOfWarLayerName);
                Debug.Log("SWITCHED LAYER " + r.name);
            }
        }
    }

    private void RenderRoom(GameObject room, bool render)
    {
        if (room == null)
        {
            return;
        }

        renderers.Clear();
        renderers.AddRange(room.GetComponentsInChildren<Renderer>());

        foreach (Renderer r in renderers)
        {
            //if (LayerMask.NameToLayer(fogOfWarLayerName) == r.gameObject.layer && render == false)
            //    continue;

            r.enabled = render;
        }
    }
}
