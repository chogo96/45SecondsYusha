using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerPortraitVisual : MonoBehaviour
{

    public CharacterAsset charAsset;
    [Header("Text Component References")]
    //public Text NameText;
    [Header("Image References")]
    public Image HeroPowerIconImage;
    public Image HeroPowerBackgroundImage;
    public Image PortraitImage;
    public Image PortraitBackgroundImage;

    void Awake()
    {
        if (charAsset != null)
            ApplyLookFromAsset();
    }

    public void ApplyLookFromAsset()
    {
        if (HeroPowerIconImage != null)
        {
            HeroPowerIconImage.sprite = charAsset.HeroPowerIconImage;
            HeroPowerBackgroundImage.sprite = charAsset.HeroPowerBGImage;
            HeroPowerBackgroundImage.color = charAsset.HeroPowerBGTint;
        }

        PortraitImage.sprite = charAsset.AvatarImage;
        PortraitBackgroundImage.sprite = charAsset.AvatarBGImage;
        PortraitBackgroundImage.color = charAsset.AvatarBGTint;

    }

    public void TakeDamage(int amount, int healthAfter)
    {
        if (amount > 0)
        {
            DamageEffect.CreateDamageEffect(transform.position, amount);
        }
    }

    public void Explode()
    {
        Instantiate(GlobalSettings.instance.ExplosionPrefab, transform.position, Quaternion.identity);
        Sequence s = DOTween.Sequence();
        s.PrependInterval(2f);
        s.OnComplete(() => GlobalSettings.instance.GameOverPanel.SetActive(true));
    }



}
