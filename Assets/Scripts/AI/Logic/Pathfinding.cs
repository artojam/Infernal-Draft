using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine.Tilemaps;

public class Pathfinding
{
    public bool[,] grid { private set; get; }

    public byte width { private set; get; }
    public byte height { private set; get; }

    public Vector2Int offsetRoom { private set; get; }

    public Pathfinding(RoomData room)
    {
        grid = room.grid;
        width = (byte)room.size.x;
        height = (byte)room.size.y;

        offsetRoom = room.offset;
    }

    public async Task<List<Vector2>> FindPathAsync(Vector2 startPos, Vector2 targetPos)
    {
        return await Task.Run(() => FindPath(startPos, targetPos));
    }

    public static bool[,] GenerateGridFromTilemap(Tilemap tilemap, Vector2Int posRoom, Vector2Int _sizeRoom, TileBase Floor)
    {
        BoundsInt bounds = tilemap.cellBounds;
        bool[,] grid = new bool[_sizeRoom.x, _sizeRoom.y];
        Vector2Int offsetRoom = (posRoom - _sizeRoom / 2);

        for (int x = 0; x < _sizeRoom.x; x++)
        {
            for (int y = 0; y < _sizeRoom.y; y++)
            {
                Vector3Int tilePosition = new Vector3Int(offsetRoom.x + x, offsetRoom.y + y, 0);
                TileBase tile = tilemap.GetTile(tilePosition);
                grid[x, y] = tile == Floor ? false : true;
            }
        }

        return grid;
    }

    private List<Vector2> FindPath(Vector2 startPos, Vector2 targetPos)
    {
        Node startNode = GetNodeFromWorldPoint(startPos);
        Node targetNode = GetNodeFromWorldPoint(targetPos);

        if (startNode == null || targetNode == null || grid[targetNode.x, targetNode.y] == false)
            return null;

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
                return RetracePath(startNode, targetNode);

            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (grid[neighbor.x, neighbor.y] == false || closedSet.Contains(neighbor))
                    continue;

                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }
        return null;
    }

    private List<Vector2> RetracePath(Node startNode, Node endNode)
    {
        List<Vector2> path = new List<Vector2>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(new Vector2(currentNode.x + offsetRoom.x, currentNode.y + offsetRoom.y));
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return SmoothPath(path);
    }

    private List<Vector2> SmoothPath(List<Vector2> path)
    {
        if (path == null || path.Count < 2)
            return path;

        List<Vector2> smoothedPath = new List<Vector2>();
        smoothedPath.Add(path[0]);

        for (int i = 1; i < path.Count - 1; i++)
        {
            if (!HasClearPath(smoothedPath[smoothedPath.Count - 1], path[i + 1]))
            {
                smoothedPath.Add(path[i]);
            }
        }

        smoothedPath.Add(path[path.Count - 1]);
        return smoothedPath;
    }

    private bool HasClearPath(Vector2 from, Vector2 to)
    {
        RaycastHit2D hit = Physics2D.Linecast(from, to);
        return hit.collider == null;
    }

    private int GetDistance(Node a, Node b)
    {
        int dstX = Mathf.Abs(a.x - b.x);
        int dstY = Mathf.Abs(a.y - b.y);
        return dstX > dstY ? 14 * dstY + 10 * (dstX - dstY) : 14 * dstX + 10 * (dstY - dstX);
    }

    private List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                int checkX = node.x + dx;
                int checkY = node.y + dy;
                if (checkX >= 0 && checkX < width && checkY >= 0 && checkY < height)
                {
                    neighbors.Add(new Node(checkX, checkY, grid[checkX, checkY] == true));
                }
            }
        }
        return neighbors;
    }

    public IEnumerator FindPathCoroutine(Vector2 startPos, Vector2 targetPos, System.Action<List<Vector2>> callback)
    {
        List<Vector2> path = null;
        Task<List<Vector2>> task = FindPathAsync(startPos, targetPos);
        while (!task.IsCompleted)
        {
            yield return null;
        }
        path = task.Result;
        callback?.Invoke(path);
    }

    private Node GetNodeFromWorldPoint(Vector2 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x) - offsetRoom.x;
        int y = Mathf.RoundToInt(worldPosition.y) - offsetRoom.y;
        if (x >= 0 && x < width && y >= 0 && y < height)
            return new Node(x, y, grid[x, y] == true);
        return null;
    }

}
