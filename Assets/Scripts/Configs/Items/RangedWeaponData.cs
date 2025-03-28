using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRangedWeapon", menuName = "Game/Weapons/Ranged Weapon")]
public class RangedWeaponData : WeaponData
{
    [field: SerializeField, Header("Дальное")] 
    public float fireRate { private set; get; } // Скорострельность оружия (выстрелов в секунду)

    [field: SerializeField]
    public int ammoCapacity { private set; get; } // Максимальное количество патронов

    [field: SerializeField]
    public float reloadTime { private set; get; } // Время перезарядки оружия

    [field: SerializeField]
    public Vector2 positionShotPoint { private set; get; }

    [field: SerializeField]
    public GameObject bullet { private set; get; }

    public override IEnumerator Attack( Transform _shotPoint, Vector2 dir)
    {
        while (isAttack)
        {
            Quaternion rot = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

            GameObject _bullet = Instantiate(bullet, _shotPoint.position, rot);
            Destroy(_bullet, range);
            yield return new WaitForSeconds(fireRate * Time.deltaTime);
        }
    }

    
}
