using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationCaller : MonoBehaviour
{
    public float notificationTime = 3f;

    public void ShowNotification(string text)
    {
        UIController ui = FindObjectOfType<UIController>();
        if (ui != null)
        {
            ui.ShowNotification(text, notificationTime);
        }
    }
}
