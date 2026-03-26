using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangProjectile : Poolable
{
    public string damageSFX;
    float damage;
    float knockback;
    float speed;
    float speedPercent = .0f;
    bool isTurnBack = false;

    Dictionary<Collider2D, float> lastHitTime = new();

    public float hitCooldown = 0.3f;

    public void ProjectileInit(Vector3 target, float damage, float knockback, float scale, float duration, float speed)
    {
        this.damage = damage;
        this.knockback = knockback;
        this.speed = speed;
        speedPercent = .0f;

        transform.position = transform.position + Vector3.up * .2f;
        transform.localScale = Vector3.one * scale;
        transform.rotation = Quaternion.identity;
        isTurnBack = false;

        float targetTime = Vector2.Distance(target, transform.position) * 0.3f;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(target, targetTime).SetEase(Ease.OutQuad));
        sequence.InsertCallback(duration + targetTime, ()=> {
            isTurnBack = true;
        });

        lastHitTime.Clear();
    }

    private void Update()
    {
        transform.Rotate(Vector3.forward * 1000.0f * Time.deltaTime);

        if (!isTurnBack) return;

        speedPercent = Mathf.Min(speedPercent + Time.deltaTime * 2.0f, 1.0f);
        transform.position = Vector3.MoveTowards(transform.position, GameManager.Instance.player.transform.position, speed * speedPercent * Time.deltaTime);
        if (Vector2.Distance(transform.position, GameManager.Instance.player.transform.position) < 0.1f)
            Despawn();

    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        float now = Time.time;

        if (!lastHitTime.ContainsKey(other) || now - lastHitTime[other] > hitCooldown)
        {
            lastHitTime[other] = now;
            Enemy enemy = other.GetComponent<Enemy>();
            enemy.TakeDamage(damage);

            if (0 < knockback)
                enemy.TakeKnockback(knockback, transform.position);


            if (damageSFX != string.Empty)
            {
                SoundManager.PlaySFX(damageSFX);
            }
        }
    }
}
