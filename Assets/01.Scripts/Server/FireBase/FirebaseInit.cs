using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;
using System.Threading.Tasks;
using System;
using Firebase.Firestore;

public class FirebaseInit : MonoBehaviour
{
    public static FirebaseAuth auth;
    public static FirebaseDatabase database;
    public FirebaseFirestore firestore;
    public string databaseURL = "https://your-database-name.firebaseio.com/";

    public static event Action OnFirebaseInitialized;

    private async void Awake()
    {
        Utils.Log("FirebaseInit: Initializing Firebase...");
        await InitializeFirebase();
        Utils.Log("FirebaseInit: Firebase initialization completed.");

        // 초기화 완료 이벤트 호출
        OnFirebaseInitialized?.Invoke();
    }

    /// <summary>
    /// 파이어 베이스 초기화 함수
    /// </summary>
    /// <returns></returns>
    private async Task InitializeFirebase()
    {
        try
        {
            var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
            if (dependencyStatus == DependencyStatus.Available)
            {
                FirebaseApp app;
                if (FirebaseApp.DefaultInstance == null)
                {
                    Firebase.AppOptions options = new Firebase.AppOptions
                    {
                        DatabaseUrl = new System.Uri(databaseURL)
                    };
                    app = FirebaseApp.Create(options);
                    Utils.Log("FirebaseInit: FirebaseApp created with custom options.");
                }
                else
                {
                    app = FirebaseApp.DefaultInstance;
                    Utils.Log("FirebaseInit: Using Default FirebaseApp instance.");
                }

                // 초기화 성공 시 Firebase 인스턴스 설정
                auth = FirebaseAuth.GetAuth(app);
                database = FirebaseDatabase.GetInstance(app);
                firestore = FirebaseFirestore.DefaultInstance;

                Utils.Log("FirebaseInit: FirebaseAuth, FirebaseDatabase, and FirebaseFirestore instances initialized.");
            }
            else
            {
                Utils.LogRed($"FirebaseInit: Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        }
        catch (System.Exception e)
        {
            Utils.LogRed($"FirebaseInit: Exception during Firebase initialization: {e.Message}");
        }
    }
}
