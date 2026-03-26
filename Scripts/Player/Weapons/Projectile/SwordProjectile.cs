using UnityEngine;
using UnityEngine.UIElements;

public class SwordProjectile : Projectile
{
    public override void OnSpawn()
    {
        base.OnSpawn();
    }

    void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime;
        LifeTimeCheck();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;

        // АјАн
        Enemy target = collision.GetComponent<Enemy>();
        if (target)
        {
            target.TakeDamage(damage);
            target.TakeKnockback(knockback);
        }
    }
}
