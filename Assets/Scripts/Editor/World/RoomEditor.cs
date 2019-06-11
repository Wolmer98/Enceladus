using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(Room))]
public class RoomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Room room = (Room)target;
        if (GUILayout.Button("Update Room"))
        {
            room.UpdateRoomStructure();

            EditorUtility.SetDirty(room.gameObject);
            EditorSceneManager.MarkSceneDirty(room.gameObject.scene);
        }
    }
}
