using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGraph : MonoBehaviour {
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
    }
}
