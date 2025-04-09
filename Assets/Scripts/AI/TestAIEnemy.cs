using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestAIEnemy : MonoBehaviour
{
    public Room room;

    public Transform target;


    private Pathfinding pathfinder;

    private void OnPathFound(List<Vector2> path)
    {
        if (path == null)
        {
            Debug.Log("���� �� ������!");
            return;
        }

        Debug.Log("���� ������:");
        foreach (Vector2 point in path)
        {
            Debug.Log(point);
        }
    }
}
