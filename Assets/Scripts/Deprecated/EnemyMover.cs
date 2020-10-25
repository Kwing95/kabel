using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : MonoBehaviour
{

    public AudioClip alarm;
    public AudioClip punch;
    public GameObject cross;

    private GridMover mover;
    private GridMover player;
    private GameObject crossInstance;

    private AudioSource source;

    private bool chasing = false;
    private Shooter shooter;

    private float stunCounter = 0;
    private bool courtesyStun = false;
    private bool confusedStun = false;

    private Vector2 savedDestination;

    // Start is called before the first frame update
    void Start()
    {
        crossInstance = Instantiate(cross, transform.position, Quaternion.identity);
        crossInstance.GetComponent<SpriteRenderer>().enabled = false;

        source = GetComponent<AudioSource>();
        shooter = GetComponent<Shooter>();
        player = PlayerMover.instance.GetComponent<GridMover>();
    }

    // Update is called once per frame
    void Update()
    {
        if (stunCounter > 0)
        {

            stunCounter -= Time.deltaTime;
            return;
        }
        else if (confusedStun || courtesyStun)
        {
           // if (confusedStun)
                //destination = savedDestination;
            if (courtesyStun)
            {
                SetChasing(true);
            }
            confusedStun = false;
            courtesyStun = false;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.GetDiscretePosition());

        if (TouchingPlayer())
        {
            SetChasing(true);
            //SetDestination(player.transform.position, true);

            if (!player.gameObject.GetComponent<Flasher>().IsFlashing())
            {
                //player.gameObject.GetComponent<ParticleSystem>().Play();
                Camera.main.GetComponent<Jerk>().Shake(1);
                source.PlayOneShot(punch);
                player.gameObject.GetComponent<Flasher>().Flash(1);
                player.gameObject.GetComponent<Health>().TakeDamage();
            }

        }
    }

    private void ConfuseStun(float duration)
    {
        //savedDestination = destination;
        //destination = transform.position;
        confusedStun = true;
        stunCounter = Mathf.Max(duration, stunCounter);
    }

    private void CourtesyStun(float duration)
    {
        //savedDestination = destination;
        //destination = transform.position;
        courtesyStun = true;
        stunCounter = Mathf.Max(duration, stunCounter);
    }

    private bool TouchingPlayer()
    {
        return Mathf.Abs(transform.position.x - player.transform.position.x) <= 1 &&
            Mathf.Abs(transform.position.y - player.transform.position.y) <= 1;
    }

    public void SetChasing(bool value)
    {
        if (value && !courtesyStun)
        {
            if (!source.isPlaying)
            {
                source.PlayOneShot(alarm);
                source.Play();
            }

            //mover.moveSpeed = 4;
            stunCounter = 0;
            confusedStun = false;
        }
        //else if (!value)
            //mover.moveSpeed = 2;

        chasing = value;
        gameObject.GetComponent<SightVisualizer>().SetAlert(value);
    }
}
