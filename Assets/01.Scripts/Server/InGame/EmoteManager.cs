using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class EmoteManager : MonoBehaviourPun
{
    public GameObject[] emotePrefabs; // 6���� �̸�Ƽ�� �������� �迭�� ����
    private GameObject _emotePanel; // �¿��� ��ų �г� ��ġ
    private Button _emotePanelOnOffButton; // �г� �¿��� ��ư 
    private bool _emotePanelOnOff = false; // �г� �¿��� ��Ȳ �ʱⰪ false(��Ȱ��ȭ)
    private Button[] _emoteButtons;  // �̸�Ƽ�� ��ư �迭ȭ

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

        // ��ư �迭 �ʱ�ȭ
        _emoteButtons = new Button[6];

        // ��ư ã�� �� �迭�� �߰�
        for (int i = 0; i < _emoteButtons.Length; i++)
        {
            string buttonName = $"Panel - Emote/Button - Emote{i + 1}";
            _emoteButtons[i] = transform.Find(buttonName).GetComponent<Button>();

            int index = i; // ���� ������ �ε��� ����
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

        // RPC ȣ��� ��� Ŭ���̾�Ʈ�� �̸�Ƽ�� ǥ��
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

        Vector3 playerPosition = targetObject.transform.localPosition; // ���� ��ǥ ���
        Vector3 emotePosition = GetEmotePosition(playerPosition);

        GameObject emote = Instantiate(emotePrefabs[emoteIndex], emotePosition, Quaternion.identity, _canvas.transform);
        emote.transform.localPosition = emotePosition;

        //RectTransform emoteRectTransform = emote.GetComponent<RectTransform>();
        //if (emoteRectTransform != null)
        //{
        //    emoteRectTransform.localScale = Vector3.one; // �������� 1�� ����
        //    emoteRectTransform.anchoredPosition = emotePosition; // anchoredPosition ���
        //}

        Utils.LogGreen("objectName: " + objectName);
        Utils.LogGreen("targetObject: " + targetObject);
        Utils.LogGreen("playerPosition: " + playerPosition);
        Utils.LogGreen("emotePosition: " + emotePosition);

        // ���� �ð� �� �̸�Ƽ�� ����
        Destroy(emote, 3f);
    }

    private Vector3 GetEmotePosition(Vector3 playerPosition)
    {
        if (playerPosition == new Vector3(-961, -536, 0))
        {
            Utils.LogGreen("playerPosition: 1");
            return new Vector3(-601, -99, -256); // Ư�� ��ġ
        }
        else if (playerPosition == new Vector3(-961, 16, 0))
        {
            Utils.LogGreen("playerPosition: 2");
            return new Vector3(-363, 171, -256); // Ư�� ��ġ
        }
        else if (playerPosition == new Vector3(91, 218, 0))
        {
            Utils.LogGreen("playerPosition: 3");
            return new Vector3(401, 304, -256); // Ư�� ��ġ
        }
        else if (playerPosition == new Vector3(597, -167, 0))
        {
            Utils.LogGreen("playerPosition: 4");
            return new Vector3(389, 28, -256); // Ư�� ��ġ
        }

        // �⺻ ��ġ (�ʿ��� ��� �ٸ� �⺻�� ����)
        Utils.LogGreen("playerPosition: XXXXXXXXXX");
        return Vector3.zero;
    }

    private void OnClickOnOffPanelButton()
    {
        _emotePanelOnOff = !_emotePanelOnOff;
        _emotePanel.SetActive(_emotePanelOnOff);
    }
}
