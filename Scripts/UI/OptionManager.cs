using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{
    public CanvasGroup canvas;
    public Slider bgmSlider;
    public Slider sfxSlider;

    public GameObject clearButton;
    public GameObject LoginButton;

    private void OnEnable()
    {
        // 局聪皋捞记 贸府
        canvas.DOKill();
        canvas.DOFade(1, 0.3f);
        canvas.blocksRaycasts = true;

        bgmSlider.value = SoundManager.Instance.BGMPercent;
        sfxSlider.value = SoundManager.Instance.sfxPercent;

        if (GameManager.Instance.type == SigninType.google)
        {
            clearButton.SetActive(true);
            LoginButton.SetActive(false);
        }
        else
        {
            clearButton.SetActive(false);
            LoginButton.SetActive(true);
        }

    }

    public void SetBGMVolume(float volume)
    {
        SoundManager.Instance.BGMPercent = volume;
        PlayerPrefs.SetFloat("BGM", volume);
    }

    public void SetSFXVolume(float volume)
    {
        SoundManager.Instance.sfxPercent = volume;
        PlayerPrefs.SetFloat("SFX", volume);
    }

    public void DataClear()
    {
        PopUpManager.PopUp(Localize.GetStr("DataClear OK"), () =>
        {
            DataManager.Delete(Application.Quit);
        });
    }
    public void Login()
    {
        PopUpManager.PopUp(Localize.GetStr("logintext"), () =>
        {
            GameManager.Instance.type = SigninType.google;
            PlayerPrefs.SetInt("SigninType", (int)SigninType.google);
            GPGSManager.GoogleLogin_FromGuest();
            clearButton.SetActive(true);
            LoginButton.SetActive(false);
        });
    }
    public void Close()
    {
        Sequence sequence = DOTween.Sequence();
        canvas.DOKill();
        sequence.Append(canvas.DOFade(0, 0.3f));
        sequence.AppendCallback(() =>
        {
            gameObject.SetActive(false);
        });
        canvas.blocksRaycasts = false;
    }
}
