using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeMaker : MonoBehaviour
{

    public int width = 5;
    public int height = 5;

    private List<List<bool>> visited;
    private int numUnvisited = 0;
    private Stack<Vector2> stack;
    

    // Start is called before the first frame update
    void Awake()
    {
        GenerateMaze();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GenerateMaze()
    {
        CreateEmptyGraph();
        stack = new Stack<Vector2>();
        stack.Push(GetRandomPoint());
        while(stack.Count > 0)
        {
            VisitNeighbor();
        }
    }

    private void VisitNeighbor()
    {
        Vector2 oldPoint = stack.Peek();
        Vector2 newPoint = GetRandomNeighbor(oldPoint);

        // If newPoint doesn't exist, backtrack and try again
        if(newPoint == new Vector2(-1, -1))
        {
            stack.Pop();
            // If no more points, terminate
            if(stack.Count == 0)
                return;
            VisitNeighbor();
            return;
        }

        RemoveWallGraph(oldPoint, newPoint);
        visited[(int)newPoint.y][(int)newPoint.x] = true;
        stack.Push(newPoint);
    }

    private Vector2 GetRandomNeighbor(Vector2 current)
    {
        List<Vector2> neighbors = GetUnvisitedNeighbors(current);
        return neighbors.Count > 0 ? neighbors[Random.Range(0, neighbors.Count)] : new Vector2(-1, -1);
    }

    private List<Vector2> GetUnvisitedNeighbors(Vector2 current)
    {
        List<Vector2> result = new List<Vector2>();

        foreach (Vector2 direction in Grapher.CARDINALS)
        {
            Vector2 newPoint = new Vector2(current.x + direction.x, current.y + direction.y);
            if (newPoint.x == Mathf.Clamp(newPoint.x, 0, width - 1) &&
                newPoint.y == Mathf.Clamp(newPoint.y, 0, height - 1) &&
                !visited[(int)newPoint.y][(int)newPoint.x])
            {
                result.Add(newPoint);
            }
        }

        return result;
    }

    private Vector2 GetRandomPoint()
    {
        return new Vector2(Random.Range(0, width), Random.Range(0, height));
    }

    private void CreateEmptyGraph()
    {
        visited = new List<List<bool>>();
        numUnvisited = width * height;

        for (int i = 0; i < height; ++i)
        {
            visited.Add(new List<bool>());
            for (int j = 0; j < width; ++j)
                visited[i].Add(false);
        }
    }

    private void RemoveWallGraph(Vector2 cellA, Vector2 cellB)
    {
        RemoveWallWorld(new Vector2(10 + (20 * cellA.x), 10 + (20 * cellA.y)), new Vector2(10 + (20 * cellB.x), 10 + (20 * cellB.y)));
    }

    private void RemoveWallWorld(Vector2 start, Vector2 end)
    {
        float distance = Vector2.Distance(start, end);
        RaycastHit2D[] hits = Physics2D.RaycastAll(start, end - start, distance);
        foreach(RaycastHit2D hit in hits)
            if (hit.collider.CompareTag("Divider"))
                Destroy(hit.collider.gameObject);
    }

}
