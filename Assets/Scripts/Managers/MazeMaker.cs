using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeMaker : MonoBehaviour
{

    public GameObject wallsContainer;
    public int cellLength = 10;
    public int width = 5;
    public int height = 5;

    public int enemyCount = 10;
    public int lootCount = 5;
    private int seed = 1;

    private List<List<bool>> visited;
    private List<List<int>> neighborGrid;
    private Stack<Vector2> stack;
    private List<Vector2> fullPath;

    private List<GameObject> minesweeperReadout;
    

    // Start is called before the first frame update
    void Awake()
    {
        PRNG.ForceSeed(seed);
        GenerateMaze();
        DeformMaze(2 * width * height, 10 * width * height, 5 * width * height);

        // PrintMinesweeper();
    }

    private void Start()
    {
        SpawnEnemies(enemyCount);
        SpawnLoot(lootCount);
        PlayerMover.instance.transform.position = GenerateRandomRoute(1)[0];
    }

    private void PrintMinesweeper()
    {
        foreach (GameObject elt in minesweeperReadout)
            Destroy(elt);
        
        minesweeperReadout = new List<GameObject>();
        // Fill in zeroes with number of neighbors, unless point is ineligible (in which case set to -1)
        for (int i = 0; i < height * cellLength; ++i)
            for (int j = 0; j < width * cellLength; ++j)
            {
                if (neighborGrid[i][j] != -2)
                    neighborGrid[i][j] = PointIsEligible(new Vector2(j, i)) ? GetNeighborCount(new Vector2(j, i)) : -1;

                GameObject label = Instantiate(Globals.DIGIT, new Vector2(j + 1, i + 1), Quaternion.identity);
                minesweeperReadout.Add(label);
                string output;
                if (neighborGrid[i][j] == -2)
                    output = "B";
                else if (neighborGrid[i][j] == -1)
                    output = "n";
                else
                    output = neighborGrid[i][j].ToString();

                label.GetComponent<TMPro.TextMeshPro>().text = output;
            }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void GenerateMaze()
    {
        fullPath = new List<Vector2>();
        // Create basic maze
        CreateEmptyGraph();
        stack = new Stack<Vector2>();
        stack.Push(GetRandomPoint());
        while (stack.Count > 0)
            VisitNeighbor();
    }

    private void DeformMaze(int numIslands, int thinClusters, int thickClusters)
    {
        // Generate graph
        GenerateNeighborGrid();

        // Deform graph
        CreateIslands(numIslands);
        for (int i = 0; i < thinClusters; ++i)
            ReinforceWalls(1, 1);

        for (int i = 0; i < thickClusters; ++i)
            ReinforceWalls(1, 2);
    }

    // OBSTACLE-RELATED FUNCTIONS =======================================

    private List<Vector2> GenerateRandomRoute(int routeLength=4)
    {
        List<Vector2> route = new List<Vector2>();

        int index = PRNG.Range(0, fullPath.Count - routeLength);
        for(int i = 0; i < routeLength; ++i)
            route.Add(RandomTileFromCell(fullPath[index + i]));

        return route;
    }

    private Vector2 RandomTileFromCell(Vector2 cell)
    {
        List<Vector2> tilesInCell = GetTilesFromCell(cell, 0);
        return tilesInCell[PRNG.Range(0, tilesInCell.Count)];
    }

    private List<Vector2> GetTilesFromCell(Vector2 cell, int minValue)
    {
        List<Vector2> results = new List<Vector2>();

        for(int i = (int)cell.y * cellLength; i < (int)(cell.y + 1) * cellLength; ++i)
            for(int j = (int)cell.x; j < (int)(cell.x + 1) * cellLength; ++j)
                if (neighborGrid[i][j] >= minValue)
                    results.Add(new Vector2(j, i));

        return results;
    }

    private void SpawnEnemies(int numEnemies)
    {
        for(int i = 0; i < numEnemies; ++i)
        {
            List<Vector2> enemyRoute = GenerateRandomRoute();
            GameObject newEnemy = Instantiate(Globals.ENEMY, enemyRoute[0], Quaternion.identity);
            newEnemy.transform.SetParent(ObjectContainer.instance.enemies.transform);
            AutoMover autoMover = newEnemy.GetComponent<AutoMover>();
            Inventory inventory = newEnemy.GetComponent<Inventory>();

            autoMover.route = enemyRoute;
            autoMover.randomPatrol = PRNG.Range(0, 2) == 0;
            autoMover.pointMemory = PRNG.Range(0, 3);

            Inventory.ItemType[] types = (Inventory.ItemType[])System.Enum.GetValues(typeof(Inventory.ItemType));
            Inventory.ItemType dropType = types[PRNG.Range(0, types.Length)];
            Inventory.InventoryEntry item = new Inventory.InventoryEntry(dropType, 1);
            inventory.Add(item);
            
            // Position, route, weaponequipped, leashlength, randompatrol, pointmemory, inventory
        }
    }

    private void SpawnLoot(int numLoot)
    {
        for(int i = 0; i < numLoot; ++i)
        {
            GameObject newEnemy = Instantiate(Globals.LOOT, new Vector2(), Quaternion.identity);
            newEnemy.transform.SetParent(ObjectContainer.instance.loot.transform);

            newEnemy.transform.position = GenerateRandomRoute(1)[0];
        }
    }

    // WALL-RELATED FUNCTIONS ===========================================

    private void ReinforceWalls(int numTiles, int minNeighbors=2)
    {

        // Create list of candidates
        List<Vector2> candidates = new List<Vector2>();
        for (int i = 0; i < height * cellLength; ++i)
            for (int j = 0; j < width * cellLength; ++j)
                if (neighborGrid[i][j] >= minNeighbors)
                    candidates.Add(new Vector2(j, i));

        for(int i = 0; i < numTiles; ++i)
        {
            int index = PRNG.Range(0, candidates.Count);
            GameObject newWall = Instantiate(Globals.WALL, candidates[index], Quaternion.identity);
            newWall.transform.SetParent(wallsContainer.transform);

            // Remove new wall from candidates and add neighbors if applicable
            // NOTE: Contains and Remove are O(n) operations and may slow execution
            neighborGrid[(int)candidates[index].y][(int)candidates[index].x] = -2;

            foreach (Vector2 border in GetBorders(candidates[index]))
                if (InBounds(border) && neighborGrid[(int)border.y][(int)border.x] != -2)
                {
                    neighborGrid[(int)border.y][(int)border.x] = PointIsEligible(border) ? GetNeighborCount(border) : -1;
                    if (neighborGrid[(int)border.y][(int)border.x] >= minNeighbors && !candidates.Contains(border))
                        candidates.Add(border);
                }

            candidates.Remove(candidates[index]);
        }

    }

    private void CreateIslands(int numIslands)
    {
        // FIXME: Should candidates be refreshed after each wall is added?

        // Create list of potential islands
        List<Vector2> candidates = new List<Vector2>();
        List<int> indexes = new List<int>();
        for (int i = 0; i < height * cellLength; ++i)
            for (int j = 0; j < width * cellLength; ++j)
                if (neighborGrid[i][j] == 0)
                    candidates.Add(new Vector2(j, i));

        // Generate indices
        while(indexes.Count < numIslands)
        {
            int newIndex = PRNG.Range(0, candidates.Count);
            if (!indexes.Contains(newIndex))
                indexes.Add(newIndex);
        }

        // For each island, create wall
        for(int i = 0; i < indexes.Count; ++i)
        {
            GameObject newWall = Instantiate(Globals.WALL, candidates[indexes[i]], Quaternion.identity);
            newWall.transform.SetParent(wallsContainer.transform);

            // Update 
            neighborGrid[(int)candidates[indexes[i]].y][(int)candidates[indexes[i]].x] = -2;
            foreach(Vector2 border in GetBorders(candidates[indexes[i]]))
                if(InBounds(border) && neighborGrid[(int)border.y][(int)border.x] != -2)
                    neighborGrid[(int)border.y][(int)border.x] = PointIsEligible(border) ? GetNeighborCount(border) : -1;
        }
    }

    private bool PointIsEligible(Vector2 point)
    {
        bool onGroup = false;
        int numGroups = 0;

        List<Vector2> borders = GetBorders(point);
        Vector2 last = borders[borders.Count - 1];

        foreach (Vector2 border in borders)
        {
            // If we're not on a group yet and find a wall, we've found a new group and are on a group
            if ((!InBounds(border) || neighborGrid[(int)border.y][(int)border.x] == -2) && !onGroup)
            {
                ++numGroups;
                onGroup = true;
            }
            // If we're on a group and find a vacancy, we're no longer on a group
            else if (InBounds(border) && neighborGrid[(int)border.y][(int)border.x] != -2 && onGroup)
                onGroup = false;
        }

        if((!InBounds(borders[0]) || neighborGrid[(int)borders[0].y][(int)borders[0].x] == -2) &&
            (!InBounds(last) || neighborGrid[(int)last.y][(int)last.x] == -2))
        {
            --numGroups;
        }

        // 2+ groups means we can't place a wall here
        return numGroups <= 1;
    }

    private void GenerateNeighborGrid()
    {
        // Initialize grid of -2 for walls and 0 for empty space
        neighborGrid = new List<List<int>>();
        for (int i = 0; i < height * cellLength; ++i)
        {
            neighborGrid.Add(new List<int>());
            for (int j = 0; j < width * cellLength; ++j)
                neighborGrid[i].Add(Grapher.PointIsClear(new Vector2(j, i)) ? 0 : -2);
        }

        // Fill in zeroes with number of neighbors, unless point is ineligible (in which case set to -1)
        for (int i = 0; i < height * cellLength; ++i)
            for (int j = 0; j < width * cellLength; ++j)
                if(neighborGrid[i][j] == 0)
                {
                    
                    neighborGrid[i][j] = PointIsEligible(new Vector2(j, i)) ? GetNeighborCount(new Vector2(j, i)) : -1;
                    //GameObject label = Instantiate(Globals.DIGIT, new Vector2(j + 1, i + 1), Quaternion.identity);
                    ///label.GetComponent<TMPro.TextMeshPro>().text = neighborGrid[i][j].ToString();
                }
                    
    }
    
    private int GetNeighborCount(Vector2 point)
    {
        if(!Grapher.PointIsClear(point))
            return -2;

        int numNeighbors = 0;
        // Out of bounds means a wall (-2) by definition
        foreach(Vector2 border in GetBorders(point))
            if(!InBounds(border) || neighborGrid[(int)border.y][(int)border.x] == -2)
                ++numNeighbors;

        return numNeighbors;
    }

    private bool InBounds(Vector2 point)
    {
        return Mathf.Clamp(point.x, 0, (cellLength * width) - 1) == point.x &&
                Mathf.Clamp(point.y, 0, (cellLength * height) - 1) == point.y;
    }

    // Returns all bordering tiles, culling any that fall out of bounds
    private List<Vector2> GetBorders(Vector2 point)
    {
        List<Vector2> borders = new List<Vector2>();
        foreach (Vector2 offset in Grapher.BORDERS)
        {
            float x = point.x + offset.x;
            float y = point.y + offset.y;

            //if (Mathf.Clamp(x, 0, (cellLength * width) - 1) == x &&
            //    Mathf.Clamp(y, 0, (cellLength * height) - 1) == y)
            //{
                borders.Add(new Vector2(x, y));
            //}
        }
        return borders;
    }

    // MAZE-RELATED FUNCTIONS ===========================================

    // Generates a maze by removing wall pieces

    // Visits a random neighbor, marking it as visited and removing the wall
    private void VisitNeighbor()
    {
        Vector2 oldPoint = stack.Peek();
        fullPath.Add(oldPoint);
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

    // Gets all unvisited neighbors of current, then returns a random item from the list
    private Vector2 GetRandomNeighbor(Vector2 current)
    {
        List<Vector2> neighbors = GetUnvisitedNeighbors(current);
        return neighbors.Count > 0 ? neighbors[PRNG.Range(0, neighbors.Count)] : new Vector2(-1, -1);
    }

    // Checks adds all neighboring points to current that have not been visited
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

    // Selects random point inside graph
    private Vector2 GetRandomPoint()
    {
        return new Vector2(PRNG.Range(0, width), PRNG.Range(0, height));
    }

    // Initializes visited to a grid of all unvisited points
    private void CreateEmptyGraph()
    {
        visited = new List<List<bool>>();

        for (int i = 0; i < height; ++i)
        {
            visited.Add(new List<bool>());
            for (int j = 0; j < width; ++j)
                visited[i].Add(false);
        }
    }

    // Removes a Divider given cell coordinates rather than world coordinates
    private void RemoveWallGraph(Vector2 cellA, Vector2 cellB)
    {
        int halfLength = cellLength / 2;
        Vector2 centerA = new Vector2(halfLength + (cellLength * cellA.x), halfLength + (cellLength * cellA.y));
        Vector2 centerB = new Vector2(halfLength + (cellLength * cellB.x), halfLength + (cellLength * cellB.y));

        RemoveWallWorld(centerA, centerB);
    }

    // Creates a raycast between start and end and removes any Divider caught in the middle
    private void RemoveWallWorld(Vector2 start, Vector2 end)
    {
        float distance = Vector2.Distance(start, end);
        RaycastHit2D[] hits = Physics2D.RaycastAll(start, end - start, distance);
        foreach(RaycastHit2D hit in hits)
            if (hit.collider.CompareTag("Divider"))
                Destroy(hit.collider.gameObject);
    }

}
