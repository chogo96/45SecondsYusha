using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class EnemyData : ScriptableObject
{
    [JsonIgnore]
    [PreviewSprite]
    public Sprite enemySprite;

    public string EnemyName;
    public int RequiredSwordAttack;
    public int RequiredMagicAttack;
    public int RequiredShieldAttack;
    public bool Complete;
    public List<Debuff> debuffs; // ����� ����Ʈ
    public List<SpecialEffect> specialEffects; // Ư�� ȿ�� ����Ʈ
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