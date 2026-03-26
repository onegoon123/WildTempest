using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어가 획득한 아이템들을 관리하는 클래스입니다.
/// 아이템 획득, 레벨업, 진화 시스템을 처리합니다.
/// 
/// 주요 기능:
/// - 아이템 팩토리 패턴을 통한 동적 아이템 생성
/// - 무기/액세서리 슬롯 관리
/// - 아이템 레벨업 및 진화 조건 검사
/// </summary>
public class PlayerInventory
{
    /// <summary>
    /// 아이템 팩토리 - 코드에 따른 아이템 생성 함수 매핑
    /// </summary>
    Dictionary<ItemCode, Func<ItemBase>> itemFactory = new()
    {
        { ItemCode.Fireball, () => new Fireball() },
        { ItemCode.Sword, () => new Sword() },
        { ItemCode.TripleLightning, () => new TripleLightning() },
        { ItemCode.Meteor, () => new Meteor() },
        { ItemCode.Axe, () => new Axe() },
        { ItemCode.Staff, () => new Staff() },
        { ItemCode.Grimoire, () => new Grimoire() },
        { ItemCode.Frost, () => new FrostMine() },
        { ItemCode.AttackRing, () => new AttackRing() },
        { ItemCode.SpeedBoots, () => new SpeedBoots() },
        { ItemCode.Acce, () => new Accessory() },
        { ItemCode.Robe, () => new Robe() },
        { ItemCode.Quiver, () => new Quiver() },
        { ItemCode.Book, () => new Book() },
        { ItemCode.DarkRing, () => new DarkRing() },
        { ItemCode.Hourglass, () => new Hourglass() },
    };

    private Player player;
    public Dictionary<ItemCode, ItemBase> items = new();
    public int maxWeaponCount = 5;
    public int maxAcceCount = 5;
    private int curWeaponCount = 0;
    private int curAcceCount = 0;

    public PlayerInventory(Player player)
    {
        this.player = player;
    }

    /// <summary>
    /// 모든 보유 아이템의 Tick 메서드를 호출합니다.
    /// </summary>
    public void Tick()
    {
        foreach (var item in items)
        {
            item.Value.Tick();
        }
    }

    /// <summary>
    /// 기존 아이템 인스턴스를 인벤토리에 추가합니다.
    /// </summary>
    public void AddItem(ItemCode code, ItemBase item)
    {
        item.Init(code, player);
        items.Add(code, item);
    }

    /// <summary>
    /// 아이템 코드를 기반으로 새 아이템을 생성하거나 기존 아이템을 레벨업합니다.
    /// 특수 아이템(포션, 돈, 진화 아이템)의 경우 별도 처리를 수행합니다.
    /// </summary>
    /// <param name="code">추가할 아이템의 코드</param>
    public void AddItem(ItemCode code)
    {
        // 이미 보유한 아이템인 경우 레벨업 처리
        if (items.ContainsKey(code))
        {
            if (IsMaxLevel(code))
            {
                Debug.LogError("최대레벨인 아이템을 획득하려고 합니다.");
                return;
            }
            items[code].LevelUp();
            return;
        }

        // 특수 아이템 처리
        if (code == ItemCode.Potion)
        {
            player.TakeHeal(int.MaxValue);
            return;
        }
        else if (code == ItemCode.Money)
        {
            GameManager.Instance.AddMoney(100);
            return;
        }
        else if (ItemCode.UpStart < code && code < ItemCode.UpEnd)
        {
            // 진화 아이템인 경우, 해당 무기를 찾아서 진화 처리
            ItemCode wpCode = (ItemCode)Wild.Item.Data.DataMap[(int)code].EvolutionWeapon;
            if (items.TryGetValue(wpCode, out ItemBase item))
            {
                WeaponBase weapon = item as WeaponBase;
                if (weapon != null)
                    weapon.Evolution();
            }
            return;
        }

        // 팩토리를 통한 새 아이템 생성
        if (itemFactory.TryGetValue(code, out var creator))
        {
            AddItem(code, creator());

            // 아이템 타입별 카운트 증가
            if (code < ItemCode.WeaponEnd)
                curWeaponCount++;
            else
                curAcceCount++;
        }
        else
        {
            Debug.Log("아이템이 등록되어 있지 않습니다");
        }
    }

    public void AddItem(int code) => AddItem((ItemCode)code);

    /// <summary>
    /// 무기 슬롯이 가득 찬 상태인지 확인합니다.
    /// </summary>
    public bool IsWeaponSlotFull()
    {
        return curWeaponCount == maxWeaponCount;
    }

    /// <summary>
    /// 액세서리 슬롯이 가득 찬 상태인지 확인합니다.
    /// </summary>
    public bool IsAcceSlotFull()
    {
        return curAcceCount == maxAcceCount;
    }

    /// <summary>
    /// 아이템이 최대 레벨인지 확인합니다.
    /// </summary>
    public bool IsMaxLevel(ItemCode itemCode)
    {
        if (items.ContainsKey(itemCode) == false) return false;
        return items[itemCode].IsMaxLevel();
    }

    public bool IsMaxLevel(int itemCode) => IsMaxLevel((ItemCode)itemCode);

    /// <summary>
    /// 아이템이 진화된 상태인지 확인합니다.
    /// </summary>
    public bool IsEvolution(ItemCode itemCode)
    {
        if (items.TryGetValue(itemCode, out var item))
        {
            if (item is WeaponBase weapon)
            {
                return weapon.isEvolution;
            }
        }
        return false;
    }

    /// <summary>
    /// 특정 아이템을 보유하고 있는지 확인합니다.
    /// </summary>
    public bool ContainsItem(ItemCode itemCode)
    {
        return items.ContainsKey(itemCode);
    }

    public bool ContainsItem(int itemCode) => ContainsItem((ItemCode)itemCode);

    /// <summary>
    /// 아이템이 진화할 수 있는 조건을 만족하는지 확인합니다.
    /// </summary>
    public bool CanEvolution(ItemCode id) => CanEvolution((int)id);

    public bool CanEvolution(int id)
    {
        var data = Wild.Item.Data.DataMap[id];
        int wp = data.EvolutionWeapon;          // 진화할 무기
        int ac = data.EvolutionItem;            // 필요 액세서리
        
        // 이미 진화된 무기인지 확인
        if (items.TryGetValue((ItemCode)wp, out var item))
        {
            if (item is WeaponBase weapon && weapon.isEvolution)
                return false;
        }
        
        // 진화 조건: 무기가 최대 레벨이고 필요 액세서리를 보유
        return IsMaxLevel(wp) && ContainsItem(ac);
    }

    /// <summary>
    /// 아이템의 현재 레벨을 반환합니다.
    /// </summary>
    public int GetItemLevel(int itemCode) => GetItemLevel((ItemCode)itemCode);

    public int GetItemLevel(ItemCode itemCode)
    {
        if (items.ContainsKey(itemCode) == false) return 0;
        return items[itemCode].level;
    }
}