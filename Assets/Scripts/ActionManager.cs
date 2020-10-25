using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/* 
 State manager for identifying which action state the player is in
 Implementation for all actions players and enemies may use
 Observer for piping click/tap into aiming system
*/

public class ActionManager : MonoBehaviour
{
    [Serializable]
    public enum State { Moving, MenuPause, ActionPause, Confirm, Acting }
    private static State state;

    [Serializable]
    // 0 Gun, 1 Frag, 2 Smoke, 3 Stun, 4 Distract, 5 Gauze, 6 Backstab
    public enum Action { Gun, Frag, Smoke, Stun, Distract, Gauze, Backstab };
    private static Action selectedAction;
    private static Vector2 cursorPosition;
    private static GameObject cursor;
    private static List<GameObject> previewObjects;
    private static ActionManager instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        cursor = Instantiate(Globals.CURSOR as GameObject);
        cursor.SetActive(false);
        previewObjects = new List<GameObject>();

        ClickManager.handler += _OnClick;
    }

    private void OnDestroy()
    {
        ClickManager.handler -= _OnClick;
    }

    // STATE MANAGERS ==========================================================
    
    public static void SetState(string state)
    {
        SetState((State)Enum.Parse(typeof(State), state));
    }

    public static void SetState(State newState)
    {
        state = newState;
        if (state != State.Confirm)
        {
            Sidebar.instance.confirmButtons[1].interactable = false;
            ClearPreview();
            cursor.SetActive(false);
        } else if(state == State.Moving)
        {
            Sidebar.instance.ActionPause(false);
        }
    }

    public static State GetState()
    {
        return state;
    }

    public static void SetSelectedAction(string action)
    {
        SetSelectedAction((Action)Enum.Parse(typeof(Action), action));
    }

    public static void SetSelectedAction(Action action)
    {
        if(action == Action.Gauze)
            Sidebar.instance.confirmButtons[1].interactable = true;
        
        selectedAction = action;
    }

    // ACTION FUNCTIONS ========================================================

    public static void TakeAction(GameObject unit, Vector2 target, Action action)
    {
        switch (action)
        {
            case Action.Backstab:
                break;
            case Action.Distract:
                break;
            case Action.Frag:
                break;
            case Action.Gauze:
                break;
            case Action.Gun:
                Gun(unit, target);
                break;
            case Action.Smoke:
                break;
            case Action.Stun:
                break;
        }
    }

    public static void ExecuteAction()
    {
        switch (selectedAction)
        {
            case Action.Gun:
                instance.StartCoroutine(instance.Gun(PlayerMover.instance.gameObject, cursorPosition, 3));
                break;
        }
    }

    public IEnumerator Gun(GameObject unit, Vector2 target, int numShots)
    {
        for(int i = 0; i < numShots; ++i)
        {
            yield return new WaitForSeconds(0.25f);
            Gun(unit, target);
        }
        yield return new WaitForSeconds(0.25f);
        Sidebar.instance.FinishAction();
    }

    public static void Gun(GameObject unit, Vector2 target)
    {
        // Prevent attacker from hitting themselves
        unit.GetComponent<BoxCollider2D>().enabled = false;

        Vector2 shotOrigin = unit.transform.position;
        //(Vector2)unit.transform.position + unit.GetComponent<GridMover>().GetRotator().FrontOffset();

        // 3 Focus -> 10 deg Error, 2 Focus -> 20 deg Error
        // 1 Focus -> 30 deg Error, 0 Focus -> Can't attack
        float marginOfError = 15; //+ (10 * (3 - unit.GetComponent<FieldUnit>().party[0].focus)); // min 10 err, max 30 / formerly memberIndex

        // Calculate if there's a clear line of sight
        Vector2 direction = target - (Vector2)unit.transform.position;
        RaycastHit2D hit = Physics2D.Raycast(shotOrigin, direction/*, 9, mask*/);

        float angle = Vector2.Angle(unit.GetComponent<Rotator>().FrontOffset(), direction);
        // arg1 of Angle used to be unit.GetComponent<GridMover>().GetRotator().FrontOffset()

        // Generate an actual shot
        angle += Random.Range(0, marginOfError) * (Random.Range(0, 2) == 0 ? 1 : -1); // add margin of error
        direction = Quaternion.AngleAxis(angle, Vector3.forward) * direction; // This flattens the shot somehow
        hit = Physics2D.Raycast(shotOrigin, direction/*, 9, mask*/);

        if (hit.collider != null)
        {
            // Make sure to check WHAT is being hit... Player? Wall? Friendly fire?

            // source.PlayOneShot(gunshot); // play sound
            // Camera.main.GetComponent<Jerk>().Shake(1); // Replace this with something better

            UnitStatus targetHit = hit.collider.GetComponent<UnitStatus>();
            if (targetHit != null)
            {
                targetHit.DamageHealth(); // formerly memberIndex
            }

            GameObject noise = Globals.NOISE;
            GameObject tempNoise = Instantiate(noise, unit.transform.position, Quaternion.identity);
            tempNoise.GetComponent<Noise>().Initialize(unit.CompareTag("Player"), 10);

            // Create and format line
            GameObject shotLine = DrawLine(shotOrigin, hit.point, unit.transform);
            shotLine.transform.parent = unit.transform;
            LineRenderer shotRenderer = shotLine.GetComponent<LineRenderer>();
            shotRenderer.material = Globals.BRIGHT_WHITE;
            shotRenderer.startWidth = shotRenderer.endWidth = 0.07f;

            shotLine.AddComponent<AutoVanish>().timeToLive = 0.1f;
        }

        unit.GetComponent<BoxCollider2D>().enabled = true;
    }

    // PREVIEW FUNCTIONS =======================================================

    private static void ClearPreview()
    {
        foreach (GameObject elt in previewObjects)
            Destroy(elt);

        previewObjects.Clear();
    }

    private static void PreviewAction()
    {
        ClearPreview();
        Vector2 start = PlayerMover.instance.transform.position;
        Vector2 direction = cursorPosition - start;
        RaycastHit2D hit = Physics2D.Raycast(start, direction, 30, ~LayerMask.GetMask("Player"));

        switch (selectedAction)
        {
            case Action.Backstab:
                break;
            case Action.Gun:

                GameObject coneObject = new GameObject();
                FieldOfView cone = coneObject.AddComponent<FieldOfView>();
                cone.viewRadius = 30;
                cone.viewAngle = 30;
                coneObject.transform.position = start;
                float destinationAngle = (int)Vector2.SignedAngle(Vector2.up, cursorPosition - start);
                coneObject.transform.eulerAngles = new Vector3(0, 0, destinationAngle);
                previewObjects.Add(coneObject);

                GameObject lineOfFire = DrawLine(start, hit.point);
                LineRenderer renderer = lineOfFire.GetComponent<LineRenderer>();
                renderer.material = Globals.RED;
                renderer.startWidth = renderer.endWidth = 0.07f;
                previewObjects.Add(lineOfFire);

                // Add left and right bounds

                break;
        }

    }

    public static GameObject DrawLine(Vector2 shotOrigin, Vector2 hitPoint, Transform parent=null)
    {
        GameObject shotObject = new GameObject();
        if(parent != null)
            shotObject.transform.parent = parent;

        LineRenderer shotLine = shotObject.AddComponent<LineRenderer>();
        shotLine.SetPositions(new Vector3[] { (Vector3)shotOrigin + new Vector3(0, 0, -1), (Vector3)hitPoint + new Vector3(0, 0, -1) });

        shotLine.sortingLayerName = "Effects";
        // shotLine.material = Globals.brightWhite;
        // shotLine.startWidth = shotLine.endWidth = 0.07f;

        return shotObject;
    }

    public static GameObject DrawCircle(Vector2 center, float radius, int segments=50)
    {
        GameObject circleObject = new GameObject();
        LineRenderer line = circleObject.AddComponent<LineRenderer>();
        line.positionCount = segments + 1;
        line.sortingLayerName = "Effects";

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius; // xRadius for ellipse
            float y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            Debug.Log(new Vector2(x, y));

            line.SetPosition(i, new Vector3(x, y, -1));

            angle += (360f / segments);
        }

        return circleObject;
    }

    // LISTENERS ===============================================================

    public void _OnClick(Vector2 mousePosition)
    {
        if (state == State.Confirm && selectedAction != Action.Gauze)
        {
            Sidebar.instance.confirmButtons[1].interactable = true;
            cursor.SetActive(true);
            cursor.transform.position = new Vector3(mousePosition.x, mousePosition.y, -2);
            cursorPosition = mousePosition;
            PreviewAction();
        }
    }

}
