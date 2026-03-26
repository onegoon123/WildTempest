using DG.Tweening;
using UnityEngine;

public class MeteorProjectile : Projectile
{
    Transform childTransform;
    private void Awake()
    {
        childTransform = transform.GetChild(0).transform;
    }
    public override void OnSpawn()
    {
        base.OnSpawn();
        childTransform.DOKill();
        childTransform.localPosition = new Vector3(2, 10, 0);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(childTransform.DOLocalMove(Vector3.zero, 1).SetEase(Ease.InFlash));
        sequence.AppendCallback(() =>
        {
            Despawn();
            Explosion();
        });
    }

}
