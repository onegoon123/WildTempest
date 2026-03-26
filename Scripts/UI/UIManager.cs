using System;
using DG.Tweening;
using GooglePlayGames;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

/// <summary>
/// 게임 인터페이스를 위한 메인 UI 매니저
/// UI 업데이트, 데미지 표시, 게임 상태 관리 처리
/// </summary>
public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Player player;
    [SerializeField]
    private LevelUpManager levelUpManager;
    [SerializeField]
    private UIInventory inventory;
    [SerializeField]
    private Slider expSlider;
    [SerializeField]
    private TextMeshProUGUI moneyText;

    /// <summary>
    /// 부드러운 애니메이션으로 경험치 바 업데이트
    /// </summary>
    /// <param name="percent">경험치 비율 (0.0 ~ 1.0)</param>
    public void SetEXPValue(float percent)
    {
        expSlider.DOKill();
        expSlider.DOValue(percent, 0.5f).SetEase(Ease.OutQuad);
    }

    /// <summary>
    /// 레벨업 시퀀스 트리거 및 아이템 선택
    /// </summary>
    /// <param name="percent">현재 경험치 비율</param>
    public void LevelUp(float percent)
    {
        CustomEvent.Trigger(gameObject, "LevelUp", percent);
        levelUpManager.RandomItem();
    }

    int beforeMoney = 0;
    
    /// <summary>
    /// 카운터 애니메이션으로 돈 표시 업데이트
    /// </summary>
    /// <param name="money">표시할 돈</param>
    public void SetMoney(int money)
    {
        moneyText.DOKill(true);
        moneyText.DOCounter(beforeMoney, money, .5f);
        beforeMoney = money;
    }

    /// <summary>
    /// 레벨업 메뉴에서 아이템 선택 처리
    /// </summary>
    /// <param name="index">선택한 아이템 인덱스</param>
    public void SelectItem(int index)
    {
        levelUpManager.SelectItem(index);
        inventory.InventoryUpdate();
    }

    /// <summary>
    /// 색상 코딩과 애니메이션으로 데미지 숫자 표시
    /// </summary>
    /// <param name="worldPos">표시할 월드 위치</param>
    /// <param name="damage">표시할 데미지</param>
    public void ShowDamage(Vector3 worldPos, float damage)
    {
        Poolable obj = ObjectPoolManager.Instance.Get<Poolable>("DamageText");
        TMP_Text text = obj.GetComponent<TMP_Text>();
        text.transform.position = worldPos;
        text.text = (Mathf.Floor(damage * 10f) / 10f).ToString();
        text.color = Color.clear;

        DG.Tweening.Sequence sequence = DOTween.Sequence();

        // 데미지량에 따른 색상 코딩
        Color targetColor;
        if (100 <= damage)
            targetColor = Color.red;
        else if (30 <= damage)
            targetColor = Color.yellow;
        else
            targetColor = Color.white;
        targetColor.a = 0.75f;

        text.DOKill();
        sequence.Append(text.DOColor(targetColor, .2f).SetEase(Ease.OutQuad));
        sequence.Join(text.rectTransform.DOMoveY(text.transform.position.y + 0.5f, 0.2f).SetEase(Ease.OutQuad));
        sequence.Insert(0.6f, text.DOColor(Color.clear, .8f).SetEase(Ease.InQuad));
        sequence.AppendCallback(() =>
        {
            obj.Despawn();
        });
    }
    
    /// <summary>
    /// 인벤토리 표시 업데이트
    /// </summary>
    public void InventoryUpdate()
    {
        inventory.InventoryUpdate();
    }
    
    /// <summary>
    /// 스테이지 클리어 시퀀스 처리
    /// </summary>
    public void ClearStage()
    {
        FindFirstObjectByType<SurvivalTimer>().StopTimer();
        CustomEvent.Trigger(gameObject, "Clear");
        if (PlayGamesPlatform.Instance.IsAuthenticated())
            PlayGamesPlatform.Instance.UnlockAchievement(GPGSIds.achievement_2);
    }
}