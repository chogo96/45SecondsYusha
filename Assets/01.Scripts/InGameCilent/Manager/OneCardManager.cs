//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;
//using System;

//// holds the refs to all the Text, Images on the card
//public class OneCardManager : MonoBehaviour
//{
//    public CardAsset cardAsset;
//    public OneCardManager PreviewManager;
//    [Header("Text Component References")]
//    public Text NameText;
//    public Text DescriptionText;
//    //public TMP_Text AttackText;
//    [Header("Image References")]
//    public Image CardTopRibbonImage;
//    public Image CardLowRibbonImage;
//    public Image CardGraphicImage;
//    public Image CardBodyImage;
//    public Image CardFaceFrameImage;
//    public Image CardFaceGlowImage;
//    public Image CardBackGlowImage;
//    public Image RarityStoneImage;

//    // �߰��� �κ�: ���׶�� �̹��� ������
//    [Header("Attack Circle Prefabs")]
//    public GameObject SwordCirclePrefab;
//    public GameObject MagicCirclePrefab;
//    public GameObject ShieldCirclePrefab;
//    public GameObject RandomCirclePrefab;

//    [Header("DebuffImagePrefabs")]
//    public GameObject BleedPrefab;
//    public GameObject BlindPrefab;
//    public GameObject ConfusionPrefab;
//    public GameObject AllDebuffPrefab;


//    [Header("UI Parents for Attack Circles")]
//    public Transform SwordParent; // �� ���� ���׶�̸� �߰��� �θ� ��ü
//    public Transform MagicParent; // ���� ���� ���׶�̸� �߰��� �θ� ��ü
//    public Transform ShieldParent; // ���� ���� ���׶�̸� �߰��� �θ� ��ü
//    public Transform RandomParent; // ���� ���� ���׶�̸� �߰��� �θ� ��ü

//    [Header("UI Parents for CheckCardEffect")]
//    public Transform ConfusionParent; //ȥ�� ����� ������ �ǹ��ϴ� ��������Ʈ�� �߰��� �θ�ü
//    public Transform BleedParent; //���� ����� ������ �ǹ��ϴ� ��������Ʈ�� �߰��� �θ�ü
//    public Transform BlindParent; //�Ǹ� ����� ������ �ǹ��ϴ� ��������Ʈ�� �߰��� �θ�ü
//    public Transform AllDebuffParent; //��� ����� ������ �ǹ��ϴ� ��������Ʈ�� �߰��� �θ�ü

//    void Awake()
//    {
//        if (cardAsset != null)
//            ReadCardFromAsset();
//    }

//    private bool canBePlayedNow = false;
//    public bool CanBePlayedNow
//    {
//        get
//        {
//            return canBePlayedNow;
//        }

//        set
//        {
//            canBePlayedNow = value;

//            CardFaceGlowImage.enabled = value;
//        }
//    }

//    public void ReadCardFromAsset()
//    {
//        // ī�� �������� �� �� ����
//        // 1) ���� �߰�
//        if (cardAsset.CharacterAsset != null)
//        {
//            CardBodyImage.color = cardAsset.CharacterAsset.ClassCardTint;
//            CardFaceFrameImage.color = cardAsset.CharacterAsset.ClassCardTint;
//            CardTopRibbonImage.color = cardAsset.CharacterAsset.ClassRibbonsTint;
//            CardLowRibbonImage.color = cardAsset.CharacterAsset.ClassRibbonsTint;
//        }
//        else
//        {
//            CardFaceFrameImage.color = Color.white;
//        }
//        // 2) ī�� �̸� �߰�
//        NameText.text = cardAsset.name;
//        // 3) ���� �߰�
//        DescriptionText.text = cardAsset.Description;
//        // 4) �׷��� �̹��� ����
//        CardGraphicImage.sprite = cardAsset.CardImage;

//        if (PreviewManager != null)
//        {
//            PreviewManager.cardAsset = cardAsset;
//            PreviewManager.ReadCardFromAsset();
//        }

//        if (RarityStoneImage == null)
//            Debug.Log("RarityStoneImage is null on object:" + gameObject.name);

//        // NEW apply rarity color to a card 
//        RarityStoneImage.color = RarityColors.instance.ColorsDictionary[cardAsset.Rarity];

//        // ���� ��ġ�� ���׶�� �̹����� ǥ��
//        UpdateAttackImages();
//        //����� ���� ��Ҹ� ���������� ǥ��
//        UpdateDebuffImages();
//    }

