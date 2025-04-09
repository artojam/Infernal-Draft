using UnityEngine;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour
{
    public static GameController controller;

    [field: SerializeField, Header("TileMaps")]
    public Tilemap mainTilemapWall // Tilemap ����
        { private set; get; } 

    [field: SerializeField]
    public Tilemap mainTilemapFloor // Tilemap ����
        { private set; get; } 

    [field: SerializeField]
    public Tilemap colladerTilemap // Tilemap ���������
        { private set; get; }

    [field: SerializeField, Header("Tile")]
    public TileConfig tiles
        { private set; get; }

    public Pathfinding pathfinding;

    private void Awake()
    {
        controller = this;
    }
}
