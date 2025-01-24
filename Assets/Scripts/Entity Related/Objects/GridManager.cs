using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* -----------------------------------------------------------
 * Author:
 * Sameer Reza
 *
 * Modified By:
 *
 */
// --------------------------------------------------------

/* -----------------------------------------------------------
 * Pupose:
 * Creates a grid of visible cells that whose state is tracked in a dictionary.
 * For use in puzzles, with components like the pushable object.
 */
// --------------------------------------------------------
public class GridManager : MonoBehaviour
{
    [SerializeField]
    private float cellSize = 1f;

    [SerializeField]
    private Vector2Int gridSize = new Vector2Int(10, 10);

    [SerializeField]
    private bool showGrid = true;

    [SerializeField]
    private Color gridColor = new Color(1f, 1f, 1f, 0.2f); // Semi-transparent white by default

    [SerializeField]
    private float gridLineWidth = 0.02f;

    private Dictionary<Vector2Int, GridCell> grid = new Dictionary<Vector2Int, GridCell>();
    private GameObject gridVisual;

    private void Awake()
    {
        InitializeGrid();
        if (showGrid)
        {
            CreateGridVisual();
        }
    }

    private void InitializeGrid()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                grid[pos] = new GridCell(pos);
            }
        }
    }

    private void CreateGridVisual()
    {
        // Create parent object for grid lines
        gridVisual = new GameObject("Grid Visual");
        gridVisual.transform.SetParent(transform);

        // Create material for lines
        Material lineMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        lineMaterial.color = gridColor;

        // Create horizontal lines
        for (int z = 0; z <= gridSize.y; z++)
        {
            GameObject line = CreateLine(
                new Vector3(0, 0, z * cellSize),
                new Vector3(gridSize.x * cellSize, 0, z * cellSize),
                lineMaterial
            );
            line.transform.SetParent(gridVisual.transform);
        }

        // Create vertical lines
        for (int x = 0; x <= gridSize.x; x++)
        {
            GameObject line = CreateLine(
                new Vector3(x * cellSize, 0, 0),
                new Vector3(x * cellSize, 0, gridSize.y * cellSize),
                lineMaterial
            );
            line.transform.SetParent(gridVisual.transform);
        }
    }

    private GameObject CreateLine(Vector3 start, Vector3 end, Material material)
    {
        GameObject lineObj = new GameObject("GridLine");
        LineRenderer line = lineObj.AddComponent<LineRenderer>();

        line.material = material;
        line.startWidth = gridLineWidth;
        line.endWidth = gridLineWidth;
        line.positionCount = 2;
        line.useWorldSpace = true;

        // Slightly raise the lines above the ground to prevent z-fighting
        start.y += 0.01f;
        end.y += 0.01f;

        line.SetPosition(0, start);
        line.SetPosition(1, end);

        return lineObj;
    }

    public Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        return new Vector2Int(
            Mathf.RoundToInt(worldPosition.x / cellSize),
            Mathf.RoundToInt(worldPosition.z / cellSize)
        );
    }

    public Vector3 GridToWorldPosition(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x * cellSize, 0, gridPosition.y * cellSize);
    }

    public bool IsCellOccupied(Vector2Int gridPosition)
    {
        return grid.ContainsKey(gridPosition) && grid[gridPosition].IsOccupied;
    }

    public void SetCellOccupied(Vector2Int gridPosition, bool isOccupied)
    {
        if (grid.ContainsKey(gridPosition))
        {
            grid[gridPosition].IsOccupied = isOccupied;
        }
    }

    public void ToggleGridVisibility(bool show)
    {
        if (gridVisual != null)
        {
            gridVisual.SetActive(show);
        }
    }

    public bool IsValidPosition(Vector2Int position)
    {
        return position.x >= 0
            && position.x < gridSize.x
            && position.y >= 0
            && position.y < gridSize.y;
    }

    private void OnDestroy()
    {
        if (gridVisual != null)
        {
            Destroy(gridVisual);
        }
    }
}

public class GridCell
{
    public Vector2Int Position { get; private set; }
    public bool IsOccupied { get; set; }

    public GridCell(Vector2Int position)
    {
        Position = position;
        IsOccupied = false;
    }
}
