using System.Runtime.ExceptionServices;
using UnityEngine;



public class RoomData : MonoBehaviour
{
    [SerializeField, Range(0, 100)] 
    public byte chance;

    public Vector2Int size = Vector2Int.zero;
    public Vector2Int pos = Vector2Int.zero;
    public TypeDir dir;

    public bool[,] grid;

    public Vector2Int offset;

    private void OnDrawGizmos()
    {
        if (grid.Length != 0)
        {
            Vector2 _offset = offset + new Vector2(0.5f, 0.5f) ;

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Vector3 pos = new Vector3(_offset.x + x, _offset.y + y, 0);
                    Gizmos.DrawSphere(pos, 0.2f);
                }
            }
        }
    }
}
