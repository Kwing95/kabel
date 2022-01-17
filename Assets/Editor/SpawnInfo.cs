using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpawnService)), CanEditMultipleObjects]
public class SpawnInfo : Editor
{
    //public GUIStyle style;

    void OnSceneGUI()
    {
        SpawnService spawner = (SpawnService)target;

        GUI.color = new Color(1, 0, 0, 1);
        GUI.backgroundColor = Color.black;

        Handles.color = Color.red;
        //GUI.BeginGroup(Rect.zero);
        foreach (Vector2 spawnPoint in spawner.spawnPoints)
            Handles.DrawSolidDisc(spawnPoint, Vector3.forward, 0.4f);

        Handles.color = Color.green;

        foreach (Vector2 patrolPoint in spawner.patrolPoints)
            Handles.DrawSolidDisc(patrolPoint, Vector3.forward, 0.4f);

        //GUI.EndGroup();

    }

}
