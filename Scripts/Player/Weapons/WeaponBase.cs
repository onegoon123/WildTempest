using System.Linq;
using UnityEngine;

/// <summary>
/// 게임 내 모든 무기의 추상 기본 클래스
/// 쿨다운 관리를 통한 자동 공격 시스템 구현
/// 적 탐지 및 타겟팅 유틸리티 제공
/// </summary>
public abstract class WeaponBase : ItemBase
{
    private float damage = 0;
    public static float DamagePercent = 1.0f;   // 전역 데미지 배율
    public float knockback = 0;
    public static int ProjectileCount = 0;      // 추가 투사체 개수
    public int projectileCounter = 0;
    public bool isEvolution = false;

    /// <summary>
    /// 전역 데미지 배율을 적용한 최종 데미지 값
    /// </summary>
    public float Damage
    {
        set { damage = value; }
        get { return damage * DamagePercent; }
    }

    /// <summary>
    /// 자동 무기 활성화를 위한 메인 업데이트 루프
    /// 쿨다운 관리 및 추가 투사체 로직 처리
    /// </summary>
    public override void Tick()
    {
        cooldownTimer -= Time.deltaTime;

        // 0.1초 지연으로 추가 투사체 처리
        if (0 < projectileCounter && cooldownTimer < cooldown - 0.1f)
        {
            Activate();

            if (--projectileCounter == 0)
                cooldownTimer = cooldown * (1.0f - cooldownReduction);
            else
                cooldownTimer = cooldown;

            return;
        }

        if (cooldownTimer < 0)
        {
            if (Activate())
            {
                cooldownTimer = cooldown * (1.0f - cooldownReduction);

                // 추가 투사체 처리
                if (0 < ProjectileCount)
                {
                    projectileCounter = ProjectileCount;
                    cooldownTimer = cooldown;
                }
            }
        }
    }

    /// <summary>
    /// 무기 진화를 위한 추상 메서드
    /// 각 무기는 자신만의 진화 로직을 구현
    /// </summary>
    public abstract void Evolution();

    /// <summary>
    /// 지정된 반경 내의 모든 적을 찾음
    /// 광역 공격 무기에 사용
    /// </summary>
    /// <param name="origin">탐지 중심점</param>
    /// <param name="radius">탐지 반경</param>
    /// <returns>범위 내 모든 적의 배열</returns>
    public static Enemy[] FindAllEnemies(Vector2 origin, float radius)
    {
        LayerMask enemyMask = LayerMask.GetMask("Enemy");
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, radius, enemyMask);
        Enemy[] enemies = hits.Select(hit => hit.GetComponent<Enemy>()).Where(enemy => enemy != null).ToArray();

        return enemies;
    }

    /// <summary>
    /// 지정된 반경 내의 가장 가까운 적을 찾음
    /// 타겟팅 무기에 사용
    /// </summary>
    /// <param name="origin">탐지 중심점</param>
    /// <param name="radius">탐지 반경</param>
    /// <returns>가장 가까운 적 또는 없으면 null</returns>
    public static Enemy FindNearestEnemy(Vector2 origin, float radius)
    {
        LayerMask enemyMask = LayerMask.GetMask("Enemy");
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, radius, enemyMask);

        Transform nearest = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            float dist = Vector2.Distance(origin, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = hit.transform;
            }
        }

        if (nearest == null) return null;
        return nearest.GetComponent<Enemy>();
    }

    /// <summary>
    /// 지정된 반경 내의 가장 먼 적을 찾음
    /// 먼 적을 타겟팅하는 특수 무기에 사용
    /// </summary>
    /// <param name="origin">탐지 중심점</param>
    /// <param name="radius">탐지 반경</param>
    /// <returns>가장 먼 적 또는 없으면 null</returns>
    public static Enemy FindFarthestEnemy(Vector2 origin, float radius)
    {
        LayerMask enemyMask = LayerMask.GetMask("Enemy");
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, radius, enemyMask);

        Transform farthest = null;
        float maxDist = float.MinValue;

        foreach (var hit in hits)
        {
            float dist = Vector2.Distance(origin, hit.transform.position);
            if (dist > maxDist)
            {
                maxDist = dist;
                farthest = hit.transform;
            }
        }

        if (farthest == null) return null;
        return farthest.GetComponent<Enemy>();
    }
}