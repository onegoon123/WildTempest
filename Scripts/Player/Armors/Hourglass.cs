using System.Linq;
using UnityEngine;

public class Hourglass : ItemBase
{
    public override void OnEquip()
    {
    }

    public override void LevelUp()
    {
        level++;
        var data = Wild.Item.LevelData.LevelDataMap["208_" + level];
        GameManager.Instance.player.invincibilityTime += data.Damage;
    }
}