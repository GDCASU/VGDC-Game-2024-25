using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/* -----------------------------------------------------------
* Author:
* Cami
* 
* Modified By:
*/// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Creates a way for the Wiffle NPC to go from place to place
*/// --------------------------------------------------------

public class WiffleNPC : NPC
{
    List<int> finishedPaths = new List<int>();

    Vector3 nextLocation; // local location where wiffle should go next
    List<Vector3> locations;
    Vector3 newLocation; // final destination where wiffle should head towards
    GameObject newDrop;
    int index;

    bool isMoving;
    bool resetLocation; // creates new path between two points
    public float speed;
    [SerializeField] Animator animator;

    private void Update()
    {
        if (isMoving) { MoveToNextLocation(); }
        else { animator.SetFloat("x", 0); animator.SetFloat("y", 0); }
    }

    void MoveToNextLocation()
    {
        if (Vector3.Distance(this.transform.position, nextLocation) < 1)
        {
            index++;

            if (locations != null && index < locations.Count) { nextLocation = locations[index]; }
            else { isMoving = false; DropItem(); return; }
        }

        Vector3 direction = (nextLocation - this.transform.position).normalized;
        direction.y = 0;
        if (doDebugLog) Debug.Log(direction);
        animator.SetFloat("x", direction.x > 0 ? 1 : direction.x < 0 ? -1 : 0);
        animator.SetFloat("y", direction.z > 0 ? 1 : direction.z < 0 ? -1 : 0);
        float offset = speed * 0.1f;
        this.transform.position += direction * offset;
    }

    /// <summary> Creates new path based on start and stop locations </summary>
    void FindLocationPath(Vector3 location)
    {
        float distance = 1;
        location.y = this.transform.position.y;

        locations = FindShortestPath(this.transform.position, location, distance);
        isMoving = true;
    }

    /// <summary> Creates the shortest path between the current location and the next one </summary>
    static List<Vector3> FindShortestPath(Vector3 pos1, Vector3 pos2, float tileScale)
    {
        PriorityQueue<(Vector3, List<Vector3>, int)> fringe = new PriorityQueue<(Vector3, List<Vector3>, int)>();

        Vector3 currentState = pos1; // start from position of door1

        // Create first list of paths that can be taken
        foreach (Vector3 successor in GetSuccessors(currentState, tileScale))
        {
            List<Vector3> path = new List<Vector3>();
            path.Add(successor);
            int cost = 1;
            int heuristic = (int)(Mathf.Abs(pos2.x - successor.x) + Mathf.Abs(pos2.y - successor.y));
            fringe.Push((successor, path, cost), heuristic + cost);
        }

        List<Vector3> closed = new List<Vector3>();
        closed.Add(currentState);
        List<Vector3> solution = new List<Vector3>();
        List<Vector3> currentPath = new List<Vector3>();
        int currentCost = 0;
        (Vector3 pos, List<Vector3> path, int cost) node;

        //While fringe set is not empty
        while (!fringe.IsEmpty)
        {
            node = fringe.Pop();
            currentState = node.pos;
            currentPath = node.path;
            currentCost = node.cost;

            // If have arrived at destination, set path and stop
            if (Vector3.Distance(pos2, currentState) < tileScale / 2)
            {
                solution = currentPath;
                return solution;
            }

            if (2*Vector3.Distance(pos1, pos2) < Vector3.Distance(pos2, currentState))
            {
                break;
            }

            // Check if state is already expanded
            if (!closed.Contains(currentState))
            {
                closed.Add(currentState);

                foreach (Vector3 successor in GetSuccessors(currentState, tileScale))
                {
                    List<Vector3> path = new(currentPath);
                    path.Add(successor);
                    int heuristic = (int)(Mathf.Abs(pos2.x - successor.x) + Mathf.Abs(pos2.y - successor.y));
                    fringe.Push((successor, path, currentCost + 1), heuristic + currentCost + 1);
                }
            }
        }

        Debug.Log("No path found");
        return null;

    }

    /// <summary> Finds paths next to current path </summary>
    static List<Vector3> GetSuccessors(Vector3 parent, float scale)
    {
        List<Vector3> successors = new List<Vector3>();
        successors.Add(parent + new Vector3(scale, 0, 0));
        successors.Add(parent + new Vector3(-scale, 0, 0));
        successors.Add(parent + new Vector3(0, 0, scale));
        successors.Add(parent + new Vector3(0, 0, -scale));

        return successors;
    }

    /// <summary> Changes the path start & stop locations </summary>
    public void SwitchLocation(int index, Vector3 startLocation, Vector3 stopLocation, Transform newDropPoint)
    {
        if (!finishedPaths.Contains(index))
        {
            transform.position = startLocation;
            newLocation = stopLocation;
            FindLocationPath(newLocation);
            newDrop = drops[index];
            dropPoint = newDropPoint;
            finishedPaths.Add(index);
        }
    }

    /// <summary> Determine whether or not to drop an item at end of path </summary>
    private void DropItem()
    {
        if (newDrop != null)
        {
            Instantiate(newDrop, dropPoint.position, Quaternion.identity);
        }
    }
}


public class PriorityQueue<T>
{
    private readonly SortedDictionary<int, Queue<T>> _list;
    private int _count;

    public PriorityQueue()
    {
        _list = new SortedDictionary<int, Queue<T>>();
    }

    public void Push(T item, int priority)
    {
        if (!_list.ContainsKey(priority))
        {
            _list[priority] = new Queue<T>();
        }
        _list[priority].Enqueue(item);
        _count++;
    }

    public T Pop()
    {
        if (_count == 0) throw new InvalidOperationException("Queue is empty.");
        var firstKey = _list.First();
        T item = firstKey.Value.Dequeue();
        if (firstKey.Value.Count == 0) _list.Remove(firstKey.Key);
        _count--;
        return item;
    }

    public bool IsEmpty => _count == 0;
}

