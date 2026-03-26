using DG.Tweening;
using System;
using UnityEngine;

public class EXPOrb : Poolable
{
    public Transform targetTransform;
    public SpriteRenderer spriteRenderer;
    public CircleCollider2D col;

    public int expValue;
    public float speed;
    private float speedPercent = 0.0f;
    private bool isMove = false;

    public static float ColRadius;

    public override void OnSpawn()
    {
        base.OnSpawn();
        isMove = false;
        col.enabled = true;
        col.radius = ColRadius;
    }

    private void Update()
    {
        if (!isMove) return;
        speedPercent = Math.Min(speedPercent + Time.deltaTime * 4, 1.0f);
        transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, speed * speedPercent * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetTransform.position) < .1f)
        {
            targetTransform.GetComponent<Player>().AddEXP(expValue);
            Despawn();
        }
    }

    public void UseMagnet()
    {
        targetTransform = GameManager.Instance.player.transform;
        isMove = true;
        col.enabled = false;
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
