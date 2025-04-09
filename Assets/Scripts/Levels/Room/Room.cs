using System.Collections.Generic;
using UnityEngine;

public enum TypeRoom : byte
{
    None = 0,
    Shop,
    Treasure,
    Combat  
}



public class Room : MonoBehaviour
{
    public RoomConfig data 
        { private set; get; }
    public byte[] grid 
        { private set; get; }

    public Vector2Int pos = Vector2Int.zero;
    public Vector2Int offset;

    public Vector2Int size => data.size;

    private RoomTriggered roomTrigger;

    private List<Vector2Int> dirProh = new List<Vector2Int>();

    public void Init(RoomConfig data, Vector2Int pos, Vector2Int offset, List<Vector2Int> dirProh)
    {
        if(data == null)
        {
            Debug.LogError($"[Room:{ name }/Init]: комнате с позицией { pos } не задан конфиг");
            return;
        }

        this.data = data;
        this.pos = pos;
        this.offset = offset;

        this.dirProh.AddRange(dirProh);
        
        roomTrigger = gameObject.GetComponentInChildren<RoomTriggered>();
        
        if (this.data.type == TypeRoom.Combat)
        {

            grid = Pathfinding.GenerateGridFromTilemap(data.roomTilemap,
                pos,
                data.size,
                GameController.controller.tiles.tileWallTop
                );

            roomTrigger.OnTriggeretPlayer += StartCombat;
            roomTrigger.test = true;
            return;
        }
        if (roomTrigger != null)
        {
            Destroy(roomTrigger.gameObject);
            roomTrigger = null;
        }
    }

    private void StartCombat()
    {
        GameController.controller.pathfinding.grid = grid;
        GameController.controller.pathfinding.gridHeight = size.y;
        GameController.controller.pathfinding.gridWidth = size.x;
        GameController.controller.pathfinding.gridOffset = offset;
    }

    private void OnDestroy()
    {
        if(roomTrigger != null)
            roomTrigger.OnTriggeretPlayer -= StartCombat;
    }

    private void OnDrawGizmos()
    {
        if (data.type == TypeRoom.Combat)
        {
            Vector2 _offset = offset;

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Vector3 pos = new Vector3(_offset.x + x, _offset.y + y, 0);
                    if (grid[x * size.y + y] == 255)
                    {
                        Gizmos.color = Color.white;
                        Gizmos.DrawSphere(pos, 0.2f);
                    }
                    else
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawSphere(pos, 0.2f);
                    }
                }
            }
        }
        else
            return;
    }
}
