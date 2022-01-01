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

    [Serializable]
    // 0 Gun, 1 Frag, 2 Smoke, 3 Stun, 4 Distract, 5 Gauze, 6 Backstab
    public enum Action { Gun, Frag, Smoke, Gas, Distract, Gauze, Knife };
    private static Action selectedAction;
    public static ActionManager instance;

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
        ResetStatics();
    }

    private void Update()
    {

    }

    private void ResetStatics()
    {
        state = State.Moving;
        /*private static Vector2 cursorPosition;
        private static Vector2 attackPosition;
        private static GameObject cursor;
        private static List<GameObject> previewObjects;
        private static ActionManager instance;*/
    }

    // STATE MANAGERS ==========================================================
    
    public static void SetState(string state)
    {
        SetState((State)Enum.Parse(typeof(State), state));
    }

    public static void SetState(State newState)
    {
        state = newState;

        // Necessary for un-dimming during aiming
        Sidebar.instance.pauseDimmer.targetAlpha = state == State.ActionPause ? 0.5f : 0;

        if (state != State.Aiming && state != State.ConfirmMove)
        {
            // When navigating away from aiming, preview UI must reset
            Sidebar.instance.actionConfirmButtons[1].interactable = false;
            PreviewManager.ClearPreview();
            PreviewManager.cursor.SetActive(false);
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

    public static Action GetSelectedAction()
    {
        return selectedAction;
    }

    // ACTION FUNCTIONS ========================================================

    public static void ExecuteAction()
    {
        Vector2 attackPosition = PreviewManager.GetAttackPosition();

        switch (selectedAction)
        {
            case Action.Gun:
                instance.StartCoroutine(instance.Gun(PlayerMover.instance.gameObject, attackPosition, PlayerMover.instance.gameObject.GetComponent<UnitStatus>().UnitsAlive()));
                break;
            case Action.Frag:
                instance.StartCoroutine(instance.Grenade(PlayerMover.instance.gameObject, attackPosition));
                break;
            case Action.Smoke:
                break;
            case Action.Gas:
                instance.StartCoroutine(instance.GasGrenade(PlayerMover.instance.gameObject, attackPosition));
                break;
            case Action.Knife:
                instance.StartCoroutine(instance.Knife(PlayerMover.instance.gameObject, attackPosition));
                break;
            case Action.Distract:
                instance.StartCoroutine(instance.Grenade(PlayerMover.instance.gameObject, attackPosition, true));
                break;
            case Action.Gauze:
                instance.StartCoroutine(instance.Gauze());
                break;
        }
    }

    public IEnumerator Gauze()
    {
        PlayerMover.instance.GetComponent<Inventory>().Consume(Inventory.ItemType.Gauze);

        PlayerMover.instance.GetComponent<UnitStatus>().Heal();
        yield return new WaitForSeconds(0.25f);
        Sidebar.instance.FinishAction();

        PlayerMover.instance.GetComponent<Navigator>().enabled = false;
        yield return new WaitForSeconds(3);
        PlayerMover.instance.GetComponent<Navigator>().enabled = true;
    }

    // Coroutine for player firing gun
    public IEnumerator Gun(GameObject unit, Vector2 target, int numShots)
    {
        PlayerMover.instance.GetComponent<Inventory>().Consume(Inventory.ItemType.Gun);

        for (int i = 0; i < numShots; ++i)
        {
            yield return new WaitForSeconds(0.25f);
            Gun(unit, target);
        }
        yield return new WaitForSeconds(0.25f);
        Sidebar.instance.FinishAction();
    }

    // Single gunshot, usable by both player and enemy
    public static void Gun(GameObject unit, Vector2 target)
    {
        bool attackerIsPlayer = unit.GetComponent<PlayerMover>();
        
        // Prevent attacker from hitting themselves
        unit.GetComponent<BoxCollider2D>().enabled = false;

        Vector2 shotOrigin = unit.transform.position;
        //(Vector2)unit.transform.position + unit.GetComponent<GridMover>().GetRotator().FrontOffset();

        // 3 Focus -> 10 deg Error, 2 Focus -> 20 deg Error
        // 1 Focus -> 30 deg Error, 0 Focus -> Can't attack
        float marginOfError = attackerIsPlayer ? 13 : 15; //+ (10 * (3 - unit.GetComponent<FieldUnit>().party[0].focus)); // min 10 err, max 30 / formerly memberIndex

        // Calculate if there's a clear line of sight
        Vector2 direction = target - (Vector2)unit.transform.position;
        // RaycastHit2D hit = Physics2D.Raycast(shotOrigin, direction/*, 9, mask*/);

        float angle = Vector2.Angle(unit.GetComponent<Rotator>().FrontOffset(), direction);
        // arg1 of Angle used to be unit.GetComponent<GridMover>().GetRotator().FrontOffset()

        // Generate an actual shot
        angle += Random.Range(0, marginOfError) * (Random.Range(0, 2) == 0 ? 1 : -1); // add margin of error
        direction = Quaternion.AngleAxis(angle, Vector3.forward) * direction; // This flattens the shot somehow
        RaycastHit2D hit = Physics2D.Raycast(shotOrigin, direction, 255, ~LayerMask.GetMask(LayerMask.LayerToName(unit.layer)));
        // Debug.Log("layer: " + LayerMask.LayerToName(unit.layer));

        if (hit.collider != null)
        {
            // Make sure to check WHAT is being hit... Player? Wall? Friendly fire?

            // source.PlayOneShot(gunshot); // play sound
            // Camera.main.GetComponent<Jerk>().Shake(1); // Replace this with something better

            UnitStatus targetHit = hit.collider.GetComponent<UnitStatus>();
            if (targetHit != null)
            {
                //Debug.Log("unit      " + Convert.ToString(~unit.layer, 2));
                //Debug.Log(targetHit.gameObject.name);
                //Debug.Log("targetHit " + Convert.ToString(targetHit.gameObject.layer, 2));
                targetHit.DamageHealth(); // formerly memberIndex
            }

            GameObject tempNoise = Instantiate(Globals.NOISE, unit.transform.position, Quaternion.identity);
            tempNoise.GetComponent<Noise>().Initialize(attackerIsPlayer, Globals.GUN_VOLUME, Noise.Source.Gun);

            // Create and format line
            GameObject shotLine = ShapeManager.DrawLine(shotOrigin, hit.point, Globals.BRIGHT_WHITE, unit.transform);
            shotLine.transform.parent = unit.transform;
            LineRenderer shotRenderer = shotLine.GetComponent<LineRenderer>();
            shotRenderer.startWidth = shotRenderer.endWidth = 0.07f;

            shotLine.AddComponent<AutoVanish>().timeToLive = 0.1f;
        }

        unit.GetComponent<BoxCollider2D>().enabled = true;
    }

    // Coroutine for player-deployed grenade
    public IEnumerator Grenade(GameObject unit, Vector2 target, bool isDistraction=false)
    {
        PlayerMover.instance.GetComponent<Inventory>().Consume(isDistraction ?
            Inventory.ItemType.Distract : Inventory.ItemType.Frag);

        SpawnProjectile(unit, target, isDistraction ? Projectile.Type.Distract : Projectile.Type.Frag);

        // Time to detonate
        yield return new WaitForSeconds(1);

        // Animate pause
        yield return new WaitForSeconds(0.25f);
        Sidebar.instance.FinishAction();

    }

    public static void SpawnProjectile(GameObject user, Vector2 target, Projectile.Type projectileType)
    {
        GameObject projectile = Instantiate(Globals.PROJECTILE, user.transform.position, Quaternion.identity);
        projectile.GetComponent<PointFollower>().target = target;
        projectile.GetComponent<PointFollower>().panSpeed = 3;
        projectile.GetComponent<Projectile>().Initialize(1, projectileType, user);
    }

    public IEnumerator GasGrenade(GameObject unit, Vector2 target)
    {
        // Animate grenade
        GameObject grenadeSprite = Instantiate(Globals.PROJECTILE, unit.transform.position, Quaternion.identity);
        grenadeSprite.GetComponent<PointFollower>().target = target;

        yield return new WaitForSeconds(1);

        GameObject explosion = Instantiate(Globals.EXPLOSION, target, Quaternion.identity);
        explosion.transform.localScale = (0.2f + (0.4f * Globals.GRENADE_YELLOW_RANGE)) * Vector2.one;

        GameObject tempNoise = Instantiate(Globals.NOISE, target, Quaternion.identity);
        tempNoise.GetComponent<Noise>().Initialize(unit.CompareTag("Player"), Globals.GAS_VOLUME, Noise.Source.Grenade);

        GameObject gasCloud = Instantiate(Globals.GAS_CLOUD, target, Quaternion.identity);

        if (unit.GetComponent<PlayerMover>())
        {
            yield return new WaitForSeconds(0.25f);
            Sidebar.instance.FinishAction();
        }
    }

    public IEnumerator Knife(GameObject unit, Vector2 target)
    {
        Vector2 origin = unit.transform.position;
        Vector2 direction = target - (Vector2)unit.transform.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, Globals.KNIFE_RANGE, ~LayerMask.GetMask("Player"));

        if (hit.collider != null)
        {
            UnitStatus targetHit = hit.collider.GetComponent<UnitStatus>();
            //AutoMover targetMover = hit.collider.GetComponent<AutoMover>();
            //UnitStatus attacker = unit.GetComponent<UnitStatus>();
            if (targetHit != null /*&& targetMover.GetAwareness() != AutoMover.State.Alert*/)
            {
                targetHit.DamageHealth(3);
                // There is a risk of being hurt attempting to knife an enemy
                //if(attacker && Random.Range(0, 11) == 0)
                //    attacker.DamageHealth(1);
            }

            // May want to make amount of noise random
            GameObject tempNoise = Instantiate(Globals.NOISE, unit.transform.position, Quaternion.identity);
            tempNoise.GetComponent<Noise>().Initialize(unit.CompareTag("Player"), Globals.KNIFE_VOLUME, Noise.Source.Knife);

            // Create and format line
            GameObject shotLine = ShapeManager.DrawLine(origin, hit.point, Globals.BRIGHT_RED, unit.transform);
            shotLine.transform.parent = unit.transform;
            LineRenderer shotRenderer = shotLine.GetComponent<LineRenderer>();
            shotRenderer.startWidth = shotRenderer.endWidth = 0.07f;

            shotLine.AddComponent<AutoVanish>().timeToLive = 0.1f;
        }

        yield return new WaitForSeconds(0.25f);
        Sidebar.instance.FinishAction();
    }

    // Converts distance from grenade to target to damage level (0 = miss, 3 = point-blank)
    public static int DistanceToLevel(float distance)
    {
        if (distance < Globals.GRENADE_RED_RANGE)
            return 3;
        else if (distance < Globals.GRENADE_ORANGE_RANGE)
            return 2;
        else if (distance < Globals.GRENADE_YELLOW_RANGE)
            return 1;

        return 0;
    }

}
