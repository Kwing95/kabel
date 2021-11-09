using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PointFollower))]
public class Projectile : AutoVanish
{

    public enum Type { Distract, Frag, Gas }
    public Type type;
    private GameObject user;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Initialize(float _timeToLive, Type _type, GameObject _user)
    {
        timeToLive = _timeToLive;
        type = _type;
        user = _user;
    }

    protected override void SelfDestruct()
    {
        float noiseVolume = 0;
        Noise.Source source = Noise.Source.Distract;

        switch (type)
        {
            case Type.Distract:
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
        tempNoise.GetComponent<Noise>().Initialize(user.CompareTag("Player"), noiseVolume, source, user.transform.position);

        Destroy(gameObject);
    }

    private void DamageRadius()
    {
        Vector2 target = GetComponent<PointFollower>().target;
        List<GameObject> units = ObjectContainer.GetAllUnits();
        foreach (GameObject elt in units)
        {
            // elt == user for don't hurt self, CompareTag("Enemy") for any ally
            if (user.CompareTag("Enemy") && elt.CompareTag("Enemy")) 
                continue;

            RaycastHit2D hit = Physics2D.Raycast(target, (Vector2)elt.transform.position - target, Globals.GRENADE_YELLOW_RANGE);
            if (hit.collider != null && hit.collider.gameObject == elt)
            {
                UnitStatus status = elt.GetComponent<UnitStatus>();
                if (status)
                    status.DamageHealth(ActionManager.DistanceToLevel(Vector2.Distance(elt.transform.position, target)));
            }
        }
    }

    private void SpawnGas()
    {
        Vector2 target = GetComponent<PointFollower>().target;
        GameObject explosion = Instantiate(Globals.EXPLOSION, target, Quaternion.identity);
        explosion.transform.localScale = (0.2f + (0.4f * Globals.GRENADE_YELLOW_RANGE)) * Vector2.one;

        GameObject tempNoise = Instantiate(Globals.NOISE, target, Quaternion.identity);
        tempNoise.GetComponent<Noise>().Initialize(user.CompareTag("Player"), Globals.GAS_VOLUME, Noise.Source.Grenade);

        GameObject gasCloud = Instantiate(Globals.GAS_CLOUD, target, Quaternion.identity);
    }

}
