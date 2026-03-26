using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.U2D;

public class Enemy : Poolable
{
    public SpriteRenderer spriteRenderer;
    public CircleCollider2D col;
    public CircleCollider2D atkCol;
    private Rigidbody2D rigid;
    private Player player;
    private Transform target;

    public float hp = 10;
    public int atk = 1;
    public float speed;
    float knockbackResist = 0;
    string dropItem;

    bool isBoss;

    public override void OnDespawn()
    {
        base.OnDespawn();
        rigid.simulated = true;
        enabled = true;
        spriteRenderer.DOKill();
        spriteRenderer.color = Color.white;
        transform.DOKill();

        if (isBoss)
        {
            GameManager.Instance.player.isClear = true;
            GameManager.Instance.UIManager.ClearStage();
            isBoss = false;
        }
    }

    public void TakeDamage(float damage)
    {
        if (hp <= 0)
        {
            return;
        }

        hp -= damage;
        GameManager.Instance.UIManager.ShowDamage(transform.position, damage);

        if (hp <= 0)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(spriteRenderer.DOColor(Color.red, .1f).SetEase(Ease.OutQuad));
            sequence.Append(spriteRenderer.DOColor(Color.clear, .3f));
            sequence.AppendCallback(Despawn);
            sequence.Play();
            Drop();
            rigid.simulated = false;
            enabled = false;
        }
        else
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(spriteRenderer.DOColor(Color.red, .1f).SetEase(Ease.OutQuad));
            sequence.Append(spriteRenderer.DOColor(Color.white, .1f));
            sequence.Play();
        }
    }

    public void TakeKnockback(float power)
    {
        if (1.0f <= knockbackResist) return;
        power *= 1.0f - knockbackResist;
        float knockbackTime = 0.1f * (1.0f - knockbackResist);
        Vector3 dir = transform.position - player.transform.position;
        dir.Normalize();

        Vector3 pos = transform.position + (dir * power);

        transform.DOMove(pos, knockbackTime).SetEase(Ease.OutQuad);
    }
    public void TakeKnockback(float power, Vector3 atkPos)
    {
        if (1.0f <= knockbackResist) return;
        power *= 1.0f - knockbackResist;
        float knockbackTime = 0.1f * (1.0f - knockbackResist);

        Vector3 dir = transform.position - atkPos;
        dir.Normalize();

        Vector3 pos = transform.position + (dir * power);

        transform.DOMove(pos, knockbackTime).SetEase(Ease.OutQuad);
    }

    // 아이템 드랍
    void Drop()
    {
        // 확정 드랍
        ObjectPoolManager.Instance.Get(dropItem, transform.position);

        // 랜덤 드랍
        foreach (var drop in Wild.Enemy.Drop.DropList)
        {
            if (UnityEngine.Random.value < drop.DropRate)
            {
                GameObject obj = ObjectPoolManager.Instance.Get(drop.ItemName, transform.position);
                Vector3 randomDirection = UnityEngine.Random.insideUnitCircle.normalized * .5f; // 반지름 2의 랜덤 방향
                Vector3 targetPosition = obj.transform.position + new Vector3(randomDirection.x, 0, 0);

                obj.transform.DOJump(targetPosition, jumpPower: .5f, numJumps: 1, duration: 0.5f).SetEase(Ease.OutCubic);
            }
        }
    }

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        target = GameObject.FindWithTag("Player").transform;
        player = target.GetComponent<Player>();
    }

    public void EnemyInit(int id)
    {
        var data = Wild.Enemy.Data.DataMap[id];
        spriteRenderer.sprite = AssetManager.Get<SpriteAtlas>("EnemySprite").GetSprite(data.Sprite);
        hp = data.HP;
        atk = data.ATK;
        speed = data.Speed;
        col.radius = data.ColRadius;
        col.offset = Vector2.up * data.ColOffset;
        atkCol.radius = data.ColRadius;
        atkCol.offset = Vector2.up * data.ColOffset;
        dropItem = data.DropItem;
        knockbackResist = data.KnockbackResist;
        transform.localScale = Vector3.one * data.Scale;
        if (data.EnemyCode == 499)
        {
            isBoss = true;
        }
    }

    void FixedUpdate()
    {
        if (!target) return;

        Vector2 dirVec = target.position - transform.position;
        Vector2 moveVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + moveVec);
        rigid.linearVelocity = Vector2.zero;

        spriteRenderer.flipX = dirVec.x > 0;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player.TakeDamage(atk);
        }
    }

}
