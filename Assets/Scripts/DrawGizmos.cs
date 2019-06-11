using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGizmos : MonoBehaviour
{
    public static int xLength;
    public static int yLength;
    public static int cellSize = 4;

    public static Mesh obj;
    public static Vector3 pos;
    public static Quaternion rot;

    public static bool drawGrid = true;
    public static bool drawObjPreview = true;

    void OnDrawGizmos()
    {
        if (drawGrid)
        {
            for (int x = 0; x < xLength + 1; x++)
                Gizmos.DrawLine(new Vector3(x * cellSize, 0, 0), new Vector3(x * cellSize, 0, cellSize * yLength));

            for (int y = 0; y < yLength + 1; y++)
                Gizmos.DrawLine(new Vector3(0, 0, y * cellSize), new Vector3(xLength * cellSize, 0, y * cellSize));
        }

        if (drawObjPreview)
            Gizmos.DrawMesh(obj, pos, rot);
    }
}