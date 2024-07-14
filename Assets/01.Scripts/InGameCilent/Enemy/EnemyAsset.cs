using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class EnemyData : ScriptableObject
{
    public Sprite enemySprite;
    public int RequiredSwordAttack;
    public int RequiredMagicAttack;
    public int RequiredShieldAttack;
    public bool Complete;
    public List<Debuff> debuffs; // 디버프 리스트
    public List<SpecialEffect> specialEffects; // 특수 효과 리스트
}

[System.Serializable]
public class Debuff
{
    public string name;
    public float duration;
    public float effectValue;
}

[System.Serializable]
public class SpecialEffect
{
    public string Name;
    public float Cooldown;
    public float effectValue;
}