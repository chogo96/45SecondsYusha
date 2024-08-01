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
//    /// �ܼ� ������ư�� �������� ����� �Լ�.
//    /// </summary>
//    public void OnCLickSettingButton()
//    {
//        _settingPanel.SetActive(true);
//    }

//    /// <summary>
//    /// �˾�â �ȿ� �ִ� ��ư�� ��Ƶ� ��
//    /// </summary>
//    /// <param name="num">������ ��ȣ(0�� : �׺� | 1�� : ���̺� | 2�� : ���)</param>
//    public void OnClickUiPopUpButtons(int num)
//    {
//        switch (num)
//        {
//            case 0:
//                // �׺� ��ư �������� ����� ����.
//                photonView.RPC("GiveUpVote", RpcTarget.All);
//                _settingPanel.SetActive(false);

//                break;
//            case 1:
//                // ���� ��ư �������� ����� ����.

//                break;
//            case 2:
//                // ��� ��ư �������� ����� ����.
//                _settingPanel.SetActive(false);
//                break;
//        }
//    }

//    /// <summary>
//    /// �׺� ��ư �������� ����� ����
//    /// </summary>
//    [PunRPC]
//    public void GiveUpVote()
//    {
//        // �˾� â�� Ȱ��ȭ�ϰ�, �ؽ�Ʈ�� ��ư�� �����մϴ�.
//        _votingPanel.SetActive(true);
//    }


//    /// <summary>
//    /// �׺� ��ǥ ����
//    /// </summary>
//    /// <param name="vote">��ǥ (0�� : Yes | 1�� : NO)</param>
//    private void OnGiveUpVoring(int vote)
//    {
//        switch (vote)
//        {
//            case 0:
//                // �׺� ��ư Ŭ�� �� ������ �׺� ��ǥ�� �˸��ϴ�.
//                photonView.RPC("RegisterVote", RpcTarget.MasterClient, true);
//                break;
//            case 1:
//                // ��� ��ư Ŭ�� �� ������ ���� ��ǥ�� �˸��ϴ�.
//                photonView.RPC("RegisterVote", RpcTarget.MasterClient, false); 
//                break;
//        }
//        _yes.interactable = false;
//        _no.interactable = false;
//    }

//    /// <summary>
//    /// ��ǥ ���� �� �� �����ϴ� �Լ�
//    /// </summary>
//    /// <param name="giveUp"> �׺� ��ǥ bool�� (True = �׺����� | False : �׺��ݴ�)</param>
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

//        // ��� �÷��̾ ��ǥ�� �Ϸ��ߴ��� Ȯ��
//        if (_votesCount == _totalPlayers)
//        {
//            CheckVoteResult();
//        }
//    }

//    private void CheckVoteResult()
//    {
//        // ���ݼ� �̻��� �׺��� �����ߴ��� Ȯ��
//        if (_giveUpVotes > _totalPlayers / 2)
//        {
//            // �׺� ����Ʈ ����� ����� ��

//            _votingPanel.SetActive(false);
//            // �� ����
//            PhotonNetwork.LoadLevel("NewSceneName"); // �׺� �� ������ �� �̸�
//        }
//        else
//        {
//            // �г� �����
//            _votingPanel.SetActive(false);

//            // ��ǥ �ʱ�ȭ
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
                // ���� ��ư �������� ����� ����.
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
