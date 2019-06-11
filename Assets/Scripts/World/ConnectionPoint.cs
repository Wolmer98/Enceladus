using UnityEngine;
using UnityEngine.AI;

public class ConnectionPoint : MonoBehaviour
{
    [SerializeField] Door door;
    [SerializeField] NavMeshLink navMeshLink;

    public bool Connected { get; set; }
    public Door Door { get { return door; } }
    public NavMeshLink NavMeshLink {get {return navMeshLink; } }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);
        Gizmos.DrawLine(transform.position + transform.forward, transform.position + transform.forward * 0.6f + transform.right * 0.25f);
        Gizmos.DrawLine(transform.position + transform.forward, transform.position + transform.forward * 0.6f + transform.right * -0.25f);
    }
}
