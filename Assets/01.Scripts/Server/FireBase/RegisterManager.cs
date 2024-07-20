using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;
using Photon.Pun;

public class RegisterManager : MonoBehaviour
{
    private TMP_InputField _registerId;
    private TMP_InputField _registerPw;
    private TMP_InputField _nickName;

    private Button _confirm;

    private TMP_Text _errorMessage;

    private FirebaseAuth auth;
    private DatabaseReference databaseReference;
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


        auth = FirebaseAuth.DefaultInstance;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

        _confirm.onClick.AddListener(SignUp);
    }

    public async void SignUp()
    {
        string email = _registerId.text;
        string password = _registerPw.text;
        string nickname = _nickName.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(nickname))
        {
            _errorMessage.text = "All fields must be filled!";
            return;
        }

        try
        {
            // Create the user
            AuthResult authResult = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            FirebaseUser newUser = authResult.User;

            // Save nickname to database
            User newUserProfile = new User(nickname);
            string json = JsonUtility.ToJson(newUserProfile);

            // 이메일을 키로 사용하기 위해 인코딩
            string encodedEmail = EncodeEmail(email);

            await databaseReference.Child("users").Child(encodedEmail).SetRawJsonValueAsync(json);
            _errorMessage.text = "User signed up and nickname saved!";
        }
        catch (System.Exception e)
        {
            _errorMessage.text = "Sign up error: " + e.Message;
        }
    }

    private string EncodeEmail(string email)
    {
        return email.Replace(".", ",");
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
