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


    private GameObject _registerPopUp;
    private bool isHide = true;

    private FirebaseAuth auth;
    private DatabaseReference databaseReference;

    private TMP_Text _errorMessage;

    public static string Email
    {
        get; private set;
    }



    private void Awake()
    {
        _id = transform.Find("Panel - Login/InputField (TMP) - ID").GetComponent<TMP_InputField>();
        _pw = transform.Find("Panel - Login/InputField (TMP) - PW").GetComponent<TMP_InputField>();

        _tryLogin = transform.Find("Panel - Login/Button - Login").GetComponent<Button>();
        _register = transform.Find("Panel - Login/Button - Register").GetComponent<Button>();
        _cancel = transform.Find("Panel - Register/Button - Cancel").GetComponent<Button>();

        _registerPopUp = transform.Find("Panel - Register").gameObject;

        _errorMessage = transform.Find("Panel - Login/Text (TMP) - ErrorMessage").GetComponent<TMP_Text>();

        FirebaseInit.OnFirebaseInitialized += OnFirebaseInitialized;
    }

    private void Start()
    {
        _registerPopUp.SetActive(false);

        _tryLogin.onClick.AddListener(Login);
        _register.onClick.AddListener(OnClickRegisterButton);
        _cancel.onClick.AddListener(OnClickRegisterButton);

    }
    private void OnDestroy()
    {
        // 이벤트 구독 해제
        FirebaseInit.OnFirebaseInitialized -= OnFirebaseInitialized;
    }

    private void OnFirebaseInitialized()
    {
        Debug.Log("Firebase initialized, setting auth and databaseReference");

        auth = FirebaseInit.auth; // 아픈 손가락
        databaseReference = FirebaseInit.database.RootReference;

        if (auth == null)
        {
            Debug.LogError("FirebaseAuth instance is null.");
        }
        else
        {
            Debug.Log("FirebaseAuth instance initialized.");
        }

        if (databaseReference == null)
        {
            Debug.LogError("DatabaseReference is null.");
        }
        else
        {
            Debug.Log("DatabaseReference initialized.");
        }
    }




    public async void Login()
    {
        string email = _id.text;
        string password = _pw.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            _errorMessage.text = "Email and Password must be filled!";
            return;
        }

        try
        {
            // Sign in the user
            AuthResult authResult = await auth.SignInWithEmailAndPasswordAsync(email, password);
            FirebaseUser user = authResult.User;
            Email = user.Email;

            _errorMessage.text = "User signed in successfully!";

            await NickNameSetting(user);

            StartCoroutine(C_Loding());
        }
        catch (System.Exception e)
        {
            _errorMessage.text = "Login error: " + e.Message;
        }
    }

    public async Task NickNameSetting(FirebaseUser user)
    {
        string email = user.Email;
        Debug.Log("nicknameTask 요청전");

        // 이메일을 키로 사용하기 위해 인코딩
        string encodedEmail = EncodeEmail(email);
        var nicknameTask = databaseReference.Child("users").Child(encodedEmail).Child("nickname").GetValueAsync();
        await nicknameTask;

        Debug.Log("nicknameTask 요청후");

        if (nicknameTask.Exception != null)
        {
            Debug.LogError("Failed to retrieve nickname from Firebase: " + nicknameTask.Exception);
            return;
        }

        DataSnapshot snapshot = nicknameTask.Result;
        if (snapshot.Exists)
        {
            string nickname = snapshot.Value.ToString();
            PhotonNetwork.NickName = nickname;
            Debug.Log("Photon Nickname set to: " + nickname);
        }
        else
        {
            Debug.LogError("Nickname does not exist in Firebase.");
        }
    }

    private string EncodeEmail(string email)
    {
        return email.Replace(".", ",");
    }

    private IEnumerator C_Loding()
    {
        // 포톤 서버 연결 확인
        if (PhotonNetwork.IsConnected)
        {

            // 씬 이동
            //AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("01.Start Scene");
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainScene");

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
        else
        {
            Debug.Log("Not connected to Photon Server!");
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
            _registerPopUp.SetActive(false);
        }
        isHide = !isHide;
    }

}
