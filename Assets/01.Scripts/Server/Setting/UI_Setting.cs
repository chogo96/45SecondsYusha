using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//public class UI_Setting : MonoBehaviourPunCallbacks
//{
//    private Button _setting;
//    private Button _giveUp;
//    private Button _cancel;
//    private Button _save;
//    // -----------------------------
//    private Button _yes;
//    private Button _no;


//    private GameObject _settingPanel;
//    // -----------------------------
//    private GameObject _votingPanel;


//    private TMP_Text _votingResults;


//    private int _totalPlayers;
//    private int _giveUpVotes = 0;
//    private int _votesCount = 0;
//    private int _noGiveUpVotes = 0;




//    private void Awake()
//    {
//        _setting = transform.Find("Button - Setting").GetComponent<Button>();
//        _giveUp = transform.Find("Panel - SettingPopUp/Buttons/Button - GiveUp").GetComponent<Button>();
//        _cancel = transform.Find("Panel - SettingPopUp/Buttons/Button - Cancel").GetComponent<Button>();
//        _save = transform.Find("Panel - SettingPopUp/Buttons/Button - Save").GetComponent<Button>();
//        // ----------------------------------------------
//        _yes = transform.Find("Panel - GiveUpVote/Buttons/Button - GiveUp").GetComponent<Button>();
//        _no = transform.Find("Panel - GiveUpVote/Buttons/Button - Cancel").GetComponent<Button>();


//        _settingPanel = transform.Find("Panel - SettingPopUp").gameObject;
//        // ----------------------------------------------
//        _votingPanel = transform.Find("Panel - GiveUpVote").gameObject;


//        _votingResults = transform.Find("Panel - GiveUpVote/Texts/Text (TMP) - VotingResults").GetComponent<TMP_Text>();

//    }

//    private void Start()
//    {
//        _setting.onClick.AddListener(OnCLickSettingButton);
//        _giveUp.onClick.AddListener(() => OnClickUiPopUpButtons(0));
//        _cancel.onClick.AddListener(() => OnClickUiPopUpButtons(2));
//        _save.onClick.AddListener(() => OnClickUiPopUpButtons(1));
//        // ----------------------------------------------------------
//        _yes.onClick.AddListener(() => OnGiveUpVoring(0));
//        _no.onClick.AddListener(() => OnGiveUpVoring(1));

//        _settingPanel.SetActive(false);
//        _votingPanel.SetActive(false);

//        _totalPlayers = PhotonNetwork.PlayerList.Length;
//    }


//    /// <summary>
//    /// 단순 설정버튼을 눌렀을때 실행될 함수.
//    /// </summary>
//    public void OnCLickSettingButton()
//    {
//        _settingPanel.SetActive(true);
//    }

//    /// <summary>
//    /// 팝업창 안에 있는 버튼을 모아둔 곳
//    /// </summary>
//    /// <param name="num">실행할 번호(0번 : 항복 | 1번 : 세이브 | 2번 : 취소)</param>
//    public void OnClickUiPopUpButtons(int num)
//    {
//        switch (num)
//        {
//            case 0:
//                // 항복 버튼 눌렀을때 실행될 내용.
//                photonView.RPC("GiveUpVote", RpcTarget.All);
//                _settingPanel.SetActive(false);

//                break;
//            case 1:
//                // 저장 버튼 눌렀을때 실행될 내용.

//                break;
//            case 2:
//                // 취소 버튼 눌렀을때 실행될 내용.
//                _settingPanel.SetActive(false);
//                break;
//        }
//    }

//    /// <summary>
//    /// 항복 버튼 눌렀을때 실행될 내용
//    /// </summary>
//    [PunRPC]
//    public void GiveUpVote()
//    {
//        // 팝업 창을 활성화하고, 텍스트와 버튼을 설정합니다.
//        _votingPanel.SetActive(true);
//    }


//    /// <summary>
//    /// 항복 투표 여부
//    /// </summary>
//    /// <param name="vote">투표 (0번 : Yes | 1번 : NO)</param>
//    private void OnGiveUpVoring(int vote)
//    {
//        switch (vote)
//        {
//            case 0:
//                // 항복 버튼 클릭 시 서버에 항복 투표를 알립니다.
//                photonView.RPC("RegisterVote", RpcTarget.MasterClient, true);
//                break;
//            case 1:
//                // 취소 버튼 클릭 시 서버에 거절 투표를 알립니다.
//                photonView.RPC("RegisterVote", RpcTarget.MasterClient, false); 
//                break;
//        }
//        _yes.interactable = false;
//        _no.interactable = false;
//    }

