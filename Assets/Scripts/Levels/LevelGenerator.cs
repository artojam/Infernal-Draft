using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    public List<GameObject> debuglist = new List<GameObject>();
    
    [SerializeField, Header("Static Rooms")] 
    private RoomConfig startRoomPrefab; // Начальная комната

    [SerializeField] 
    private RoomConfig endRoomPrefab; // Конечная комната

    [SerializeField, Header("Rooms")] 
    private RoomConfig[] roomPrefabs; // Обычные комнаты
    
    
    [SerializeField, Header("Data")] 
    private int roomCount = 5; // Количество обычных комнат

    [SerializeField]
    private int maxCountShop = 1; // Максимальная колво магазинов
    [SerializeField]
    private int maxCountTreasure = 1; // Максимальная колво сокровещниц




    private int countShop;
    private int countTreasure;


    private Dictionary<Vector2Int, RoomConfig> rooms = new Dictionary<Vector2Int, RoomConfig>();
    private List<Vector2Int> Dirs = new List<Vector2Int>();

    private Vector2Int currentPosition = Vector2Int.zero;
    private Vector2Int oldPosition = Vector2Int.zero;
    private Vector2Int dir;
    
    private RoomConfig thisRoom = null;
    private RoomConfig oldRoom = null;

    private bool isCreateRoom = true;

    void Start()
    {
        GenerateLevel();
    }

    private void GenerateLevel()
    {
        // Создаём стартовую комнату
        CreateRoom(startRoomPrefab, Vector2Int.zero);
        oldRoom = startRoomPrefab;
        rooms[Vector2Int.zero] = startRoomPrefab;

        while (rooms.Count < roomCount + 1)
        {
            thisRoom = ToRoom();

            DetermineRoomDirection();

            NextPosition();
            if(isCreateRoom) 
                CreateRoom(thisRoom, currentPosition);


            CreateCorridor(oldPosition, currentPosition, oldRoom.size, thisRoom.size);

            oldRoom = thisRoom;
        }

        DetermineRoomDirection();

        // Добавляем конечную комнату в конце пути
        
        NextPosition();
        CreateRoom(endRoomPrefab, currentPosition);
        thisRoom = endRoomPrefab;
        CreateCorridor(oldPosition, currentPosition, oldRoom.size, thisRoom.size);
        
        // конец генерации
        CreateCollider();

        Debug.Log($"конец генерации { rooms.Count }");
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
    }

    private void CreateLevel()
    {

    } 


    private RoomConfig ToRoom()
    {
        RoomConfig newRoom = RandomSelector.SelectRandom(roomPrefabs);
        switch (newRoom.type)
        {
            case (TypeRoom.Shop):
                if (countShop >= maxCountShop || rooms.Count < Mathf.RoundToInt(roomCount / 3))
                {
                    return ToRoom();
                }
                break;
            case (TypeRoom.Treasure):
                if (countTreasure >= maxCountTreasure || rooms.Count < Mathf.RoundToInt(roomCount / 3))
                {
                    return ToRoom();
                }
                break;
        }

        return newRoom;

    } 


    private void CreateRoom(RoomConfig roomData, Vector2Int position)
    {
        List<Vector2Int> dirProh = new List<Vector2Int>();

        if (Dirs.Count > 0)
        {
            dirProh.Add(-dir);
            dirProh.Add(Dirs[Dirs.Count - 1]);
        }
        else
        {
            dirProh.Add(-dir);
        }

        Room room = Instantiate(roomData.room, new Vector3(position.x - 0.5f, position.y - 0.5f), Quaternion.identity);

        Vector2Int offset = currentPosition - (roomData.size / 2) + new Vector2Int(0, 1);
        room.Init(roomData, currentPosition, offset, dirProh);
        
        CopyTilesToMainTilemap(room, position);
    }

    private void CopyTilesToMainTilemap(Room room, Vector2Int position)
    {
        Tilemap roomTilemap = room.data.roomTilemap;

        if (roomTilemap == null) return;

        BoundsInt bounds = roomTilemap.cellBounds;
        TileBase[] allTiles = roomTilemap.GetTilesBlock(bounds);

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    Vector3Int tilePosition = new Vector3Int(bounds.x + x, bounds.y + y, 0) + (Vector3Int)position;
                    
                    GameController.controller.mainTilemapFloor
                        .SetTile(tilePosition, GameController.controller.tiles.tileFloor);

                    if(tile != GameController.controller.tiles.tileFloor)
                        GameController.controller.mainTilemapWall.SetTile(tilePosition, tile);
                    
                }
            }
        }
        Destroy(room.gameObject.transform.GetChild(0).gameObject);
    }

    public void CreateCorridor(Vector2Int startRoom, Vector2Int endRoom, Vector2Int startSize, Vector2Int endSize)
    {
        Vector2Int startOffset = (dir.x < 0 || dir.y > 0) ? dir : Vector2Int.zero;
        Vector2Int startOffsetVertical = (dir.y > 0) ? dir : Vector2Int.zero;

        Vector2Int startEdge = startRoom + dir * (startSize / 2) + startOffset - startOffsetVertical;
        Vector2Int startEdgeWall = startEdge + startOffsetVertical;
        
        Vector2Int startEdgeWallX = startEdge + dir;

        Vector2Int endOffset = (dir.x < 0 || dir.y > 0) ? -dir : Vector2Int.zero;
        Vector2Int endOffsetVertical = (dir.y < 0) ? dir : Vector2Int.zero;
        
        Vector2Int endEdge = endRoom - dir * (endSize / 2) - endOffset + endOffsetVertical;
        Vector2Int endEdgeWall = endEdge - endOffsetVertical;
        
        Vector2Int endEdgeWallX = endEdge - dir;

        int steps = Mathf.Abs(endEdge.x - startEdge.x) + Mathf.Abs(endEdge.y - startEdge.y);
        
        Vector3Int offsetFloor = new Vector3Int(dir.y < 0 ? dir.y : -dir.y,
                                                dir.x < 0 ? -dir.x : dir.x);

        bool isVertical = dir.x == 0;
        bool isDown = (dir.y < 0);
        bool isLeft = (dir.x < 0);
        bool shouldWall = (endEdgeWall.y <= 0 && isDown) || isDown;
        bool shouldWallX = (endEdgeWall.x <= 0 && isLeft) || isLeft;

        // Подготавливаем массивы позиций
        List<Vector3Int> floorTiles = new List<Vector3Int>();
        List<Vector3Int> wallTiles = new List<Vector3Int>();
        List<Vector3Int> targetTiles = new List<Vector3Int>();

        for (int i = 0; i < steps; i++)
        {
            Vector3Int posFloor = (Vector3Int)(startEdge + dir * i);
            floorTiles.Add(posFloor);
            floorTiles.Add(posFloor + offsetFloor);

            Vector3Int posWall = (Vector3Int)(startEdgeWall + dir * i);
            bool isEndWall = shouldWall ? 
                                          (!isVertical || posWall.y > endEdgeWall.y) : 
                                          (!isVertical || posWall.y < endEdgeWall.y) ;

            if (isEndWall)
            {
                targetTiles.Add(posWall - offsetFloor);
                targetTiles.Add(posWall + offsetFloor * (isVertical ? 2 : 3 ));
            }

            posWall = (Vector3Int)(startEdgeWallX + dir * i);
            isEndWall = shouldWallX ? 
                                      posWall.x > endEdgeWallX.x : 
                                      posWall.x < endEdgeWallX.x;

            if (!isVertical)
            {
                wallTiles.Add(posFloor + offsetFloor * 2);
                if (isEndWall)
                    wallTiles.Add(posWall - offsetFloor * 2);
            }
        }

        foreach (Vector3Int tile in floorTiles)
        {
            GameController.controller.mainTilemapFloor.SetTile(tile, GameController.controller.tiles.tileFloor);
            GameController.controller.mainTilemapWall.SetTile(tile, null);
        }
        foreach (Vector3Int tile in wallTiles)
            GameController.controller.mainTilemapWall.SetTile(tile, GameController.controller.tiles.tileWallSide);

        foreach (Vector3Int tile in targetTiles)
            GameController.controller.mainTilemapWall.SetTile(tile, GameController.controller.tiles.tileWallTop);
    }


    private void CreateCollider()
    {
        BoundsInt bounds = GameController.controller.mainTilemapWall.cellBounds;

        List<Vector3Int> positionsToUpdate = new List<Vector3Int>();

        // Перебираем все тайлы в целевом тайлмапе и запоминаем позиции для обновления
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (GameController.controller.mainTilemapWall.GetTile(pos) == GameController.controller.tiles.tileWallTop)
            {
                positionsToUpdate.Add(pos);
            }
        }

        // Устанавливаем новые тайлы одним вызовом
        if (positionsToUpdate.Count > 0)
        {
            foreach (Vector3Int pos in positionsToUpdate)
            {
                GameController.controller.colladerTilemap.SetTile(pos, GameController.controller.tiles.tileFloor);
            }
        }
        GameController.controller.colladerTilemap.gameObject.AddComponent<TilemapCollider2D>().usedByComposite = true;
        GameController.controller.colladerTilemap.gameObject.AddComponent<CompositeCollider2D>();

        GameController.controller.colladerTilemap.CompressBounds();
    }


    private void DetermineRoomDirection()
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        List<Vector2Int> availableDirections = new List<Vector2Int>(directions);
        Vector2Int newPosRoom = Vector2Int.zero;

        for (int i = 0; i < 10 && availableDirections.Count > 0; i++)
        {
            dir = availableDirections[Random.Range(0, availableDirections.Count)];
            newPosRoom = GetNextPosition(currentPosition, dir);

            if (!rooms.ContainsKey(newPosRoom))
            {
                switch (thisRoom.type)
                {
                    case TypeRoom.Shop:
                        countShop++;
                        break;

                    case TypeRoom.Treasure:
                        countTreasure++;
                        break;
                }


                isCreateRoom = true;

                Dirs.Add(dir);
                rooms[currentPosition] = thisRoom;

                return;
            }
            else
            {
                isCreateRoom = false;
                thisRoom = rooms[newPosRoom];
            }

             availableDirections.Remove(dir);
        }

        
    }


    private void NextPosition()
    {
        Vector2Int newPos = GetNextPosition(currentPosition, dir);

        oldPosition = currentPosition;

        currentPosition = newPos;
    }


    private Vector2Int GetNextPosition(Vector2Int _pos, Vector2Int _dir) => 
        _pos + (_dir * 32);
}
