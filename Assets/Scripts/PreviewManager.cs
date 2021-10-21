using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**/
public class PreviewManager : MonoBehaviour
{

    public static PreviewManager instance;

    public static GameObject cursor;
    private static Vector2 cursorPosition;
    private static List<GameObject> previewObjects;
    private static Vector2 attackPosition;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        cursor = Instantiate(Globals.CURSOR as GameObject);
        cursor.SetActive(false);
        previewObjects = new List<GameObject>();

        ClickManager.releaseHandler += _OnClick;
    }

    private void OnDestroy()
    {
        ClickManager.releaseHandler -= _OnClick;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // PREVIEW FUNCTIONS =======================================================

    public static void ClearPreview()
    {
        foreach (GameObject elt in previewObjects)
            Destroy(elt);

        previewObjects.Clear();
    }

    public static void PreviewAction(ActionManager.Action selectedAction)
    {
        ClearPreview();

        switch (selectedAction)
        {
            case ActionManager.Action.Knife:
                PreviewKnife();
                break;
            case ActionManager.Action.Gun:
                PreviewGun();
                break;
            case ActionManager.Action.Frag:
            case ActionManager.Action.Smoke:
            case ActionManager.Action.Gas:
                PreviewGrenade();
                break;
            case ActionManager.Action.Distract:
                PreviewDistraction();
                break;
        }

    }

    private static void PreviewSmoke()
    {

    }

    private static void PreviewKnife()
    {
        attackPosition = cursorPosition;

        Vector2 start = PlayerMover.instance.transform.position;
        Vector2 direction = cursorPosition - start;
        RaycastHit2D hit = Physics2D.Raycast(start, direction, Globals.KNIFE_RANGE, ~LayerMask.GetMask("Player"));
        Vector2 end = hit.collider ? hit.point : start + (direction.normalized * Globals.KNIFE_RANGE);
        LineRenderer renderer = ShapeManager.DrawLine(start, end).GetComponent<LineRenderer>();

        bool targetInRange = hit.collider != null && hit.collider.CompareTag("Enemy");
        bool targetUnaware = targetInRange && hit.collider.gameObject.GetComponent<AutoMover>() && hit.collider.gameObject.GetComponent<AutoMover>().GetAwareness() != AutoMover.State.Alert;

        if (!targetInRange)
        {
            Sidebar.instance.actionConfirmButtons[1].interactable = false;
            Toast.ToastWrapper("No target in range");
        }
        else if (!targetUnaware)
            Toast.ToastWrapper("Cannot knife alerted target");
        else
            Toast.ToastWrapper("Valid target selected");

        renderer.material = (targetInRange && targetUnaware) ? Globals.BRIGHT_RED : Globals.BRIGHT_WHITE;

        renderer.startWidth = renderer.endWidth = 0.07f;
        previewObjects.Add(renderer.gameObject);
    }

    private static void PreviewDistraction()
    {
        // Generate point where grenade will land
        Vector2 playerPosition = PlayerMover.instance.transform.position;
        float distance = Vector2.Distance(playerPosition, cursorPosition);
        RaycastHit2D throwHit = Physics2D.Raycast(playerPosition, cursorPosition - playerPosition, Mathf.Min(10, distance), ~LayerMask.GetMask("Player"));
        Vector2 center = throwHit.collider == null ? cursorPosition : throwHit.point;
        attackPosition = center;

        previewObjects.Add(ShapeManager.DrawLine(playerPosition, center, Globals.BRIGHT_WHITE)); // player-to-grenade line
        previewObjects.Add(ShapeManager.DrawCircle(center, Globals.DISTRACTION_VOLUME, Globals.BRIGHT_RED));
    }

    private static void PreviewGrenade()
    {
        // Generate point where grenade will land
        Vector2 playerPosition = PlayerMover.instance.transform.position;
        float distance = Vector2.Distance(playerPosition, cursorPosition);
        RaycastHit2D throwHit = Physics2D.Raycast(playerPosition, cursorPosition - playerPosition, Mathf.Min(10, distance), ~LayerMask.GetMask("Player"));
        Vector2 center = throwHit.collider == null ? cursorPosition : throwHit.point;
        attackPosition = center;

        int damageLevel = ActionManager.DistanceToLevel(Vector2.Distance(playerPosition, center));

        if (ActionManager.GetSelectedAction() != ActionManager.Action.Frag)
        {
            previewObjects.Add(ShapeManager.DrawLine(playerPosition, center, damageLevel > 0 ? Globals.BRIGHT_RED : Globals.BRIGHT_WHITE)); // player-to-grenade line
            previewObjects.Add(ShapeManager.DrawCircle(center, Globals.GRENADE_YELLOW_RANGE, Globals.BRIGHT_WHITE));
            return;
        }

        previewObjects.Add(ShapeManager.DrawLine(playerPosition, center, LevelToColor(damageLevel))); // player-to-grenade line
        previewObjects.Add(ShapeManager.DrawCircle(center, Globals.GRENADE_YELLOW_RANGE, Globals.BRIGHT_YELLOW));
        previewObjects.Add(ShapeManager.DrawCircle(center, Globals.GRENADE_ORANGE_RANGE, Globals.ORANGE));
        previewObjects.Add(ShapeManager.DrawCircle(center, Globals.GRENADE_RED_RANGE, Globals.BRIGHT_RED));

        // Circle should emanate from throw point, not cursor pos
        // Does this include player?
        List<GameObject> units = ObjectContainer.GetAllEnemies();
        foreach (GameObject unit in units)
        {
            damageLevel = ActionManager.DistanceToLevel(Vector2.Distance(unit.transform.position, center));
            if (damageLevel == 0)
                continue;

            RaycastHit2D hit = Physics2D.Raycast(center, (Vector2)unit.transform.position - center, Globals.GRENADE_YELLOW_RANGE);
            if (hit.collider != null && hit.collider.gameObject == unit)
            {
                // Possibly use ternary for damage/stunTime with smoke/stun grenade, OR use damage for both
                LineRenderer hitLine = ShapeManager.DrawLine(center, unit.transform.position).GetComponent<LineRenderer>();
                distance = Vector2.Distance(center, unit.transform.position);
                hitLine.material = LevelToColor(damageLevel);

                previewObjects.Add(hitLine.gameObject);
            }
            else
            {
                LineRenderer blueLine = ShapeManager.DrawLine(center, hit.point).GetComponent<LineRenderer>();
                blueLine.material = Globals.BRIGHT_BLUE;
                previewObjects.Add(blueLine.gameObject);
            }
        }

    }

    private static void PreviewGun()
    {
        attackPosition = cursorPosition;
        Vector2 start = PlayerMover.instance.transform.position;
        Vector2 direction = cursorPosition - start;
        RaycastHit2D hit = Physics2D.Raycast(start, direction, 99, ~LayerMask.GetMask("Player"));

        GameObject coneObject = new GameObject();
        FieldOfView cone = coneObject.AddComponent<FieldOfView>();
        cone.viewRadius = 24;
        cone.viewAngle = 24;
        coneObject.transform.position = start;
        float destinationAngle = (int)Vector2.SignedAngle(Vector2.up, cursorPosition - start);
        coneObject.transform.eulerAngles = new Vector3(0, 0, destinationAngle);
        previewObjects.Add(coneObject);

        LineRenderer renderer = ShapeManager.DrawLine(start, hit.point).GetComponent<LineRenderer>();
        renderer.material = Globals.BRIGHT_RED;
        renderer.startWidth = renderer.endWidth = 0.07f;
        previewObjects.Add(renderer.gameObject);
    }



    /*public static int DistanceToLevel(float distance)
    {
        if (distance < Globals.GRENADE_RED_RANGE)
            return 3;
        else if (distance < Globals.GRENADE_ORANGE_RANGE)
            return 2;
        else if (distance < Globals.GRENADE_YELLOW_RANGE)
            return 1;

        return 0;
    }*/

    public static Vector2 GetAttackPosition()
    {
        return attackPosition;
    }

    private static Material LevelToColor(int level)
    {
        switch (level)
        {
            case 1:
                return Globals.BRIGHT_YELLOW;
            case 2:
                return Globals.ORANGE;
            case 3:
                return Globals.BRIGHT_RED;
            case 0:
            default:
                return Globals.BRIGHT_WHITE;
        }
    }

    public void _OnClick(Vector2 mousePosition)
    {
        if (ActionManager.GetState() == ActionManager.State.Aiming && ActionManager.GetSelectedAction() != ActionManager.Action.Gauze)
        {
            Sidebar.instance.actionConfirmButtons[1].interactable = true;
            cursor.SetActive(true);
            cursor.transform.position = new Vector3(mousePosition.x, mousePosition.y, -2 * Globals.EPSILON);
            cursorPosition = mousePosition;
            PreviewAction(ActionManager.GetSelectedAction());
        }
    }

}
/**/