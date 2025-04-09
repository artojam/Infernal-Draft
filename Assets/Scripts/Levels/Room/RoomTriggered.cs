using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RoomTriggered : MonoBehaviour
{
    public event Action OnTriggeretPlayer;
    public bool test;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //Debug.Log($"Trigger { test }");
            OnTriggeretPlayer?.Invoke();
        }
    }
}
