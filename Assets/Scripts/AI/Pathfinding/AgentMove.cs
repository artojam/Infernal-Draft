using System;
using System.Collections;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class AgentMove : MonoBehaviour
{
    public event Action OnStopAgent;
    public event Action OnStartMovingAgent;
    
    public float speed;
    public float distanseToStop = 0.5f;

    public bool isToStop 
        { private set; get; }

    [SerializeField]
    private Transform target;

    [SerializeField]
    private Pathfinding pathfinding;
    
    private Rigidbody2D rb;
    private Coroutine coroutineMove;



    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void StartMove() =>
        coroutineMove = StartCoroutine(Move());

    public void StopMove() =>
        StopCoroutine(coroutineMove);

    private IEnumerator Move()
    {
        if(target == null)
        {
            isToStop = true;
            OnStopAgent?.Invoke();
        }

        Vector3 lastTargetPos = target.position;
        Vector3[] path = new Vector3[(pathfinding.gridHeight)];


        int length = pathfinding.FindPath(rb.position, lastTargetPos, ref path);
        int index = pathfinding.gridHeight - length;

        Vector3 nextPoint = path[index];
        Vector2 dir = Vector2.zero;

        float correctionSpeed = 750f; // увеличиная скорость при столкновении со стеной

        int offset() => path.Length - length;

        while (true)
        {
            // Если агент дошёл до последней точки и таргет не двигается, он должен стоять
            if (Vector3.Distance(rb.position, target.position) <= distanseToStop && 
                pathfinding.HasObstacleBetweenWorld(rb.position, target.position))
            {
                rb.velocity = Vector2.zero;
                OnStopAgent?.Invoke();
                isToStop = true;
                yield return new WaitForFixedUpdate(); // Ждём, пока таргет не двинется
                continue;
            }

            
            // Если агент видет таргет то идем на него
            if (pathfinding.HasObstacleBetweenWorld(rb.position, target.position))
            {
                if (isToStop)
                    OnStartMovingAgent?.Invoke();
                isToStop = false;

                lastTargetPos = target.position;
                length = 1;
                path[offset()] = target.position;

                index = offset();
            }
            // Если таргет сменил позицию, пересчитываем путь
            else if (Vector3.Distance(lastTargetPos, target.position) > 1f)
            {
                if (isToStop)
                    OnStartMovingAgent?.Invoke();
                isToStop = false;

                lastTargetPos = target.position;
                
                length = pathfinding.FindPath(rb.position, lastTargetPos, ref path);
                
                index = offset();
            }
            // Двигаемся к следующей точке пути
            nextPoint = path[index];

            // Обходим стены
            dir = -pathfinding.CheckDirWallsAround(rb.position);
            if (dir == Vector2.zero)
            {
                dir = ((Vector2)nextPoint - rb.position).normalized;
                correctionSpeed = 1f;
            }
            else
            {
                dir = (dir + ((Vector2)nextPoint - rb.position).normalized).normalized;
            }

            rb.velocity = dir * speed * correctionSpeed;

            // Если достигли текущей точки пути, переходим к следующей
            if (Vector3.Distance(rb.position, nextPoint) <= 0.05f && 
                index < path.Length - 1)
            {
                index++;
            }

            yield return new WaitForFixedUpdate();
        }
    }
}
