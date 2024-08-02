using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameShop : MonoBehaviourPunCallbacks
{
    private Button _plusTime;
    private Button _plusCard;

    private TMP_Text _timeText;
    private TMP_Text _cardText;

    private int _plusTimeCount = 0;
    private int _plusCardCount = 0;

    private bool _isAllVoting = false;
    private bool _isAllDone = false;

    private EnemySpawner _enemySpawner;
    private UI_Timer uI_Timer;

    private void Awake()
    {
        _plusTime = transform.Find("AddTimeButton").GetComponent<Button>();
        _timeText = transform.Find("AddTimeButton/Text (TMP)").GetComponent<TMP_Text>();

        _plusCard = transform.Find("AddCardButton").GetComponent<Button>();
        _cardText = transform.Find("AddCardButton/Text (TMP)").GetComponent<TMP_Text>();
    }

    void Start()
    {
        _plusTime.interactable = true;
        _plusCard.interactable = true;

        _enemySpawner = FindObjectOfType<EnemySpawner>();
        uI_Timer = FindObjectOfType<UI_Timer>();

        if (uI_Timer == null || uI_Timer.photonView == null)
        {
            Debug.LogError("UI_Timer or its PhotonView component is not found.");
            return;
        }

        _plusTime.onClick.AddListener(() => VotingPlusCardOrPlusTime(0));
        _plusCard.onClick.AddListener(() => VotingPlusCardOrPlusTime(1));

        _plusTimeCount = 0;
        _plusCardCount = 0;
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
                _plusTimeCount++;
                break;
            case "plusCard":
                _plusCardCount++;
                break;
        }
        photonView.RPC("ExecuteVotingResult", RpcTarget.All, _plusCardCount, _plusTimeCount);

    }

    [PunRPC]
    private void ExecuteVotingResult(int plusCardVotes, int plusTimeVotes)
    {
        _plusCardCount = plusCardVotes;
        _plusTimeCount = plusTimeVotes;

        _timeText.text = $"AddTime\nVote : {_plusTimeCount}";
        _cardText.text = $"AddCard\nVote : {_plusCardCount}";

        if (PhotonNetwork.PlayerList.Length == (_plusTimeCount + _plusCardCount))
        {
            _isAllVoting = true;
        }
        if (_isAllVoting)
        {
            if (plusCardVotes > plusTimeVotes)
            {
                // ī���߰� ��ǥ�� �� ���ų�
                Debug.Log("ī�尡 ������");
                AddRandomCardToDeck();
                _isAllDone = true;
            }

            else if (plusCardVotes < plusTimeVotes && PhotonNetwork.IsMasterClient)
            {
                // �ð��߰� ��ǥ�� �� ���ų�
                Debug.Log("�ð��� ������");
                uI_Timer.photonView.RPC("TimePlusMinus", RpcTarget.All, 10f);
                _isAllDone = true;
            }
            else
            {
                // ���� ��ǥ�� ���ٸ� �������� ����
                int randomVoting = Random.Range(0, 2);
                if (PhotonNetwork.IsMasterClient)
                {

                    if (randomVoting == 0)
                    {
                        Debug.Log("�ð��� ������");
                        uI_Timer.photonView.RPC("TimePlusMinus", RpcTarget.All, 10f);
                        _isAllDone = true;
                    }
                }

                else
                {
                    Debug.Log("ī�尡 ������");
                    AddRandomCardToDeck();
                    _isAllDone = true;
                }
            }
        }
        if (_isAllDone && PhotonNetwork.IsMasterClient)
        {
            _enemySpawner.OnShopButtonPressed();
            _isAllDone = false;
            _isAllVoting = false;
            Destroy(gameObject); // ���� UI ����
        }
        

    }

    private void AddRandomCardToDeck()
    {
        //���� �Ȼ�����? �̰� ������ �ٲ����!!!!!!!!!!!
        Deck playerDeck = FindObjectOfType<Deck>();
        if (playerDeck != null)
        {
            playerDeck.ReturnRandomCardsFromDiscard(10);
        }
        else
        {
            Debug.LogError("Player deck not found.");
        }
    }
}
