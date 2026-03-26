using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SpeedBoots : ItemBase
{
    public override void OnEquip()
    {
    }

    public override void LevelUp()
    {
        level++;
        var data = Wild.Item.LevelData.LevelDataMap["202_" + level];
        PlayerMovement.SpeedPercent += data.Damage;
    }
}