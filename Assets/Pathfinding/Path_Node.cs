using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path_Node<T>
{

    public T data;  //T is a generic, so can use this for anything, not just tiles

    public Path_Edge<T>[] edges;  //Nodes leading OUT from this node


}