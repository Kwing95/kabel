using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PointFollower))]
public class Projectile : AutoVanish
{

    public enum Type { Distract, Frag, Gas }
    public Type type;
    private GameObject user;
    private PointFollower follower;
    private string userTag;

    // Start is called before the first frame update
    void Awake()
    {
        follower = GetComponent<PointFollower>();
    }

    private void OnDisable()
    {
        follower.enabled = false;
    }

    private void OnEnable()
    {
        follower.enabled = true;
    }

    public void Initialize(float _timeToLive, Type _type, GameObject _user)
    {
        timeToLive = _timeToLive;
        type = _type;
        user = _user;
        userTag = _user.tag;
    }

    protected override void SelfDestruct()
    {
        float noiseVolume = 0;
        Noise.Source source = Noise.Source.Distract;

        switch (type)
        {
            case Type.Distract:
                SoundManager.instance.Play(SoundManager.Sound.Glass);
                noiseVolume = Globals.DISTRACTION_VOLUME;
                source = Noise.Source.Distract;
                break;
            case Type.Frag:
                // Explosion
                GameObject explosion = Instantiate(Globals.EXPLOSION, transform.position, Quaternion.identity);
                explosion.transform.localScale = (0.2f + (0.4f * Globals.GRENADE_YELLOW_RANGE)) * Vector2.one;
                // Damage
                DamageRadius();
                // Noise volume
                SoundManager.instance.Play(SoundManager.Sound.Explosion);
                noiseVolume = Globals.GRENADE_VOLUME;
                source = Noise.Source.Grenade;
                break;
            case Type.Gas:
                SpawnGas();
                noiseVolume = Globals.DISTRACTION_VOLUME;
                break;
        }

        // Noise
        GameObject tempNoise = Instantiate(Globals.NOISE, transform.position, Quaternion.identity);
        Vector2 secondaryPosition = (userTag == "Player" ? (Vector2)user.transform.position : Vector2.zero);
        tempNoise.GetComponent<Noise>().Initialize(userTag == "Player", noiseVolume, source, secondaryPosition);

        Destroy(gameObject);
    }

    private void DamageRadius()
    {
        Vector2 target = GetComponent<PointFollower>().target;
        List<GameObject> units = ObjectContainer.GetAllUnits();
        foreach (GameObject elt in units)
        {
            // elt == user for don't hurt self, CompareTag("Enemy") for any ally
            if (userTag == "Enemy" && elt.CompareTag("Enemy")) 
                continue;

            RaycastHit2D hit = Physics2D.Raycast(target, (Vector2)elt.transform.position - target, Globals.GRENADE_YELLOW_RANGE);
            if (hit.collider != null && hit.collider.gameObject == elt)
            {
                UnitStatus status = elt.GetComponent<UnitStatus>();
                int damage = ActionManager.DistanceToLevel(Vector2.Distance(elt.transform.position, target));
                if (userTag == "Enemy" && elt.CompareTag("Player"))
                    damage = 1;
                if (status)
                    status.DamageHealth(damage);
            }
        }
    }

    private void SpawnGas()
    {
        Vector2 target = GetComponent<PointFollower>().target;
        GameObject explosion = Instantiate(Globals.EXPLOSION, target, Quaternion.identity);
        explosion.transform.localScale = (0.2f + (0.4f * Globals.GRENADE_YELLOW_RANGE)) * Vector2.one;

        GameObject tempNoise = Instantiate(Globals.NOISE, target, Quaternion.identity);
        tempNoise.GetComponent<Noise>().Initialize(userTag == "Player", Globals.GAS_VOLUME, Noise.Source.Grenade);

        GameObject gasCloud = Instantiate(Globals.GAS_CLOUD, target, Quaternion.identity);
    }

}
