using DG.Tweening;
using TMPro;
using UnityEngine;

public class MoneyText : MonoBehaviour
{
    private TextMeshProUGUI text;
    public bool textAnim = false;
    int beforeValue = 0;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        beforeValue = DataManager.data.Money;
        text.text = beforeValue.ToString();

        DataManager.data.onMoneyChangeEvent += MoneyUpdate;
    }
    void OnDestroy()
    {
        DataManager.data.onMoneyChangeEvent -= MoneyUpdate;
    }
    void MoneyUpdate(int value)
    {
        if (textAnim)
        {
            text.DOKill();
            text.DOCounter(beforeValue, value, .5f);
        }
        else
        {
            text.text = value.ToString();
        }
        beforeValue = value;
    }
}
