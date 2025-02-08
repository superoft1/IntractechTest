using UnityEngine;
using TMPro;
using System;
using System.Collections;
public class NotificationHelper : MonoSingleton<NotificationHelper>
{
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private float showTime;
    [SerializeField] private Color errorColor;
    [SerializeField] private Color successColor;
    [SerializeField] private Color warningColor;

    public static Action<string> SHOW_ERROR_NOTI;
    public static Action<string> SHOW_SUCCESS_NOTI;
    public static Action<string> SHOW_WARNING_NOTI;

    private Coroutine showTextCor;
    private WaitForSeconds waitTime;

    private void Start() {
        waitTime = new WaitForSeconds(showTime);

        SHOW_ERROR_NOTI += ShowErrorNotification;
        SHOW_SUCCESS_NOTI += ShowSuccessNotification;
        SHOW_WARNING_NOTI += ShowWarningNotification;
    }

    private void OnDestroy() {
        SHOW_ERROR_NOTI -= ShowErrorNotification;
        SHOW_SUCCESS_NOTI -= ShowSuccessNotification;
        SHOW_WARNING_NOTI -= ShowWarningNotification;
    }

    private void ShowErrorNotification(string text)
    {
        if (notificationText)
        {
            notificationText.color = errorColor;
            ShowNoti(text);
        }
    }

    private void ShowSuccessNotification(string text)
    {
        if (notificationText)
        {
            notificationText.color = successColor;
            ShowNoti(text);
        }
    }

    private void ShowWarningNotification(string text)
    {
        if (notificationText)
        {
            notificationText.color = warningColor;
            ShowNoti(text);
        }
    }

    private void ShowNoti(string text)
    {
        if (notificationText)
        {
            notificationText.text = text;
            StopText_Cor();
            showTextCor = StartCoroutine(ShowText_Cor());
        }
    }

    IEnumerator ShowText_Cor()
    {
        ToggleText(true);
        yield return waitTime;
        ToggleText(false);
        showTextCor = null;
    }

    private void StopText_Cor()
    {
        if (showTextCor != null)
        {
            StopCoroutine(showTextCor);
            showTextCor = null;
            ToggleText(false);
        }
    }

    private void ToggleText(bool toggle)
    {
        if (notificationText)
        {
            notificationText.gameObject.SetActive(toggle);
        }
    }
}
