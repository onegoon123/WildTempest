using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Accessory : ItemBase
{
    public override void OnEquip()
    {
    }

    public override void LevelUp()
    {
        level++;
        var data = Wild.Item.LevelData.LevelDataMap["203_" + level];
        GameManager.Instance.player.AddMaxHP(data.Count);
    }
}