using DG.Tweening;
using System.Collections.Generic;
using UGS;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum SigninType { none, guest, google, }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public UIManager UIManager;
    public Player player;
    public GameObject buttons;

    public static float MoneyPercent;
    int inGameMoney = 0;

    public int currentStage = 1;

    public SigninType type = SigninType.none;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Application.targetFrameRate = 60;
        SceneManager.sceneLoaded += OnSceneLoaded;

        type = (SigninType)PlayerPrefs.GetInt("SigninType", 0);
        
        AssetManager.LoadAssetByLabel("UI", null, () =>
        {
            if (type == SigninType.google)
            {
                GPGSManager.GoogleLogin();
            }
            else if (type == SigninType.guest)
            {
                DataManager.Load();
            }
            else
            {
                GameObject.Find("Fade").GetComponent<DOTweenAnimation>().DOPlay();
                buttons.SetActive(true);
            }
        });

    }

    public void OnDataLoaded()
    {
        if (buttons && buttons.gameObject.activeSelf)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(GameObject.Find("Fade").GetComponent<Image>().DOFade(1, .2f));
            sequence.AppendCallback(() => SceneManager.LoadScene("Lobby"));
        }
        else
        {
            SceneManager.LoadScene("Lobby");
        }
    }

    public void GuestSignin()
    {
        type = SigninType.guest;
        PlayerPrefs.SetInt("SigninType", (int)SigninType.guest);
        DataManager.Load();
    }

    public void GoogleSignin()
    {
        type = SigninType.google;
        PlayerPrefs.SetInt("SigninType", (int)SigninType.google);
        GPGSManager.GoogleLogin();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            player = FindFirstObjectByType<Player>();
            UIManager = FindFirstObjectByType<UIManager>();
            inGameMoney = 0;
        }
        else if (scene.name == "Lobby")
        {
            if (0 < inGameMoney)
            {
                DataManager.data.Money += inGameMoney;
                Leaderboard.ReportScore(inGameMoney);
                DataManager.Save();
            }
            inGameMoney = 0;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void AddMoney(int value)
    {
        inGameMoney += Mathf.RoundToInt(value * MoneyPercent);
        UIManager.SetMoney(inGameMoney);
    }
}
