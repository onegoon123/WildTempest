using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Grimoire : WeaponBase
{
    float duration;
    float atkRate;
    float healRate;

    public override void OnEquip()
    {
        ObjectPoolManager.Instance.Create("Grimoire", 3);
        cooldownTimer = cooldown * .2f;
    }

    public override bool Activate()
    {
        GameObject obj = ObjectPoolManager.Instance.Get(isEvolution ? "GrimoireEx" : "Grimoire", player.transform);

        obj.GetComponent<GrimoireBreath>().BreathInit(Damage, knockback, atkRate, duration, healRate);
        SoundManager.PlayLoopSFX("Explosion8", duration);
        return true;
    }

    public override void LevelUp()
    {
        level++;
        var data = Wild.Item.LevelData.LevelDataMap["107_" + level];
        Damage = data.Damage;
        cooldown = data.Cooldown;
        knockback = data.Knockback;
        duration = data.Duration;
        atkRate = data.Speed;
        healRate = data.Range;
    }

    public override void Evolution()
    {
        isEvolution = true;
        var data = Wild.Item.LevelData.LevelDataMap["157"];
        ObjectPoolManager.Instance.Create("GrimoireEx", 3);
        Damage = data.Damage;
        cooldown = data.Cooldown;
        knockback = data.Knockback;
        duration = data.Duration;
        atkRate = data.Speed;
        healRate = data.Range;
    }
}
