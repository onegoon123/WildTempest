using UnityEngine;

public class DamageTrigger : Poolable
{
    public string damageSFX;
    public float damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;

        if (damageSFX != string.Empty)
        {
            SoundManager.PlaySFX(damageSFX);
        }
        Enemy enemy = collision.GetComponent<Enemy>();
        enemy.TakeDamage(damage);
    }
}
