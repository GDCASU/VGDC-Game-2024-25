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
 * Handle the global grid of rooms in the game (for per-room grid, see SubGrid.cs)
 */
// --------------------------------------------------------


/// <summary>
/// Handles everything about the global grid of rooms in the game
/// </summary>
public class RoomGrid : MonoBehaviour
{
    [SerializeField]
    private Vector2Int gridSize;

    [SerializeField]
    private float roomSize = 10f;

    private Room[,] rooms;

    private void Awake()
    {
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        rooms = new Room[gridSize.x, gridSize.y];
    }

    public Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        Vector3 localPosition = worldPosition - transform.position;
        int x = Mathf.FloorToInt(localPosition.x / roomSize);
        int y = Mathf.FloorToInt(localPosition.z / roomSize);
        return new Vector2Int(x, y);
    }

    public bool PlaceRoom(Room room, Vector2Int gridPosition)
    {
        if (IsValidPosition(gridPosition) && rooms[gridPosition.x, gridPosition.y] == null)
        {
            rooms[gridPosition.x, gridPosition.y] = room;
            Vector3 worldPosition =
                transform.position
                + new Vector3(gridPosition.x * roomSize, 0, gridPosition.y * roomSize);
            room.transform.position = worldPosition;
            return true;
        }
        return false;
    }

    private bool IsValidPosition(Vector2Int position)
    {
        return position.x >= 0
            && position.x < gridSize.x
            && position.y >= 0
            && position.y < gridSize.y;
    }

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }
}

public class Room : MonoBehaviour
{
    [SerializeField]
    private SubGrid subGrid;

    public SubGrid SubGrid => subGrid;
}
