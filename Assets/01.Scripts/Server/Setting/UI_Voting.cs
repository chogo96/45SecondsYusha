using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Voting : MonoBehaviourPunCallbacks
{
    private UI_Setting ui_Setting;
    private Button _giveUp;
    private Button _yes;
    private Button _no;
    private GameObject _votingPanel;
    private TMP_Text _votingResults;
    private int _totalPlayers;
    private int _giveUpVotes = 0;
    private int _votesCount = 0;
    private int _noGiveUpVotes = 0;
    private GameOverManager gameOverManager;

    private void Awake()
    {
        _giveUp = transform.Find("Panel - SettingPopUp/Buttons/Button - GiveUp").GetComponent<Button>();
        _yes = transform.Find("Panel - GiveUpVote/Buttons/Button - GiveUp").GetComponent<Button>();
        _no = transform.Find("Panel - GiveUpVote/Buttons/Button - Cancel").GetComponent<Button>();
        _votingPanel = transform.Find("Panel - GiveUpVote").gameObject;
        _votingResults = transform.Find("Panel - GiveUpVote/Texts/Text (TMP) - VotingResults").GetComponent<TMP_Text>();
        gameOverManager = FindObjectOfType<GameOverManager>(); // GameOverManager를 찾습니다.
        ui_Setting = GetComponent<UI_Setting>();
    }

    private void Start()
    {
        _giveUp.onClick.AddListener(() => OnClickUiPopUpButtons());
        _yes.onClick.AddListener(() => OnGiveUpVoting(0));
        _no.onClick.AddListener(() => OnGiveUpVoting(1));
        _votingPanel.SetActive(false);
        _totalPlayers = PhotonNetwork.PlayerList.Length;
    }



    public void OnClickUiPopUpButtons()
    {
        photonView.RPC("GiveUpVote", RpcTarget.All);
        ui_Setting.SettingPanel.SetActive(false);
    }

    [PunRPC]
    public void GiveUpVote()
    {
        _votingPanel.SetActive(true);
    }

    private void OnGiveUpVoting(int vote)
    {
        _yes.interactable = false;
        _no.interactable = false;
        switch (vote)
        {
            case 0:
                _giveUpVotes++;

                break;
            case 1:
                _noGiveUpVotes++;

                break;
            default:
                break;
        }
        photonView.RPC("RegisterVote", RpcTarget.All, _giveUpVotes, _noGiveUpVotes);
    }

    [PunRPC]
    public void RegisterVote(int _giveUp, int noGiveUp)
    {
        _giveUpVotes = _giveUp;
        _noGiveUpVotes = noGiveUp;
        _votesCount = _giveUpVotes + _noGiveUpVotes;


        _votingResults.text = $"Agree: {_giveUpVotes} | Opposite: {_noGiveUpVotes}";

        if (_votesCount == _totalPlayers)
        {
            CheckVoteResult();
        }
    }

    private void CheckVoteResult()
    {
        if (_giveUpVotes > _totalPlayers / 2)
        {
            _votingPanel.SetActive(false);

            // 항복 시 DisplayLose 호출
            if (gameOverManager != null)
            {
                gameOverManager.DisplayLose();
            }
        }
        else
        {
            _votingPanel.SetActive(false);
            _giveUpVotes = 0;
            _votesCount = 0;
            _noGiveUpVotes = 0;
            _yes.interactable = true;
            _no.interactable = true;
        }
    }
}
