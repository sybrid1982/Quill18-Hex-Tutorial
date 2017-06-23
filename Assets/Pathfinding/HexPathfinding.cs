using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

public class HexPath {

    Stack<Hex> hexStack;

    public HexPath(HexMap hexMap, Hex start, Hex end)
    {
        hexStack = new Stack<Hex>();

        if (hexMap.hexGraph == null)
        {
            hexMap.hexGraph = new HexGraph(hexMap);
        }

        /*Declare the closed and open sets
        ClosedSet tracks all the tiles that have been evaluated
        OpenSet tracks all the tiles we've encountered while pathfinding that we
        haven't yet evaluated*/
        Dictionary<Hex, Path_Node<Hex>> nodes = hexMap.hexGraph.nodes;

        List<Path_Node<Hex>> closedSet = new List<Path_Node<Hex>>(); ;

        SimplePriorityQueue<Path_Node<Hex>> openSet = new SimplePriorityQueue<Path_Node<Hex>>();
        openSet.Enqueue(nodes[start], 0);

        Dictionary<Path_Node<Hex>, Path_Node<Hex>> cameFrom = new Dictionary<Path_Node<Hex>, Path_Node<Hex>>();

        Dictionary<Path_Node<Hex>, float> gScore = new Dictionary<Path_Node<Hex>, float>();
        Dictionary<Path_Node<Hex>, float> fScore = new Dictionary<Path_Node<Hex>, float>();

        //Check if the hex is impassible
        if (nodes.ContainsKey(end) == false)
        {
            Debug.Log("Impassible hex cannot be entered");
            return;
        }

        //initialize all gScore and fScore values to 'infinity'
        //Also initialize the gScore and fScore of the starting hex
        InitializeScores(start, end, nodes, gScore, fScore);

        while (openSet.Count > 0)
        {
            //Get the hex with the best estimated cost to the end hex
            //At the start, this will just be the start hex
            Path_Node<Hex> current = openSet.Dequeue();
            //if the top of the queue is the end hex, we're done looking
            if (current.data == end)
            {
                hexStack = ReconstructPath(cameFrom, current);
                return;
            }

            closedSet.Add(current);

            //Look at current's neighbors
            EvaluateNeighbors(end, closedSet, openSet, cameFrom, gScore, fScore, current);
            //If we get here then we failed to find a path
        }
    }

    private void InitializeScores(Hex start, Hex end, Dictionary<Hex, Path_Node<Hex>> nodes, Dictionary<Path_Node<Hex>, float> gScore, Dictionary<Path_Node<Hex>, float> fScore)
    {
        foreach (Path_Node<Hex> n in nodes.Values)
        {
            gScore[n] = Mathf.Infinity;
            fScore[n] = Mathf.Infinity;
        }
        //intialize the values for the starting node
        gScore[nodes[start]] = 0;
        fScore[nodes[start]] = bestPossiblefScore(start, end);
    }

    private void EvaluateNeighbors(Hex end, List<Path_Node<Hex>> closedSet, SimplePriorityQueue<Path_Node<Hex>> openSet, Dictionary<Path_Node<Hex>, Path_Node<Hex>> cameFrom, Dictionary<Path_Node<Hex>, float> gScore, Dictionary<Path_Node<Hex>, float> fScore, Path_Node<Hex> current)
    {
        foreach (Path_Edge<Hex> e in current.edges)
        {
            //if we already evaluated this hex, abort
            if (closedSet.Contains(e.node))
            {
                continue;
            }

            float tentative_gScore = gScore[current] + e.cost;

            //If we've already a path that gets to this edge which is faster, abort
            if (openSet.Contains(e.node) && tentative_gScore >= gScore[e.node])
            {
                continue;
            }

            //Record this path as it is currently the best
            cameFrom[e.node] = current;
            gScore[e.node] = tentative_gScore;
            fScore[e.node] = gScore[e.node] + bestPossiblefScore(e.node.data, end);
            if (openSet.Contains(e.node))
            {
                //This is a faster way to a hex we've already had and can update its score
                openSet.UpdatePriority(e.node, fScore[e.node]);
            }
            else
            {
                //We've found a path to a new hex and need to add it to the open set
                openSet.Enqueue(e.node, fScore[e.node]);
            }
        }
    }

    Stack<Hex> ReconstructPath(Dictionary<Path_Node<Hex>, Path_Node<Hex>> cameFrom, Path_Node<Hex> currentHex)
    {
        Stack<Hex> totalPath = new Stack<Hex>();
        totalPath.Push(currentHex.data);
        while (cameFrom.ContainsKey(currentHex))
        {
            currentHex = cameFrom[currentHex];
            if (currentHex != null)
                totalPath.Push(currentHex.data);
        }
        totalPath.Pop();
        return totalPath;
    }

    float bestPossiblefScore(Hex start, Hex end)
    {
        if (start != null && end != null)
            return Hex.HexDistance(start, end);
        else
            return -1f;
    }
    
    public Hex GetNextHex()
    {
        if (hexStack == null && hexStack.Count > 0)
            return null;
        else
            return hexStack.Pop();
    }

    public int Length()
    {
        if (hexStack == null)
        {
            return 0;
        }
        else
            return hexStack.Count;
    }

    public Stack<Hex> ClonePathStack()
    {
        var clone = new Stack<Hex>(new Stack<Hex>(hexStack));
        return clone;
    }
}
