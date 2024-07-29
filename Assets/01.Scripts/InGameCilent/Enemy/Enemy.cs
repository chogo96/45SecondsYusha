//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;


//public class Enemy : MonoBehaviour
//{
//    public int requiredSword;
//    public int requiredMagic;
//    public int requiredShield;
//    private List<Debuff> _debuffs;
//    private List<SpecialEffect> _specialEffects;

//    public bool IsFinalBoss { get; set; } // 최종 보스 여부

//    public delegate void EnemyDeath();
//    public static event EnemyDeath OnEnemyDeath;
//    private PlayerScripts _playerScripts;
//    private EnemyUIManager _enemyUIManager;
//    public void Initialize(EnemyData data, List<PlayerScripts> players, bool isFinalBoss = false)
//    {
//        requiredSword = data.RequiredSwordAttack;
//        requiredMagic = data.RequiredMagicAttack;
//        requiredShield = data.RequiredShieldAttack;
//        _debuffs = data.debuffs;
//        _specialEffects = data.specialEffects;
//        IsFinalBoss = isFinalBoss;
//        // 특수 효과 초기화
//        foreach (var effect in _specialEffects)
//        {
//            StartCoroutine(HandleSpecialEffect(effect));
//        }

//        // 디버프 적용
//        foreach (var debuff in _debuffs)
//        {
//            if (debuff.name == "출혈")
//            {
//                ApplyRandomBleedDebuffToPlayer(players);
//            }
//            if (debuff.name == "실명")
//            {
//                ApplyRandomBlindDebuffToPlayer(players);
//            }
//            if (debuff.name == "혼란")
//            {
//                ApplyRandomConfusionDebuffToPlayer(players);     
//            }
//        }
//        // 플레이어 스크립트 참조
//        _playerScripts = FindObjectOfType<PlayerScripts>();
//        if (_playerScripts != null)
//        {
//            _playerScripts.SetCurrentEnemy(this);
//            InGameManager.instance.ResetValues();
//        }
//        _enemyUIManager = FindObjectOfType<EnemyUIManager>();
//        if(_enemyUIManager != null)
//        {
//            _enemyUIManager.UpdateUI(requiredSword, requiredMagic, requiredShield);
//        }
//    }


//    private void ApplyRandomBleedDebuffToPlayer(List<PlayerScripts> players)
//    {
//        if (players.Count > 0)
//        {
//            int randomIndex = Random.Range(0, players.Count);
//            players[randomIndex].ApplyBleedToPlayer();
//        }
//    }
//    private void ApplyRandomBlindDebuffToPlayer(List<PlayerScripts> players)
//    {
//        if (players.Count > 0)
//        {
//            int randomIndex = Random.Range(0, players.Count);
//            players[randomIndex].ApplyBlindToPlayer();
//        }
//    }
//    private void ApplyRandomConfusionDebuffToPlayer(List<PlayerScripts> players)
//    {
//        if (players.Count > 0)
//        {
//            int randomIndex = Random.Range(0, players.Count);
//            players[randomIndex].ApplyConfusionToPlayer();
//        }
//    }
//    void Update()
//    {
//        // 적의 업데이트 로직 (필요 시 추가)
//    }

//    public void Die()
//    {
//        if (IsFinalBoss)
//        {
//            Debug.Log("승리!");
//        }
//        Debug.Log("사망!");
//        OnEnemyDeath?.Invoke();
//        Destroy(gameObject);
//    }

//    public void CheckDeathCondition(int sword, int magic, int shield)
//    {

//        Debug.Log("죽었음? 혹은 넘어감?");
//        if ((requiredSword == 0 || sword >= requiredSword) &&
//            (requiredMagic == 0 || magic >= requiredMagic) &&
//            (requiredShield == 0 || shield >= requiredShield))
//        {
//            Die();
//        }
//    }

//    private IEnumerator HandleSpecialEffect(SpecialEffect effect)
//    {
//        while (true)
//        {
//            yield return new WaitForSeconds(effect.Cooldown);

//            // 특수 효과 적용 로직 (예: 다른 디버프 적용)
//        }
//    }

//}
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private PlayerScripts _playerScripts;
    private EnemyUIManager _enemyUIManager;
    private PlayerSetManager playerSetManager;


    private void Awake()
    {
        playerSetManager = FindObjectOfType<PlayerSetManager>();
    }


    public void Initialize(EnemyData data, List<PlayerScripts> players, bool isFinalBoss = false)
    {
        if (players == null || players.Count == 0)
        {
            Debug.LogError("플레이어 리스트가 null이거나 비어 있습니다.");
            return;
        }

        requiredSword = data.RequiredSwordAttack;
        requiredMagic = data.RequiredMagicAttack;
        requiredShield = data.RequiredShieldAttack;
        _debuffs = data.debuffs;
        _specialEffects = data.specialEffects;
        IsFinalBoss = isFinalBoss;

        // 특수 효과 초기화
        foreach (var effect in _specialEffects)
        {
            StartCoroutine(HandleSpecialEffect(effect));
        }

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
    }

    private void ApplyRandomBleedDebuffToPlayer(List<PlayerScripts> players)
    {
        if (players != null && players.Count > 0)
        {
            int randomIndex = Random.Range(0, players.Count);
            players[randomIndex].ApplyBleedToPlayer();
            int playerID = players[randomIndex].PlayerID;
            playerSetManager.photonView.RPC("DeBuffImageOn", RpcTarget.All, playerID, "bleed");
        }
        else
        {
            Debug.LogError("플레이어 리스트가 null이거나 비어 있습니다. 출혈 디버프를 적용할 수 없습니다.");
        }
    }

    private void ApplyRandomBlindDebuffToPlayer(List<PlayerScripts> players)
    {
        if (players != null && players.Count > 0)
        {
            int randomIndex = Random.Range(0, players.Count);
            players[randomIndex].ApplyBlindToPlayer();
            int playerID = players[randomIndex].PlayerID;
            playerSetManager.photonView.RPC("DeBuffImageOn", RpcTarget.All, playerID, "blind");
        }
        else
        {
            Debug.LogError("플레이어 리스트가 null이거나 비어 있습니다. 실명 디버프를 적용할 수 없습니다.");
        }
    }

    private void ApplyRandomConfusionDebuffToPlayer(List<PlayerScripts> players)
    {
        if (players != null && players.Count > 0)
        {
            int randomIndex = Random.Range(0, players.Count);
            players[randomIndex].ApplyConfusionToPlayer();
            int playerID = players[randomIndex].PlayerID;
            playerSetManager.photonView.RPC("DeBuffImageOn", RpcTarget.All, playerID, "confusion");
        }
        else
        {
            Debug.LogError("플레이어 리스트가 null이거나 비어 있습니다. 혼란 디버프를 적용할 수 없습니다.");
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
            Debug.Log("승리!");
        }
        Debug.Log("사망!");
        OnEnemyDeath?.Invoke();
        Destroy(gameObject);
    }

    public void CheckDeathCondition(int sword, int magic, int shield)
    {
        Debug.Log("죽었음? 혹은 넘어감?");
        if ((requiredSword == 0 || sword >= requiredSword) &&
            (requiredMagic == 0 || magic >= requiredMagic) &&
            (requiredShield == 0 || shield >= requiredShield))
        {
            Die();
        }
    }

    private IEnumerator HandleSpecialEffect(SpecialEffect effect)
    {
        while (true)
        {
            yield return new WaitForSeconds(effect.Cooldown);

            // 특수 효과 적용 로직 (예: 다른 디버프 적용)
        }
    }
}
