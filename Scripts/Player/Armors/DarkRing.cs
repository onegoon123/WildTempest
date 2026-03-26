using System.Linq;
using UnityEngine;

public class DarkRing : ItemBase
{
    public override void OnEquip()
    {
    }

    public override void LevelUp()
    {
        level++;
        var data = Wild.Item.LevelData.LevelDataMap["207_" + level];
        WeaponBase.ProjectileCount = data.Count;
    }
}