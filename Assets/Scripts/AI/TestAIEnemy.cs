using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestAIEnemy : MonoBehaviour
{
    public RoomData room;

    public Transform target;


    private Pathfinding pathfinder;

    void Start()
    {
        pathfinder = new Pathfinding(room);
    }

    private void Update()
    {
        FindPath(transform.position, target.position);
    }

    public void FindPath(Vector3 start, Vector3 target)
    {
        if (room != null)
        {
            pathfinder = new Pathfinding(room);
            StartCoroutine(pathfinder.FindPathCoroutine(start, target, OnPathFound));
        }
    }

    private void OnPathFound(List<Vector2> path)
    {
        if (path == null)
        {
            Debug.Log("Путь не найден!");
            return;
        }

        Debug.Log("Путь найден:");
        foreach (Vector2 point in path)
        {
            Debug.Log(point);
        }
    }
}
