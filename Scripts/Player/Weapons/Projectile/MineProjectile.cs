using DG.Tweening;
using UnityEngine;

public class MineProjectile : Projectile
{
    private float timer = 3.0f;
    public override void OnSpawn()
    {
        base.OnSpawn();
        timer = 3.0f;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0f)
        {
            Explosion();
            Despawn();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;

        Explosion();
        Despawn();
    }
}
