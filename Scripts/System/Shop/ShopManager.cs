using System;
using DG.Tweening;
using GoogleMobileAds.Api;
using GooglePlayGames;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{

    public static ShopManager Instance { get; private set; }

    public TextMeshProUGUI nameText;    // 업그레이드 이름
    public TextMeshProUGUI infoText;    // 업그레이드 정보
    public TextMeshProUGUI priceText;   // 업그레이드 가격
    public TextMeshProUGUI upText;      // 업그레이드 시 상승하는 스텟 정보
    public CanvasGroup lowerGroup;
    public Button buyButton;
    UpgradeItem selectItem;
    int price = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        DataManager.data.onMoneyChangeEvent += MoneyUpdate;
    }

    void OnDestroy()
    {
        DataManager.data.onMoneyChangeEvent -= MoneyUpdate;
    }

    public void Select(UpgradeItem item)
    {
        if (selectItem != null && selectItem != item)
        {
            selectItem.Cancel();
        }
        else
        {
            lowerGroup.blocksRaycasts = true;
            lowerGroup.transform.DOMoveX(0, .3f);
        }
        selectItem = item;
        int lv = selectItem.upgradeLevel;

        var data = Wild.Player.UpgradeData.UpgradeDataMap[item.upgradeID];
        nameText.text = Localize.GetStr(data.Name);
        infoText.text = Localize.GetStr(data.Name+"_Info");
        price = data.Price * (lv == 0 ? 1 : (int)(lv * data.UpPrice));
        priceText.text = price.ToString();

        buyButton.interactable = price <= DataManager.data.Money;

        if (lv == data.MaxLevel)
        {
            buyButton.interactable = false;
            switch (selectItem.upgradeID)
            {
                case 1001:
                    upText.text = (lv * data.Value).ToString();
                    break;
                case 1005:
                    upText.text = $"{lv * data.Value}/sec";
                    break;
                case 1003:
                    upText.text = Math.Round(lv * data.Value * 100).ToString() + "%";
                    break;
                case 1002:
                case 1004:
                case 1006:
                case 1007:
                case 1008:
                    upText.text = Math.Round((1.0 + lv * data.Value) * 100).ToString() + "%";
                    break;
            }
            return;
        }

        switch (selectItem.upgradeID)
        {
            case 1001:
                upText.text = (lv * data.Value).ToString() + " → " + (100 + (lv + 1) * data.Value).ToString();
                break;
            case 1005:
                upText.text = $"{lv * data.Value}/sec → {(lv + 1) * data.Value}/sec";
                break;
            case 1003:
                upText.text = Math.Round(lv * data.Value * 100).ToString() + "% → " + Math.Round((lv + 1) * data.Value * 100).ToString() + "%";
                break;
            case 1002:
            case 1004:
            case 1006:
            case 1007:
            case 1008:
                upText.text = Math.Round((1.0 + lv * data.Value) * 100).ToString() + "% → " + Math.Round((1.0 + (lv + 1) * data.Value) * 100).ToString() + "%";
                break;
        }
    }

    public void Buy()
    {
        SoundManager.PlaySFX("Menu_Select_00");
        selectItem.Upgrade();
        DataManager.data.upgrades[selectItem.upgradeID - 1001]++;
        DataManager.data.Money -= price;
        Select(selectItem);
        DataManager.Save();
        if (PlayGamesPlatform.Instance.IsAuthenticated())
            PlayGamesPlatform.Instance.UnlockAchievement(GPGSIds.achievement_3);
    }

    public void Purchase(string productID)
    {
        IAPManager.Instance.Purchase(productID);
    }

    public void MoneyUpdate(int value)
    {
        if (selectItem != null)
        {
            buyButton.interactable = price <= DataManager.data.Money;
        }
    }

    public void OnAdButtonClick()
    {
        AdMobManager.Instance.ShowRewardedAd((Reward reward) =>
        {
            DataManager.data.Money += 1000;
            DataManager.Save();
        });
    }
}
