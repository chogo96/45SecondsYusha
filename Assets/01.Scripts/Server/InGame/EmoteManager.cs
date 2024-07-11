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
    private void Awake()
    {
        _emotePanel = transform.Find("Panel - Emote").gameObject;
        _emotePanelOnOffButton = transform.Find("Button - Emote").GetComponent<Button>();
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

    /// <summary>
    /// �� �̸�Ƽ�� ��ư�� ���� �� ȣ��Ǵ� �Լ�
    /// </summary>
    /// <param name="emoteIndex">�̸�Ƽ�� �ε��� ��</param>
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

        // RPC ȣ��� ��� Ŭ���̾�Ʈ�� �̸�Ƽ�� ǥ��
        photonView.RPC("ShowEmote", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, emoteIndex);
        _emotePanelOnOff = false;
        _emotePanel.SetActive(false);
    }

    /// <summary>
    /// �̸�Ƽ�� ��ȯ �Լ�
    /// </summary>
    /// <param name="actorNumber">� ��ư�� ��������</param>
    /// <param name="emoteIndex"> �̸�Ƽ�� �ε��� ��</param>
    [PunRPC]
    void ShowEmote(int actorNumber, int emoteIndex)
    {
        Debug.Log("ShowEmote RPC called, actorNumber: " + actorNumber + ", emoteIndex: " + emoteIndex);

        if (emoteIndex < 0 || emoteIndex >= emotePrefabs.Length)
        {
            Debug.LogError("Invalid emote index");
            return;
        }

        // ���� ���� ��� �÷��̾���� �˻�
        foreach (var player in FindObjectsOfType<PlayerManager>())
        {
            if (player.photonView.Owner.ActorNumber == actorNumber)
            {
                // �÷��̾��� �Ӹ� ���� �̸�Ƽ�� ����
                Vector3 emotePosition = player.transform.position + Vector3.up * 2; // �Ӹ� ���� ��ġ
                GameObject emote = Instantiate(emotePrefabs[emoteIndex], emotePosition, Quaternion.identity);

                Debug.Log("Emote instantiated at position: " + emotePosition);

                // ���� �ð� �� �̸�Ƽ�� ����
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
