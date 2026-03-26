using DG.Tweening;
using System;
using UnityEngine;

public class Magnet : Poolable
{
    public Transform targetTransform;
    public Collider2D col;

    public float speed;
    private float speedPercent = 0.0f;
    private bool isMove = false;

    public override void OnSpawn()
    {
        base.OnSpawn();
        isMove = false;
        col.enabled = true;
    }

    private void Update()
    {
        if (!isMove) return;
        speedPercent = Math.Min(speedPercent + Time.deltaTime * 4, 1.0f);
        transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, speed * speedPercent * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetTransform.position) < .1f)
        {
            EXPOrb[] exps = FindObjectsByType<EXPOrb>(FindObjectsSortMode.None);
            foreach (var exp in exps)
            {
                exp.UseMagnet();
            }
            Money[] money = FindObjectsByType<Money>(FindObjectsSortMode.None);
            foreach (var m in money)
            {
                m.UseMagnet();
            }
            Despawn();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            targetTransform = other.transform;
            isMove = true;
            col.enabled = false;
        }
    }
}
