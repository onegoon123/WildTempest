using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class FrostMine : WeaponBase
{
    private float range;
    public override void OnEquip()
    {
        ObjectPoolManager.Instance.Create("Frost", 4);
        ObjectPoolManager.Instance.Create("Frost Explosion", 6);
    }

    public override bool Activate()
    {
        MineProjectile mine = ObjectPoolManager.Instance.Get(isEvolution ? "FrostEX" : "Frost", player.transform.position).GetComponent<MineProjectile>();
        mine.ProjectileInit(Damage, knockback, 0, range);
        mine.SetExplosionScale(range * .5f);
        return true;
    }

    public override void LevelUp()
    {
        level++;
        var data = Wild.Item.LevelData.LevelDataMap["108_" + level];
        Damage = data.Damage;
        cooldown = data.Cooldown;
        knockback = data.Knockback;
        range = data.Range;
    }

    public override void Evolution()
    {
        isEvolution = true;
        ObjectPoolManager.Instance.Create("FrostEX", 8);
        ObjectPoolManager.Instance.Create("FrostEX Explosion", 16);
        var data = Wild.Item.LevelData.LevelDataMap["158"];
        Damage = data.Damage;
        cooldown = data.Cooldown;
        knockback = data.Knockback;
        range = data.Range;
    }
}
