using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "NewTileData", menuName = "Game/Configs/Tile Data")]
public class TileConfig : ScriptableObject
{
    [field: SerializeField]
    public TileBase tileWallTop // ���� ����� ����� � ���� ��� �������� � TileMap ��������� 
        { private set; get; }
    
    [field: SerializeField]
    public TileBase tileWallSide // ���� ���� �����
        { private set; get; }
    
    [field: SerializeField]
    public TileBase tileFloor // ���� ���� 
        { private set; get; }
}
