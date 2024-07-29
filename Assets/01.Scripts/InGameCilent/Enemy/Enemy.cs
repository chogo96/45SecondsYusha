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

//    public bool IsFinalBoss { get; set; } // ���� ���� ����

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
//        // Ư�� ȿ�� �ʱ�ȭ
//        foreach (var effect in _specialEffects)
//        {
//            StartCoroutine(HandleSpecialEffect(effect));
//        }

//        // ����� ����
//        foreach (var debuff in _debuffs)
//        {
//            if (debuff.name == "����")
//            {
//                ApplyRandomBleedDebuffToPlayer(players);
//            }
//            if (debuff.name == "�Ǹ�")
//            {
//                ApplyRandomBlindDebuffToPlayer(players);
//            }
//            if (debuff.name == "ȥ��")
//            {
//                ApplyRandomConfusionDebuffToPlayer(players);     
//            }
//        }
//        // �÷��̾� ��ũ��Ʈ ����
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
//        // ���� ������Ʈ ���� (�ʿ� �� �߰�)
//    }

//    public void Die()
//    {
//        if (IsFinalBoss)
//        {
//            Debug.Log("�¸�!");
//        }
//        Debug.Log("���!");
//        OnEnemyDeath?.Invoke();
//        Destroy(gameObject);
//    }

//    public void CheckDeathCondition(int sword, int magic, int shield)
//    {

//        Debug.Log("�׾���? Ȥ�� �Ѿ?");
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

//            // Ư�� ȿ�� ���� ���� (��: �ٸ� ����� ����)
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

    public bool IsFinalBoss { get; set; } // ���� ���� ����

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
            Debug.LogError("�÷��̾� ����Ʈ�� null�̰ų� ��� �ֽ��ϴ�.");
            return;
        }

        requiredSword = data.RequiredSwordAttack;
        requiredMagic = data.RequiredMagicAttack;
        requiredShield = data.RequiredShieldAttack;
        _debuffs = data.debuffs;
        _specialEffects = data.specialEffects;
        IsFinalBoss = isFinalBoss;

        // Ư�� ȿ�� �ʱ�ȭ
        foreach (var effect in _specialEffects)
        {
            StartCoroutine(HandleSpecialEffect(effect));
        }

        // ����� ����
        foreach (var debuff in _debuffs)
        {
            if (debuff.name == "����")
            {
                ApplyRandomBleedDebuffToPlayer(players);
            }
            if (debuff.name == "�Ǹ�")
            {
                ApplyRandomBlindDebuffToPlayer(players);
            }
            if (debuff.name == "ȥ��")
            {
                ApplyRandomConfusionDebuffToPlayer(players);
            }
        }

        // �÷��̾� ��ũ��Ʈ ����
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
            Debug.LogError("�÷��̾� ����Ʈ�� null�̰ų� ��� �ֽ��ϴ�. ���� ������� ������ �� �����ϴ�.");
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
            Debug.LogError("�÷��̾� ����Ʈ�� null�̰ų� ��� �ֽ��ϴ�. �Ǹ� ������� ������ �� �����ϴ�.");
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
            Debug.LogError("�÷��̾� ����Ʈ�� null�̰ų� ��� �ֽ��ϴ�. ȥ�� ������� ������ �� �����ϴ�.");
        }
    }

    void Update()
    {
        // ���� ������Ʈ ���� (�ʿ� �� �߰�)
    }

    public void Die()
    {
        if (IsFinalBoss)
        {
            Debug.Log("�¸�!");
        }
        Debug.Log("���!");
        OnEnemyDeath?.Invoke();
        Destroy(gameObject);
    }

    public void CheckDeathCondition(int sword, int magic, int shield)
    {
        Debug.Log("�׾���? Ȥ�� �Ѿ?");
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

            // Ư�� ȿ�� ���� ���� (��: �ٸ� ����� ����)
        }
    }
}
