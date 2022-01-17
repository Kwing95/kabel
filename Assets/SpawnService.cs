using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnService : MonoBehaviour
{

    public static SpawnService instance;

    public int pointsPerRoute = 3;
    private float minDistanceFromPlayer = 20;
    public List<Vector2> spawnPoints;
    public List<Vector2> patrolPoints;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private List<Vector2> GenerateRoute()
    {
        List<Vector2> route = new List<Vector2>();
        route.Add(GetSpawnPoint());

        // If the route needs more points than exist, there's a risk of an infinite loop
        bool safeguardIsActive = pointsPerRoute >= patrolPoints.Count;

        while(route.Count < pointsPerRoute)
        {
            Vector2 newPoint = patrolPoints[Random.Range(0, patrolPoints.Count)];
            if (!route.Contains(newPoint) || safeguardIsActive)
            {
                route.Add(newPoint);
            }
        }

        return route;
    }

    private Vector2 GetSpawnPoint()
    {
        List<Vector2> eligiblePoints = new List<Vector2>();

        foreach(Vector2 point in spawnPoints)
            if (Vector2.Distance(PlayerMover.instance.transform.position, point) >= minDistanceFromPlayer)
                eligiblePoints.Add(point);

        return eligiblePoints[Random.Range(0, eligiblePoints.Count)];
    }

    public void SpawnEnemy(Vector2 firstDestination)
    {
        List<Vector2> enemyRoute = GenerateRoute();
        GameObject newEnemy = Instantiate(Globals.ENEMY, enemyRoute[0], Quaternion.identity);

        newEnemy.transform.SetParent(ObjectContainer.instance.enemies.transform);
        AutoMover autoMover = newEnemy.GetComponent<AutoMover>();
        Inventory inventory = newEnemy.GetComponent<Inventory>();

        autoMover.route = enemyRoute;
        autoMover.randomPatrol = true;
        autoMover.pointMemory = PRNG.Range(0, 3);

        Inventory.ItemType[] types = (Inventory.ItemType[])System.Enum.GetValues(typeof(Inventory.ItemType));
        Inventory.ItemType dropType = types[PRNG.Range(0, types.Length)];
        Inventory.InventoryEntry item = new Inventory.InventoryEntry(dropType, 1);
        inventory.Add(item);

        StartCoroutine(DelayedCall(autoMover, firstDestination));
        //autoMover.SoundToPosition(firstDestination, true, Noise.Source.Gun, Vector2.zero);
    }

    public IEnumerator DelayedCall(AutoMover autoMover, Vector2 firstDestination)
    {
        yield return new WaitForSeconds(1);
        autoMover.SoundToPosition(firstDestination, true, Noise.Source.Gun, Vector2.zero);
    }

}
