using UnityEngine;
using UnityEngine.UIElements;

public abstract class Projectile : Poolable
{
    public string explosionObj = "";

    protected Enemy target = null;
    protected float damage;
    protected float knockback;
    protected float speed;
    protected float explosionRange;
    float explosionScale = 0.5f;

    float lifeTime = 0.0f;

    public void ProjectileInit(float damage, float kncokback, float speed = 0, float explosionRange = 0)
    {
        this.damage = damage;
        this.knockback = kncokback;
        this.speed = speed;
        this.explosionRange = explosionRange;
    }
    public void ProjectileInit(Enemy target, float damage, float knockback, float speed = 0, float explosionRange = 0)
    {
        this.target = target;
        this.damage = damage;
        this.knockback = knockback;
        this.speed = speed;
        this.explosionRange = explosionRange;
    }
    public void SetExplosionScale(float scale) => explosionScale = scale;
    public override void OnSpawn()
    {
        base.OnSpawn();
        lifeTime = 0.0f;
    }

    protected void LifeTimeCheck()
    {
        lifeTime += Time.deltaTime;

        if (lifeTime > 3.0f)
        {
            Despawn();
        }
    }
    protected void Explosion()
    {
        // 이펙트 생성
        GameObject exObj = ObjectPoolManager.Instance.Get(explosionObj, transform.position);
        exObj.transform.localScale = Vector3.one * explosionScale;

        // 공격
        Enemy[] targets = WeaponBase.FindAllEnemies(transform.position, explosionRange);
        foreach (Enemy target in targets)
        {
            target.TakeDamage(damage);
            if (0 < knockback)
                target.TakeKnockback(knockback, transform.position);
        }
    }
}
