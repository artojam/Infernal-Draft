using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int x, y;
    public bool walkable;
    public int gCost, hCost;
    public Node parent;
    public int fCost => gCost + hCost;

    public Node(int x, int y, bool walkable)
    {
        this.x = x;
        this.y = y;
        this.walkable = walkable;
    }
}
