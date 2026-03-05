using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    public int damage = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();

            if (enemy != null)
                enemy.TakeDamage(damage);
            BossEnemy boss = other.GetComponent<BossEnemy>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
            }
            Medusa medusa = other.GetComponent<Medusa>();

            if (medusa != null)
                medusa.TakeDamage(damage);
        }
    }
}
