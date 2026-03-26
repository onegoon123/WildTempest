using DG.Tweening;
using System;
using UnityEngine;

public class TripleLightningObject : Poolable
{
    public DamageTrigger[] triggers;

    private float speed;
    private float duration;

    float timer;
    bool isEnd = false;

    public void Init(float damage, float speed, float duration)
    {
        transform.rotation = Quaternion.identity;
        timer = 0;
        isEnd = false;
        this.speed = speed;
        this.duration = duration;

        foreach (var trigger in triggers)
        {
            trigger.damage = damage;
        }
    }

    private void Update()
    {
        transform.Rotate(0, 0, speed * Time.deltaTime);
        if (isEnd) return;

        timer += Time.deltaTime;
        if (duration <= timer)
        {
            isEnd = true;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InQuad));
            sequence.AppendCallback(() =>
            {
                Despawn();
            });
        }
    }

}
