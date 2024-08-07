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

    // �߰��� �κ�: ���׶�� �̹��� ������
    [Header("Attack Circle Prefabs")]
    public GameObject SwordCirclePrefab;
    public GameObject MagicCirclePrefab;
    public GameObject ShieldCirclePrefab;
    public GameObject RandomCirclePrefab;

    [Header("UI Parents for Attack Circles")]
    public Transform SwordParent; // �� ���� ���׶�̸� �߰��� �θ� ��ü
    public Transform MagicParent; // ���� ���� ���׶�̸� �߰��� �θ� ��ü
    public Transform ShieldParent; // ���� ���� ���׶�̸� �߰��� �θ� ��ü
    public Transform RandomParent; // ���� ���� ���׶�̸� �߰��� �θ� ��ü

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
        // ī�� �������� �� �� ����
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

        if (PreviewManager != null)
        {
            PreviewManager.cardAsset = cardAsset;
            PreviewManager.ReadCardFromAsset();
        }

        if (RarityStoneImage == null)
            Debug.Log("RarityStoneImage is null on object:" + gameObject.name);

        // NEW apply rarity color to a card 
        RarityStoneImage.color = RarityColors.instance.ColorsDictionary[cardAsset.Rarity];

        // ���� ��ġ�� ���׶�� �̹����� ǥ��
        UpdateAttackImages();
    }

    private void UpdateAttackImages()
    {
        // ���� ���� �̹����� ��� ����
        ClearChildren(SwordParent);
        ClearChildren(MagicParent);
        ClearChildren(ShieldParent);
        ClearChildren(RandomParent);

        // �� ���� Ÿ�Ժ��� ���׶�� �̹��� ����
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
