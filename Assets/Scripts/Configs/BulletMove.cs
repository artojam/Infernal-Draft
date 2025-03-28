using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMove : MonoBehaviour
{
    [SerializeField]
    private float speed = 1f;

    private Rigidbody2D rb;
    private Transform tr;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        tr = transform;
    }

    private void FixedUpdate()
    {
        Vector2 dir = new Vector2(tr.right.x, tr.right.y);
        rb.MovePosition(rb.position + dir * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(this.gameObject);
    }
}
