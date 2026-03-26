using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Staff : WeaponBase
{
    float range;
    float speed;
    int count;

    public override void OnEquip()
    {
        ObjectPoolManager.Instance.Create("Staff Projectile", 20);
        ObjectPoolManager.Instance.Create("Staff Explosion", 40);
    }

    public override void Tick()
    {
        base.Tick();
    }

    public override bool Activate()
    {
        Enemy target = WeaponBase.FindNearestEnemy(player.transform.position, 4.0f);
        if (target == null) return false;

        SoundManager.PlaySFX("Plasma3");
        Vector2 targetDir = target.transform.position - player.transform.position;
        targetDir.Normalize();

        float baseAngle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        // 총 부채꼴 각도
        float totalAngle = isEvolution ? 360-45 : 45.0f;
        float startAngle = baseAngle - totalAngle / 2f;

        for (int i = 0; i < count; i++)
        {
            // i번째 이펙트의 회전 각도
            float t = (count == 1) ? 0.5f : (float)i / (count - 1); // 0 ~ 1
            float angle = startAngle + t * totalAngle;

            // 이펙트 생성
            GameObject obj = ObjectPoolManager.Instance.Get("Staff Projectile", player.transform.position);
            obj.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
            StaffProjectile projectile = obj.GetComponent<StaffProjectile>();
            projectile.ProjectileInit(Damage, knockback, speed, range);
            //projectile.SetExplosionScale(1.0f);
        }
        return true;
    }

    public override void LevelUp()
    {
        level++;
        var data = Wild.Item.LevelData.LevelDataMap["106_"+level];
        Damage = data.Damage;
        cooldown = data.Cooldown;
        knockback = data.Knockback;
        speed = data.Speed;
        count = data.Count;
        range = data.Range;
    }

    public override void Evolution()
    {
        isEvolution = true;
        var data = Wild.Item.LevelData.LevelDataMap["156"];
        Damage = data.Damage;
        cooldown = data.Cooldown;
        knockback = data.Knockback;
        speed = data.Speed;
        count = data.Count;
        range = data.Range;
    }
}
