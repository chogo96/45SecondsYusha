using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameShop: MonoBehaviourPunCallbacks
{
    private Button _plusTime;
    private Button _plusCard;

    private TMP_Text _timeText;
    private TMP_Text _cardText;

    private int _plusTimeCount = 0;
    private int _plusCardCount = 0;

    private bool _isALlVoting = false;

    private EnemySpawner _enemySpawner;
    private UI_Timer uI_Timer;

    private void Awake()
    {
        _plusTime = transform.Find("AddTimeButton").GetComponent<Button>();
        _timeText = transform.Find("AddCardButton/Text (TMP)").GetComponent<TMP_Text>();


        _plusCard = transform.Find("AddCardButton").GetComponent<Button>();
        _cardText = transform.Find("AddCardButton/Text (TMP)").GetComponent<TMP_Text>();
    }
    void Start()
    {
        _plusTime.interactable = true;
        _plusCard.interactable = true;

        _enemySpawner = FindObjectOfType<EnemySpawner>();
        uI_Timer = FindObjectOfType<UI_Timer>();

        _plusTime.onClick.AddListener(() => VotingPlusCardOrPlusTime(0));
        _plusCard.onClick.AddListener(() => VotingPlusCardOrPlusTime(1));
    }

    public void VotingPlusCardOrPlusTime(int voteNum)
    {
        _plusTime.interactable = false;
        _plusCard.interactable = false;

        switch (voteNum)
        {
            case 0:
                photonView.RPC("OnAddTimeOrCardButton", RpcTarget.All, "plusTime");
                break;
            case 1:
                photonView.RPC("OnAddTimeOrCardButton", RpcTarget.All, "plusCard");
                break;
        }
    }

    [PunRPC]
    void OnAddTimeOrCardButton(string votingName)
    {

        switch (votingName)
        {
            case "plusTime":
                _timeText.text = $"AddTime\nVote : {_plusTimeCount + 1}";
                break;
            case "plusCard":
                _cardText.text = $"AddTime\nVote : {_plusCardCount + 1}";
                break;
        }


        if (PhotonNetwork.PlayerList.Length == (_plusTimeCount + _plusCardCount))    _isALlVoting = true;
        if (_isALlVoting && PhotonNetwork.IsMasterClient)
        {
            if (_plusCardCount > _plusTimeCount) // ī���߰� ��ǥ�� �� ���ų�
            {
                // ��ư2 Ŭ�� �� ó���� ����
                Debug.Log("ī�尡 ������");
                if (_enemySpawner != null)
                {

                    _enemySpawner.OnShopButtonPressed();
                }
                Destroy(gameObject); // ���� UI ����
            }
            else if (_plusCardCount < _plusTimeCount) // �ð��߰� ��ǥ�� �� ���ų�
            {
                // ��ư 1 Ŭ�� �� ó���� ����
                Debug.Log("�ð��� ������");

                uI_Timer.photonView.RPC("TimePlusMinus", RpcTarget.All, 10f);

                if (_enemySpawner != null)
                {
                    _enemySpawner.OnShopButtonPressed();
                }
                Destroy(gameObject); // ���� UI ����

            }
            else // ���� ��ǥ�� ���ٸ�
            {
                int randomVoting = Random.Range(0, 2);
                if(randomVoting == 0)
                {
                    // ��ư 1 Ŭ�� �� ó���� ����
                    Debug.Log("�ð��� ������");

                    uI_Timer.photonView.RPC("TimePlusMinus", RpcTarget.All, 10f);

                    if (_enemySpawner != null)
                    {
                        _enemySpawner.OnShopButtonPressed();
                    }
                    Destroy(gameObject); // ���� UI ����
                }
                else
                {
                    // ��ư2 Ŭ�� �� ó���� ����
                    Debug.Log("ī�尡 ������");
                    if (_enemySpawner != null)
                    {

                        _enemySpawner.OnShopButtonPressed();
                    }
                    Destroy(gameObject); // ���� UI ����
                }
            }
        }
       
    }
}
