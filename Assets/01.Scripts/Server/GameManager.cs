using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.SocialPlatforms.Impl;
using static Enemy;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject otherPlayerPrefab;
    public GameObject playerPrefab;
    private Transform canvasTransform;
    private Vector3 localPlayerPosition = new Vector3(-961, -536, 0);
    private Vector3[] otherPlayerPositions;

    public static Action AllPlayersSpawned;

    private int _cardCount;
    private int _timeCount;
    private int _sumCount;

    // 상점 먹통이라 여기서함
    private InGameShop inGameShop;
    //private EnemySpawner enemySpawner;
    //private GameObject _transformGameObject;
    //private TMP_Text _cardVoteText;
    //private TMP_Text _timeVoteText;

    //private bool _isAllDone = false;
    //private string _cardVote;
    //private string _timeVote;

    //RealTimeBossStatusCheck 하기위해 만듬
    PlayerScripts playerScripts;
    private EnemyUIManager _enemyUIManager;

    int _swordPoint;
    int _magicPoint;
    int _shieldPoint;

    int _Sword;
    int _Magic;
    int _Shield;

    int _requiredSword;
    int _requiredMagic;
    int _requiredShield;

    int __previousSword;
    int __previousMagic;
    int __previousShield;

    private void Awake()
    {
        _enemyUIManager = FindObjectOfType<EnemyUIManager>();
        //uI_Timer = FindObjectOfType<UI_Timer>();
        //enemySpawner = FindObjectOfType<EnemySpawner>();
        InsertScripts.OnScriptsInserted += GameManagerFindPlayerScriptComponent;
        InGameShop.InGameShopSpawn += InGameShopSpawn;
    }

    public void GameManagerFindPlayerScriptComponent()
    {
        playerScripts = FindObjectOfType<PlayerScripts>();
    }
    public void InGameShopSpawn()
    {
        inGameShop = FindObjectOfType<InGameShop>();

    }

    private void Start()
    {
        // Find the Canvas - PlayerSpawn object
        canvasTransform = GameObject.Find("Canvas - PlayerSpawn").transform;
        if (canvasTransform == null)
        {
            Utils.LogRed("Canvas - PlayerSpawn object not found in Start.");
            return;
        }
        else
        {
            Utils.Log("Canvas - PlayerSpawn found in Start: " + canvasTransform.name);
        }

        otherPlayerPositions = new Vector3[]
        {
            new Vector3(-961, 16, 0),    // Player 2 (9 o'clock)
            new Vector3(91, 218, 0),     // Player 3 (12 o'clock)
            new Vector3(597, -167, 0)    // Player 4 (3 o'clock)
        };

        if (PhotonNetwork.IsConnectedAndReady)
        {
            SpawnAllPlayers();
        }
        else
        {
            Utils.LogRed("PhotonNetwork is not connected and ready.");
        }
    }

    private void SpawnAllPlayers()
    {
        int otherPlayerCount = 0;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Vector3 spawnPosition;

            if (player.IsLocal)
            {
                spawnPosition = localPlayerPosition;
                GameObject playerObject = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
                playerObject.name = $"{player.NickName}";
                playerObject.transform.SetParent(canvasTransform, false);
                playerObject.transform.localPosition = spawnPosition;
            }
            else
            {
                spawnPosition = otherPlayerPositions[otherPlayerCount % otherPlayerPositions.Length];
                otherPlayerCount++;
                GameObject playerObject = PhotonNetwork.Instantiate(otherPlayerPrefab.name, Vector3.zero, Quaternion.identity);
                playerObject.name = $"{player.NickName}";
                playerObject.transform.SetParent(canvasTransform, false);
                playerObject.transform.localPosition = spawnPosition;
            }
        }

        // Check if all players have spawned and invoke event
        CheckAllPlayersSpawned();
    }

    private void CheckAllPlayersSpawned()
    {
        if (PhotonNetwork.PlayerList.Length == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            AllPlayersSpawned?.Invoke();
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            // 마스터 클라이언트가 아니면
            PhotonNetwork.LoadLevel("02.Lobby Scene");
        }
    }

    [PunRPC]
    public void OnAddTimeOrCardButton(int plusTime, int plusCard)
    {

        Utils.LogGreen($"plusTime {plusTime}");
        Utils.LogGreen($"plusCard {plusCard}");

        _cardCount =  plusCard;
        _timeCount =  plusTime;

        inGameShop._plusTimeCount = plusTime;
        inGameShop._plusCardCount = plusCard;

        Utils.LogGreen($"_cardCount {_cardCount}");
        Utils.LogGreen($"_timeCount {_timeCount}");

        Utils.LogGreen($"inGameShop {inGameShop}");
        Utils.LogGreen($"inGameShop._timeText {inGameShop._timeText}");
        _sumCount = _cardCount + _timeCount;

        inGameShop._timeText.text = $"AddTime\nVote : {_timeCount}";
        inGameShop._cardText.text = $"AddCard\nVote : {_cardCount}";

        Debug.Log($"Votes updated on {PhotonNetwork.LocalPlayer.NickName}: AddTime: {_timeCount}, AddCard: {_cardCount}");

        if (_sumCount >= PhotonNetwork.PlayerList.Length)
        {
            inGameShop.ExecuteVotingResult(_cardCount, _timeCount);
        }
    }
    



    [PunRPC]
    public void RealTimeBossStatusCheck(int sword, int magic, int shield, int Sword, int Magic, int Shield, int requiredSword, int requiredMagic, int requiredShield, int _previousSword, int _previousMagic, int _previousShield)
    {
        if (playerScripts._currentEnemy == null)
        {
            Debug.LogWarning("현재 적이 null 상태입니다. RealTimeBossStatusCheck 호출을 무시합니다.");
            return;
        }

        _swordPoint = sword;
        _magicPoint = magic;
        _shieldPoint = shield;

        _Sword = Sword;
        _Magic = Magic;
        _Shield = Shield;

        _requiredSword = requiredSword;
        _requiredMagic = requiredMagic;
        _requiredShield = requiredShield;

        __previousSword = _previousSword;
        __previousMagic = _previousMagic;
        __previousShield = _previousShield;

        InGameManager.instance.Sword = _swordPoint;
        InGameManager.instance.Magic = _magicPoint;
        InGameManager.instance.Shield = _shieldPoint;

        int swordIncrement = _swordPoint - __previousSword;
        int magicIncrement = _magicPoint - __previousMagic;
        int shieldIncrement = _shieldPoint - __previousShield;

        _enemyUIManager.ChangeAlphaForIncrement(swordIncrement, _enemyUIManager.swordImageParent, _Sword, _requiredSword);
        _enemyUIManager.ChangeAlphaForIncrement(magicIncrement, _enemyUIManager.magicImageParent, _Magic, _requiredMagic);
        _enemyUIManager.ChangeAlphaForIncrement(shieldIncrement, _enemyUIManager.shieldImageParent, _Shield, _requiredShield);

        Utils.Log($"Sword: {InGameManager.instance.Sword}, Magic: {InGameManager.instance.Magic}, Shield: {InGameManager.instance.Shield}");
        Utils.Log($"CurrentEnemy: {playerScripts._currentEnemy}");
        Utils.LogGreen($"{_swordPoint},{_magicPoint},{_shieldPoint},{swordIncrement},{magicIncrement},{shieldIncrement}");

        // 현재 적의 생존 조건 확인
        playerScripts._currentEnemy?.CheckDeathCondition(_swordPoint, _magicPoint, _shieldPoint);
    }
}
