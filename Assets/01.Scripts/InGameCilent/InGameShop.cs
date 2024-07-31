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
            if (_plusCardCount > _plusTimeCount) // 카드추가 투표가 더 많거나
            {
                // 버튼2 클릭 시 처리할 로직
                Debug.Log("카드가 더해짐");
                if (_enemySpawner != null)
                {

                    _enemySpawner.OnShopButtonPressed();
                }
                Destroy(gameObject); // 상점 UI 제거
            }
            else if (_plusCardCount < _plusTimeCount) // 시간추가 투표가 더 많거나
            {
                // 버튼 1 클릭 시 처리할 로직
                Debug.Log("시간이 더해짐");

                uI_Timer.photonView.RPC("TimePlusMinus", RpcTarget.All, 10f);

                if (_enemySpawner != null)
                {
                    _enemySpawner.OnShopButtonPressed();
                }
                Destroy(gameObject); // 상점 UI 제거

            }
            else // 둘의 투표가 같다면
            {
                int randomVoting = Random.Range(0, 2);
                if(randomVoting == 0)
                {
                    // 버튼 1 클릭 시 처리할 로직
                    Debug.Log("시간이 더해짐");

                    uI_Timer.photonView.RPC("TimePlusMinus", RpcTarget.All, 10f);

                    if (_enemySpawner != null)
                    {
                        _enemySpawner.OnShopButtonPressed();
                    }
                    Destroy(gameObject); // 상점 UI 제거
                }
                else
                {
                    // 버튼2 클릭 시 처리할 로직
                    Debug.Log("카드가 더해짐");
                    if (_enemySpawner != null)
                    {

                        _enemySpawner.OnShopButtonPressed();
                    }
                    Destroy(gameObject); // 상점 UI 제거
                }
            }
        }
       
    }
}
