using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "NewRoom", menuName = "Game/Configs/Room")]
public class RoomConfig : ScriptableObject
{

#if UNITY_EDITOR
    [SerializeField]
    private string nameTileConfig; // имя конфига для тайлов
#endif

    [field: SerializeField, Range(0, 100)]
    public byte chance
        { private set; get; }

    [field: SerializeField]
    public TypeRoom type
        { private set; get; }

    [field: SerializeField]
    public Room room
        { private set; get; }

    public Tilemap roomTilemap
        { private set; get; }

    public Vector2Int size 
        { private set; get; }


    private void OnValidate()
    {
        if (room != null)
        {
            if (roomTilemap == null)
                roomTilemap = room.GetComponentInChildren<Tilemap>();

            if (roomTilemap == null)
            {
                Debug.LogError($"[RoomConfig:{name}/OnValidate]: у комнаты {room.name} нет Tilemap");
                return;
            }

            RoomTriggered roomTrigger = null;

            if (type == TypeRoom.Combat)
                roomTrigger = room.GetComponentInChildren<RoomTriggered>();

            if (roomTrigger == null && type == TypeRoom.Combat)
            {
                Debug.LogError($"[RoomConfig:{name}/OnValidate]: у комнаты {room.name} нет RoomTriggered");
                return;
            }

            BoundsInt bounds = roomTilemap.cellBounds;
            Vector2Int startPosFromSizeRoom = Vector2Int.zero;

            TileConfig tileConfig = FindTileConfig();

            bool isEndFound = false;

            for (int x = 0; x < bounds.size.x; x++)
            {
                for (int y = 0; y < bounds.size.y; y++)
                {
                    Vector3Int tilePos = new Vector3Int(bounds.x + x, bounds.y + y);
                    TileBase tile = roomTilemap.GetTile(tilePos);

                    if (tile != null)
                    {
                        startPosFromSizeRoom = new Vector2Int(x, y);

                        isEndFound = true;

                        Debug.Log($"[RoomConfig:{name}/OnValidate]: Стартовая позиция для вычесления размера комнаты {startPosFromSizeRoom}");

                        break;
                    }
                }
                if (isEndFound)
                    break;
            }

            if (!isEndFound)
            {
                Debug.LogError($"[RoomConfig:{name}/OnValidate]: Не удалось найти стартовую позицию для вычисления размера комнаты.");
                return;
            }

            int newSizeX = 0;
            int newSizeY = 0;

            for (int x = startPosFromSizeRoom.x; x < bounds.size.x; x++)
            {
                Vector3Int tilePos = new Vector3Int(bounds.x + x, bounds.y + startPosFromSizeRoom.y);
                TileBase tile = roomTilemap.GetTile(tilePos);

                if (tile == null)
                    break;

                newSizeX++;

            }
            Debug.Log($"[RoomConfig:{name}/OnValidate]: новая ширина комнаты {newSizeX}");


            for (int y = startPosFromSizeRoom.y; y < bounds.size.y; y++)
            {
                Vector3Int tilePos = new Vector3Int(bounds.x + startPosFromSizeRoom.x, bounds.y + y);
                TileBase tile = roomTilemap.GetTile(tilePos);

                if (tile == null)
                    break;

                newSizeY++;

            }
            Debug.Log($"[RoomConfig:{name}/OnValidate]: новая высота комнаты {newSizeY}");

            size = new Vector2Int(newSizeX-2, newSizeY-3);
        }
    }

#if UNITY_EDITOR
    private TileConfig FindTileConfig()
    {
        string[] guids = AssetDatabase.FindAssets($"t:TileConfig {nameTileConfig}");

        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            Debug.Log($"{path}");
            return AssetDatabase.LoadAssetAtPath<TileConfig>(path);
        }

        Debug.LogError($"{nameTileConfig} не найден");

        return null;
    }
#endif

}
