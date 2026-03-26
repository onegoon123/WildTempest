using System;
using Unity.VisualScripting;
using UnityEngine;

public class HomingProjectile : Projectile
{
    public bool isEvolution;

    private void Update()
    {
        if (target == null)
        {
            ObjectPoolManager.Instance.Get(explosionObj, transform.position);
            Despawn();
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.transform.position) < .1f)
        {
            if (isEvolution)
            {
                Explosion();
                Despawn();
                return;
            }
            target.TakeDamage(damage);
            target.TakeKnockback(knockback);
            ObjectPoolManager.Instance.Get(explosionObj, transform.position);
            Despawn();
        }
    }

}
