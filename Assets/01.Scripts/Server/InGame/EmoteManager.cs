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
    private void Awake()
    {
        _emotePanel = transform.Find("Panel - Emote").gameObject;
        _emotePanelOnOffButton = transform.Find("Button - Emote").GetComponent<Button>();
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

    /// <summary>
    /// 각 이모티콘 버튼을 누를 때 호출되는 함수
    /// </summary>
    /// <param name="emoteIndex">이모티콘 인덱스 값</param>
    private void OnEmoteButtonClicked(int emoteIndex)
    {
        Debug.Log("Emote button clicked, index: " + emoteIndex);

        // Ensure photonView is properly assigned
        if (photonView == null)
        {
            Debug.LogError("PhotonView component is missing on EmoteManager object.");
            return;
        }

        if (PhotonNetwork.LocalPlayer == null)
        {
            Debug.LogError("Local player is not yet connected to Photon Network.");
            return;
        }

        // RPC 호출로 모든 클라이언트에 이모티콘 표시
        photonView.RPC("ShowEmote", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, emoteIndex);
        _emotePanelOnOff = false;
        _emotePanel.SetActive(false);
    }

    /// <summary>
    /// 이모티콘 소환 함수
    /// </summary>
    /// <param name="actorNumber">어떤 버튼을 눌렀는지</param>
    /// <param name="emoteIndex"> 이모티콘 인덱스 값</param>
    [PunRPC]
    void ShowEmote(int actorNumber, int emoteIndex)
    {
        Debug.Log("ShowEmote RPC called, actorNumber: " + actorNumber + ", emoteIndex: " + emoteIndex);

        if (emoteIndex < 0 || emoteIndex >= emotePrefabs.Length)
        {
            Debug.LogError("Invalid emote index");
            return;
        }

        // 현재 방의 모든 플레이어들을 검색
        foreach (var player in FindObjectsOfType<PlayerManager>())
        {
            if (player.photonView.Owner.ActorNumber == actorNumber)
            {
                // 플레이어의 머리 위에 이모티콘 생성
                Vector3 emotePosition = player.transform.position + Vector3.up * 2; // 머리 위의 위치
                GameObject emote = Instantiate(emotePrefabs[emoteIndex], emotePosition, Quaternion.identity);

                Debug.Log("Emote instantiated at position: " + emotePosition);

                // 일정 시간 후 이모티콘 삭제
                Destroy(emote, 3f);
                break;
            }
        }
    }

    private void OnClickOnOffPanelButton()
    {
        _emotePanelOnOff = !_emotePanelOnOff;
        _emotePanel.SetActive(_emotePanelOnOff);
    }
}
