using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

/// <summary>
/// 레벨업 아이템 옵션 표시를 위한 UI 컴포넌트
/// 아이템 정보 표시 및 시각적 업데이트 처리
/// </summary>
public class LevelUpItem : MonoBehaviour
{
    public int itemCode;
    public Image iconImage;
    public TMP_Text levelText;
    public TMP_Text itemNameText;
    public TMP_Text itemInfoText;
    public Image recipeImage;
    public GameObject recipeText;

    /// <summary>
    /// 주어진 아이템 코드로 아이템 표시 설정
    /// </summary>
    /// <param name="itemCode">설정할 아이템 코드</param>
    public void SetItem(int itemCode)
    {
        gameObject.SetActive(true);

        // 1. 아이템 데이터를 가져옴
        this.itemCode = itemCode;
        var data = Wild.Item.Data.DataMap[itemCode];

        // 2. 아이템 이름 설정
        itemNameText.text = Localize.GetStr(data.itemName);

        // 3. 아이템 아이콘 설정
        iconImage.sprite = AssetManager.Get<SpriteAtlas>("Atlas").GetSprite(data.icon);

        // 포션, 돈 아이템 처리
        if ((int)ItemCode.Potion <= itemCode)
        {
            itemInfoText.text = Localize.GetStr(data.ItemCode.ToString());
            levelText.text = "";
            recipeImage.gameObject.SetActive(false);
            recipeText.SetActive(false);
            return;
        }
        
        // 4. 아이템 레벨 확인 후 데이터를 가져옴
        int level = GameManager.Instance.player.Inventory.GetItemLevel(itemCode) + 1;
        Wild.Item.LevelData levelData;

        if ((int)ItemCode.UpStart < itemCode && itemCode < (int)ItemCode.UpEnd)
        {
            // 진화 아이템
            levelData = Wild.Item.LevelData.LevelDataMap[itemCode.ToString()];
            levelText.text = Localize.GetStr("evo");
            recipeImage.gameObject.SetActive(false);
            recipeText.SetActive(false);
        }
        else
        {
            // 일반 아이템
            levelData = Wild.Item.LevelData.LevelDataMap[itemCode + "_" + level];

            // 5. 레벨 텍스트 설정
            levelText.text = "Level ";
            if (level >= data.MaxLevel)
                levelText.text += "Max";
            else
                levelText.text += level.ToString();

            // 진화 레시피 표시 설정
            var weaponData = Wild.Item.Data.DataMap[data.EvolutionItem];
            recipeImage.sprite = AssetManager.Get<SpriteAtlas>("Atlas").GetSprite(weaponData.icon);
            recipeImage.gameObject.SetActive(true);
            recipeText.SetActive(true);
        }

        // 6. 아이템 설명 텍스트 설정
        itemInfoText.text = Localize.GetStr(levelData.levelCode.ToString());
    }
}