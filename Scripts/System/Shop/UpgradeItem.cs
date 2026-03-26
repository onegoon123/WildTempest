using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class UpgradeItem : MonoBehaviour
{
    public int upgradeID;
    public int upgradeLevel = 0;
    public GameObject[] upgradeToggles;
    public Image icon;
    public TextMeshProUGUI nameText;
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();

        upgradeLevel = DataManager.data.upgrades[upgradeID - 1001];
        for (int i = 0; i < upgradeLevel; i++)
        {
            upgradeToggles[i].SetActive(true);
        }

        var data = Wild.Player.UpgradeData.UpgradeDataMap[upgradeID];

        SetSprite(data.Icon);

        nameText.text = Localize.GetStr(data.Name);
    }

    private async void SetSprite(string name)
    {
        SpriteAtlas spriteAtlas = await AssetManager.GetAsync<SpriteAtlas>("Atlas");
        icon.sprite = spriteAtlas.GetSprite(name);
    }

    public void OnButtonClick()
    {
        SoundManager.PlaySFX("Menu_Select_00");
        button.interactable = false;
        ShopManager.Instance.Select(this);
    }

    public void Cancel()
    {
        button.interactable = true;
    }

    public void Upgrade()
    {
        upgradeToggles[upgradeLevel++].SetActive(true);
    }
}
