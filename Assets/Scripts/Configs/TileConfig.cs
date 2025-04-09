using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "NewTileData", menuName = "Game/Configs/Tile Data")]
public class TileConfig : ScriptableObject
{
    [field: SerializeField]
    public TileBase tileWallTop // Тайл верха стены и тайл для перевода в TileMap колайдера 
        { private set; get; }
    
    [field: SerializeField]
    public TileBase tileWallSide // Тайл бока стены
        { private set; get; }
    
    [field: SerializeField]
    public TileBase tileFloor // Тайл пола 
        { private set; get; }
}