//    /// <summary>
//    /// 투표 집계 및 씬 변경하는 함수
//    /// </summary>
//    /// <param name="giveUp"> 항복 투표 bool값 (True = 항복찬성 | False : 항복반대)</param>
//    [PunRPC]
//    public void RegisterVote(bool giveUp)
//    {
//        _votesCount++;
//        if (giveUp)
//        {
//            _giveUpVotes++;
//        }
//        else
//        {
//            _noGiveUpVotes++;
//        }

//        _votingResults.text = $"Agree : {_giveUpVotes} | opposite : {_noGiveUpVotes}";

//        // 모든 플레이어가 투표를 완료했는지 확인
//        if (_votesCount == _totalPlayers)
//        {
//            CheckVoteResult();
//        }
//    }

//    private void CheckVoteResult()
//    {
//        // 과반수 이상이 항복을 선택했는지 확인
//        if (_giveUpVotes > _totalPlayers / 2)
//        {
//            // 항복 이팩트 여기다 만들면 됨

//            _votingPanel.SetActive(false);
//            // 씬 변경
//            PhotonNetwork.LoadLevel("NewSceneName"); // 항복 시 변경할 씬 이름
//        }
//        else
//        {
//            // 패널 숨기기
//            _votingPanel.SetActive(false);

//            // 투표 초기화
//            _giveUpVotes = 0;
//            _votesCount = 0;
//            _noGiveUpVotes = 0;

//        }
//    }

//}
using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Setting : MonoBehaviourPunCallbacks
{
    private Button _setting;
    private Button _giveUp;
    private Button _cancel;
    private Button _save;
    private Button _yes;
    private Button _no;

    private GameObject _settingPanel;
    private GameObject _votingPanel;

    private TMP_Text _votingResults;

    private int _totalPlayers;
    private int _giveUpVotes = 0;
    private int _votesCount = 0;
    private int _noGiveUpVotes = 0;

    private void Awake()
    {
        _setting = transform.Find("Button - Setting").GetComponent<Button>();
        _giveUp = transform.Find("Panel - SettingPopUp/Buttons/Button - GiveUp").GetComponent<Button>();
        _cancel = transform.Find("Panel - SettingPopUp/Buttons/Button - Cancel").GetComponent<Button>();
        _save = transform.Find("Panel - SettingPopUp/Buttons/Button - Save").GetComponent<Button>();
        _yes = transform.Find("Panel - GiveUpVote/Buttons/Button - GiveUp").GetComponent<Button>();
        _no = transform.Find("Panel - GiveUpVote/Buttons/Button - Cancel").GetComponent<Button>();

        _settingPanel = transform.Find("Panel - SettingPopUp").gameObject;
        _votingPanel = transform.Find("Panel - GiveUpVote").gameObject;

        _votingResults = transform.Find("Panel - GiveUpVote/Texts/Text (TMP) - VotingResults").GetComponent<TMP_Text>();
    }

    private void Start()
    {
        _setting.onClick.AddListener(OnCLickSettingButton);
        _giveUp.onClick.AddListener(() => OnClickUiPopUpButtons(0));
        _cancel.onClick.AddListener(() => OnClickUiPopUpButtons(2));
        _save.onClick.AddListener(() => OnClickUiPopUpButtons(1));
        _yes.onClick.AddListener(() => OnGiveUpVoting(0));
        _no.onClick.AddListener(() => OnGiveUpVoting(1));

        _settingPanel.SetActive(false);
        _votingPanel.SetActive(false);

        _totalPlayers = PhotonNetwork.PlayerList.Length;
    }

    public void OnCLickSettingButton()
    {
        _settingPanel.SetActive(true);
    }

    public void OnClickUiPopUpButtons(int num)
    {
        switch (num)
        {
            case 0:
                photonView.RPC("GiveUpVote", RpcTarget.All);
                _settingPanel.SetActive(false);
                break;
            case 1:
                // 저장 버튼 눌렀을때 실행될 내용.
                break;
            case 2:
                _settingPanel.SetActive(false);
                break;
        }
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
        photonView.RPC("RegisterVote", RpcTarget.MasterClient, vote == 0);
    }

    [PunRPC]
    public void RegisterVote(bool giveUp)
    {
        _votesCount++;
        if (giveUp)
        {
            _giveUpVotes++;
        }
        else
        {
            _noGiveUpVotes++;
        }

        photonView.RPC("UpdateVoteResults", RpcTarget.All, _giveUpVotes, _noGiveUpVotes, _votesCount);
    }

    [PunRPC]
    public void UpdateVoteResults(int giveUpVotes, int noGiveUpVotes, int votesCount)
    {
        _giveUpVotes = giveUpVotes;
        _noGiveUpVotes = noGiveUpVotes;
        _votesCount = votesCount;

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
            PhotonNetwork.LoadLevel("MainScene");
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
