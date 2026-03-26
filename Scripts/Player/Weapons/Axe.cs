using Unity.VisualScripting;
using UnityEngine;

public class Axe : WeaponBase
{
    float scale;
    float duration;
    float speed;
    public override void OnEquip()
    {
        ObjectPoolManager.Instance.Create("Axe", 6);
    }

    public override bool Activate()
    {
        Enemy target = WeaponBase.FindFarthestEnemy(player.transform.position, 3.0f);
        if (target == null) return false; 

        GameObject obj = ObjectPoolManager.Instance.Get(isEvolution ? "AxeEx" : "Axe", player.transform.position);
        obj.GetComponent<BoomerangProjectile>().ProjectileInit(target.transform.position, Damage, knockback, scale, duration, speed);
        return true;
    }

    public override void LevelUp()
    {
        level++;
        var data = Wild.Item.LevelData.LevelDataMap["105_"+level];
        Damage = data.Damage;
        cooldown = data.Cooldown;
        knockback = data.Knockback;
        scale = data.Range;
        duration = data.Duration;
        speed = data.Speed;
    }

    public override void Evolution()
    {
        isEvolution = true;
        ObjectPoolManager.Instance.Create("AxeEx", 6);
        var data = Wild.Item.LevelData.LevelDataMap["155"];
        Damage = data.Damage;
        cooldown = data.Cooldown;
        knockback = data.Knockback;
        scale = data.Range;
        duration = data.Duration;
        speed = data.Speed;
    }
}
