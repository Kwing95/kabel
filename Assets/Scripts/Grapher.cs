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
    public static List<Vector2> graph;

    public Vector2 entryPoint;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this; // We DO want the object to be destroyed on load

        graph = MakeGraph(entryPoint);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static List<Vector2> GetGraph()
    {
        return graph;
    }

    // FLOOD FILL FUNCTIONS

    public List<Vector2> MakeGraph(Vector2 start, int radius = INFINITY)
    {
        List<Vector2> visited = new List<Vector2>(new Vector2[] { start });
        Queue<Vector2> queue = new Queue<Vector2>(new Vector2[] { start });

        //int graphSize = 0;
        while (queue.Count > 0 && queue.Count <= QUEUE_MAX)
        {
            Vector2 newPoint = queue.Dequeue();

            if(ManhattanDistance(newPoint, start) >= radius)
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

    public static int ManhattanDistance(Vector2 pointA, Vector2 pointB)
    {
        return (int)(Mathf.Abs(pointA.x - pointB.x) + Mathf.Abs(pointA.y - pointB.y));
    }

    private bool PointIsClear(Vector2 point)
    {
        RaycastHit2D hit = Physics2D.Raycast(point, Vector2.zero, 0/*, mask*/);
        if (hit.collider != null)
        {
            return !hit.collider.CompareTag("Wall");
        }
        return true;
    }

    // PATHFINDING FUNCTIONS

    public static List<int> FindPath(int start, int end)
    {
        List<int> path = new List<int>();
        List<int> queue = new List<int>();
        
        List<int> dist = new List<int>();
        List<int> prev = new List<int>();

        for (int i = 0; i < graph.Count; ++i)
        {
            queue.Add(i);
            dist.Add(INFINITY);
            prev.Add(-1);
        }

        int startingIndex = -1;
        for (int i = 0; i < graph.Count; ++i)
        {
            if (graph[i] == graph[start]) // if this works, change to i == start
            {
                startingIndex = i;
                break;
            }
        }
        if (startingIndex == -1)
            return path;
        dist[startingIndex] = 0;

        while (queue.Count > 0)
        {

            int u = ClosestPoint(queue, dist);
            queue.Remove(u);

            if (u == end && (prev[u] != -1 || u == startingIndex))
            {
                while (u != -1)
                {
                    path.Insert(0, u);
                    u = prev[u];
                }
                dist.Clear();
                prev.Clear();
                return path;
            }

            List<int> neighbors = GetNeighbors(queue, graph[u]);
            for (int v = 0; v < neighbors.Count; ++v)
            {
                int alt = dist[u] + 1;
                if (alt < dist[neighbors[v]])
                {
                    dist[neighbors[v]] = alt;
                    prev[neighbors[v]] = u;
                }
            }
        }

        return path;
    }

    private static List<int> GetNeighbors(List<int> queue, Vector2 point)
    {
        List<int> neighbors = new List<int>();
        for (int i = 0; i < queue.Count; ++i)
            if (Vector2.Distance(graph[queue[i]], point) == 1)
                neighbors.Add(queue[i]);
        return neighbors;
    }

    // Requires that "distances" and "graph" have same length
    // REMEMBER: dist and prev are the size of the ENTIRE graph, not just queue
    private static int ClosestPoint(List<int> queue, List<int> dist)
    {
        int minimum = INFINITY;
        int closestPoint = -1;
        for (int i = 0; i < queue.Count; ++i)
            if (dist[queue[i]] < minimum) // replace with queue[i] ? ? ?
            {
                minimum = dist[queue[i]];
                closestPoint = queue[i];
            }
        return closestPoint;
    }

    public static int VectorToIndex(Vector2 point)
    {
        for (int i = 0; i < graph.Count; ++i)
        {
            if (Mathf.RoundToInt(graph[i].x) == Mathf.RoundToInt(point.x) &&
                Mathf.RoundToInt(graph[i].y) == Mathf.RoundToInt(point.y))
            {
                return i;
            }
        }

        return -1;
    }

}
