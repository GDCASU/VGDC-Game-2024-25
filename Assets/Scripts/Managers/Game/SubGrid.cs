using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* -----------------------------------------------------------
 * Author:
 * Sameer Reza
 */
// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Handle the grid interactions for the per-room sub-grid
 */
// --------------------------------------------------------


/// <summary>
/// Handles everything about the per-room sub-grid
/// </summary>
public class SubGrid : MonoBehaviour
{
    [SerializeField]
    private Vector2Int gridSize;

    [SerializeField]
    private float tileSize = 1f;

    private Tile[,] tiles;

    private void Awake()
    {
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        tiles = new Tile[gridSize.x, gridSize.y];

        // Create tiles
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 worldPos = transform.position + new Vector3(x * tileSize, 0, y * tileSize);
                tiles[x, y] = new Tile(new Vector2Int(x, y), worldPos);
            }
        }
    }

    public Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        Vector3 localPosition = worldPosition - transform.position;
        int x = Mathf.FloorToInt(localPosition.x / tileSize);
        int y = Mathf.FloorToInt(localPosition.z / tileSize);
        return new Vector2Int(x, y);
    }

    public Tile GetTile(Vector2Int gridPosition)
    {
        if (IsValidPosition(gridPosition))
        {
            return tiles[gridPosition.x, gridPosition.y];
        }
        return null;
    }

    private bool IsValidPosition(Vector2Int position)
    {
        return position.x >= 0
            && position.x < gridSize.x
            && position.y >= 0
            && position.y < gridSize.y;
    }
}

public class Tile
{
    public Vector2Int GridPosition { get; private set; }
    public Vector3 WorldPosition { get; private set; }
    public bool IsOccupied { get; set; }

    public Tile(Vector2Int gridPosition, Vector3 worldPosition)
    {
        GridPosition = gridPosition;
        WorldPosition = worldPosition;
        IsOccupied = false;
    }
}
