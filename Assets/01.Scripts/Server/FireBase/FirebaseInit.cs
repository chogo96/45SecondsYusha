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
        await InitializeFirebase();

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
                }
                else
                {
                    app = FirebaseApp.DefaultInstance;
                }

                // 초기화 성공 시 Firebase 인스턴스 설정
                auth = FirebaseAuth.GetAuth(app);
                database = FirebaseDatabase.GetInstance(app);
                firestore = FirebaseFirestore.DefaultInstance;

            }
            else
            {
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"FirebaseInit: Exception during Firebase initialization: {e.Message}");
        }
    }
}
