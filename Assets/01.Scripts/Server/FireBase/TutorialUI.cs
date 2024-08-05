using Firebase.Auth;
using Firebase.Database;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    // ���� ������Ʈ
    private GameObject _tutorialPanel; // �г� ��ġ

    // ����Ʈ
    public List<Sprite> images;  // �̹��� ����Ʈ�� Unity Inspector���� ����

    // �̹���
    private Image _displayImage;   // �̹����� ǥ���� UI Image

    // ��ư��
    private Button _frontButton; // ���� ��ư
    private Button _backButton;  // ���� ��ư
    private Button _exitButton;  // ������ ��ư
    private Button _tutorialOpen;  // Ʃ�丮�� ���� ��ư

    // �ڷ���
    private int currentIndex = 0; // ����Ʈ�� �ε��� ��ȣ

    // ���̾�̽�
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
    /// Ʃ�丮�� �ߴ����� ���̾�̽��� TutorialCompleted�� bool ������ Ȯ���� �г� �¿��� �ϴ� �Լ�
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
    /// Ʃ�丮�� �Ϸ������� ���̾�̽��� ����ϴ� �� + �������ư �������� �����
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
    /// ���� �̹��� , ���� �̹��� �����ִ°�
    /// </summary>
    /// <param name="num">0 = �����̹���, 1 = �����̹���</param>
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
    /// ��ư(�ε���) �� �����ϴ°�
    /// </summary>
    void UpdateButtons()
    {
        _backButton.interactable = currentIndex > 0;
        _frontButton.interactable = currentIndex < images.Count - 1;
    }

    /// <summary>
    /// Ʃ�丮�� �ٽú��� ��ư �������� ������ �Լ�.
    /// </summary>
    private void OnClickTutorialOpen()
    {
        _tutorialPanel.SetActive(true);
    }
}
