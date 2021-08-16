using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AutoMover)), CanEditMultipleObjects]
public class EnemyInfo : Editor
{
    //public GUIStyle style;

    void OnSceneGUI()
    {
        AutoMover autoMover = (AutoMover)target;

        GUI.color = new Color(1, 0, 0, 1);
        GUI.backgroundColor = Color.black;

        //GUI.BeginGroup(Rect.zero);
        for (int i = 0; i < autoMover.route.Count; ++i)
        {
            Handles.DrawSolidDisc(autoMover.route[i], Vector3.forward, 0.4f);
            Handles.Label(autoMover.route[i], i.ToString());
        }
        //GUI.EndGroup();
        
    }

}
