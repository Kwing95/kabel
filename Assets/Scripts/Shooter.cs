using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{

    public Material white;
    public GameObject noise;
    public AudioClip gunshot;

    private FieldUnit unit;
    private AudioSource source;

    public enum Actions { Gun, Frag, Smoke, Stun, Distract, Gauze, Backstab };

    //private static float peripheralVision = 30;

    static Shooter()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        //white = Resources.Load("Materials/White.mat", typeof(Material)) as Material;
        source = GetComponent<AudioSource>();
        //SoundManager.onGunshot += 
    }

    // Update is called once per frame
    void Update()
    {

    }

    /* NEEDS:
     *      shooter (GameObject)
     *      target (GameObject)
     *      mask (LayerMask)
     *      peripheralVision (float)
     *      accuracy (float)
     
         */

    public static void Action(GameObject unit, Vector2 target, Actions action)
    {

    }

    public void GunAttack(GameObject shooter, GameObject target)
    {
        StartCoroutine(GunAttackHelper(shooter, target));
    }

    private void FireShot(GameObject shooter, GameObject target, int memberIndex)
    {
        shooter.GetComponent<BoxCollider2D>().enabled = false;

        Vector2 shotOrigin = (Vector2)shooter.transform.position; // + shooter.GetComponent<GridMover>().GetRotator().FrontOffset();

        // 3 Focus -> 10 deg Error, 2 Focus -> 20 deg Error
        // 1 Focus -> 30 deg Error, 0 Focus -> Can't attack
        float marginOfError = 15;// 10 + (10 * (3 - shooter.GetComponent<FieldUnit>().party[memberIndex].focus));

        // Calculate if there's a clear line of sight
        Vector2 direction = target.transform.position - shooter.transform.position;
        RaycastHit2D hit = Physics2D.Raycast(shotOrigin, direction, 9/*, mask*/);
        float angle = Vector2.Angle(Vector2.up, direction); // shooter.GetComponent<GridMover>().GetRotator().FrontOffset()

        if (hit.collider != null && hit.collider.gameObject != target /*||  hit.collider.GetComponent<FieldUnit>() == null  || angle > peripheralVision*/)
        {
            //DrawLine(shooter, hit);
            shooter.GetComponent<BoxCollider2D>().enabled = true;
            return;
        }

        // Generate an actual shot
        angle += Random.Range(0, marginOfError) * (Random.Range(0, 2) == 0 ? 1 : -1); // add margin of error
        direction = Quaternion.AngleAxis(angle, Vector3.forward) * direction; // This flattens the shot somehow
        hit = Physics2D.Raycast(shotOrigin, direction, 9/*, mask*/);

        if (hit.collider != null)
        {
            source.PlayOneShot(gunshot);
            Camera.main.GetComponent<Jerk>().Shake(1);
            /*
            FieldUnit targetHit = hit.collider.GetComponent<FieldUnit>();
            if (targetHit != null)
            {
                targetHit.TakeDamage(shooter.GetComponent<FieldUnit>().party[memberIndex]);
            }*/

            // Generate noise graphic
            GameObject tempNoise = Instantiate(noise, transform.position, Quaternion.identity);
            tempNoise.GetComponent<Noise>().Initialize(CompareTag("Player"), 10);

            DrawLine(shotOrigin, hit.point);
        }

        shooter.GetComponent<BoxCollider2D>().enabled = true;
    }

    public void DrawLine(Vector2 shotOrigin, Vector2 hitPoint)
    {
        GameObject shotObject = new GameObject();
        AutoVanish vanisher = shotObject.AddComponent<AutoVanish>();
        vanisher.timeToLive = 0.25f;
        LineRenderer shotLine = shotObject.AddComponent<LineRenderer>();
        shotLine.SetPositions(new Vector3[] { (Vector3)shotOrigin + new Vector3(0, 0, -1), (Vector3)hitPoint + new Vector3(0, 0, -1) });
        shotLine.sortingLayerName = "ForegroundFX";
        shotLine.material = white;
        shotLine.startWidth = shotLine.endWidth = 0.07f;
    }

    IEnumerator GunAttackHelper(GameObject shooter, GameObject target)
    {
        // shooter.GetComponent<GridMover>().rotator.FacePoint(target.transform.position);
        //LayerMask mask = LayerMask.NameToLayer(shooter.GetComponent<PlayerMover>() ? "Enemy" : "Player");

        for (int i = 0; i < 1; ++i)
        {
            FireShot(shooter, target, i);
            if (!target)
                break;
            
            yield return new WaitForSeconds(0.25f);
        }

        yield return new WaitForSeconds(0.25f);
        if (shooter.GetComponent<PlayerMover>())
        {
            /*if (PlayerMover.instance.GetComponent<FieldUnit>().ap <= 0)
                CommandManager.EndTurn();
            else
                MenuNode.RefreshMenu();*/
        }
        //if (shooter.GetComponent<AutoMover>())
        //    shooter.GetComponent<AutoMover>().StopWaiting();
    }

}
