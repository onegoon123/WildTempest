using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;

/// <summary>
/// 인앱 결제 매니저 - Unity IAP를 통한 상품 구매 처리
/// </summary>
public class IAPManager : MonoBehaviour, IStoreListener
{
    public static IAPManager Instance;

    [SerializeField]
    private TMP_Text stateText;
    [SerializeField]
    private GameObject noAdsButton;

    private IStoreController storeController;

    const string money_1500 = "money_1500";
    const string money_3500 = "money_3500";
    const string money_7600 = "money_7600";
    const string money_16800 = "money_16800";

    void Start()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitIAP();
    }

    /// <summary>
    /// IAP 초기화 - 상품 등록
    /// </summary>
    private void InitIAP()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(money_1500, ProductType.Consumable);
        builder.AddProduct(money_3500, ProductType.Consumable);
        builder.AddProduct(money_7600, ProductType.Consumable);
        builder.AddProduct(money_16800, ProductType.Consumable);

#pragma warning disable CS0618 // 사용되지 않는 멤버에 대한 경고를 표시하지 않습니다.
        UnityPurchasing.Initialize(this, builder);
#pragma warning restore CS0618 // 사용되지 않는 멤버에 대한 경고를 표시하지 않습니다.
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        //CheckNonConsumable(noAds);
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError("초기화 실패 " + error);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.LogError("초기화 실패 " + error + message);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.LogError("구매 실패");
    }

    /// <summary>
    /// 구매 처리 - 상품별 돈 지급
    /// </summary>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        var product = purchaseEvent.purchasedProduct;
        Debug.Log("구매 완료 : " + product);

        switch (product.definition.id)
        {
            case money_1500:
                DataManager.data.Money += 1500;
                break;
            case money_3500:
                DataManager.data.Money += 3500;
                break;
            case money_7600:
                DataManager.data.Money += 7600;
                break;
            case money_16800:
                DataManager.data.Money += 16800;
                break;
        }
        DataManager.Save();

        return PurchaseProcessingResult.Complete;
    }

    /// <summary>
    /// 상품 구매 시작
    /// </summary>
    public void Purchase(string productID)
    {
        // 구매 이벤트 시작
        storeController.InitiatePurchase(productID);
    }

    /// <summary>
    /// 비소모품 구매 상태 확인
    /// </summary>
    private void CheckNonConsumable(string id)
    {
        // 구매 상태 확인
        var product = storeController.products.WithID(id);

        if (product != null)
        {
            bool isCheck = product.hasReceipt;
            noAdsButton.SetActive(isCheck);
        }
    }
}