//    private void UpdateAttackImages()
//    {
//        // ���� ���� �̹����� ��� ����
//        ClearChildren(SwordParent);
//        ClearChildren(MagicParent);
//        ClearChildren(ShieldParent);
//        ClearChildren(RandomParent);

//        // �� ���� Ÿ�Ժ��� ���׶�� �̹��� ����
//        CreateAttackCircles(SwordParent, SwordCirclePrefab, cardAsset.SwordAttack);
//        CreateAttackCircles(MagicParent, MagicCirclePrefab, cardAsset.MagicAttack);
//        CreateAttackCircles(ShieldParent, ShieldCirclePrefab, cardAsset.ShieldAttack);
//        CreateAttackCircles(RandomParent, RandomCirclePrefab, cardAsset.RandomAttack);
//    }


//    private void UpdateDebuffImages()
//    {
//        ClearDebuffChildren(ConfusionParent);
//        ClearDebuffChildren(BleedParent);
//        ClearDebuffChildren(BlindParent);
//        ClearDebuffChildren(AllDebuffParent);
//    }

//    private void ClearDebuffChildren(Transform parent)
//    {
//        foreach (Transform child in parent)
//        {
//            Destroy(child.gameObject);
//        }
//    }

//    private void ClearChildren(Transform parent)
//    {
//        foreach (Transform child in parent)
//        {
//            Destroy(child.gameObject);
//        }
//    }

//    private void CreateAttackCircles(Transform parent, GameObject prefab, int count)
//    {
//        for (int i = 0; i < count; i++)
//        {
//            Instantiate(prefab, parent);
//        }
//    }
//}
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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

    [Header("DebuffImagePrefabs")]
    public GameObject BleedPrefab;
    public GameObject BlindPrefab;
    public GameObject ConfusionPrefab;
    public GameObject AllDebuffPrefab;


    [Header("UI Parents for Attack Circles")]
    public Transform SwordParent; // �� ���� ���׶�̸� �߰��� �θ� ��ü
    public Transform MagicParent; // ���� ���� ���׶�̸� �߰��� �θ� ��ü
    public Transform ShieldParent; // ���� ���� ���׶�̸� �߰��� �θ� ��ü
    public Transform RandomParent; // ���� ���� ���׶�̸� �߰��� �θ� ��ü

    [Header("UI Parents for CheckCardEffect")]
    public Transform ConfusionParent; //ȥ�� ����� ������ �ǹ��ϴ� ��������Ʈ�� �߰��� �θ�ü
    public Transform BleedParent; //���� ����� ������ �ǹ��ϴ� ��������Ʈ�� �߰��� �θ�ü
    public Transform BlindParent; //�Ǹ� ����� ������ �ǹ��ϴ� ��������Ʈ�� �߰��� �θ�ü
    public Transform AllDebuffParent; //��� ����� ������ �ǹ��ϴ� ��������Ʈ�� �߰��� �θ�ü

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
        // 3) ���� �߰� �������� ���� ÷��ó�� �ʿ� �� ÷�� �����ϰ� �ϸ��
        //DescriptionText.text = cardAsset.Description;
        DescriptionText.text = null;
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
        //����� ���� ��Ҹ� ���������� ǥ��
        UpdateDebuffImages();
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

    private void UpdateDebuffImages()
    {
        // ���� ����� �̹����� ��� ����
        ClearDebuffChildren(ConfusionParent);
        ClearDebuffChildren(BleedParent);
        ClearDebuffChildren(BlindParent);
        ClearDebuffChildren(AllDebuffParent);

        // cardAsset�� RemoveDebuff ���� Ȯ���Ͽ� �ش��ϴ� �̸�Ƽ���� �߰�
        switch (cardAsset.RemoveDebuff)
        {
            case "ȥ��":
                Instantiate(ConfusionPrefab, ConfusionParent);
                break;
            case "����":
                Instantiate(BleedPrefab, BleedParent);
                break;
            case "�Ǹ�":
                Instantiate(BlindPrefab, BlindParent);
                break;
            case "���":
                Instantiate(AllDebuffPrefab, AllDebuffParent);
                break;
            case "����":
                Instantiate(AllDebuffPrefab, AllDebuffParent);
                break;
            case "0":
                break;
            default: 
                break;
        }
    }

    private void ClearDebuffChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
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
