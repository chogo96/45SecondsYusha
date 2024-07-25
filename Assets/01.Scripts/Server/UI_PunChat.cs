using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PunChat : MonoBehaviour
{
    public GameObject ChatPrefab;
    private TMP_InputField _chatInputField;
    private Transform _trContent;
    private ScrollRect _scrollRect;
    private PunChatManager _punChatManager;

    private void Awake()
    {
        _chatInputField = transform.Find("PunChat/InputField (TMP)").GetComponent<TMP_InputField>();
        _trContent = transform.Find("PunChat/Scroll View/Viewport/Content").transform;
        _scrollRect = transform.Find("PunChat/Scroll View").GetComponent<ScrollRect>();

        _punChatManager = FindObjectOfType<PunChatManager>();
    }

    private void OnEnable()
    {
        _punChatManager.OnMessageReceived += DisplayChatMessage;
    }

    private void OnDisable()
    {
        _punChatManager.OnMessageReceived -= DisplayChatMessage;
    }

    private void Start()
    {
        _chatInputField.onEndEdit.AddListener(OnEndEdit);
    }

    private void SendMessage()
    {
        if (!string.IsNullOrEmpty(_chatInputField.text))
        {
            _punChatManager.SendChatMessage(_chatInputField.text);
            _chatInputField.text = string.Empty;
            _chatInputField.ActivateInputField(); // 다시 입력 필드를 활성화
        }
    }

    private void OnEndEdit(string input)
    {
        if (Input.GetKeyDown(KeyCode.Return)) // 엔터 키 확인
        {
            SendMessage();
        }
    }

    public void DisplayChatMessage(string sender, string message)
    {
        GameObject newMessage = Instantiate(ChatPrefab, _trContent);
        TMP_Text messageText = newMessage.GetComponent<TMP_Text>();
        messageText.text = $"{sender}: {message}";
        Canvas.ForceUpdateCanvases();
        _scrollRect.verticalNormalizedPosition = 0f; // 자동 스크롤
    }
}
