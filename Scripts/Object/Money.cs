using DG.Tweening;
using System;
using UnityEngine;

public class Money : Poolable
{
    public Transform targetTransform;
    public SpriteRenderer spriteRenderer;
    public CircleCollider2D col;

    public int moneyValue;
    public float speed;
    private float speedPercent = 0.0f;
    private bool isMove = false;

    private void Awake()
    {
        col.radius *= 1.0f + DataManager.data.upgrades[5] * Wild.Player.UpgradeData.UpgradeDataMap[1006].Value;
    }

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
            GameManager.Instance.AddMoney(moneyValue);
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
