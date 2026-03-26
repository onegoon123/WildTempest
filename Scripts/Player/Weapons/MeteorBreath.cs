using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MeteorBreath : Poolable
{
    public Collider2D col;

    float damage;
    float knockback;
    float rate;
    float timer;
    float duration;
    float durationTimer;

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


        var data = Wild.Item.LevelData.LevelDataMap["154"];
        damage = data.Count;
        knockback = data.Knockback;
        rate = data.Speed;
        duration = data.Duration;
        durationTimer = duration;
        timer = rate;
    }

    public override void OnSpawn()
    {
        base.OnSpawn();
        durationTimer = duration;
        timer = rate;
    }

    void Update()
    {
        durationTimer -= Time.deltaTime;
        if (durationTimer < 0)
        {
            particle.Stop(true);

            if (durationTimer < -1.0f)
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
                enemy.TakeKnockback(knockback, transform.position);
            }
        }
    }
}
