using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Meteor : WeaponBase
{
    float scale = 1.0f;
    const float rangeMin = 1.5f;
    const float rangeMax = 6.0f;
    public override void OnEquip()
    {
        ObjectPoolManager.Instance.Create("Meteor", 2);
        ObjectPoolManager.Instance.Create("Meteor Explosion", 4);
    }

    public override bool Activate()
    {
        Vector2 spawnDir = Random.insideUnitCircle.normalized;
        float dist = Random.Range(rangeMin, rangeMax);
        Vector2 spawnPos = (Vector2)player.transform.position + spawnDir * dist;

        MeteorProjectile meteor = ObjectPoolManager.Instance.Get(isEvolution ? "MeteorEx" : "Meteor", spawnPos).GetComponent<MeteorProjectile>();
        meteor.transform.localScale = Vector3.one * scale;
        meteor.SetExplosionScale(scale * 0.5f);
        meteor.ProjectileInit(Damage, knockback, 0, 2.0f * scale);
        return true;
    }

    public override void LevelUp()
    {
        level++;
        var data = Wild.Item.LevelData.LevelDataMap["104_" + level];
        Damage = data.Damage;
        cooldown = data.Cooldown;
        knockback = data.Knockback;
        scale = data.Range;
    }

    public override void Evolution()
    {
        isEvolution = true;
        ObjectPoolManager.Instance.Create("MeteorEx", 4);
        ObjectPoolManager.Instance.Create("MeteorEx Explosion", 8);
        var data = Wild.Item.LevelData.LevelDataMap["154"];
        Damage = data.Damage;
        cooldown = data.Cooldown;
        knockback = data.Knockback;
        scale = data.Range;
    }
}
