using UnityEngine;

public class PlayerWeaponRotation : MonoBehaviour
{
    [SerializeField]
    private Transform TransformWeaponPoint;
    [field: SerializeField]
    public Joystick WeaponRotationJoystick { private set; get; }

    private Transform tr;

    private void Start()
    {
        tr = transform;
    }

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        //RotateWeaponToMouse();
#endif
        RotateWeaponJoystick();
    }

    private void RotateWeaponToMouse()
    {
        float offset = 0f;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = (mousePosition - TransformWeaponPoint.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (angle > -90 && angle < 90)
        {
            TransformWeaponPoint.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            TransformWeaponPoint.localScale = new Vector3(1f, -1f, 1f);

        }

        TransformWeaponPoint.rotation = Quaternion.Euler(0f, 0f, angle + offset);
        
    }

    private void RotateWeaponJoystick()
    {
        float offset = 0f;

        float horizontal = WeaponRotationJoystick.Horizontal;
        float vertical = WeaponRotationJoystick.Vertical;

        if (horizontal != 0 || vertical != 0)
        {
            float angle = Mathf.Atan2(vertical, horizontal) * Mathf.Rad2Deg;

            if (angle > -90 && angle < 90)
            {
                TransformWeaponPoint.localScale = new Vector3(1f, 1f, 1f);
            }
            else
            {
                TransformWeaponPoint.localScale = new Vector3(1f, -1f, 1f);

            }

            TransformWeaponPoint.rotation = Quaternion.Euler(0f, 0f, angle + offset);
        }
    }
}
