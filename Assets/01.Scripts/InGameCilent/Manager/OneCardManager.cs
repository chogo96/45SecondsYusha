using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

// holds the refs to all the Text, Images on the card
public class OneCardManager : MonoBehaviour
{

    public CardAsset cardAsset;
    public OneCardManager PreviewManager;
    [Header("Text Component References")]
    public TMP_Text NameText;
    public TMP_Text DescriptionText;
    public TMP_Text AttackText;
    [Header("Image References")]
    public Image CardTopRibbonImage;
    public Image CardLowRibbonImage;
    public Image CardGraphicImage;
    public Image CardBodyImage;
    public Image CardFaceFrameImage;
    public Image CardFaceGlowImage;
    public Image CardBackGlowImage;
    public Image RarityStoneImage;

    void Awake()
    {
        if (cardAsset != null)
            ReadCardFromAsset();
    }

    private bool canBePlayedNow = false;
    public bool CanBePlayedNow
    {
        get
        {
            return canBePlayedNow;
        }

        set
        {
            canBePlayedNow = value;

            CardFaceGlowImage.enabled = value;
        }
    }

    public void ReadCardFromAsset()
    {
        // ī�� �������� ���� ����
        // 1) ���� �߰�
        if (cardAsset.CharacterAsset != null)
        {
            CardBodyImage.color = cardAsset.CharacterAsset.ClassCardTint;
            CardFaceFrameImage.color = cardAsset.CharacterAsset.ClassCardTint;
            CardTopRibbonImage.color = cardAsset.CharacterAsset.ClassRibbonsTint;
            CardLowRibbonImage.color = cardAsset.CharacterAsset.ClassRibbonsTint;
        }
        else
        {
            CardFaceFrameImage.color = Color.white;
        }
        // 2) ī�� �̸� �߰�
        NameText.text = cardAsset.name;
        // 3) ���� �߰�
        DescriptionText.text = cardAsset.Description;
        // 4) �׷��� �̹��� ����
        CardGraphicImage.sprite = cardAsset.CardImage;

        if (cardAsset.TypeOfCard == TypesOfCards.Attacks)
        {
            //���� ī����
            AttackText.text = cardAsset.SwordAttack.ToString();
        }

        if (PreviewManager != null)
        {
            PreviewManager.cardAsset = cardAsset;
            PreviewManager.ReadCardFromAsset();
        }

        if (RarityStoneImage == null)
            Debug.Log("RarityStoneImage is null on object:" + gameObject.name);

        // NEW apply rarity color to a card 
        RarityStoneImage.color = RarityColors.Instance.ColorsDictionary[cardAsset.Rarity];
    }
}
