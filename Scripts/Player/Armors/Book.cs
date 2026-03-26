using System.Linq;
using UnityEngine;

public class Book : ItemBase
{
    public override void OnEquip()
    {
    }

    public override void LevelUp()
    {
        level++;
        var data = Wild.Item.LevelData.LevelDataMap["206_" + level];
        Player.EXPPercent += data.Damage;
    }
}