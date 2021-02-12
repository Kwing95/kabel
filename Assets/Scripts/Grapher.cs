using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapher : MonoBehaviour
{

    public static Grapher instance;

    public const int INFINITY = 1073741823;
    public const int QUEUE_MAX = 45;
    public static readonly List<Vector2> cardinals =
        new List<Vector2>(new Vector2[] { Vector2.right, Vector2.left,
            Vector2.up, Vector2.down });

    public static int mapHeight = 50;
    public static int mapWidth = 50;

    public static bool[,] graph = new bool[mapHeight, mapWidth];

    public Vector2 entryPoint;
    private static LayerMask mask;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this; // We DO want the object to be destroyed on load

        ResetGraph();
        mask = LayerMask.GetMask("Default");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void PrintGraph()
    {
        string dump = "";
        for (int y = 0; y < mapHeight; ++y)
        {
            string line = "";
            for (int x = 0; x < mapWidth; ++x)
            {
                line += (graph[y, x] ? " " : "X");
            }
            dump = line + "\n" + dump;
        }
        Debug.Log(dump);
    }

    // POINTS AND DISTANCES

    public void ResetGraph()
    {
        for (int y = 0; y < mapHeight; ++y)
            for (int x = 0; x < mapWidth; ++x)
                graph[y, x] = PointIsClear(entryPoint + new Vector2(x, y));
    }

    private static bool InBounds(Vector2 point)
    {
        return point.x >= 0 && point.y >= 0 && point.x < mapWidth && point.y < mapHeight;
    }

    // Returns true if the specified point is vacant and available
    public static bool CheckGraph(Vector2 point)
    {
        if (!InBounds(point))
            return false;
        return graph[(int)point.y, (int)point.x];
    }

    public static float ManhattanDistance(Vector2 pointA, Vector2 pointB)
    {
        return Mathf.Abs(pointA.x - pointB.x) + Mathf.Abs(pointA.y - pointB.y);
    }

    private static bool PointIsClear(Vector2 point)
    {
        RaycastHit2D hit = Physics2D.Raycast(point, Vector2.zero, 0/*, mask*/);
        if (hit.collider != null)
        {
            //return false;
            return !hit.collider.CompareTag("Wall");
        }
        return true;
    }

    // DIAMOND FUNCTIONS

    public List<Vector2> Diamond(Vector2 start, int radius)
    {
        List<Vector2> visited = new List<Vector2>(new Vector2[] { start });
        Queue<Vector2> queue = new Queue<Vector2>(new Vector2[] { start });

        while (queue.Count > 0 && queue.Count <= QUEUE_MAX)
        {
            Vector2 newPoint = queue.Dequeue();

            if (ManhattanDistance(newPoint, start) >= radius)
                return visited;

            AddAdjacent(queue, visited, newPoint);
        }
        return visited;
    }

    private void AddAdjacent(Queue<Vector2> queue, List<Vector2> visited, Vector2 point)
    {
        for (int i = 0; i < cardinals.Count; ++i)
        {
            if (!visited.Contains(point + cardinals[i]) && PointIsClear(point + cardinals[i]))
            {
                queue.Enqueue(point + cardinals[i]);
                visited.Add(point + cardinals[i]);
            }
        }
    }

    // Add tiles to a path every unit of 1 distance; if two tiles are diagonal, make a stepping stone
    // Returns a path from start to end, assuming unobstructed line of sight between them
    private static List<Vector2> FindDirectPath(Vector2 start, Vector2 end)
    {
        List<Vector2> path = new List<Vector2>();
        Vector2 step = Vector3.Normalize(end - start);
        Vector2 diagonal = new Vector2(AbsCeil(step.x), AbsCeil(step.y));
        Vector2 lineProgress = start;

        path.Add(start);

        while (RoundedVector(lineProgress) != end)
        {
            Vector2 oldTile = RoundedVector(lineProgress);
            Vector2 newTile = RoundedVector(lineProgress + step);
            
            // If new tile differs on two axes (diagonal step,) a tile has been skipped
            if(newTile.x != oldTile.x && newTile.y != oldTile.y)
            {
                int pathLength = path.Count;

                if (CheckGraph(oldTile + new Vector2(diagonal.x, 0)))
                    path.Add(RoundedVector(oldTile + new Vector2(diagonal.x, 0)));
                else if (CheckGraph(oldTile + new Vector2(0, diagonal.y)))
                    path.Add(RoundedVector(oldTile + new Vector2(0, diagonal.y)));

                // If no tile was added, no path exists
                if (pathLength == path.Count)
                    return new List<Vector2>();
            }

            // Add the tile one step forward
            if (CheckGraph(newTile))
                path.Add(RoundedVector(newTile));
            else
                return new List<Vector2>();

            lineProgress += step;
        }

        path.Add(end);
        return path;
    }

    public static float AbsCeil(float number)
    {
        return number > 0 ? Mathf.Ceil(number) : Mathf.Floor(number);
    }

    public static Vector2 RoundedVector(Vector2 vector)
    {
        return new Vector2(Mathf.Round(vector.x), Mathf.Round(vector.y));
    }

    // PATHFINDING FUNCTIONS

    public static List<Vector2> FindPath(Vector2 start, Vector2 end, int maxPathLength = -1)
    {
        List<Vector2> path = FindDirectPath(start, end);
        return path.Count == 0 ? FindIndirectPath(start, end, maxPathLength) : path;
    }

    // Returns a list of positions from start to end detailing a path (Dijkstra's algorithm)
    public static List<Vector2> FindIndirectPath(Vector2 start, Vector2 end, int maxPathLength=-1)
    {
        List<Vector2> path = new List<Vector2>();
        List<Vector2> queue = new List<Vector2>();

        if(!CheckGraph(start) || !CheckGraph(end))
            return path;

        int[,] dist = new int[mapHeight, mapWidth];
        Vector2[,] prev = new Vector2[mapHeight, mapWidth];

        // Initialize memo
        for (int x = 0; x < graph.GetLength(0); ++x)
        {
            for (int y = 0; y < graph.GetLength(1); ++y)
            {
                dist[y, x] = INFINITY;
                prev[y, x] = new Vector2(-1, -1);
                if (graph[y, x] || ((int)start.y == y && (int)start.x == x))
                    queue.Add(new Vector2(x, y));
            }
        }

        // Handle starting point
        if (!InBounds(start))
            return path; // RETURN IF START POINT IS INVALID

        dist[(int)start.y, (int)start.x] = 0;

        // Loop through queue
        while(queue.Count > 0)
        {
            Vector2 u = ClosestPoint(queue, dist);
            if (u == new Vector2(-1, -1))
                return path;
            
            queue.Remove(u);

            // Handle case where a path is found
            if (u == end && (prev[(int)u.y, (int)u.x] != new Vector2(-1, -1) || u == start))
            {
                while (u != new Vector2(-1, -1))
                {
                    path.Insert(0, u);
                    u = prev[(int)u.y, (int)u.x];
                }

                return path; // RETURN WHEN PATH IS FOUND
            }

            List<Vector2> neighbors = GetNeighbors(queue, u);
            for (int v = 0; v < neighbors.Count; ++v)
            {
                int alt = dist[(int)u.y, (int)u.x] + 1;

                // If there is a maximum path length, adhere to it
                // "Out of bounds" return empty list
                if (maxPathLength != -1 && alt > maxPathLength)
                    return path;
                    

                if (alt < dist[(int)neighbors[v].y, (int)neighbors[v].x])
                {
                    dist[(int)neighbors[v].y, (int)neighbors[v].x] = alt;
                    prev[(int)neighbors[v].y, (int)neighbors[v].x] = u;
                }
            }
        }

        // RETURN IF NO PATH IS FOUND
        return path;
    }

    // New 2D array implementation
    private static List<Vector2> GetNeighbors(List<Vector2> queue, Vector2 point)
    {
        List<Vector2> neighbors = new List<Vector2>();

        for(int i = 0; i < queue.Count; ++i)
            if(Vector2.Distance(queue[i], point) == 1)
                neighbors.Add(queue[i]);

        return neighbors;
    }

    private static Vector2 ClosestPoint(List<Vector2> queue, int[,] dist)
    {
        int minimum = INFINITY;
        Vector2 closest = new Vector2(-1, -1);

        for(int i = 0; i < queue.Count; ++i)
            if(dist[(int)queue[i].y, (int)queue[i].x] < minimum)
            {
                minimum = dist[(int)queue[i].y, (int)queue[i].x];
                closest = queue[i];
            }

        return closest;
    }

}
