using System.Collections.Generic;
using System.Linq;
using UGS;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;

/// <summary>
/// 레벨업 아이템 선택 시스템 관리
/// 플레이어의 현재 인벤토리 상태를 고려한 랜덤 아이템 생성 처리
/// </summary>
public class LevelUpManager : MonoBehaviour
{
    private Player player;

    public LevelUpItem[] items;

    private void Start()
    {
        player = GameManager.Instance.player;
    }

    /// <summary>
    /// 레벨업 선택을 위한 랜덤 아이템 생성
    /// 무기 슬롯, 액세서리 슬롯, 진화 요구사항을 고려
    /// </summary>
    public void RandomItem()
    {
        // 아이템 ID 범위 설정
        int wpStart = (int)ItemCode.WeaponStart + 1;
        int wpCount = (int)ItemCode.WeaponEnd - wpStart;
        int upWpStart = (int)ItemCode.UpStart + 1;
        int upWpCount = (int)ItemCode.UpEnd - upWpStart;
        int armStart = (int)ItemCode.AcceStart + 1;
        int armCount = (int)ItemCode.AcceEnd - armStart;

        // 선택 가능한 아이템 ID 리스트 생성
        List<int> numbers = new();

        // 무기 슬롯이 가득 찬 경우
        if (IsWeaponSlotFull())
            numbers.AddRange(Enumerable.Range(wpStart, wpCount).Where(id => !IsMaxLevel(id) && ContainsItem(id)));
        else
            numbers.AddRange(Enumerable.Range(wpStart, wpCount).Where(id => !IsMaxLevel(id)));
        
        // 액세서리 슬롯이 가득 찬 경우
        if (IsAcceSlotFull())
            numbers.AddRange(Enumerable.Range(armStart, armCount).Where(id => !IsMaxLevel(id) && ContainsItem(id)));
        else
            numbers.AddRange(Enumerable.Range(armStart, armCount).Where(id => !IsMaxLevel(id)));

        // 진화 가능한 아이템 추가
        numbers.AddRange(Enumerable.Range(upWpStart, upWpCount).Where(id => CanEvolution(id)));

        // Fisher-Yates 셔플 알고리즘으로 랜덤화
        System.Random random = new System.Random();
        for (int i = numbers.Count - 1; i >= 0; i--)
        {
            int j = random.Next(0, i + 1);
            (numbers[i], numbers[j]) = (numbers[j], numbers[i]);
        }

        // 선택 가능한 아이템이 없는 경우
        if (numbers.Count == 0)
        {
            items[0].SetItem((int)ItemCode.Potion);
            items[1].SetItem((int)ItemCode.Money);
            items[2].gameObject.SetActive(false);
            return;
        }
        else if (numbers.Count < 3)
        {
            // 선택 가능한 아이템이 3개 미만인 경우
            for (int i = 0; i < numbers.Count; i++)
            {
                items[i].SetItem(numbers[i]);
            }
            for (int i = numbers.Count; i < 3; i++)
            {
                items[i].gameObject.SetActive(false);
            }
            return;
        }

        // 3개 아이템 선택
        for (int i = 0; i < 3; i++)
        {
            items[i].SetItem(numbers[i]);
        }
    }

    /// <summary>
    /// 플레이어가 해당 아이템을 가지고 있는지 확인
    /// </summary>
    /// <param name="id">확인할 아이템 ID</param>
    /// <returns>아이템 보유 여부</returns>
    public bool ContainsItem(int id)
    {
        return player.Inventory.ContainsItem(id);
    }

    /// <summary>
    /// 무기 슬롯이 가득 찼는지 확인
    /// </summary>
    /// <returns>무기 슬롯 가득참 여부</returns>
    public bool IsWeaponSlotFull()
    {
        return player.Inventory.IsWeaponSlotFull();
    }

    /// <summary>
    /// 액세서리 슬롯이 가득 찼는지 확인
    /// </summary>
    /// <returns>액세서리 슬롯 가득참 여부</returns>
    public bool IsAcceSlotFull()
    {
        return player.Inventory.IsAcceSlotFull();
    }

    /// <summary>
    /// 아이템이 최대 레벨인지 확인
    /// </summary>
    /// <param name="id">확인할 아이템 ID</param>
    /// <returns>최대 레벨 여부</returns>
    public bool IsMaxLevel(int id)
    {
        return player.Inventory.IsMaxLevel(id);
    }

    /// <summary>
    /// 아이템이 진화 가능한지 확인
    /// </summary>
    /// <param name="id">확인할 아이템 ID</param>
    /// <returns>진화 가능 여부</returns>
    public bool CanEvolution(int id)
    {
        return player.Inventory.CanEvolution(id);
    }

    /// <summary>
    /// 지정된 인덱스의 아이템 선택
    /// </summary>
    /// <param name="index">선택할 아이템 인덱스</param>
    public void SelectItem(int index)
    {
        player.Inventory.AddItem((ItemCode)items[index].itemCode);
    }
}