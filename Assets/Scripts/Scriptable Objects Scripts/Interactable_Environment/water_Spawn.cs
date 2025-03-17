using UnityEngine;

public class WaterFlow3D : MonoBehaviour
{
    public int gridSizeX = 10; // Size of the grid in X direction
    public int gridSizeY = 10; // Size of the grid in Y direction
    public int gridSizeZ = 10; // Size of the grid in Z direction
    public GameObject waterTilePrefab; // Prefab for the water tile
    public GameObject blockingTilePrefab; // Prefab for the blocking tile

    private GameObject[,,] grid; // 3D grid to store tiles
    private bool isGridFull = false;
    private bool isDone = false;

    void Start()
    {
        Initialize3DGrid();
        PlaceInitialWaterTile();
    }

    void Update()
    {
        while (!isGridFull && !isDone)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    for (int z = 0; z < gridSizeZ; z++)
                    {
                        if (grid[x, y, z] != null && grid[x, y, z].tag == "Water")
                        {
                            SpreadWater(new Vector3Int(x, y, z));
                        }
                    }
                }
            }

            // Check if the grid is full or if we're done
            isGridFull = CheckIfGridIsFull();
            if (isGridFull || isDone)
            {
                break;
            }
        }
    }

    void Initialize3DGrid()
    {
        grid = new GameObject[gridSizeX, gridSizeY, gridSizeZ];

        // Initialize the grid with blocking tiles or empty spaces
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    if (Random.Range(0, 10) < 2) // Randomly place blocking tiles
                    {
                        grid[x, y, z] = Instantiate(blockingTilePrefab, new Vector3(x, y, z), Quaternion.identity);
                        grid[x, y, z].tag = "Blocking";
                    }
                }
            }
        }
    }

    void PlaceInitialWaterTile()
    {
        // Place the initial water tile at a specific position
        Vector3Int initialPosition = new Vector3Int(0, 0, 0); // Example position
        grid[initialPosition.x, initialPosition.y, initialPosition.z] = Instantiate(waterTilePrefab, initialPosition, Quaternion.identity);
        grid[initialPosition.x, initialPosition.y, initialPosition.z].tag = "Water";
    }

    void SpreadWater(Vector3Int currentPosition)
    {
        Vector3Int[] directions = {
            new Vector3Int(1, 0, 0),   // East
            new Vector3Int(-1, 0, 0),  // West
            new Vector3Int(0, 1, 0),    // North
            new Vector3Int(0, -1, 0),  // South
            new Vector3Int(0, 0, 1),    // Up
            new Vector3Int(0, 0, -1)    // Down
        };

        foreach (var direction in directions)
        {
            Vector3Int newPosition = currentPosition + direction;

            if (IsPositionValid(newPosition))
            {
                if (grid[newPosition.x, newPosition.y, newPosition.z] != null && grid[newPosition.x, newPosition.y, newPosition.z].tag == "Blocking")
                {
                    // Option 1: Redirect water to the opposite direction
                    Vector3Int oppositeDirection = -direction;
                    Vector3Int redirectPosition = currentPosition + oppositeDirection;

                    if (IsPositionValid(redirectPosition) && grid[redirectPosition.x, redirectPosition.y, redirectPosition.z] == null)
                    {
                        grid[redirectPosition.x, redirectPosition.y, redirectPosition.z] = Instantiate(waterTilePrefab, redirectPosition, Quaternion.identity);
                        grid[redirectPosition.x, redirectPosition.y, redirectPosition.z].tag = "Water";
                    }
                    // Option 2: Skip this direction
                    // continue;
                }
                else if (grid[newPosition.x, newPosition.y, newPosition.z] == null)
                {
                    grid[newPosition.x, newPosition.y, newPosition.z] = Instantiate(waterTilePrefab, newPosition, Quaternion.identity);
                    grid[newPosition.x, newPosition.y, newPosition.z].tag = "Water";
                }
            }
        }

        // Check if this is the last water tile
        if (IsLastWaterTile())
        {
            isDone = true;
        }
    }

    bool IsPositionValid(Vector3Int position)
    {
        return position.x >= 0 && position.x < gridSizeX &&
               position.y >= 0 && position.y < gridSizeY &&
               position.z >= 0 && position.z < gridSizeZ;
    }

    bool CheckIfGridIsFull()
    {
        // Check if the grid is full (no more empty spaces)
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    if (grid[x, y, z] == null)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    bool IsLastWaterTile()
    {
        // Check if this is the last water tile to be placed
        int waterTileCount = 0;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    if (grid[x, y, z] != null && grid[x, y, z].tag == "Water")
                    {
                        waterTileCount++;
                    }
                }
            }
        }
        return waterTileCount == 1; // Assuming only one water tile is left
    }
}