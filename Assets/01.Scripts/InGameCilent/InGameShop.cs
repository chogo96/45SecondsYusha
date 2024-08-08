using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameShop : MonoBehaviourPunCallbacks
{
    private Button _plusTime;
    private Button _plusCard;

    public TMP_Text _timeText;
    public TMP_Text _cardText;

    public int _plusTimeCount = 0;
    public int _plusCardCount = 0;
    public int _sumCount = 0;

    private bool _isAllDone = false;

    private EnemySpawner _enemySpawner;
    private UI_Timer uI_Timer;
    private GameManager gameManager;

    private void Awake()
    {
        _plusTime = transform.Find("AddTimeButton").GetComponent<Button>();
        _timeText = transform.Find("AddTimeButton/Text (TMP)").GetComponent<TMP_Text>();

        _plusCard = transform.Find("AddCardButton").GetComponent<Button>();
        _cardText = transform.Find("AddCardButton/Text (TMP)").GetComponent<TMP_Text>();

        _enemySpawner = FindObjectOfType<EnemySpawner>();
        uI_Timer = FindObjectOfType<UI_Timer>();
        gameManager = FindObjectOfType<GameManager>();
    }

    void Start()
    {
        _plusTime.interactable = true;
        _plusCard.interactable = true;

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
                _plusTimeCount++;
                break;
            case 1:
                _plusCardCount++;
                break;
            default:
                break;
        }
        gameManager.photonView.RPC("OnAddTimeOrCardButton", RpcTarget.All, _plusTimeCount, _plusCardCount);
    }


    public void ExecuteVotingResult(int plusCardVotes, int plusTimeVotes)
    {

        if (plusCardVotes > plusTimeVotes)
        {
            // 카드추가 투표가 더 많거나
            Debug.Log("카드가 더해짐");
            _isAllDone = true;
            AddRandomCardToDeck();
        }
        else if (plusCardVotes < plusTimeVotes)
        {
            // 시간추가 투표가 더 많거나
            Debug.Log("시간이 더해짐");
            uI_Timer.photonView.RPC("TimePlusMinus", RpcTarget.All, 10f);
            _isAllDone = true;
        }
        else
        {
            // 둘의 투표가 같다면 랜덤으로 결정
            int randomVoting = Random.Range(0, 2);
            if (PhotonNetwork.IsMasterClient)
            {
                if (randomVoting == 0)
                {
                    Debug.Log("시간이 더해짐");
                    uI_Timer.photonView.RPC("TimePlusMinus", RpcTarget.All, 10f);
                    _isAllDone = true;
                }
                else
                {
                    Debug.Log("카드가 더해짐");
                    _isAllDone = true;
                    AddRandomCardToDeck();
                }
            }
        }
        
        if (_isAllDone && PhotonNetwork.IsMasterClient)
        {
            _enemySpawner.OnShopButtonPressed();
            _isAllDone = false;
            Destroy(gameObject); // 상점 UI 제거
        }
    }

    private void AddRandomCardToDeck()
    {
        //덱이 안뽑힌다? 이거 참조를 바꿔야함!!!!!!!!!!!
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
