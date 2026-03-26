using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 파이어볼 무기 - 호밍 투사체를 가진 원거리 무기
/// </summary>
public class Fireball : WeaponBase
{
    float range;
    float speed;
    
    /// <summary>
    /// 무기 장착 시 객체 풀 초기화
    /// </summary>
    public override void OnEquip()
    {
        ObjectPoolManager.Instance.Create("Fireball", 3);
        ObjectPoolManager.Instance.Create("Fireball Explosion", 6);
    }

    /// <summary>
    /// 가장 가까운 적을 찾아 호밍 투사체 발사
    /// </summary>
    public override bool Activate()
    {
        Enemy target = WeaponBase.FindNearestEnemy(player.transform.position, range);
        if (target == null) return false;

        GameObject obj = ObjectPoolManager.Instance.Get(isEvolution ? "FireballEx" : "Fireball",
        player.transform.position);
        HomingProjectile projectile = obj.GetComponent<HomingProjectile>();
        
        projectile.ProjectileInit(target, Damage, knockback, speed, 1);
        projectile.isEvolution = isEvolution;

        return true;
    }

    /// <summary>
    /// 무기 스탯 레벨업
    /// </summary>
    public override void LevelUp()
    {
        level++;
        var data = Wild.Item.LevelData.LevelDataMap["101_" + level];
        Damage = data.Damage;
        cooldown = data.Cooldown;
        knockback = data.Knockback;
        range = data.Range;
        speed = data.Speed;
    }
    
    /// <summary>
    /// 무기 진화 - 새로운 객체 풀 생성 및 스탯 업데이트
    /// </summary>
    public override void Evolution()
    {
        isEvolution = true;
        ObjectPoolManager.Instance.Create("FireballEx", 5);
        ObjectPoolManager.Instance.Create("FireballEx Explosion", 8);
        var data = Wild.Item.LevelData.LevelDataMap["151"];
        Damage = data.Damage;
        cooldown = data.Cooldown;
        knockback = data.Knockback;
        range = data.Range;
        speed = data.Speed;
    }
}
