using DG.Tweening;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class TripleLightning : WeaponBase
{
    float duration;
    float scale = 1;
    float speed = 360.0f;

    Vector3[] posVectors = {
            new Vector3(-1.3f, 0.75f),
            new Vector3(1.3f, 0.75f),
            new Vector3(0.0f, -1.5f)
    };

    public override void OnEquip()
    {
        ObjectPoolManager.Instance.Create("Triple Lightning", 2);
        cooldownTimer = cooldown * .2f;
    }

    public override bool Activate()
    {
        TripleLightningObject skillObject = ObjectPoolManager.Instance.Get(isEvolution ? "Triple LightningEx" : "Triple Lightning", player.transform, new Vector3(0, 0.2f)).GetComponent<TripleLightningObject>();
        skillObject.transform.localScale = Vector3.zero;
        skillObject.transform.DOScale(Vector3.one * scale, 0.5f).SetEase(Ease.InOutQuad);
        skillObject.Init(Damage, speed, duration);
        return true;
    }

    public override void LevelUp()
    {
        level++;
        var data = Wild.Item.LevelData.LevelDataMap["103_"+level];
        Damage = data.Damage;
        cooldown = data.Cooldown;
        knockback = data.Knockback;
        duration = data.Duration;
        scale = data.Range;
        speed = data.Speed;
    }

    public override void Evolution()
    {
        isEvolution = true;
        ObjectPoolManager.Instance.Create("Triple LightningEx", 3);
        var data = Wild.Item.LevelData.LevelDataMap["153"];
        Damage = data.Damage;
        cooldown = data.Cooldown;
        knockback = data.Knockback;
        duration = data.Duration;
        scale = data.Range;
        speed = data.Speed;
    }
}
