using GooglePlayGames;
using UnityEngine.SocialPlatforms;
using UnityEngine;
using static System.Net.WebRequestMethods;
using UnityEngine.SocialPlatforms.Impl;

public class Leaderboard : MonoBehaviour
{
    public void ShowLeaderboard()
    {
        if (PlayGamesPlatform.Instance.IsAuthenticated() == false)
        {
            PopUpManager.PopUp_OneButton(Localize.GetStr("leaderboard"));
            return;
        }
        PlayGamesPlatform.Instance.ShowLeaderboardUI();
    }
    public void ShowAchievementUI()
    {
        if (PlayGamesPlatform.Instance.IsAuthenticated() == false)
        {
            PopUpManager.PopUp_OneButton(Localize.GetStr("leaderboard"));
            return;
        }
        PlayGamesPlatform.Instance.ShowAchievementsUI();
    }

    public static void ReportScore(int score)
    {
        if (PlayGamesPlatform.Instance.IsAuthenticated() == false)
        {
            return;
        }
        PlayGamesPlatform.Instance.ReportScore(score, GPGSIds.leaderboard, (bool success) =>
        {
            if (success)
            {
                Debug.Log("스코어 등록됨");
            }
            else
            {
                Debug.Log("스코어 등록 실패");
            }
        });
    }

    public static void ReportTime(float time)
    {
        if (PlayGamesPlatform.Instance.IsAuthenticated() == false)
        {
            return;
        }
        long score = (long)(time * 1000);
        PlayGamesPlatform.Instance.ReportScore(score, GPGSIds.leaderboard_2, (bool success) =>
        {
            if (success)
            {
                Debug.Log("스코어 등록됨");
            }
            else
            {
                Debug.Log("스코어 등록 실패");
            }
        });
    }
}
