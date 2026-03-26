using GooglePlayGames;
using System;
using System.Collections.Generic;
using UGS;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 직렬화 가능한 저장 데이터 구조
/// 모든 플레이어 진행 상황과 설정을 포함
/// </summary>
[System.Serializable]
public class SaveData
{
    [NonSerialized]
    public Action<int> onMoneyChangeEvent;

    [SerializeField]
    private int money;
    /// <summary>
    /// 변경 이벤트 알림과 함께하는 플레이어의 현재 돈
    /// </summary>
    public int Money
    {
        get { return money; }
        set
        {
            if (money == value) return;
            money = value;
            onMoneyChangeEvent?.Invoke(value);
        }
    }
    [SerializeField]
    public int[] upgrades;        // 각 스탯의 업그레이드 레벨
    [SerializeField]
    public int currentHero;       // 현재 선택된 영웅
    [SerializeField]
    public List<int> hasHero;     // 해금된 영웅 목록

    /// <summary>
    /// 기본값으로 저장 데이터 초기화
    /// </summary>
    public SaveData()
    {
        upgrades = new int[8];
        currentHero = 0;
        hasHero = new List<int>() { 1, 0, 0, 0, 0, 0, 0};
        Array.Clear(upgrades, 0, upgrades.Length);
    }

    /// <summary>
    /// PlayerPrefs를 사용하여 로컬 저장소에 데이터 저장
    /// </summary>
    public void SaveLocal()
    {
        PlayerPrefs.SetInt("money", money);
        for (int i = 0; i < upgrades.Length; i++)
        {
            PlayerPrefs.SetInt("upgrade_" + i, upgrades[i]);
        }
        PlayerPrefs.SetInt("currentHero", currentHero);
        for (int i = 0; i < hasHero.Count; i++)
        {
            PlayerPrefs.SetInt("hasHero_" + i, hasHero[i]);
        }
    }

    /// <summary>
    /// PlayerPrefs를 사용하여 로컬 저장소에서 데이터 로드
    /// </summary>
    public void LoadLocal()
    {
        money = PlayerPrefs.GetInt("money", 0);
        for (int i = 0; i < upgrades.Length; i++)
        {
            upgrades[i] = PlayerPrefs.GetInt("upgrade_" + i, 0);
        }

        currentHero = PlayerPrefs.GetInt("currentHero", 0);
        hasHero[0] = PlayerPrefs.GetInt("hasHero_0", 1);
        for (int i = 1; i < Wild.Player.Hero.HeroList.Count; i++)
        {
            hasHero[i] = PlayerPrefs.GetInt("hasHero_" + i, 0);
        }
    }
}

/// <summary>
/// 게임 데이터 지속성 및 동기화 관리
/// 로컬 및 클라우드 저장 시스템 모두 처리
/// 게스트 모드 및 Google Play Games 통합 지원
/// </summary>
public class DataManager : MonoBehaviour
{
    public static SaveData data = new SaveData();

    private static DataManager Instance = null;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Google Sheets에서 게임 데이터 로드
        UnityGoogleSheet.LoadAllData();
    }

    /// <summary>
    /// 애플리케이션이 일시정지될 때 자동 저장
    /// </summary>
    void OnApplicationPause(bool pause)
    {
        if (pause)
            Save();
    }

    /// <summary>
    /// 애플리케이션이 종료될 때 자동 저장
    /// </summary>
    void OnApplicationQuit()
    {
        Save();
    }

    /// <summary>
    /// 현재 로그인 타입에 따라 데이터 저장
    /// </summary>
    public static void Save()
    {
        switch (GameManager.Instance.type)
        {
            case SigninType.guest:
                data.SaveLocal();
                break;
            case SigninType.google:
                {
                    if (PlayGamesPlatform.Instance.IsAuthenticated())
                    {
                        GPGSManager.Instance.Save(data);
                    }
                }
                break;
        }
    }

    /// <summary>
    /// 현재 로그인 타입에 따라 데이터 로드
    /// </summary>
    public static void Load()
    {
        switch (GameManager.Instance.type)
        {
            case SigninType.guest:
                data.LoadLocal();
                GameManager.Instance.OnDataLoaded();
                break;
            case SigninType.google:
            {
                if (PlayGamesPlatform.Instance.IsAuthenticated())
                {
                    GPGSManager.Instance.Load((loadData) =>
                    {
                        data = loadData;
                        GameManager.Instance.OnDataLoaded();
                    });
                }
                else
                {
                    data.LoadLocal();
                    GameManager.Instance.OnDataLoaded();
                }
                break;
            }
        }
    }

    /// <summary>
    /// 모든 저장 데이터 삭제 (로컬 및 클라우드 모두)
    /// </summary>
    /// <param name="onSucesss">삭제 완료 시 콜백</param>
    public static void Delete(Action onSucesss)
    {
        if (PlayGamesPlatform.Instance.IsAuthenticated())
        {
            PlayerPrefs.DeleteAll();
            GPGSManager.Instance.Clear(onSucesss);
        }
        else
        {
            PlayerPrefs.DeleteAll();
            onSucesss.Invoke();
        }
    }

    /// <summary>
    /// 모든 PlayerPrefs를 지우는 디버그 메서드
    /// </summary>
    [ContextMenu("PlayerPrefs Delete")]
    public void PlayerPrefsDeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }
}