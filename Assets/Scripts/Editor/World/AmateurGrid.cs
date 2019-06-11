using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AmateurGridManager))]
public class AmateurGrid : Editor
{
    //These values are based on the modules measurements multiplied with the scale factor which in this case is: 26.
    public static Vector3 gridSpacing = new Vector3(2.08f, 2.08f, 2.6f);
    Vector3 currentGridSpacing = new Vector3();
    Grid grid;

    void Init()
    {
        Transform gridManager = Selection.activeGameObject.transform;

        while (gridManager.name != "Environment" && gridManager.parent != null)
        {
            gridManager = gridManager.parent;
        }

        if (gridManager.parent != null)
        {
            grid = gridManager.GetComponent<Grid>();
            grid.cellSize = currentGridSpacing;
        }
    }

    private void OnSceneGUI()
    {
        if (Event.current.shift)
            currentGridSpacing = gridSpacing;
        else
            currentGridSpacing = gridSpacing * 2;

        if (grid == null || grid.cellSize != currentGridSpacing)
            Init();

        /*
        Vector3 mousePosition = Event.current.mousePosition;
        mousePosition.y = -mousePosition.y;
        Plane plane = new Plane(Selection.activeTransform.position + Camera.current.transform.position, Vector3.up);

        Ray ray = Camera.current.ScreenPointToRay(mousePosition);
        float distance = 0;
        Vector3 pos = new Vector3();
        if (plane.Raycast(ray, out distance))
        {
            pos = ray.GetPoint(distance);
        }
       // pos.x -= Camera.current.transform.position.x;
       // pos.z -= Camera.current.transform.position.z;
        pos.y = Selection.activeTransform.position.y;
        Debug.Log(pos);

        if (Event.current.button == 1 && Event.current.control && Event.current.type == EventType.MouseUp && Selection.activeGameObject != null &&
            Selection.activeGameObject.name != "Environment")
        {
            Object prefabRoot = PrefabUtility.GetPrefabParent(Selection.activeGameObject);

            Transform gridManager = Selection.activeGameObject.transform;
            while (gridManager.name != "Environment" && gridManager.parent != null)
            {
                gridManager = gridManager.parent;
            }

            if (prefabRoot != null && gridManager != null)
            {
                GameObject spawnedObject = (GameObject)PrefabUtility.InstantiatePrefab(prefabRoot, gridManager);

                //Vector3 mousePosition = Event.current.mousePosition;
                //mousePosition.y = -mousePosition.y;
                //Plane plane = new Plane(new Vector3(0,1,0), Vector3.zero);

                //Ray ray = Camera.current.ScreenPointToRay(mousePosition);
                //float distance = 0;
                //Vector3 pos = new Vector3();
                //if (plane.Raycast(ray, out distance))
                //{
                //    pos = ray.GetPoint(distance);
                //}

                //Debug.Log(pos);
                ////Vector3 pos = grid.GetCellCenterWorld(grid.WorldToCell(mousePosition));
                //pos.y = Selection.activeTransform.position.y;

                spawnedObject.transform.position = pos;
                spawnedObject.transform.rotation = Selection.activeTransform.rotation;
            }
        }
        */

        /*if (Selection.activeGameObject != null && Event.current.type == EventType.MouseMove && !Event.current.alt && Event.current.button == 1 && !Event.current.shift)
       {
           Vector3 mousePosition = Event.current.mousePosition;
           SceneView sceneView = SceneView.currentDrawingSceneView;
           mousePosition.z = Vector3.Distance(Selection.activeGameObject.transform.position, Camera.current.transform.position);

           //mousePosition.y = sceneView.camera.pixelHeight - mousePosition.y;
           mousePosition = Camera.current.ScreenToWorldPoint(mousePosition);
           mousePosition.y = -mousePosition.y;

           Debug.Log(mousePosition);
           Selection.activeGameObject.transform.position = grid.WorldToCell(mousePosition);
       }*/
    }
}
