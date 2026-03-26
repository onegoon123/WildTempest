using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class ItemPopup : MonoBehaviour
{
    public static ItemPopup Instance { get; private set; }

    public RectTransform popupTransform;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemInfoText;
    public TextMeshProUGUI levelText;
    public Image iconImage;

    private Queue<Tuple<ItemCode, int>> queue = new(3);

    bool wait = false;

    public void AddItem(ItemCode code)
    {
        AddItem(code, GameManager.Instance.player.Inventory.GetItemLevel(code)+1);
    }
    public void AddItem(ItemCode code, int level)
    {
        if (code == ItemCode.Money) return;
        queue.Enqueue(Tuple.Create(code, level));
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (wait) return;

        if (queue.Count > 0)
            Popup();
    }

    private void Popup()
    {
        wait = true;
        Tuple<ItemCode, int> item = queue.Dequeue();

        ItemDataSetting(item.Item1, item.Item2);

        Sequence sequence = DOTween.Sequence();
        sequence.SetUpdate(true);
        sequence.Append(popupTransform.DOAnchorPosX(0, 0.5f).SetEase(Ease.OutQuart).SetUpdate(true));
        sequence.Insert(1.5f, popupTransform.DOAnchorPosX(-600, 0.5f).SetEase(Ease.InQuart).SetUpdate(true));
        sequence.AppendCallback(()=> wait = false);
    }

    private void ItemDataSetting(ItemCode itemCode, int level)
    {
        // 1. 아이템 데이터 받아오기
        var data = Wild.Item.Data.DataMap[(int)itemCode];

        // 2. 아이템 이름 지정
        itemNameText.text = Localize.GetStr(data.itemName);

        // 3. 아이템 이미지 설정
        iconImage.sprite = AssetManager.Get<SpriteAtlas>("Atlas").GetSprite(data.icon);

        // 포션, 돈인 경우
        if (ItemCode.Potion <= itemCode)
        {
            itemInfoText.text = Localize.GetStr(data.ItemCode.ToString());
            levelText.text = "";
            return;
        }

        // 4. 아이템 레벨 확인 및 데이터 받아오기
        Wild.Item.LevelData levelData;

        if (ItemCode.UpStart < itemCode && itemCode < ItemCode.UpEnd)
        {
            // 진화인 경우
            levelData = Wild.Item.LevelData.LevelDataMap[((int)itemCode).ToString()];
            levelText.text = Localize.GetStr("evo");
        }
        else
        {
            // 일반 아이템인 경우
            levelData = Wild.Item.LevelData.LevelDataMap[(int)itemCode + "_" + level];

            // 5. 레벨 텍스트 지정
            levelText.text = "Level ";
            if (level >= data.MaxLevel)
                levelText.text += "Max";
            else
                levelText.text += level.ToString();
        }

        // 6. 레벨별 아이템 설명 텍스트 지정
        itemInfoText.text = Localize.GetStr(levelData.levelCode.ToString());
    }
}
