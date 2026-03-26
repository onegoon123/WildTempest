using DG.Tweening;
using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PopUpManager : MonoBehaviour
{
    private static PopUpManager Instance;

    [SerializeField]
    private TextMeshProUGUI message;
    [SerializeField]
    private Button okButton;
    [SerializeField]
    private Button cancelButton;
    [SerializeField]
    private CanvasGroup canvas;

    private Action okAction;
    private Action cancelAction;
    private Sequence sequence;

    void Awake()
    {
        Instance = this;
    }

    public static void PopUp(string str, Action okAction = null, Action cancelAction = null)
    {
        Instance.message.text = str;

        Instance.okAction = okAction;
        Instance.cancelAction = cancelAction;
        Instance.cancelButton.gameObject.SetActive(true);

        Instance.canvas.interactable = false;
        Instance.canvas.blocksRaycasts = true;

        Instance.canvas.DOKill();
        Instance.sequence.Kill();
        Instance.sequence = DOTween.Sequence();
        Instance.sequence.Append(Instance.canvas.DOFade(1, 0.3f));
        Instance.sequence.InsertCallback(0.2f, () =>
        {
            Instance.canvas.interactable = true;
        });
    }

    public static void PopUp_OneButton(string str, Action okAction = null)
    {
        Instance.message.text = str;

        Instance.okAction = okAction;
        Instance.cancelButton.gameObject.SetActive(false);

        Instance.canvas.interactable = false;
        Instance.canvas.blocksRaycasts = true;

        Instance.canvas.DOKill();
        Instance.sequence.Kill();
        Instance.sequence = DOTween.Sequence();
        Instance.sequence.Append(Instance.canvas.DOFade(1, 0.3f));
        Instance.sequence.InsertCallback(0.2f, () =>
        {
            Instance.canvas.interactable = true;
        });
    }

    public void OnClickOk()
    {
        PopUpClose();
        okAction?.Invoke();
    }

    public void OnClickCancel()
    {
        PopUpClose();
        cancelAction?.Invoke();
    }

    private void PopUpClose()
    {
        Instance.canvas.DOKill();
        Instance.canvas.DOFade(0, 0.3f);
        Instance.canvas.blocksRaycasts = false;
    }

    public static void PopUpQuit()
    {
        Instance.message.text = Localize.GetStr("quit");
#if UNITY_EDITOR
        Instance.okAction = () => EditorApplication.isPlaying = false;
#else
        Instance.okAction = Application.Quit;
#endif
        Instance.cancelButton.gameObject.SetActive(true);
        Instance.cancelAction = null;

        Instance.canvas.interactable = false;
        Instance.canvas.blocksRaycasts = true;
        
        Instance.canvas.DOKill();
        Instance.sequence.Kill();
        Instance.sequence = DOTween.Sequence();
        Instance.sequence.Append(Instance.canvas.DOFade(1, 0.3f));
        Instance.sequence.InsertCallback(0.2f, () =>
        {
            Instance.canvas.interactable = true;
        });
    }
}
