using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WiffleNPC : NPC
{
    Vector3 nextLocation;
    List<Vector3> locations;
    int index;
    bool isMoving;
    public bool resetLocation;
    public Vector3 newLocation;

    private void Update()
    {
        if (resetLocation) { FindLocationPath(newLocation); }
        else if (isMoving) { MoveToNextLocation(); }
    }

    void MoveToNextLocation()
    {
        if (this.transform.position == nextLocation)
        {
            index++; nextLocation = locations[index];
        }

        Vector3 direction = (this.transform.position - nextLocation).normalized;
        float offset = 1;
        this.transform.position += direction * offset;
    }

    void FindLocationPath(Vector3 location)
    {
        float distance = 5;

        locations = FindShortestPath(this.transform.position, location, distance);
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
            fringe.Push((successor, path, cost), (int)Vector3.Distance(successor, pos2) + cost);
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

            // Check if state is already expanded
            if (!closed.Contains(currentState))
            {
                closed.Add(currentState);

                foreach (Vector3 successor in GetSuccessors(currentState, tileScale))
                {
                    List<Vector3> path = new(currentPath);
                    path.Add(successor);
                    fringe.Push((successor, path, currentCost + 1), (int)Vector3.Distance(successor, pos2) + currentCost + 1); // change to heuristic later
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

