using UnityEngine;
using UnityEngine.UI;
using TMPro;

// holds the refs to all the Text, Images on the card
public class OneCardManager : MonoBehaviour
{
    public CardAsset cardAsset;
    public OneCardManager PreviewManager;
    [Header("Text Component References")]
    public Text NameText;
    public Text DescriptionText;
    //public TMP_Text AttackText;
    [Header("Image References")]
    public Image CardTopRibbonImage;
    public Image CardLowRibbonImage;
    public Image CardGraphicImage;
    public Image CardBodyImage;
    public Image CardFaceFrameImage;
    public Image CardFaceGlowImage;
    public Image CardBackGlowImage;
    public Image RarityStoneImage;

    // 추가된 부분: 동그라미 이미지 프리팹
    [Header("Attack Circle Prefabs")]
    public GameObject SwordCirclePrefab;
    public GameObject MagicCirclePrefab;
    public GameObject ShieldCirclePrefab;
    public GameObject RandomCirclePrefab;

    [Header("UI Parents for Attack Circles")]
    public Transform SwordParent; // 검 공격 동그라미를 추가할 부모 객체
    public Transform MagicParent; // 마법 공격 동그라미를 추가할 부모 객체
    public Transform ShieldParent; // 방패 공격 동그라미를 추가할 부모 객체
    public Transform RandomParent; // 랜덤 공격 동그라미를 추가할 부모 객체

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
        // 카드 전부한테 쓸 수 있음
        // 1) 색조 추가
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
        // 2) 카드 이름 추가
        NameText.text = cardAsset.name;
        // 3) 설명 추가
        DescriptionText.text = cardAsset.Description;
        // 4) 그래픽 이미지 변경
        CardGraphicImage.sprite = cardAsset.CardImage;

        if (PreviewManager != null)
        {
            PreviewManager.cardAsset = cardAsset;
            PreviewManager.ReadCardFromAsset();
        }

        if (RarityStoneImage == null)
            Debug.Log("RarityStoneImage is null on object:" + gameObject.name);

        // NEW apply rarity color to a card 
        RarityStoneImage.color = RarityColors.instance.ColorsDictionary[cardAsset.Rarity];

        // 공격 수치를 동그라미 이미지로 표시
        UpdateAttackImages();
    }

    private void UpdateAttackImages()
    {
        // 기존 공격 이미지를 모두 제거
        ClearChildren(SwordParent);
        ClearChildren(MagicParent);
        ClearChildren(ShieldParent);
        ClearChildren(RandomParent);

        // 각 공격 타입별로 동그라미 이미지 생성
        CreateAttackCircles(SwordParent, SwordCirclePrefab, cardAsset.SwordAttack);
        CreateAttackCircles(MagicParent, MagicCirclePrefab, cardAsset.MagicAttack);
        CreateAttackCircles(ShieldParent, ShieldCirclePrefab, cardAsset.ShieldAttack);
        CreateAttackCircles(RandomParent, RandomCirclePrefab, cardAsset.RandomAttack);
    }

    private void ClearChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    private void CreateAttackCircles(Transform parent, GameObject prefab, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Instantiate(prefab, parent);
        }
    }
}
