using UnityEditor;
using UnityEngine;

public class Raycast : EditorWindow
{
    static bool active;

    // Open this from Window menu
    [MenuItem("Window/Raycast in editor test")]
    static void Init()
    {
        var window = (Raycast)EditorWindow.GetWindow(typeof(Raycast));
        window.Show();
    }

    // Listen to scene event
    //void OnEnable() => SceneView.duringSceneGui += OnSceneGUI;
    //void OnDisable() => SceneView.duringSceneGui -= OnSceneGUI;

    // Receives scene events
    // Use event mouse click for raycasting
    void OnSceneGUI(SceneView view)
    {
        if (!active)
        {
            return;
        }

        if (Event.current.type == EventType.MouseDown)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;

            // Spawn cube on hit location
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Hit: " + hit.collider.gameObject.name);

                var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obj.transform.position = hit.point;
            }
        }

        Event.current.Use();
    }

    // Creates a editor window with button 
    // to toggle raycasting on/off
    void OnGUI()
    {
        if (GUILayout.Button("Enable Raycasting"))
        {
            active = !active;
        }

        GUILayout.Label("Active:" + active);
    }

}