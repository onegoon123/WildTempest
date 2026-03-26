using Terresquall;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class GrimoireBreath : Poolable
{
    public Collider2D col;

    float damage;
    float knockback;
    float rate;
    float timer;
    float duration;
    float healRate;
    Vector2 moveDir;

    public ParticleSystem particle;
    public LayerMask targetLayer;
    private ContactFilter2D filter;
    private Collider2D[] results = new Collider2D[20];

    private void Start()
    {
        filter = new ContactFilter2D();
        filter.SetLayerMask(targetLayer);
        filter.useLayerMask = true;
        filter.useTriggers = false;
    }

    public void BreathInit(float damage, float knockback, float rate, float duration, float healRate)
    {
        this.damage = damage;
        this.knockback = knockback;
        this.rate = rate;
        this.duration = duration;
        this.healRate = healRate;
        timer = rate;
    }

    public override void OnSpawn()
    {
        base.OnSpawn();
        duration = 0.0f;

        moveDir = GameManager.Instance.player.Movement.MoveDir.normalized;
        moveDir = GameManager.Instance.player.Movement.MoveDir.normalized;
        float rotZ = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.Euler(0, 0, rotZ - 90);
    }

    void Update()
    {
        if (GameManager.Instance.player.Movement.LastDir != Vector2.zero)
        {
            moveDir = GameManager.Instance.player.Movement.LastDir.normalized;
            float rotZ = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg - 90;
            transform.rotation = Quaternion.Euler(0, 0, rotZ - 90);
        }

        duration -= Time.deltaTime;
        if (duration < 0)
        {
            particle.Stop(true);

            if (duration < -1.0f)
                Despawn();

            return;
        }

        timer -= Time.deltaTime;
        if (timer < 0)
        {
            ApplyBreathDamage();
            timer = rate;
        }
    }

    public void ApplyBreathDamage()
    {
        int hitCount = col.Overlap(filter, results);

        for (int i = 0; i < hitCount; i++)
        {
            var enemy = results[i].GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                enemy.TakeKnockback(knockback);
            }
        }

        if (healRate > 0)
        {
            GameManager.Instance.player.TakeHeal((int)(damage * hitCount * healRate));
        }
    }
}
