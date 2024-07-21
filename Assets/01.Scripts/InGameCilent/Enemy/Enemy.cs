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

    public bool IsFinalBoss { get; set; } // 최종 보스 여부

    public delegate void EnemyDeath();
    public static event EnemyDeath OnEnemyDeath;

    public void Initialize(EnemyData data, List<PlayerScripts> players, bool isFinalBoss = false)
    {
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
