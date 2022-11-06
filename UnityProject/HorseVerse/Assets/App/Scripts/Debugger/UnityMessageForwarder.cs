using System;
using System.Collections.Generic;
using UnityEngine;

public class UnityMessageForwarder : MonoBehaviour
{
    private static UnityMessageForwarder _instance;
    public static UnityMessageForwarder Instance
    {
        get
        {
            if (_instance == null && Application.isPlaying)
            {
                GameObject go = new GameObject("UnityMessageForwarder");
                _instance = go.AddComponent<UnityMessageForwarder>();
                go.name = _instance.GetType().ToString();
            }

            return _instance;
        }
    }
    /// <summary>
    /// Different types of messages that are configured to be listend to
    /// </summary>
    public enum MessageType
    {
        FixedUpdate,
        LateUpdate,
        OnApplicationPause,
        OnApplicationQuit,
        OnGUI,
        Update,
    }

    /// <summary>
    /// A callback delegate for when the Unity message has been broadcast
    /// </summary>
    public delegate void MessageCallback();

    /// <summary>
    /// A dictionary mapping of message types and their corresponding delegate callbacks
    /// </summary>
    private Dictionary<MessageType, MessageCallback> _messageCallbacks = new Dictionary<MessageType, MessageCallback>() {
            { MessageType.FixedUpdate, null },
            { MessageType.LateUpdate, null },
            { MessageType.OnApplicationPause, null },
            { MessageType.OnApplicationQuit, null },
            { MessageType.OnGUI, null },
            { MessageType.Update, null },
        };

    /// <summary>
    /// Broadcasts a message to all listeners
    /// (Note): Do not use this function unless you know what you're doing
    /// </summary>
    /// <param name="messageType">The message type to broadcast</param>
    public static void BroadcastMessage(MessageType messageType)
    {
        Instance.BroadcastUnityMessage(messageType);
    }
    private void BroadcastUnityMessage(MessageType messageType)
    {
        MessageCallback callback;
        if (_messageCallbacks.TryGetValue(messageType, out callback))
        {
            if (callback != null)
            {
                callback();
            }
        }
    }

    //Functions for setting and disabling listeners
    #region Listener Functions
    /// <summary>
    /// Sets up a callback for a unity message
    /// </summary>
    /// <param name="messageType">The unity message to listen to</param>
    /// <param name="callback">The callback</param>
    public static void AddListener(MessageType messageType, MessageCallback callback)
    {
        Instance.AddListenerToUnityMessage(messageType, callback);
    }
    private void AddListenerToUnityMessage(MessageType messageType, MessageCallback callback)
    {
        if (!_messageCallbacks.ContainsKey(messageType))
        {
            _messageCallbacks.Add(messageType, null);
        }
        _messageCallbacks[messageType] += callback;
    }
    /// <summary>
    /// Removes a callback from a unity message
    /// </summary>
    /// <param name="messageType">The unity message being listened to</param>
    /// <param name="callback">The callback to remove</param>
    public static void RemoveListener(MessageType messageType, MessageCallback callback)
    {
        _instance?.RemoveListenerToUnityMessage(messageType, callback);
    }
    private void RemoveListenerToUnityMessage(MessageType messageType, MessageCallback callback)
    {
        if (_messageCallbacks.ContainsKey(messageType))
        {
            _messageCallbacks[messageType] -= callback;
        }
    }
    #endregion

    //The unity callback messages to forward to listeners
    #region Unity Event Message functions
    private void FixedUpdate()
    {
        BroadcastMessage(MessageType.FixedUpdate);
    }
    private void LateUpdate()
    {
        BroadcastMessage(MessageType.LateUpdate);
    }
    private void OnApplicationPause(bool pauseStatus)
    {
        //pauseStatus can be easily accessed by checking Application.isPlaying
        BroadcastMessage(MessageType.OnApplicationPause);
    }
    private void OnApplicationQuit()
    {
        BroadcastMessage(MessageType.OnApplicationQuit);
    }
    private void OnGUI()
    {
        BroadcastMessage(MessageType.OnGUI);
    }
    private void Update()
    {
        BroadcastMessage(MessageType.Update);
    }

    private void OnDestroy()
    {
        if (this == _instance)
        {
            _messageCallbacks.Clear();
        }
    }

    #endregion
}
