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
    public enum State { Moving, ActionPause, Aiming, ConfirmMove, Acting }
    private static State state;
    private static bool paused;

    [Serializable]
    // 0 Gun, 1 Frag, 2 Smoke, 3 Stun, 4 Distract, 5 Gauze, 6 Backstab
    public enum Action { Gun, Frag, Smoke, Gas, Distract, Gauze, Backstab };
    private static Action selectedAction;
    private static Vector2 cursorPosition;
    private static Vector2 attackPosition;
    private static GameObject cursor;
    private static List<GameObject> previewObjects;
    private static ActionManager instance;

    private static float grenadeRedRange = 1.5f;
    private static float grenadeOrangeRange = 3f;
    private static float grenadeYellowRange = 4.5f;

    /*
     Quick reference
       Click Action: ActionPause true, SetState actionPause, ShowMenu 1
       Click Firearm: ShowMenu 3, SelectAction Gun, SetState Aiming
       Click Confirm: StartAction
            StartAction calls ExecuteAction in ActionManager; implementation inside switch/case

        Previewing grenades:
            Frag - Show damage rings
            Stun - Show damage (stun) rings
            Smoke - Show circle
         
         */

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

    // STATE MANAGERS ==========================================================
    
    public static void SetState(string state)
    {
        SetState((State)Enum.Parse(typeof(State), state));
    }

    public static void SetState(State newState)
    {
        state = newState;

        if (state != State.Aiming && state != State.ConfirmMove)
        {
            // When navigating away from aiming, preview UI must reset
            Sidebar.instance.actionConfirmButtons[1].interactable = false;
            ClearPreview();
            cursor.SetActive(false);
        }
        else if (state == State.Moving)
        {
            // Unpause action if state is Moving
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
            Sidebar.instance.actionConfirmButtons[1].interactable = true;
        
        selectedAction = action;
    }

    // ACTION FUNCTIONS ========================================================

    public static void ExecuteAction()
    {
        switch (selectedAction)
        {
            case Action.Gun:
                instance.StartCoroutine(instance.Gun(PlayerMover.instance.gameObject, cursorPosition, 3));
                break;
            case Action.Frag:
                instance.StartCoroutine(instance.Grenade(PlayerMover.instance.gameObject, attackPosition));
                break;
            case Action.Smoke:
                break;
            case Action.Gas:
                instance.StartCoroutine(instance.GasGrenade(PlayerMover.instance.gameObject, attackPosition));
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

            GameObject tempNoise = Instantiate(Globals.NOISE, unit.transform.position, Quaternion.identity);
            tempNoise.GetComponent<Noise>().Initialize(unit.CompareTag("Player"), 10);

            // Create and format line
            GameObject shotLine = DrawLine(shotOrigin, hit.point, Globals.BRIGHT_WHITE, unit.transform);
            shotLine.transform.parent = unit.transform;
            LineRenderer shotRenderer = shotLine.GetComponent<LineRenderer>();
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

        switch (selectedAction)
        {
            case Action.Backstab:
                break;
            case Action.Gun:
                PreviewGun();
                break;
            case Action.Frag:
            case Action.Smoke:
            case Action.Gas:
                PreviewGrenade();
                break;
        }

    }

    private static void PreviewSmoke()
    {

    }

    public IEnumerator Grenade(GameObject unit, Vector2 target)
    {
        // Animate grenade
        GameObject grenadeSprite = Instantiate(Globals.PROJECTILE, unit.transform.position, Quaternion.identity);
        grenadeSprite.GetComponent<PointFollower>().target = target;

        yield return new WaitForSeconds(1);

        GameObject explosion = Instantiate(Globals.EXPLOSION, target, Quaternion.identity);
        explosion.transform.localScale = (0.2f + (0.4f * grenadeYellowRange)) * Vector2.one;

        GameObject tempNoise = Instantiate(Globals.NOISE, target, Quaternion.identity);
        tempNoise.GetComponent<Noise>().Initialize(unit.CompareTag("Player"), 10);

        List<GameObject> units = EnemyList.GetAllUnits();
        foreach(GameObject elt in units)
        {
            UnitStatus status = elt.GetComponent<UnitStatus>();
            if (status)
                status.DamageHealth(DistanceToLevel(Vector2.Distance(elt.transform.position, target)));
        }

        if (unit.GetComponent<PlayerMover>())
        {
            yield return new WaitForSeconds(0.25f);
            Sidebar.instance.FinishAction();
        }
    }

    public IEnumerator GasGrenade(GameObject unit, Vector2 target)
    {
        // Animate grenade
        GameObject grenadeSprite = Instantiate(Globals.PROJECTILE, unit.transform.position, Quaternion.identity);
        grenadeSprite.GetComponent<PointFollower>().target = target;

        yield return new WaitForSeconds(1);

        GameObject explosion = Instantiate(Globals.EXPLOSION, target, Quaternion.identity);
        explosion.transform.localScale = (0.2f + (0.4f * grenadeYellowRange)) * Vector2.one;

        GameObject tempNoise = Instantiate(Globals.NOISE, target, Quaternion.identity);
        tempNoise.GetComponent<Noise>().Initialize(unit.CompareTag("Player"), 5);

        GameObject gasCloud = Instantiate(Globals.GAS_CLOUD, target, Quaternion.identity);

        if (unit.GetComponent<PlayerMover>())
        {
            yield return new WaitForSeconds(0.25f);
            Sidebar.instance.FinishAction();
        }
    }

    private static void PreviewGrenade()
    {
        // Generate point where grenade will land
        Vector2 playerPosition = PlayerMover.instance.transform.position;
        float distance = Vector2.Distance(playerPosition, cursorPosition);
        RaycastHit2D throwHit = Physics2D.Raycast(playerPosition, cursorPosition - playerPosition, Mathf.Min(10, distance), ~LayerMask.GetMask("Player"));
        Vector2 center = throwHit.collider == null ? cursorPosition : throwHit.point;
        attackPosition = center;

        int damageLevel = DistanceToLevel(Vector2.Distance(playerPosition, center));

        if (selectedAction != Action.Frag)
        {
            previewObjects.Add(DrawLine(playerPosition, center, damageLevel > 0 ? Globals.BRIGHT_RED : Globals.BRIGHT_WHITE)); // player-to-grenade line
            previewObjects.Add(DrawCircle(center, grenadeYellowRange, Globals.BRIGHT_WHITE));
            return;
        }

        previewObjects.Add(DrawLine(playerPosition, center, LevelToColor(damageLevel))); // player-to-grenade line
        previewObjects.Add(DrawCircle(center, grenadeYellowRange, Globals.BRIGHT_YELLOW));
        previewObjects.Add(DrawCircle(center, grenadeOrangeRange, Globals.ORANGE));
        previewObjects.Add(DrawCircle(center, grenadeRedRange, Globals.BRIGHT_RED));

        // Circle should emanate from throw point, not cursor pos
        // Does this include player?
        List<GameObject> units = EnemyList.GetAllEnemies();
        foreach(GameObject unit in units)
        {
            damageLevel = DistanceToLevel(Vector2.Distance(unit.transform.position, center));
            if (damageLevel == 0)
                continue;

            RaycastHit2D hit = Physics2D.Raycast(center, (Vector2)unit.transform.position - center, grenadeYellowRange);
            if(hit.collider != null && hit.collider.gameObject == unit)
            {
                // Possibly use ternary for damage/stunTime with smoke/stun grenade, OR use damage for both
                LineRenderer hitLine = DrawLine(center, unit.transform.position).GetComponent<LineRenderer>();
                distance = Vector2.Distance(center, unit.transform.position);
                hitLine.material = LevelToColor(damageLevel);

                previewObjects.Add(hitLine.gameObject);
            }
            else
            {
                LineRenderer blueLine = DrawLine(center, hit.point).GetComponent<LineRenderer>();
                blueLine.material = Globals.BRIGHT_BLUE;
                previewObjects.Add(blueLine.gameObject);
            }
        }

    }

    // Converts distance from grenade to target to damage level (0 = miss, 3 = point-blank)
    private static int DistanceToLevel(float distance)
    {
        if (distance < grenadeRedRange)
            return 3;
        else if (distance < grenadeOrangeRange)
            return 2;
        else if (distance < grenadeYellowRange)
            return 1;

        return 0;
    }

    // Converts damage level (see DistanceToLevel() to corresponding material color)
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

    private static void PreviewGun()
    {
        Vector2 start = PlayerMover.instance.transform.position;
        Vector2 direction = cursorPosition - start;
        RaycastHit2D hit = Physics2D.Raycast(start, direction, 30, ~LayerMask.GetMask("Player"));

        GameObject coneObject = new GameObject();
        FieldOfView cone = coneObject.AddComponent<FieldOfView>();
        cone.viewRadius = 30;
        cone.viewAngle = 30;
        coneObject.transform.position = start;
        float destinationAngle = (int)Vector2.SignedAngle(Vector2.up, cursorPosition - start);
        coneObject.transform.eulerAngles = new Vector3(0, 0, destinationAngle);
        previewObjects.Add(coneObject);

        LineRenderer renderer = DrawLine(start, hit.point).GetComponent<LineRenderer>();
        renderer.material = Globals.BRIGHT_RED;
        renderer.startWidth = renderer.endWidth = 0.07f;
        previewObjects.Add(renderer.gameObject);
    }

    public static GameObject DrawLine(Vector2 shotOrigin, Vector2 hitPoint, Material material=null, Transform parent=null, float width=0.07f)
    {
        GameObject shotObject = new GameObject();
        if(parent != null)
            shotObject.transform.parent = parent;

        LineRenderer shotLine = shotObject.AddComponent<LineRenderer>();
        shotLine.SetPositions(new Vector3[] { (Vector3)shotOrigin + new Vector3(0, 0, -1), (Vector3)hitPoint + new Vector3(0, 0, -1) });

        shotLine.startWidth = shotLine.endWidth = width;
        shotLine.sortingLayerName = "Effects";
        shotLine.material = (material == null ? Globals.BRIGHT_WHITE : material);
        // shotLine.startWidth = shotLine.endWidth = 0.07f;

        return shotObject;
    }

    public static GameObject DrawCircle(Vector2 center, float radius, Material material, int segments=50, float width=0.07f)
    {
        GameObject circleObject = new GameObject();
        LineRenderer line = circleObject.AddComponent<LineRenderer>();
        line.material = material;
        line.startWidth = line.endWidth = width;
        line.positionCount = segments + 1;
        line.sortingLayerName = "Effects";

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius; // xRadius for ellipse
            float y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            line.SetPosition(i, new Vector3(center.x + x, center.y + y, -1));
            angle += (360f / segments);
        }

        return circleObject;
    }

    // LISTENERS ===============================================================

    public void _OnClick(Vector2 mousePosition)
    {        
        if (state == State.Aiming && selectedAction != Action.Gauze)
        {
            Sidebar.instance.actionConfirmButtons[1].interactable = true;
            cursor.SetActive(true);
            cursor.transform.position = new Vector3(mousePosition.x, mousePosition.y, -2);
            cursorPosition = mousePosition;
            PreviewAction();
        }
    }

}
