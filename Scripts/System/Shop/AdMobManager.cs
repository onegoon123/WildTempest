using GoogleMobileAds.Api;
using System;
using UnityEngine;

/// <summary>
/// AdMob 광고 매니저 - 리워드 광고 처리
/// </summary>
public class AdMobManager : MonoBehaviour
{
    public static AdMobManager Instance { get; private set; }

    private RewardedAd rewardedAd;
    private string adUnitId = "ca-app-pub-3940256099942544/5224354917"; // 테스트 광고 유닛 ID
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        MobileAds.Initialize(initStatus =>
        {
            LoadRewardedAd();
            Debug.Log("AdMob Initialized");
        });
    }

    /// <summary>
    /// 리워드 광고 로드
    /// </summary>
    private void LoadRewardedAd()
    {
        var adRequest = new AdRequest();
        RewardedAd.Load(adUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("리워드 광고 로드 실패: " + error);
                return;
            }
            Debug.Log("리워드 광고 로드 성공");
            rewardedAd = ad;

            // 광고 닫기 이벤트
            rewardedAd.OnAdFullScreenContentClosed += () => {
                Debug.Log("광고 닫힘");
                LoadRewardedAd(); // 광고 다시 로드
            };

            // 광고 표시 실패 이벤트
            rewardedAd.OnAdFullScreenContentFailed += (err) => {
                Debug.LogError("광고 표시 실패: " + err);
            };
        });
    }

    /// <summary>
    /// 리워드 광고 표시 (콜백 포함)
    /// </summary>
    public void ShowRewardedAd(Action<Reward> reward)
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            // 리워드 광고 표시
            rewardedAd.Show(reward);
        }
        else
        {
            Debug.Log("리워드 광고가 로드되지 않았거나 표시할 수 없습니다.");
        }
    }

    /// <summary>
    /// 리워드 광고 표시 (기본 콜백)
    /// </summary>
    public void ShowRewardedAd()
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            // 리워드 광고 표시
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log("리워드 획득");
                // 여기서 리워드 처리 로직
            });
        }
        else
        {
            Debug.Log("리워드 광고가 로드되지 않았거나 표시할 수 없습니다.");
        }
    }
}