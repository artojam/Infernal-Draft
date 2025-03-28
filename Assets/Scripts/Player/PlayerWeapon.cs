using System.Collections;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField]
    private WeaponData data;
    
    [SerializeField]
    private SpriteRenderer spriteWeapon;
    [SerializeField]
    private Transform attackPoint;

    public PlayerWeaponRotation pwr;

    private IEnumerator attack;

    private void Start()
    {
        SetWeapon(data);
        pwr = GetComponent<PlayerWeaponRotation>();
    }


    public void SetWeapon(WeaponData _data)
    {
        data = _data;

        spriteWeapon.sprite = _data.sprite;

        switch (_data)
        {
            case RangedWeaponData ranged:
                Vector3 pos = new Vector3(ranged.positionShotPoint.x, ranged.positionShotPoint.y);
                attackPoint.localPosition = pos;
                attack = _data.Attack( attackPoint, pwr.WeaponRotationJoystick.Direction);
                break;

            case MeleeWeaponData melle:
                attackPoint.localPosition = Vector3.zero;
                attack = _data.Attack(attackPoint, pwr.WeaponRotationJoystick.Direction);
                break;
        }
    }

    public void OnJoystickRelease() // ֲûחמג טח UI (PointerUp)
    {
        data.isAttack = false;
        StopCoroutine(attack);
    }

    public void OnJoystickMove() // ֲûחמג טח UI (PointerDown)
    {
        data.isAttack = true;
        StartCoroutine(attack);
    }
}
