using DG.Tweening;
using System;
using UnityEngine;

public class Treasure : Poolable
{
    public int Count;
    private Transform arrow;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TreasureManager.Instance.TreasureOpen(Count);
            Despawn();
        }
    }

    private void Awake()
    {
        arrow = transform.GetChild(0);
        arrow.gameObject.SetActive(false);
    }

    public float edgeOffset = 100.0f;
    public float angleOffset = 100.0f;
    private void FixedUpdate()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        bool isOffScreen =
        screenPos.z <= 0 ||
        screenPos.x < 0 || screenPos.x > Screen.width ||
        screenPos.y < 0 || screenPos.y > Screen.height;

        arrow.gameObject.SetActive(isOffScreen);
        if (isOffScreen)
        {
            // 플레이어에서 부터 보물 방향 벡터 계산
            Vector3 dir = (transform.position - GameManager.Instance.player.transform.position).normalized;

            // 방향에 따라 위치를 플레이어 근처에 고정
            Vector3 arrowWorldPos = GameManager.Instance.player.transform.position + dir * edgeOffset;
            arrow.position = new Vector3(arrowWorldPos.x, arrowWorldPos.y, arrow.position.z);

            // 방향을 회전 (Z축 기준)
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            arrow.rotation = Quaternion.Euler(0f, 0f, angle + angleOffset);
        }
    }
}
