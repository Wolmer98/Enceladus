using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OcclusionCulling : MonoBehaviour
{
    [SerializeField] float occlusionRange = 20;

    List<Renderer> renderers = new List<Renderer>();
    List<GameObject> visibleRooms = new List<GameObject>();

    [SerializeField] LayerMask occlusionLayer;
    [SerializeField] LayerMask occlusionExclusion;

    bool canUpdate = true;

    public void Init()
    {
        WorldGenerator wg = FindObjectOfType<WorldGenerator>();
        renderers.AddRange(wg.GetComponentsInChildren<Renderer>());

        foreach (Renderer r in renderers)
        {
            if (r != null)
            {
                r.enabled = false;
            }
        }

        renderers.Clear();
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
        UpdateCulling();
    }

    private void UpdateCulling()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, occlusionRange, occlusionLayer);

        foreach (Collider room in colliders)
        {
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
            else
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
            r.enabled = render;
        }
    }
}
