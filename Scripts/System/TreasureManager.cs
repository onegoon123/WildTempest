using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TreasureManager : MonoBehaviour
{
    public static TreasureManager Instance { get; private set; }
    public int itemCount = 1;

    public UISlot[] slots;
    private PlayerInventory inventory;
    List<ItemCode> items;
    [SerializeField] private InputAction doubleTapAction;

    public void Awake()
    {
        Instance = this;
    }

    public void TreasureOpen(int count)
    {
        itemCount = count;
        if (inventory == null) inventory = GameManager.Instance.player.Inventory;
        items = GetRewards(inventory.items);
        for (int i = 0; i < itemCount; i++)
        {
            slots[i].gameObject.SetActive(true);
        }
        for (int i = itemCount; i < slots.Length; i++)
        {
            slots[i].gameObject.SetActive(false);
        }
        CustomEvent.Trigger(gameObject, "Start");

    }

    public void AddItem()
    {
        foreach (var item in items)
        {
            ItemPopup.Instance.AddItem(item);
            inventory.AddItem(item);
        }
        GameManager.Instance.UIManager.InventoryUpdate();
        doubleTapAction.performed -= OnDoubleTap;
        doubleTapAction.Disable();
    }

    List<ItemCode> GetRewards(Dictionary<ItemCode, ItemBase> items)
    {
        // 모든 아이템 코드
        List<ItemCode> allItemCodes = Enum.GetValues(typeof(ItemCode)).Cast<ItemCode>().ToList();
        allItemCodes.Remove(ItemCode.WeaponStart);
        allItemCodes.Remove(ItemCode.AcceStart);
        allItemCodes.Remove(ItemCode.UpStart);
        allItemCodes.Remove(ItemCode.WeaponEnd);
        allItemCodes.Remove(ItemCode.AcceEnd);
        allItemCodes.Remove(ItemCode.UpEnd);
        allItemCodes.Remove(ItemCode.Potion);
        allItemCodes.Remove(ItemCode.Money);

        // 아이템의 현제레벨
        Dictionary<ItemCode, int> currentLevels = allItemCodes.ToDictionary(
            code => code,
            code => items.TryGetValue(code, out var item) ? item.level : 0
        );

        // 아이템의 최대레벨
        Dictionary<ItemCode, int> maxLevels = allItemCodes.ToDictionary(
            code => code,
            code => Wild.Item.Data.DataMap[(int)code].MaxLevel
        );

        // 드롭되는 아이템 리스트
        List<ItemCode> droppable = allItemCodes
        .Where(code =>
        {
            // 1. 무기
            if (code > ItemCode.WeaponStart && code < ItemCode.WeaponEnd)
            {
                if (inventory.ContainsItem(code))
                    return !inventory.IsMaxLevel(code);
                else
                    return false;
            }

            // 2. 방어구
            if (code > ItemCode.AcceStart && code < ItemCode.AcceEnd)
            {
                if (inventory.ContainsItem(code))
                    return !inventory.IsMaxLevel(code);
                else
                    return false;
            }

            // 3. 진화무기
            if (code > ItemCode.UpStart && code < ItemCode.UpEnd)
            {
                int wp = Wild.Item.Data.DataMap[(int)code].EvolutionWeapon;
                int ar = Wild.Item.Data.DataMap[(int)code].EvolutionItem;

                if (inventory.IsEvolution((ItemCode)wp))
                    return false;

                return inventory.ContainsItem(wp) && inventory.ContainsItem(ar);
            }
            return false;
        }).ToList();

        List<ItemCode> rewards = new();
        int rewardCount = itemCount;

        for (int i = 0; i < rewardCount; i++)
        {
            // droppable 갱신: 아직 최대 레벨에 도달하지 않은 것 또는 진화무기
            List<ItemCode> available = droppable
                .Where( code =>
                (code > ItemCode.UpStart && code < ItemCode.UpEnd && currentLevels[code] == 0
                && currentLevels[(ItemCode)((int)code - 50)] == maxLevels[(ItemCode)((int)code - 50)])
                || currentLevels[code] < maxLevels[code])
                .ToList();

            if (available.Count == 0)
                break; // 선택 불가

            // 무작위 선택
            ItemCode chosen = available[UnityEngine.Random.Range(0, available.Count)];
            rewards.Add(chosen);

            // 레벨 1 증가 (가상)
            currentLevels[chosen]++;
        }
        for (int i = rewards.Count; i < rewardCount; i++)
        {
            rewards.Add(ItemCode.Money);
        }
        return rewards;
    }

    public void SlotStart()
    {
        for (int i = 0; i < itemCount; i++)
        {
            slots[i].SlotStart(items[i]);
        }
        for (int i = itemCount; i < slots.Length; i++)
        {
            slots[i].Clear();
        }

        doubleTapAction.Enable();
        doubleTapAction.performed += OnDoubleTap;
    }

        void OnDoubleTap(InputAction.CallbackContext context)
    {
        // 더블탭 동작
        for (int i = 0; i < itemCount; i++)
        {
            slots[i].Skip();
            CustomEvent.Trigger(gameObject, "Skip");
        }
    }

}
