using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path_Edge<T>
{
    //<T> is a generic so this also doesn't care;

    public Path_Node<T> node;   //Node this edge leads to
    public float cost;          //Cost to travel this edge
                                //(based on the node's cost)

}