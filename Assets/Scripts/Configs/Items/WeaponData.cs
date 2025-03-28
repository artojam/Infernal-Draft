using System.Collections;
using UnityEngine;

public class WeaponData : ScriptableObject
{

    [field: SerializeField]
    public string nameWeapon { private set; get; } // �������� ������

    [field: SerializeField]
    public int damage { private set; get; } // ����, ��������� �������

    [field: SerializeField]
    public float range { private set; get; } // ��������� �������� ������

    [field: SerializeField]
    public Sprite sprite { private set; get; } // ������ ������

    public bool isAttack = false;

    public virtual IEnumerator Attack(Transform _shotPoint, Vector2 dir) => null;
}