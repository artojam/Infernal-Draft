using System.Collections;
using UnityEngine;

public class TestAIEnemy : MonoBehaviour
{
    private float distanseAttack = 2.5f;
    

    private AgentMove move;
    private Transform tr;
    private Transform trPlayer;

    private Coroutine corourineAttack;

    private void Start()
    {
        move = GetComponent<AgentMove>();
        tr = transform;
        trPlayer = GameController.controller.player.transform;

        move.StartMove();
        move.OnStopAgent += OnStop;
        move.OnStartMovingAgent += OnMove;
    }

    private void OnStop()
    {
        corourineAttack = StartCoroutine(Attack());
    }

    private void OnMove()
    {
        StopCoroutine(corourineAttack);
    }

    private IEnumerator Attack()
    {
        Vector3 dir = (trPlayer.position - tr.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(tr.position, dir, distanseAttack);
        
        if(hit.collider.CompareTag("Player"))
        {
            PlayerData player = hit.collider.GetComponent<PlayerData>();
            Debug.Log("Player");
        }

        yield return new WaitForSeconds(0.1f);
    }


}
