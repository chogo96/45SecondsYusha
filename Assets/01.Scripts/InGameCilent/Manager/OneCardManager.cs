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

//    // 추가된 부분: 동그라미 이미지 프리팹
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
//    public Transform SwordParent; // 검 공격 동그라미를 추가할 부모 객체
//    public Transform MagicParent; // 마법 공격 동그라미를 추가할 부모 객체
//    public Transform ShieldParent; // 방패 공격 동그라미를 추가할 부모 객체
//    public Transform RandomParent; // 랜덤 공격 동그라미를 추가할 부모 객체

//    [Header("UI Parents for CheckCardEffect")]
//    public Transform ConfusionParent; //혼란 디버프 해제를 의미하는 스프라이트를 추가할 부모객체
//    public Transform BleedParent; //출혈 디버프 해제를 의미하는 스프라이트를 추가할 부모객체
//    public Transform BlindParent; //실명 디버프 해제를 의미하는 스프라이트를 추가할 부모객체
//    public Transform AllDebuffParent; //모든 디버프 해제를 의미하는 스프라이트를 추가할 부모객체

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
//        // 카드 전부한테 쓸 수 있음
//        // 1) 색조 추가
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
//        // 2) 카드 이름 추가
//        NameText.text = cardAsset.name;
//        // 3) 설명 추가
//        DescriptionText.text = cardAsset.Description;
//        // 4) 그래픽 이미지 변경
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

//        // 공격 수치를 동그라미 이미지로 표시
//        UpdateAttackImages();
//        //디버프 해제 요소를 아이콘으로 표시
//        UpdateDebuffImages();
//    }

//    private void UpdateAttackImages()
//    {
//        // 기존 공격 이미지를 모두 제거
//        ClearChildren(SwordParent);
//        ClearChildren(MagicParent);
//        ClearChildren(ShieldParent);
//        ClearChildren(RandomParent);

//        // 각 공격 타입별로 동그라미 이미지 생성
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

    // 추가된 부분: 동그라미 이미지 프리팹
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
    public Transform SwordParent; // 검 공격 동그라미를 추가할 부모 객체
    public Transform MagicParent; // 마법 공격 동그라미를 추가할 부모 객체
    public Transform ShieldParent; // 방패 공격 동그라미를 추가할 부모 객체
    public Transform RandomParent; // 랜덤 공격 동그라미를 추가할 부모 객체

    [Header("UI Parents for CheckCardEffect")]
    public Transform ConfusionParent; //혼란 디버프 해제를 의미하는 스프라이트를 추가할 부모객체
    public Transform BleedParent; //출혈 디버프 해제를 의미하는 스프라이트를 추가할 부모객체
    public Transform BlindParent; //실명 디버프 해제를 의미하는 스프라이트를 추가할 부모객체
    public Transform AllDebuffParent; //모든 디버프 해제를 의미하는 스프라이트를 추가할 부모객체

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
        // 3) 설명 추가 가독성을 위해 첨삭처리 필요 시 첨삭 해제하고 하면됨
        //DescriptionText.text = cardAsset.Description;
        DescriptionText.text = null;
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
        //디버프 해제 요소를 아이콘으로 표시
        UpdateDebuffImages();
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

    private void UpdateDebuffImages()
    {
        // 기존 디버프 이미지를 모두 제거
        ClearDebuffChildren(ConfusionParent);
        ClearDebuffChildren(BleedParent);
        ClearDebuffChildren(BlindParent);
        ClearDebuffChildren(AllDebuffParent);

        // cardAsset의 RemoveDebuff 값을 확인하여 해당하는 이모티콘을 추가
        switch (cardAsset.RemoveDebuff)
        {
            case "혼란":
                Instantiate(ConfusionPrefab, ConfusionParent);
                break;
            case "출혈":
                Instantiate(BleedPrefab, BleedParent);
                break;
            case "실명":
                Instantiate(BlindPrefab, BlindParent);
                break;
            case "모든":
                Instantiate(AllDebuffPrefab, AllDebuffParent);
                break;
            case "랜덤":
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
