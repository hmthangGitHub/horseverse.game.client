using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseInitializer : MonoBehaviour
{
    private void Start()
    {
        // Initialize Firebase
        Firebase.AppOptions options = new Firebase.AppOptions();
        options.ApiKey = "AIzaSyCQgM6DJKtExklhkqNOT8XaugTuamTjSlo";
        options.AppId = "1:16297503888:android:02766383e5bb809871fc15";
        options.MessageSenderId = "16297503888";
        options.ProjectId = "horse-of-legends-ee950";
        options.StorageBucket = "horse-of-legends-ee950.appspot.com";

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                // Crashlytics will use the DefaultInstance, as well;
                // this ensures that Crashlytics is initialized.
                //Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
                Firebase.FirebaseApp app = Firebase.FirebaseApp.Create(options);
                // Set a flag here for indicating that your project is ready to use Firebase.
                InitFCM();
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });

        

        
    }

    public void InitFCM()
    {
        Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
        Subscribe();
    }

        void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        UnityEngine.Debug.Log("Received Registration Token: " + token.Token);
    }

    void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        UnityEngine.Debug.Log("Received a new message from: " + e.Message.From);
    }

    void Subscribe()
    {
        Firebase.Messaging.FirebaseMessaging.SubscribeAsync("/topics/Common");
    }

}
