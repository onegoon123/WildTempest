using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Sword : WeaponBase
{
    float speed;
    float range;

    public override void OnEquip()
    {
        ObjectPoolManager.Instance.Create("Sword Effect", 4);
    }

    public override void Tick()
    {
        base.Tick();
    }

    public override bool Activate()
    {
        Vector3 moveDir = player.Movement.LastDir.normalized;
        GameObject obj = ObjectPoolManager.Instance.Get(isEvolution ? "SwordEx Effect" : "Sword Effect", player.transform.position + moveDir * 1.5f * range);
        float rotZ = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        obj.transform.rotation = Quaternion.Euler(0, 0, rotZ - 90);

        Enemy[] targets = FindAllEnemies(player.transform.position + moveDir * 1.2f * range, 1.3f * range);
        foreach (Enemy target in targets)
        {
            target.TakeDamage(Damage);
            target.TakeKnockback(knockback);
        }

        if (!isEvolution) return true;

        GameObject projectileObj = ObjectPoolManager.Instance.Get("SwordEx Projectile", player.transform.position + moveDir * 2.5f);
        projectileObj.transform.rotation = Quaternion.Euler(0, 0, rotZ - 90);
        projectileObj.GetComponent<SwordProjectile>().ProjectileInit(Damage, knockback, speed);
        return true;
    }

    public override void LevelUp()
    {
        level++;
        var data = Wild.Item.LevelData.LevelDataMap["102_"+level];
        Damage = data.Damage;
        cooldown = data.Cooldown;
        range = data.Range;
        knockback = data.Knockback;
    }

    public override void Evolution()
    {
        isEvolution = true;
        ObjectPoolManager.Instance.Create("SwordEx Effect", 16);
        ObjectPoolManager.Instance.Create("SwordEx Projectile", 16);
        var data = Wild.Item.LevelData.LevelDataMap["152"];
        Damage = data.Damage;
        cooldown = data.Cooldown;
        range = data.Range;
        knockback = data.Knockback;
        speed = data.Speed;
    }
}
