using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class UISlot : MonoBehaviour
{
    ItemCode item;
    public Image backImage;
    public Image itemImage;
    public RectTransform itemParent;
    DG.Tweening.Sequence sequence;

    private void Awake()
    {
        int spriteIndex = 0;
        for (int i = 0; i < itemParent.childCount; i++)
        {
            if (spriteIndex == Wild.Item.Data.DataList.Count) spriteIndex = 0;
            Sprite sprite = AssetManager.Get<SpriteAtlas>("Atlas").GetSprite(Wild.Item.Data.DataList[spriteIndex++].icon);
            itemParent.GetChild(i).GetComponent<Image>().sprite = sprite;
        }
    }

    public void Clear()
    {
        itemParent.gameObject.SetActive(false);
        itemImage.gameObject.SetActive(false);
    }

    public void Skip()
    {
        if (sequence != null)
        {
            sequence.Kill(true);
            sequence = null;
        }

        itemImage.gameObject.SetActive(true);
        itemParent.gameObject.SetActive(false);
    }

    public void SlotStart(ItemCode item)
    {
        this.item = item;
        itemParent.gameObject.SetActive(true);
        itemParent.anchoredPosition = Vector2.zero;
        itemImage.sprite = AssetManager.Get<SpriteAtlas>("Atlas").GetSprite(Wild.Item.Data.DataMap[(int)item].icon);
        itemImage.gameObject.SetActive(false);
        Color targetColor = backImage.color;
        backImage.color = Color.clear;

        sequence = DOTween.Sequence();
        sequence.Append(itemParent.DOAnchorPosY(itemParent.rect.height, 5.0f).SetEase(Ease.Linear).SetUpdate(true));
        sequence.Join(backImage.DOColor(targetColor, .5f).SetUpdate(true));
        sequence.AppendCallback(() =>
        {
            itemImage.gameObject.SetActive(true);
            itemParent.gameObject.SetActive(false);
            
        });
        sequence.SetUpdate(true);
        sequence.Play();
    }
}
