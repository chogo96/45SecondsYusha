using Firebase.Auth;
using Firebase.Database;
using Photon.Pun;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    private TMP_InputField _id;
    private TMP_InputField _pw;

    private Button _tryLogin;
    private Button _register;
    private Button _cancel;

    private Button _Test1;
    private Button _Test2;
    private Button _Test3;
    private Button _Test4;

    private GameObject _registerPopUp;
    private bool isHide = true;

    private FirebaseAuth auth;
    private DatabaseReference databaseReference;
    private RegisterManager registerManager;

    private TMP_Text _errorMessage;

    public static string UserId { get; private set; }

    private void Awake()
    {
        _id = transform.Find("Panel - Login/InputField (TMP) - ID").GetComponent<TMP_InputField>();
        _pw = transform.Find("Panel - Login/InputField (TMP) - PW").GetComponent<TMP_InputField>();

        _tryLogin = transform.Find("Panel - Login/Button - Login").GetComponent<Button>();
        _register = transform.Find("Panel - Login/Button - Register").GetComponent<Button>();
        _cancel = transform.Find("Panel - Register/Button - Cancel").GetComponent<Button>();

        _Test1 = transform.Find("Button - Test1").GetComponent<Button>();
        _Test2 = transform.Find("Button - Test2").GetComponent<Button>();
        _Test3 = transform.Find("Button - Test3").GetComponent<Button>();
        _Test4 = transform.Find("Button - Test4").GetComponent<Button>();

        _registerPopUp = transform.Find("Panel - Register").gameObject;

        _errorMessage = transform.Find("Panel - Login/Text (TMP) - ErrorMessage").GetComponent<TMP_Text>();

        registerManager = GetComponent<RegisterManager>();

        FirebaseInit.OnFirebaseInitialized += OnFirebaseInitialized;
    }

    private void Start()
    {
        _registerPopUp.SetActive(false);

        _tryLogin.onClick.AddListener(Login);
        _register.onClick.AddListener(OnClickRegisterButton);
        _cancel.onClick.AddListener(OnClickRegisterButton);

        _Test1.onClick.AddListener(() => TestLogin(1));
        _Test2.onClick.AddListener(() => TestLogin(2));
        _Test3.onClick.AddListener(() => TestLogin(3));
        _Test4.onClick.AddListener(() => TestLogin(4));
    }

    public void TestLogin(int num)
    {
        switch (num)
        {
            case 1:
                _id.text = "jinodumok";
                _pw.text = "jino1364";
                Login();
                break;
            case 2:
                _id.text = "jino1364";
                _pw.text = "jino1364";
                Login();
                break;
            case 3:
                _id.text = "12345";
                _pw.text = "1234567890";
                Login();
                break;
            case 4:
                _id.text = "1111";
                _pw.text = "11111111";
                Login();
                break;
            default:
                break;
        }
    }

    private void OnDestroy()
    {
        FirebaseInit.OnFirebaseInitialized -= OnFirebaseInitialized;
    }

    private void OnFirebaseInitialized()
    {
        auth = FirebaseInit.auth;
        databaseReference = FirebaseInit.database.RootReference;
    }

    public async void Login()
    {
        string userId = _id.text;
        string password = _pw.text;

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password))
        {
            _errorMessage.text = "ID and Password must be filled!";
            return;
        }

        // Firebase 인증을 위해 가상의 이메일 주소 생성
        string email = $"{userId}@example.com";

        try
        {
            AuthResult authResult = await auth.SignInWithEmailAndPasswordAsync(email, password);
            FirebaseUser user = authResult.User;
            UserId = userId;

            _errorMessage.text = "User signed in successfully!";

            await NickNameSetting(userId);

            // 튜토리얼 상태 확인 후 메인 씬으로 이동
            await CheckTutorialStatus(userId);
        }
        catch (System.Exception e)
        {
            _errorMessage.text = "Login error: " + e.Message;
        }
    }

    public async Task NickNameSetting(string userId)
    {
        var nicknameTask = databaseReference.Child("users").Child(userId).Child("nickname").GetValueAsync();
        await nicknameTask;

        if (nicknameTask.Exception != null)
        {
            Utils.LogRed("Failed to retrieve nickname from Firebase: " + nicknameTask.Exception);
            return;
        }

        DataSnapshot snapshot = nicknameTask.Result;
        if (snapshot.Exists)
        {
            string nickname = snapshot.Value.ToString();
            PhotonNetwork.NickName = nickname;
        }
        else
        {
            Utils.LogRed("Nickname does not exist in Firebase.");
        }
    }

    private async Task CheckTutorialStatus(string userId)
    {
        var tutorialStatusTask = databaseReference.Child("users").Child(userId).Child("TutorialCompleted").GetValueAsync();
        await tutorialStatusTask;

        if (tutorialStatusTask.Exception != null)
        {
            Utils.LogRed("Failed to check tutorial status from Firebase: " + tutorialStatusTask.Exception);
            return;
        }

        DataSnapshot snapshot = tutorialStatusTask.Result;
        bool tutorialCompleted = snapshot.Exists && snapshot.Value.ToString() == "true";
        PlayerPrefs.SetInt("TutorialCompleted", tutorialCompleted ? 1 : 0);

        StartCoroutine(C_LoadMainScene());
    }

    private IEnumerator C_LoadMainScene()
    {
        if (PhotonNetwork.IsConnected)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("01.MainScene");
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
        else
        {
            Utils.Log("Not connected to Photon Server!");
        }
    }

    public void OnClickRegisterButton()
    {
        if (isHide)
        {
            _registerPopUp.SetActive(true);
        }
        else
        {
            registerManager.Reset();
            _registerPopUp.SetActive(false);
        }
        isHide = !isHide;
    }
}
