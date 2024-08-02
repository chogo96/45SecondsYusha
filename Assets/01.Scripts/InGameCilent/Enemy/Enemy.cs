using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

public class Enemy : MonoBehaviourPunCallbacks
{
    public int requiredSword;
    public int requiredMagic;
    public int requiredShield;
    private List<Debuff> _debuffs;
    private List<SpecialEffect> _specialEffects;

    public bool IsFinalBoss { get; set; } // 최종 보스 여부

    public delegate void EnemyDeath();
    public static event EnemyDeath OnEnemyDeath;
    public delegate void EnemySpawned(Enemy newEnemy);
    public static event EnemySpawned OnEnemySpawned;

    private PlayerScripts _playerScripts;
    private EnemyUIManager _enemyUIManager;
    private PlayerSetManager playerSetManager;
    private GameOverManager _gameOverManager;

    private void Awake()
    {
        playerSetManager = FindObjectOfType<PlayerSetManager>();
        _gameOverManager = FindObjectOfType<GameOverManager>();
    }

    public void Initialize(EnemyData data, List<PlayerScripts> players, bool isFinalBoss = false)
    {
        if (players == null || players.Count == 0)
        {
            return;
        }

        requiredSword = data.RequiredSwordAttack;
        requiredMagic = data.RequiredMagicAttack;
        requiredShield = data.RequiredShieldAttack;
        _debuffs = data.debuffs;
        _specialEffects = data.specialEffects;
        IsFinalBoss = isFinalBoss;

        //// 특수 효과 초기화
        //foreach (var effect in _specialEffects)
        //{
        //    StartCoroutine(HandleSpecialEffect(effect));
        //}

        // 디버프 적용
        foreach (var debuff in _debuffs)
        {
            if (debuff.name == "출혈")
            {
                ApplyRandomBleedDebuffToPlayer(players);
            }
            if (debuff.name == "실명")
            {
                ApplyRandomBlindDebuffToPlayer(players);
            }
            if (debuff.name == "혼란")
            {
                ApplyRandomConfusionDebuffToPlayer(players);
            }
        }

        // 플레이어 스크립트 참조
        _playerScripts = FindObjectOfType<PlayerScripts>();
        if (_playerScripts != null)
        {
            _playerScripts.SetCurrentEnemy(this);
            InGameManager.instance.ResetValues();
        }
        _enemyUIManager = FindObjectOfType<EnemyUIManager>();
        if (_enemyUIManager != null)
        {
            _enemyUIManager.UpdateUI(requiredSword, requiredMagic, requiredShield);
        }

        // 새로운 적이 생성되었음을 알림
        OnEnemySpawned?.Invoke(this);
    }

    private void ApplyRandomBleedDebuffToPlayer(List<PlayerScripts> players)
    {
        if (players != null && players.Count > 0)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                int randomIndex = Random.Range(0, players.Count);
                players[randomIndex].ApplyBleedToPlayer();
                int playerID = players[randomIndex].PlayerID;
                playerSetManager.photonView.RPC("DeBuffImageOn", RpcTarget.All, playerID, "bleed");
            }
        }
        else
        {
        }
    }

    private void ApplyRandomBlindDebuffToPlayer(List<PlayerScripts> players)
    {
        if (players != null && players.Count > 0)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                int randomIndex = Random.Range(0, players.Count);
                players[randomIndex].ApplyBlindToPlayer();
                int playerID = players[randomIndex].PlayerID;
                playerSetManager.photonView.RPC("DeBuffImageOn", RpcTarget.All, playerID, "blind");
            }
        }
        else
        {
        }
    }

    private void ApplyRandomConfusionDebuffToPlayer(List<PlayerScripts> players)
    {
        if (players != null && players.Count > 0)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                int randomIndex = Random.Range(0, players.Count);
                players[randomIndex].ApplyConfusionToPlayer();
                int playerID = players[randomIndex].PlayerID;
                playerSetManager.photonView.RPC("DeBuffImageOn", RpcTarget.All, playerID, "confusion");
            }
        }
        else
        {
        }
    }

    void Update()
    {
        // 적의 업데이트 로직 (필요 시 추가)
    }

    public void Die()
    {
        if (IsFinalBoss)
        {
            _gameOverManager.DisplayWin();
        }

        // 플레이어의 _currentEnemy 참조를 해제
        if (_playerScripts != null)
        {
            _playerScripts.SetCurrentEnemy(null);
        }

        OnEnemyDeath?.Invoke();
        Destroy(gameObject);
    }

    public void CheckDeathCondition(int sword, int magic, int shield)
    {
        if ((requiredSword == 0 || sword >= requiredSword) &&
            (requiredMagic == 0 || magic >= requiredMagic) &&
            (requiredShield == 0 || shield >= requiredShield))
        {
            _playerScripts._swordPoint = 0;
            _playerScripts._shieldPoint = 0;
            _playerScripts._magicPoint = 0;
            Die();
        }
    }

    private IEnumerator HandleSpecialEffect(SpecialEffect effect)
    {
        while (true)
        {
            yield return new WaitForSeconds(effect.Cooldown);
        }
    }
}
