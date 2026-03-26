using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HeroSelector : MonoBehaviour
{
    public Animator animator;

    public GameObject raycastTarget;
    public Button playButton;
    public Button prevButton;
    public Button nextButton;
    public Button heroButton;
    public GameObject priceObj;
    public TextMeshProUGUI priceText;

    public RectTransform heroInfo;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI atkText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI weaponText;
    public Button buyButton;

    private int heroCount;
    private int currentHero;
    private int heroPirce;

    void Start()
    {
        // 캐릭터 개수 불러오기
        heroCount = Wild.Player.Hero.HeroMap.Count;
        // 현재 캐릭터 불러오기
        currentHero = DataManager.data.currentHero;

        ButtonUpdate();
        SetAnimator();
    }

    // 버튼 동기화
    private void ButtonUpdate()
    {
        prevButton.gameObject.SetActive(currentHero != 0);
        nextButton.gameObject.SetActive(heroCount != currentHero + 1);
    }
    private void SelectHero()
    {
        if (DataManager.data.hasHero[currentHero] == 0)
        {
            // 보유하고 있지 않음
            heroPirce = Wild.Player.Hero.HeroMap[currentHero].Price;    // 구매 가격 지정
            priceText.text = heroPirce.ToString();                      // 텍스트 지정
            priceObj.SetActive(true);
            playButton.interactable = false;
        }
        else
        {
            // 보유하고 있음. 캐릭터 선택
            DataManager.data.currentHero = currentHero;
            priceObj.SetActive(false);
            playButton.interactable = true;
        }
        SetAnimator();
    }
    private async void SetAnimator()
    {
        string res = Wild.Player.Hero.HeroMap[currentHero].Resource;

        var controller = await AssetManager.GetAsync<RuntimeAnimatorController>(res);

        if (controller != null)
        {
            animator.runtimeAnimatorController = controller;
        }
    }
    public void OnClickPrevButton()
    {
        currentHero--;
        SelectHero();
        ButtonUpdate();
    }
    public void OnClickNextButton()
    {
        currentHero++;
        SelectHero();
        ButtonUpdate();
    }
    public void OnClickHeroButton()
    {
        raycastTarget.SetActive(true);
        // 캐릭터 정보 표시
        var data = Wild.Player.Hero.HeroMap[currentHero];
        nameText.text = Localize.GetStr(data.Name);
        hpText.text = data.HP.ToString();
        atkText.text = (data.ATK * 100).ToString();
        speedText.text = (data.Speed * 100).ToString();
        weaponText.text = Localize.GetStr(Wild.Item.Data.DataMap[data.Weapon].itemName);

        buyButton.gameObject.SetActive(DataManager.data.hasHero[currentHero] == 0);

        heroInfo.DOKill();
        heroInfo.DOAnchorPosY(0, 0.3f);
        heroInfo.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
    public void OnClickBuyButton()
    {
        if (DataManager.data.Money < heroPirce)
        {
            PopUpManager.PopUp(Localize.GetStr("cantpay"), ()=>CustomEvent.Trigger(GameObject.Find("Canvas"), "OnCash"));
            return;
        }
        DataManager.data.hasHero[currentHero] = 1;
        DataManager.data.Money -= heroPirce;
        SelectHero();

        raycastTarget.SetActive(false);
        heroInfo.DOKill();
        heroInfo.DOAnchorPosY(-2000, 0.3f);
        heroInfo.GetComponent<CanvasGroup>().blocksRaycasts = false;

        DataManager.Save();
    }
    public void OnClickCancelButton()
    {
        raycastTarget.SetActive(false);
        heroInfo.DOKill();
        heroInfo.DOAnchorPosY(-2000, 0.3f);
        heroInfo.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
}
