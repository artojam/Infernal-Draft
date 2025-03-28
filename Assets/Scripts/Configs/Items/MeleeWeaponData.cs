using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugDrawHelper
{
    public static void DrawArc(Vector3 position, Vector3 direction, float radius, float angle, Color color, float duration = 1f)
    {
        int segments = 36; // Чем больше, тем плавнее дуга
        float angleStep = angle / segments;
        float startAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - angle / 2;

        Vector3 prevPoint = position + Quaternion.Euler(0, 0, startAngle) * Vector3.right * radius;

        for (int i = 1; i <= segments; i++)
        {
            float currentAngle = startAngle + i * angleStep;
            Vector3 newPoint = position + Quaternion.Euler(0, 0, currentAngle) * Vector3.right * radius;

            Debug.DrawLine(prevPoint, newPoint, color, duration); // Отрисовка дуги
            prevPoint = newPoint;
        }

        // Границы дуги
        Debug.DrawRay(position, Quaternion.Euler(0, 0, startAngle) * Vector3.right * radius, color, duration);
        Debug.DrawRay(position, Quaternion.Euler(0, 0, startAngle + angle) * Vector3.right * radius, color, duration);
    }
}



[CreateAssetMenu(fileName = "NewMeleeWeapon", menuName = "Game/Weapons/Melee Weapon")]
public class MeleeWeaponData : WeaponData
{
    
    [field: SerializeField, Header("Ближнее")] 
    public float attackSpeed { private set; get; } // Скорость атаки для ближнего боя (ударов в секунду)

    [field: SerializeField]
    public float attackRadius { private set; get; }

    [field: SerializeField]
    public float attackAngle { private set; get; }

    public LayerMask enemyLayer;

    public override IEnumerator Attack(Transform attackPoint, Vector2 dir)
    {
        Debug.Log("start");
        while (isAttack)
        {
            Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemyLayer);
            DebugDrawHelper.DrawArc(attackPoint.position, dir, attackRadius, attackAngle, Color.green);

            foreach (Collider2D enemy in enemies)
            {
                // Вектор к врагу
                Vector2 toEnemy = (enemy.transform.position - attackPoint.position).normalized;

                // Угол между направлением атаки и врагом
                float angle = Vector2.Angle(dir, toEnemy);

                // Если враг в полукруге (±90°)
                if (angle <= attackAngle / 2)
                {
                    Debug.Log("dedeckt");
                }
            }

            yield return new WaitForSeconds(attackSpeed);
        }
    }

}
