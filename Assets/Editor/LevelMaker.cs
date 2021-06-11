using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager)), CanEditMultipleObjects]
public class LevelMaker : Editor
{

    public GameObject wallsContainer;
    public bool placingBlocks = false;
    private Vector2 lastToggledTile;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        placingBlocks = EditorGUILayout.Toggle("Placing Blocks", placingBlocks);

    }

    void OnSceneGUI()
    {
        if (!placingBlocks)
            return;

        if (!wallsContainer)
            wallsContainer = GameObject.Find("Walls");

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        Vector2 mousePosition = Event.current.mousePosition;
        Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
        mousePosition = Grapher.RoundedVector(ray.origin);
        
        if (Event.current.button == 0 &&
            (Event.current.type == EventType.MouseDown ||
                (Event.current.type == EventType.MouseDrag && lastToggledTile != mousePosition)))
        {
            lastToggledTile = mousePosition;

            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.up, 0f);
            if(hit.collider && hit.collider.CompareTag("Wall"))
            {
                DestroyImmediate(hit.collider.gameObject);
            }
            else
            {
                GameObject newTile = Instantiate(Globals.WALL as GameObject);
                newTile.transform.position = mousePosition;
                newTile.transform.parent = wallsContainer.transform;
                //Debug.Log("Created " + newTile.name + " at " + newTile.transform.position);
            }

        }

        //Event.current.Use();
    }

    public Vector2 ConvertToWorldUnits(Vector2 TouchLocation)
    {
        Vector2 WorldUnitsInCamera;
        Vector2 WorldToPixelAmount;

        //Finding Pixel To World Unit Conversion Based On Orthographic Size Of Camera
        WorldUnitsInCamera.y = Camera.main.GetComponent<Camera>().orthographicSize * 2;
        WorldUnitsInCamera.x = WorldUnitsInCamera.y * Screen.width / Screen.height;

        WorldToPixelAmount.x = Screen.width / WorldUnitsInCamera.x;
        WorldToPixelAmount.y = Screen.height / WorldUnitsInCamera.y;

        Vector2 returnVec2;

        returnVec2.x = ((TouchLocation.x / WorldToPixelAmount.x) - (WorldUnitsInCamera.x / 2)) +
        Camera.main.transform.position.x;
        returnVec2.y = ((TouchLocation.y / WorldToPixelAmount.y) - (WorldUnitsInCamera.y / 2)) +
        Camera.main.transform.position.y;

        return returnVec2;
    }


}
