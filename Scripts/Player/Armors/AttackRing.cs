using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AttackRing : ItemBase
{
    public override void OnEquip()
    {
    }

    public override void LevelUp()
    {
        level++;
        var data = Wild.Item.LevelData.LevelDataMap["201_" + level];
        WeaponBase.DamagePercent += data.Damage;
    }
}