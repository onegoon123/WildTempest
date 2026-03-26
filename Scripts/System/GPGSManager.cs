using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System;
using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// Google Play Games Services 매니저 - 클라우드 저장 및 인증 처리
/// </summary>
public class GPGSManager : MonoBehaviour
{
    public static GPGSManager Instance { get; private set; }

    private const string SaveFileName = "autosave";
    ISavedGameMetadata gameMetadata;

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

    /// <summary>
    /// Google 로그인 실행
    /// </summary>
    public static void GoogleLogin()
    {
        PlayGamesPlatform.Instance.Authenticate(Instance.ProcessAuthentication);
    }

    /// <summary>
    /// 게스트에서 Google 로그인으로 전환
    /// </summary>
    public static void GoogleLogin_FromGuest()
    {
        PlayGamesPlatform.Instance.Authenticate(Instance.ProcessAuthentication_FromGuest);
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            // 로그인 성공
            DataManager.Load();
        }
        else
        {
            // 로그인 실패
            PopUpManager.PopUp_OneButton(Localize.GetStr("loginFail"), () =>
            {
                PlayerPrefs.SetInt("SigninType", (int)SigninType.none);
            });
        }
    }

    internal void ProcessAuthentication_FromGuest(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            // 로그인 성공
            DataManager.Save();
        }
        else
        {
            // 로그인 실패
            GameManager.Instance.type = SigninType.guest;
            PlayerPrefs.SetInt("SigninType", (int)SigninType.guest);
            PopUpManager.PopUp_OneButton(Localize.GetStr("loginFail"), Application.Quit);
        }
    }

    /// <summary>
    /// 클라우드에 데이터 저장
    /// </summary>
    public void Save(SaveData data)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.OpenWithAutomaticConflictResolution(
            SaveFileName,
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime,
            (status, game) =>
            {
                if (status != SavedGameRequestStatus.Success)
                {
                    Debug.LogWarning("클라우드 저장 실패");
                    return;
                }

                gameMetadata = game;

                string json = JsonUtility.ToJson(data);
                byte[] bytes = Encoding.UTF8.GetBytes(json);

                var update = new SavedGameMetadataUpdate.Builder()
                    .WithUpdatedDescription("Autosave: " + DateTime.Now)
                    .Build();

                savedGameClient.CommitUpdate(game, update, bytes, (writeStatus, _) =>
                {
                    Debug.Log(writeStatus == SavedGameRequestStatus.Success ? "저장 성공" : "저장 실패");
                });
            });
    }

    /// <summary>
    /// 클라우드에서 데이터 로드
    /// </summary>
    public void Load(Action<SaveData> onSuccess)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        savedGameClient.OpenWithAutomaticConflictResolution(
            SaveFileName,
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime,
            (status, game) =>
            {
                if (status != SavedGameRequestStatus.Success)
                {
                    onSuccess?.Invoke(new SaveData());
                    Debug.LogWarning("로드 실패");
                    return;
                }

                savedGameClient.ReadBinaryData(game, (readStatus, data) =>
                {
                    if (readStatus == SavedGameRequestStatus.Success)
                    {
                        string json = Encoding.UTF8.GetString(data);
                        SaveData saveData = JsonUtility.FromJson<SaveData>(json);
                        onSuccess?.Invoke(saveData);
                    }
                    else
                    {
                        onSuccess?.Invoke(new SaveData());
                        Debug.LogWarning("읽기 실패");
                    }
                });
            });
    }

    /// <summary>
    /// 클라우드 데이터 초기화
    /// </summary>
    public void Clear(Action onSuccess)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.OpenWithAutomaticConflictResolution(
            SaveFileName,
            DataSource.ReadCacheOrNetwork,
            ConflictResolutionStrategy.UseLongestPlaytime,
            (status, game) =>
            {
                if (status != SavedGameRequestStatus.Success)
                {
                    Debug.LogWarning("클라우드 저장 실패");
                    return;
                }

                gameMetadata = game;

                SaveData data = new SaveData();
                string json = JsonUtility.ToJson(data);
                byte[] bytes = Encoding.UTF8.GetBytes(json);

                var update = new SavedGameMetadataUpdate.Builder()
                    .WithUpdatedDescription("Autosave: " + DateTime.Now)
                    .Build();

                savedGameClient.CommitUpdate(game, update, bytes, (writeStatus, _) =>
                {
                    Debug.Log(writeStatus == SavedGameRequestStatus.Success ? "저장 성공" : "저장 실패");
                });
                onSuccess.Invoke();
            });
    }
}