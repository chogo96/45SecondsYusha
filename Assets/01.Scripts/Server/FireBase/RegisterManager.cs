using Firebase.Auth;
using Firebase.Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterManager : MonoBehaviour
{
    private TMP_InputField _registerId;
    private TMP_InputField _registerPw;
    private TMP_InputField _nickName;

    private Button _confirm;

    private TMP_Text _errorMessage;

    private FirebaseAuth _auth;
    private DatabaseReference _databaseReference;
    private LoginManager _loginManager;

    private void Awake()
    {
        _registerId = transform.Find("Panel - Register/InputField (TMP) - RegisterID").GetComponent<TMP_InputField>();
        _registerPw = transform.Find("Panel - Register/InputField (TMP) - RegisterPW").GetComponent<TMP_InputField>();
        _nickName = transform.Find("Panel - Register/InputField (TMP) - NickName").GetComponent<TMP_InputField>();

        _confirm = transform.Find("Panel - Register/Button - OK").GetComponent<Button>();

        _errorMessage = transform.Find("Panel - Register/Text (TMP) - ErrorMessage").GetComponent<TMP_Text>();
    }

    private void Start()
    {
        _auth = FirebaseAuth.DefaultInstance;
        _databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

        _confirm.onClick.AddListener(SignUp);
    }

    public async void SignUp()
    {
        string userId = _registerId.text;
        string password = _registerPw.text;
        string nickname = _nickName.text;

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(nickname))
        {
            _errorMessage.text = "All fields must be filled!";
            return;
        }

        try
        {
            // Firebase 인증은 여전히 이메일과 비밀번호로 처리해야 함
            string email = $"{userId}@example.com";

            // 유저정보 만들기
            AuthResult authResult = await _auth.CreateUserWithEmailAndPasswordAsync(email, password);
            FirebaseUser newUser = authResult.User;

            // 파이어베이스에 닉네임 저장
            User newUserProfile = new User(nickname);
            string json = JsonUtility.ToJson(newUserProfile);

            await _databaseReference.Child("users").Child(userId).SetRawJsonValueAsync(json);
            _errorMessage.text = "User signed up and nickname saved!";
        }
        catch (System.Exception e)
        {
            _errorMessage.text = "Sign up error: " + e.Message;
        }


        Reset();
        _loginManager.OnClickRegisterButton();
    }

    public void Reset()
    {
        _registerId.text = null;
        _registerPw.text = null;
        _nickName.text = null;
        _errorMessage.text= null;
    }
}



[System.Serializable]
public class User
{
    public string nickname;

    public User(string nickname)
    {
        this.nickname = nickname;
    }
}
