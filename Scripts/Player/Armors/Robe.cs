using System.Linq;
using UnityEngine;

public class Robe : ItemBase
{
    public override void OnEquip()
    {
    }

    public override void LevelUp()
    {
        level++;
        var data = Wild.Item.LevelData.LevelDataMap["204_" + level];
        Player.RegenHP += (int)data.Damage;
    }
}