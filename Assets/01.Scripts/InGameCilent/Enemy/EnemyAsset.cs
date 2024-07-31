using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


public class EnemyData : ScriptableObject
{
    [JsonIgnore]
    [PreviewSprite]
    public Sprite enemySprite;


    public int RequiredSwordAttack;
    public int RequiredMagicAttack;
    public int RequiredShieldAttack;
    public bool Complete;
    public List<Debuff> debuffs; // 디버프 리스트
    public List<SpecialEffect> specialEffects; // 특수 효과 리스트

    // Base64 문자열로 직렬화된 스프라이트 텍스처
    public string enemySpriteBase64
    {
        get
        {
            if (enemySprite != null && enemySprite.texture != null)
            {
                var texture = enemySprite.texture;
                var bytes = texture.EncodeToPNG();
                return System.Convert.ToBase64String(bytes);
            }
            return null;
        }
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                var bytes = System.Convert.FromBase64String(value);
                var texture = new Texture2D(2, 2);
                texture.LoadImage(bytes);
                enemySprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
        }
    }
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