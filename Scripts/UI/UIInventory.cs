using NUnit.Framework.Interfaces;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

[System.Serializable]
struct UIItem
{
    public Image image;
    public TMP_Text text;
}

public class UIInventory : MonoBehaviour
{
    [SerializeField]
    UIItem[] weapons;
    [SerializeField]
    UIItem[] acces;

    private void OnEnable()
    {
        InventoryUpdate();    
    }
    public void InventoryUpdate()
    {
        int w = 0;
        int a = 0;
        foreach (var item in GameManager.Instance.player.Inventory.items)
        {
            UIItem ui;
            if (item.Key < ItemCode.WeaponEnd)
                ui = weapons[w++];
            else
                ui = acces[a++];

            ui.image.gameObject.SetActive(true);
            ui.text.gameObject.SetActive(true);

            if (item.Value.level == Wild.Item.Data.DataMap[(int)item.Key].MaxLevel)
                ui.text.text = "Max";
            else
                ui.text.text = item.Value.level.ToString();

            WeaponBase weapon = item.Value as WeaponBase;
            if (weapon != null)
            {
                // 무기
                if (weapon.isEvolution)
                {
                    int evoWp = Wild.Item.Data.DataMap[(int)item.Key].EvolutionWeapon;
                    string icon = Wild.Item.Data.DataMap[evoWp].icon;
                    ui.image.sprite = AssetManager.Get<SpriteAtlas>("Atlas").GetSprite(icon);
                }
                else
                {
                    string icon = Wild.Item.Data.DataMap[(int)item.Key].icon;
                    ui.image.sprite = AssetManager.Get<SpriteAtlas>("Atlas").GetSprite(icon);
                }
            }
            else 
            {
                // 아이템
                string icon = Wild.Item.Data.DataMap[(int)item.Key].icon;
                ui.image.sprite = AssetManager.Get<SpriteAtlas>("Atlas").GetSprite(icon);
            }
        }

        for (int i = w; i < weapons.Length; i++)
        {
            weapons[i].image.gameObject.SetActive(false);
            weapons[i].text.gameObject.SetActive(false);
        }
        for (int i = a; i < acces.Length; i++)
        {
            acces[i].image.gameObject.SetActive(false);
            acces[i].text.gameObject.SetActive(false);
        }
    }
}
