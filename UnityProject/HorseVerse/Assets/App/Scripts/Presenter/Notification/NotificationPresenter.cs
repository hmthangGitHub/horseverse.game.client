using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class NotificationPresenter : IDisposable
{
    private IDIContainer container = default;

    private CancellationTokenSource cts = default;

    public NotificationPresenter(IDIContainer container)
    {
        this.container = container;
        InitAsync().Forget();
    }

    public void Dispose()
    {
        cts.SafeCancelAndDispose();
    }

    async UniTask InitAsync()
    {
        cts.SafeCancelAndDispose();
        cts = new CancellationTokenSource();

        GleyNotifications.Initialize();
        Debug.Log("init Glay");
    }


    /// <summary>
    /// Queue a notification with the given parameters.
    /// </summary>
    /// <param name="title">The title for the notification.</param>
    /// <param name="body">The body text for the notification.</param>
    /// <param name="deliveryTime">The time to deliver the notification.</param>
    /// <param name="badgeNumber">The optional badge number to display on the application icon.</param>
    /// <param name="reschedule">
    /// Whether to reschedule the notification if foregrounding and the notification hasn't yet been shown.
    /// </param>
    /// <param name="channelId">Channel ID to use. If this is null/empty then it will use the default ID. For Android
    /// the channel must be registered in <see cref="GameNotificationsManager.Initialize"/>.</param>
    /// <param name="smallIcon">Notification small icon.</param>
    /// <param name="largeIcon">Notification large icon.</param>
    public bool SendNotification(string title, string body, DateTime deliveryTime, int? badgeNumber = null,
        bool reschedule = false, string channelId = null,
        string smallIcon = null, string largeIcon = null)
    {
        Debug.Log("Send Glay notification " + deliveryTime.TimeOfDay);
        var delay = deliveryTime.TimeOfDay - DateTime.Now.TimeOfDay;
        GleyNotifications.SendNotification(title, body, delay, smallIcon, largeIcon);
        return true;
    }
}
