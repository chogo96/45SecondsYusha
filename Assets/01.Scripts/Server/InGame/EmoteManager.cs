using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class EmoteManager : MonoBehaviourPun
{
    public GameObject[] emotePrefabs; // 6개의 이모티콘 프리팹을 배열로 저장
    private GameObject _emotePanel; // 온오프 시킬 패널 위치
    private Button _emotePanelOnOffButton; // 패널 온오프 버튼 
    private bool _emotePanelOnOff = false; // 패널 온오프 상황 초기값 false(비활성화)
    private Button[] _emoteButtons;  // 이모티콘 버튼 배열화

    public static PlayerManager LocalPlayerInstance;

    private Canvas _canvas;

    private void Awake()
    {
        _emotePanel = transform.Find("Panel - Emote").gameObject;
        _emotePanelOnOffButton = transform.Find("Button - Emote").GetComponent<Button>();

        _canvas = GetComponentInParent<Canvas>();
        if (_canvas == null)
        {
            Utils.LogRed("Canvas not found in parent hierarchy.");
        }
    }

    private void Start()
    {
        _emotePanelOnOffButton.onClick.AddListener(OnClickOnOffPanelButton);

        // 버튼 배열 초기화
        _emoteButtons = new Button[6];

        // 버튼 찾기 및 배열에 추가
        for (int i = 0; i < _emoteButtons.Length; i++)
        {
            string buttonName = $"Panel - Emote/Button - Emote{i + 1}";
            _emoteButtons[i] = transform.Find(buttonName).GetComponent<Button>();

            int index = i; // 로컬 변수로 인덱스 저장
            _emoteButtons[i].onClick.AddListener(() => OnEmoteButtonClicked(index));
        }
        _emotePanel.SetActive(false);
    }

    private void OnEmoteButtonClicked(int emoteIndex)
    {
        Utils.Log("Emote button clicked, index: " + emoteIndex);

        // Ensure photonView is properly assigned
        if (photonView == null)
        {
            Utils.LogRed("PhotonView component is missing on EmoteManager object.");
            return;
        }

        if (PhotonNetwork.LocalPlayer == null)
        {
            Utils.LogRed("Local player is not yet connected to Photon Network.");
            return;
        }

        // RPC 호출로 모든 클라이언트에 이모티콘 표시
        photonView.RPC("ShowEmote", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName, emoteIndex, PhotonNetwork.LocalPlayer.ActorNumber);
        _emotePanelOnOff = false;
        _emotePanel.SetActive(false);
    }

    [PunRPC]
    void ShowEmote(string userNickname, int emoteIndex, int acterNumber)
    {
        Utils.Log("ShowEmote RPC called, userId: " + userNickname + ", emoteIndex: " + emoteIndex);

        if (emoteIndex < 0 || emoteIndex >= emotePrefabs.Length)
        {
            Utils.LogRed("Invalid emote index");
            return;
        }

        string objectName = $"{userNickname}_{acterNumber}";
        GameObject targetObject = GameObject.Find(objectName);

        if (targetObject == null)
        {
            Utils.LogRed("Target object not found: " + objectName);
            return;
        }

        Vector3 playerPosition = targetObject.transform.localPosition; // 로컬 좌표 사용
        Vector3 emotePosition = GetEmotePosition(playerPosition);

        GameObject emote = Instantiate(emotePrefabs[emoteIndex], emotePosition, Quaternion.identity, _canvas.transform);
        emote.transform.localPosition = emotePosition;

        //RectTransform emoteRectTransform = emote.GetComponent<RectTransform>();
        //if (emoteRectTransform != null)
        //{
        //    emoteRectTransform.localScale = Vector3.one; // 스케일을 1로 설정
        //    emoteRectTransform.anchoredPosition = emotePosition; // anchoredPosition 사용
        //}

        Utils.LogGreen("objectName: " + objectName);
        Utils.LogGreen("targetObject: " + targetObject);
        Utils.LogGreen("playerPosition: " + playerPosition);
        Utils.LogGreen("emotePosition: " + emotePosition);

        // 일정 시간 후 이모티콘 삭제
        Destroy(emote, 3f);
    }

    private Vector3 GetEmotePosition(Vector3 playerPosition)
    {
        if (playerPosition == new Vector3(-961, -536, 0))
        {
            Utils.LogGreen("playerPosition: 1");
            return new Vector3(-601, -99, -256); // 특정 위치
        }
        else if (playerPosition == new Vector3(-961, 16, 0))
        {
            Utils.LogGreen("playerPosition: 2");
            return new Vector3(-363, 171, -256); // 특정 위치
        }
        else if (playerPosition == new Vector3(91, 218, 0))
        {
            Utils.LogGreen("playerPosition: 3");
            return new Vector3(401, 304, -256); // 특정 위치
        }
        else if (playerPosition == new Vector3(597, -167, 0))
        {
            Utils.LogGreen("playerPosition: 4");
            return new Vector3(389, 28, -256); // 특정 위치
        }

        // 기본 위치 (필요한 경우 다른 기본값 설정)
        Utils.LogGreen("playerPosition: XXXXXXXXXX");
        return Vector3.zero;
    }

    private void OnClickOnOffPanelButton()
    {
        _emotePanelOnOff = !_emotePanelOnOff;
        _emotePanel.SetActive(_emotePanelOnOff);
    }
}
