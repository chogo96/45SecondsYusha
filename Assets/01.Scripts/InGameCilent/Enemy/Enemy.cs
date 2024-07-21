using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour
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
    public void Initialize(EnemyData data, List<PlayerScripts> players, bool isFinalBoss = false)
    {
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
        }
        // �÷��̾� ��ũ��Ʈ ����
        _playerScripts = FindObjectOfType<PlayerScripts>();
        if (_playerScripts != null)
        {
            _playerScripts.SetCurrentEnemy(this);
        }
    }

    private void ApplyRandomBleedDebuffToPlayer(List<PlayerScripts> players)
    {
        if (players.Count > 0)
        {
            int randomIndex = Random.Range(0, players.Count);
            players[randomIndex].ApplyBleedToPlayer();
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
