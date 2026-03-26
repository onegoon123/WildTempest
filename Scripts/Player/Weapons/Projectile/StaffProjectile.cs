using UnityEngine;
using UnityEngine.UIElements;

public class StaffProjectile : Projectile
{
    void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime;

        LifeTimeCheck();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;

        Explosion();
        Despawn();
    }
}
