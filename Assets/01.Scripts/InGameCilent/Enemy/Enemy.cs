using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using System.Linq;

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
    public delegate void EnemySpawned(Enemy newEnemy);
    public static event EnemySpawned OnEnemySpawned;

    private PlayerScripts _playerScripts;
    private EnemyUIManager _enemyUIManager;
    private PlayerSetManager playerSetManager;
    private GameOverManager _gameOverManager;

    private Animator _animator; // �ִϸ����� ������

    private string _debuffName;

    private void Awake()
    {
        playerSetManager = FindObjectOfType<PlayerSetManager>();
        _gameOverManager = FindObjectOfType<GameOverManager>();

        // �ڽ� ������Ʈ���� �ִϸ����� ã��
        Transform unitRoot = transform.Find("UnitRoot");
        if (unitRoot != null)
        {
            _animator = unitRoot.GetComponent<Animator>();

        }
        else
        {
            Transform HorseRoot = transform.Find("HorseRoot");
            if (HorseRoot != null)
            {
                _animator = HorseRoot.GetComponent<Animator>();
            }
            else
            {
                if (_animator == null)
                {
                    Utils.LogRed("UnitRoot�� horseRoot�� �ִϸ����� ������Ʈ�� �����ϴ�.");
                }
            }
        }
    }

    public void Initialize(EnemyData data, List<PlayerScripts> players, bool isFinalBoss = false)
    {
        if (players == null || players.Count == 0)
        {
            Utils.LogRed("�÷��̾� ����Ʈ�� null�̰ų� ��� �ֽ��ϴ�.");
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

        // ���ο� ���� �����Ǿ����� �˸�
        OnEnemySpawned?.Invoke(this);
    }
    [PunRPC]
    private void ApplyRandomDebuff(string playerName, int randomIndex, string debuffName)
    {
        Utils.LogRed($"playerName = {playerName}");
        Utils.LogRed($"_debuffName = {_debuffName}");
        if (playerName == PhotonNetwork.LocalPlayer.NickName)
        {
            Utils.LogRed($"PhotonNetwork.PlayerList[randomIndex].NickName = {PhotonNetwork.PlayerList[randomIndex].NickName}");
            _debuffName = debuffName;
            GameObject playerObject = GameObject.Find(playerName);
            if (playerObject != null)
            {
                PlayerScripts playerScripts = playerObject.GetComponent<PlayerScripts>();
                if (playerScripts != null)
                {
                    if(_debuffName == "bleed")
                    {
                        playerScripts.ApplyBleedToPlayer();
                    }
                    else if(_debuffName == "blind")
                    {
                        playerScripts.ApplyBlindToPlayer();
                    }
                    else if(_debuffName == "confusion")
                    {
                        playerScripts.ApplyConfusionToPlayer();
                    }
                    else
                    {
                        Utils.LogRed($"_debuffName = {_debuffName}");
                        Utils.LogRed($"�̷� ������� ����");
                    }
                    playerSetManager.photonView.RPC("DeBuffImageOn", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, _debuffName);
                }
            }
        }
    }

    private void ApplyRandomBleedDebuffToPlayer(List<PlayerScripts> players)
    {

        if (players != null && players.Count > 0)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                int randomIndex = Random.Range(0, players.Count);
                string playerName = players[randomIndex].name;
                _debuffName = "bleed";
                Utils.LogRed($"_debuffName = {_debuffName}");

                photonView.RPC("ApplyRandomDebuff", RpcTarget.All, playerName, randomIndex, _debuffName);
            }
        }
        else
        {
            Utils.LogRed("�÷��̾� ����Ʈ�� null�̰ų� ��� �ֽ��ϴ�. ���� ������� ������ �� �����ϴ�.");
        }
    }
    //private void ApplyRandomBleedDebuffToPlayer(List<PlayerScripts> players)
    //{
    //    if (players != null && players.Count > 0)
    //    {
    //        if (PhotonNetwork.IsMasterClient)
    //        {
    //            //int randomIndex = Random.Range(0, players.Count);
    //            int randomIndex = Random.Range(0, PhotonNetwork.PlayerList.Length);
    //            int randomActorNumber = PhotonNetwork.PlayerList[randomIndex].ActorNumber;
    //            Utils.LogGreen(randomIndex);
    //            Utils.LogGreen(randomActorNumber);
    //            if (randomActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
    //            {
    //                players[randomIndex].ApplyBleedToPlayer();
    //                playerSetManager.photonView.RPC("DeBuffImageOn", RpcTarget.All, randomActorNumber, "bleed");
    //            }
    //            else
    //            {
    //                return;
    //            }
    //            //int playerID = players[randomIndex].PlayerID;
    //            //playerSetManager.photonView.RPC("DeBuffImageOn", RpcTarget.All, playerID, "bleed");
    //        }
    //    }
    //    else
    //    {
    //        Utils.LogRed("�÷��̾� ����Ʈ�� null�̰ų� ��� �ֽ��ϴ�. ���� ������� ������ �� �����ϴ�.");
    //    }
    //}

    private void ApplyRandomBlindDebuffToPlayer(List<PlayerScripts> players)
    {
        if (players != null && players.Count > 0)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                int randomIndex = Random.Range(0, players.Count);
                string playerName = players[randomIndex].name;
                _debuffName = "blind";
                Utils.LogRed($"_debuffName = {_debuffName}");

                photonView.RPC("ApplyRandomDebuff", RpcTarget.All, playerName, randomIndex, _debuffName);
            }
        }
        else
        {
            Utils.LogRed("�÷��̾� ����Ʈ�� null�̰ų� ��� �ֽ��ϴ�. �Ǹ� ������� ������ �� �����ϴ�.");
        }
    }

    private void ApplyRandomConfusionDebuffToPlayer(List<PlayerScripts> players)
    {
        if (players != null && players.Count > 0)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                int randomIndex = Random.Range(0, players.Count);
                string playerName = players[randomIndex].name;
                _debuffName = "confusion";
                Utils.LogRed($"_debuffName = {_debuffName}");

                photonView.RPC("ApplyRandomDebuff", RpcTarget.All, playerName, randomIndex, _debuffName);
            }
        }
        else
        {
            Utils.LogRed("�÷��̾� ����Ʈ�� null�̰ų� ��� �ֽ��ϴ�. ȥ�� ������� ������ �� �����ϴ�.");
        }
    }

    void Update()
    {
        // ���� ������Ʈ ���� (�ʿ� �� �߰�)
    }

    public void Die()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("DieTrigger"); // �ִϸ��̼� Ʈ���� ����
        }
        else
        {
            Utils.LogRed("�ִϸ����Ͱ� null�Դϴ�. ��� �ִϸ��̼��� ����� �� �����ϴ�.");
        }

        SoundManager.instance.PlaySfx(SoundManager.Sfx.EnemyDown);

        if (IsFinalBoss)
        {
            Utils.Log("�¸�!");
            _gameOverManager.DisplayWin();
        }
        Utils.Log("���!");

        // �÷��̾��� _currentEnemy ������ ����
        if (_playerScripts != null)
        {
            _playerScripts.SetCurrentEnemy(null);
        }

        OnEnemyDeath?.Invoke();

        // �ִϸ��̼� ��� �� ������Ʈ ���Ÿ� ���� �ڷ�ƾ ����
        StartCoroutine(DestroyAfterAnimation());
    }

    private IEnumerator DestroyAfterAnimation()
    {
        // �ִϸ��̼��� ���̸� ��ٸ�
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);

        // ������Ʈ �ı�
        Destroy(gameObject);
    }

    public void CheckDeathCondition(int sword, int magic, int shield)
    {
        Utils.Log("�׾���? Ȥ�� �Ѿ?");
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

            // Ư�� ȿ�� ���� ���� (��: �ٸ� ����� ����)
        }
    }
}
