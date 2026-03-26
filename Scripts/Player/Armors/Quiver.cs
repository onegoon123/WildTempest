using System.Linq;
using UnityEngine;

public class Quiver : ItemBase
{
    public override void OnEquip()
    {
    }

    public override void LevelUp()
    {
        level++;
        var data = Wild.Item.LevelData.LevelDataMap["205_" + level];
        cooldownReduction += data.Damage;
    }
}