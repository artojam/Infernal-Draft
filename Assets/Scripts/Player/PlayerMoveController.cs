using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float Speed = 5f;
    [SerializeField]
    private Joystick JoystickMove;

    private Rigidbody2D rb;
    private Vector2 movement;
    private Transform tr;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        tr = transform;
    }

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        MoveInputKeyboard();
#endif
        MoveInputJoystick();
        Rotat();
    }

    private void FixedUpdate()
    {

        Move();
    }

    private void MoveInputKeyboard()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        movement = new Vector2(moveX, moveY);
    }

    private void MoveInputJoystick()
    {
        float moveX = JoystickMove.Horizontal;
        float moveY = JoystickMove.Vertical;
        movement = new Vector2(moveX, moveY);
    }

    private void Rotat()
    {
        if (movement.x > 0)
            tr.rotation = Quaternion.Euler(0f, 0f, 0f);
        if (movement.x < 0)
            tr.rotation = Quaternion.Euler(0f, 180f, 0f);
    }

    private void Move()
    {
        rb.MovePosition(rb.position + movement * Speed * Time.fixedDeltaTime);
        //rb.velocity = movement * Speed * 10 * Time.fixedDeltaTime;
    }
}
