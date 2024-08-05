using Firebase.Auth;
using Firebase.Database;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    // 게임 오브젝트
    private GameObject _tutorialPanel; // 패널 위치

    // 리스트
    public List<Sprite> images;  // 이미지 리스트를 Unity Inspector에서 설정

    // 이미지
    private Image _displayImage;   // 이미지를 표시할 UI Image

    // 버튼들
    private Button _frontButton; // 다음 버튼
    private Button _backButton;  // 이전 버튼
    private Button _exitButton;  // 나가기 버튼
    private Button _tutorialOpen;  // 튜토리얼 열기 버튼

    // 자료형
    private int currentIndex = 0; // 리스트의 인덱스 번호

    // 파이어베이스
    private FirebaseAuth auth;
    private DatabaseReference databaseReference;
    private FirebaseUser user;

    private void Awake()
    {
        _tutorialPanel = transform.Find("Panel - Tutorial").gameObject;

        _displayImage = transform.Find("Panel - Tutorial/Image - Tutorial").GetComponent<Image>();

        _frontButton = transform.Find("Panel - Tutorial/Button - Front").GetComponent<Button>();
        _backButton = transform.Find("Panel - Tutorial/Button - Back").GetComponent<Button>();
        _exitButton = transform.Find("Panel - Tutorial/Button - Exit").GetComponent<Button>();
        _tutorialOpen = transform.Find("Button - TutorialOpen").GetComponent<Button>();


        auth = FirebaseAuth.DefaultInstance;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    void Start()
    {
        _frontButton.onClick.AddListener(() => ShowNextOrPreviousImage(0));
        _backButton.onClick.AddListener(() => ShowNextOrPreviousImage(1));
        _exitButton.onClick.AddListener(CompleteTutorial);
        _tutorialOpen.onClick.AddListener(OnClickTutorialOpen);

        CheckTutorialStatus();

        if (images.Count > 0)
        {
            _displayImage.sprite = images[currentIndex];
        }
        UpdateButtons();
    }

    /// <summary>
    /// 튜토리얼 했는지를 파이어베이스의 TutorialCompleted의 bool 값으로 확인후 패널 온오프 하는 함수
    /// </summary>
    private async void CheckTutorialStatus()
    {
        user = auth.CurrentUser;
        if (user == null)
        {
            Utils.LogRed("User not logged in.");
            return;
        }

        string userId = LoginManager.UserId;
        try
        {
            DataSnapshot snapshot = await databaseReference.Child("users").Child(userId).Child("TutorialCompleted").GetValueAsync();

            if (snapshot.Exists)
            {
                bool tutorialCompleted = (bool)snapshot.Value;
                Utils.Log("Tutorial Completed: " + tutorialCompleted);

                _tutorialPanel.SetActive(!tutorialCompleted);
                Utils.Log("Tutorial Panel should be active now: " + !tutorialCompleted);
            }
            else
            {
                Utils.Log("TutorialCompleted value does not exist.");
                _tutorialPanel.SetActive(true);
            }
        }
        catch (System.Exception e)
        {
            Utils.LogRed("Failed to check tutorial status from Firebase: " + e);
            _tutorialPanel.SetActive(true);
        }
    }

    /// <summary>
    /// 튜토리얼 완료했을때 파이어베이스에 등록하는 곳 + 나가기버튼 눌렀을때 실행됨
    /// </summary>
    public void CompleteTutorial()
    {
        string userId = LoginManager.UserId;
        databaseReference.Child("users").Child(userId).Child("TutorialCompleted").SetValueAsync(true).ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
            {
                _tutorialPanel.SetActive(false);
            }
            else
            {
                Utils.LogRed("Failed to set tutorial status.");
            }
        });
        _tutorialPanel.SetActive(false);
    }

    /// <summary>
    /// 다음 이미지 , 이전 이미지 보여주는곳
    /// </summary>
    /// <param name="num">0 = 다음이미지, 1 = 이전이미지</param>
    void ShowNextOrPreviousImage(int num)
    {
        switch (num)
        {
            case 0:
                if (currentIndex < images.Count - 1)
                {
                    currentIndex++;
                    _displayImage.sprite = images[currentIndex];
                }
                break;
            case 1:
                if (currentIndex > 0)
                {
                    currentIndex--;
                    _displayImage.sprite = images[currentIndex];
                }
                break;
            default:
                break;
        }

        UpdateButtons();
    }

    /// <summary>
    /// 버튼(인덱스) 값 조정하는곳
    /// </summary>
    void UpdateButtons()
    {
        _backButton.interactable = currentIndex > 0;
        _frontButton.interactable = currentIndex < images.Count - 1;
    }

    /// <summary>
    /// 튜토리얼 다시보는 버튼 눌렀을때 실행할 함수.
    /// </summary>
    private void OnClickTutorialOpen()
    {
        _tutorialPanel.SetActive(true);
    }
}
