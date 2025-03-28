using System.Collections;
using UnityEngine;

public class WeaponData : ScriptableObject
{

    [field: SerializeField]
    public string nameWeapon { private set; get; } // Название оружия

    [field: SerializeField]
    public int damage { private set; get; } // Урон, наносимый оружием

    [field: SerializeField]
    public float range { private set; get; } // Дальность действия оружия

    [field: SerializeField]
    public Sprite sprite { private set; get; } // Спрайт оружия

    public bool isAttack = false;

    public virtual IEnumerator Attack(Transform _shotPoint, Vector2 dir) => null;
}