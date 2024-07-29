using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSetManager : MonoBehaviourPunCallbacks
{
    private Image[] _playerImage;

    private GameObject[] _bleedDebuffImage;
    private GameObject[] _blindDebuffImage;
    private GameObject[] _confusionDebuffImage;

    private Image[] _deckImage;
    private TMP_Text[] _deckCountText;

    private Image[] _handImage;
    private TMP_Text[] _handCountText;

    private int playerCount;

    private void Awake()
    {
        playerCount = PhotonNetwork.PlayerList.Length + 1;

        _playerImage = new Image[playerCount];
        _bleedDebuffImage = new GameObject[playerCount];
        _blindDebuffImage = new GameObject[playerCount];
        _confusionDebuffImage = new GameObject[playerCount];
        _deckImage = new Image[playerCount];
        _deckCountText = new TMP_Text[playerCount];
        _handImage = new Image[playerCount];
        _handCountText = new TMP_Text[playerCount];

        // GameManager의 AllPlayersSpawned 이벤트 구독
        GameManager.AllPlayersSpawned += Reset;
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        GameManager.AllPlayersSpawned -= Reset;
    }

    private void Reset()
    {
        for (int i = 1; playerCount > i; i++)
        {
            _playerImage[i] = transform.Find($"Player_{i}/Player_Image").GetComponent<Image>();

            _bleedDebuffImage[i] = transform.Find($"Player_{i}/BleedDebuffImage").gameObject;
            _blindDebuffImage[i] = transform.Find($"Player_{i}/BlindDebuffImage").gameObject;
            _confusionDebuffImage[i] = transform.Find($"Player_{i}/ConfusionDebuffImage").gameObject;

            _deckImage[i] = transform.Find($"Player_{i}/DeckImage").GetComponent<Image>();
            _deckCountText[i] = transform.Find($"Player_{i}/DeckImage/DeckCountText (TMP)").GetComponent<TMP_Text>();

            _handImage[i] = transform.Find($"Player_{i}/HandImage").GetComponent<Image>();
            _handCountText[i] = transform.Find($"Player_{i}/HandImage/HandCountText (TMP)").GetComponent<TMP_Text>();
        }
    }

    [PunRPC]
    public void DeBuffImageOn(int playerID, string deBuff)
    {
        if (deBuff == "bleed")
        {
            _bleedDebuffImage[playerID].SetActive(true);
        }
        if (deBuff == "blind")
        {
            _blindDebuffImage[playerID].SetActive(true);
        }
        if (deBuff == "confusion")
        {
            _confusionDebuffImage[playerID].SetActive(true);
        }
    }

    [PunRPC]
    public void DeBuffImageOff(int playerID, string deBuff)
    {
        if (deBuff == "bleed")
        {
            _bleedDebuffImage[playerID].SetActive(false);
        }
        if (deBuff == "blind")
        {
            _blindDebuffImage[playerID].SetActive(false);
        }
        if (deBuff == "confusion")
        {
            _confusionDebuffImage[playerID].SetActive(false);
        }
    }
}
