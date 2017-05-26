using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGraph {
    public Dictionary<Hex, Path_Node<Hex>> nodes;

    public HexGraph(HexMap hexMap)
    {
        nodes = new Dictionary<Hex, Path_Node<Hex>>();

        //loop through all the hexes of the world
        //create a node for each hex
        //no nodes for unwalkable hexes
        //TODO: Implement existence of non-walkable hexes

        int nodeCount = 0;
        for(int q=0; q< hexMap.NumColumns(); q++)
        {
            for (int r = 0; r < hexMap.NumRows(); r++)
            {
                Hex h = hexMap.GetHexFromHexMap(q, r);
                Path_Node<Hex> n = new Path_Node<Hex>();
                n.data = h;
                nodes.Add(h, n);
                nodeCount++;
            }
        }
        //Debug.Log("Node count: " + nodeCount);

        int edgeCount = 0;
        foreach(Hex h in nodes.Keys)
        {
            Path_Node<Hex> n = nodes[h];
            //get a list of neighbors for the hexes
            //if a neighbor is walkable, create an edge to the relevant node

            List<Path_Edge<Hex>> edges = new List<Path_Edge<Hex>>();
            Hex[] neighbors = h.GetNeighbors();

            for (int i = 0; i < neighbors.Length; i++)
            {
                if(neighbors[i]!=null && neighbors[i].MovementCost > 0)
                {
                    Path_Edge<Hex> e = new Path_Edge<Hex>();
                    e.cost = neighbors[i].MovementCost;
                    e.node = nodes[neighbors[i]];

                    //add the edge to the temporary and growable list
                    edges.Add(e);
                    edgeCount++;
                }
            }

            n.edges = edges.ToArray();
        }

        //Debug.Log("Edge count: " + edgeCount);
    }
}
