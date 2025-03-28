using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using UnityEngine.UIElements;

public enum TypeDir
{
    Up = 0,
    Dawn,
    Left,
    Right
}

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] 
    private Tilemap mainTilemapWall; // Tilemap Стен

    [SerializeField]
    private Tilemap mainTilemapFloor; // Tilemap пола

    [SerializeField] 
    private RoomData startRoomPrefab; // Начальная комната
    [SerializeField] 
    private RoomData endRoomPrefab; // Конечная комната

    [SerializeField] 
    private RoomData[] roomPrefabs; // Обычные комнаты
    
    [SerializeField] 
    private int roomCount = 5; // Количество обычных комнат

    [SerializeField]
    private Tilemap sourceTilemap;
    [SerializeField]
    private TileBase targetTile;
    [SerializeField]
    private TileBase newTile;
    [SerializeField]
    private TileBase wallTile;

    private List<Vector2Int> occupiedPositions = new List<Vector2Int>();
    private Dictionary<Vector2Int, RoomData> rooms = new Dictionary<Vector2Int, RoomData>();

    private Vector2Int currentPosition = Vector2Int.zero;
    private Vector2Int oldPosition = Vector2Int.zero;
    private Vector2Int dir;
    
    private RoomData thisRoom = null;
    private RoomData oldRoom = null;

    private int numberRoom = 0;

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
        rooms[Vector2Int.zero] = oldRoom;

        for (numberRoom = 0; numberRoom < roomCount; numberRoom++)
        {
            thisRoom = RandomSelector.SelectRandom(roomPrefabs);

            GetColledorPosition();

            GetNextPosition(thisRoom);
            if(isCreateRoom) CreateRoom(thisRoom, currentPosition);


            CreateColledor(oldPosition, currentPosition, oldRoom.size, thisRoom.size);
            oldRoom = thisRoom;
        }

        GetColledorPosition();

        // Добавляем конечную комнату в конце пути
        
        GetNextPosition(endRoomPrefab);
        CreateRoom(endRoomPrefab, currentPosition);
        thisRoom = endRoomPrefab;
        CreateColledor(oldPosition, currentPosition, oldRoom.size, thisRoom.size);

        MergeTiles();
        rooms.Clear();
        rooms = null;
        occupiedPositions.Clear();
        occupiedPositions = null;
        roomPrefabs = null;


        Debug.Log("конец генерации");
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
        Destroy(this);
    }


    private void CreateRoom(RoomData roomPrefab, Vector2Int position)
    {
        RoomData room = Instantiate(roomPrefab, new Vector3(position.x, position.y), Quaternion.identity);
        CopyTilesToMainTilemap(room, position);
        Destroy(room.gameObject.transform.GetChild(0).gameObject);
        //Destroy(room);
        occupiedPositions.Add(position);
    }

    private void CopyTilesToMainTilemap(RoomData room, Vector2Int position)
    {
        Tilemap roomTilemap = room.GetComponentInChildren<Tilemap>();

        room.grid = Pathfinding.GenerateGridFromTilemap(roomTilemap, room.pos, room.size, newTile);

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
                    mainTilemapFloor.SetTile(tilePosition, newTile);
                    if(tile != newTile)
                        mainTilemapWall.SetTile(tilePosition, tile);
                    
                }
            }
        }
    }

    public void CreateColledor(Vector2Int startRoom, Vector2Int endRoom, Vector2Int startSize, Vector2Int endSize)
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
        Vector3Int offsetFloor = new Vector3Int(dir.y < 0 ? dir.y : -dir.y, dir.x < 0 ? -dir.x : dir.x);

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
            bool isEndWall = shouldWall ? (!isVertical || posWall.y > endEdgeWall.y) : (!isVertical || posWall.y < endEdgeWall.y);

            if (isEndWall)
            {
                targetTiles.Add(posWall - offsetFloor);
                targetTiles.Add(posWall + offsetFloor * (isVertical ? 2 : 3));
            }

            posWall = (Vector3Int)(startEdgeWallX + dir * i);
            isEndWall = shouldWallX ? posWall.x > endEdgeWallX.x : posWall.x < endEdgeWallX.x;

            if (!isVertical)
            {
                wallTiles.Add(posFloor + offsetFloor * 2);
                if (isEndWall)
                    wallTiles.Add(posWall - offsetFloor * 2);
            }
        }

        // Массовое применение тайлов, минимизируя вызовы SetTile()
        foreach (var tile in floorTiles)
        {
            mainTilemapFloor.SetTile(tile, newTile);
            mainTilemapWall.SetTile(tile, null);
        }
        foreach (var tile in wallTiles)
            mainTilemapWall.SetTile(tile, wallTile);

        foreach (var tile in targetTiles)
            mainTilemapWall.SetTile(tile, targetTile);
    }


    private void MergeTiles()
    {
        BoundsInt bounds = mainTilemapWall.cellBounds;

        List<Vector3Int> positionsToUpdate = new List<Vector3Int>();

        // Перебираем все тайлы в целевом тайлмапе и запоминаем позиции для обновления
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (mainTilemapWall.GetTile(pos) == targetTile)
            {
                positionsToUpdate.Add(pos);
            }
        }

        // Устанавливаем новые тайлы одним вызовом
        if (positionsToUpdate.Count > 0)
        {
            foreach (Vector3Int pos in positionsToUpdate)
            {
                sourceTilemap.SetTile(pos, newTile);
            }
        }
        sourceTilemap.gameObject.AddComponent<TilemapCollider2D>().usedByComposite = true;
        sourceTilemap.gameObject.AddComponent<CompositeCollider2D>();

        sourceTilemap.CompressBounds(); // Уменьшает потребление памяти, если тайлы изменяются
    }


    private void GetColledorPosition()
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        List<Vector2Int> availableDirections = new List<Vector2Int>(directions);
        Vector2Int newPosRoom = Vector2Int.zero;

        for (int i = 0; i < 10 && availableDirections.Count > 0; i++)
        {
            dir = availableDirections[Random.Range(0, availableDirections.Count)];
            newPosRoom = GetNextPosition(thisRoom, currentPosition, dir);

            if (!rooms.ContainsKey(newPosRoom))
            {
                isCreateRoom = true;
                return;
            }

            availableDirections.Remove(dir);
        }

        isCreateRoom = false;
        thisRoom = rooms[newPosRoom];
    }


    private void GetNextPosition(RoomData _room)
    {
        Vector2Int newPos = GetNextPosition(_room, currentPosition, dir);
        _room.dir = GetDir(dir * -1);
        _room.pos = newPos;
        _room.offset = (newPos - (_room.size / 2));
        oldPosition = currentPosition;

        currentPosition = newPos;

        rooms[currentPosition] = thisRoom;
    }


    private Vector2Int GetNextPosition(RoomData _room, Vector2Int _pos, Vector2Int _dir)
    {
        return _pos + (_dir * 32);
    }

    private TypeDir GetDir(int index)
    {
        System.Array dirValues = System.Enum.GetValues(typeof(TypeDir));
        TypeDir randomValue = (TypeDir)dirValues.GetValue(index);
        return randomValue;
    }

    private TypeDir GetDir(Vector2Int value)
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        System.Array dirValues = System.Enum.GetValues(typeof(TypeDir));

        int index = System.Array.IndexOf(directions, value);

        TypeDir randomValue = (TypeDir)dirValues.GetValue(index);
        return randomValue;
    }
}
