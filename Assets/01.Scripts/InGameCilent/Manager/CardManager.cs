using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public GameObject cardPrefab; // 카드의 기본 prefab
    public Transform handTransform; // 카드를 배치할 위치 (예: 손 패 위치)

    public void CreateCard(CardAsset cardAsset)
    {
        // 카드 prefab 인스턴스화
        GameObject card = Instantiate(cardPrefab, handTransform);

        // 카드 이미지 설정
        Image cardImage = card.transform.Find("CardImage").GetComponent<Image>();
        cardImage.sprite = cardAsset.CardImage;

        // 카드 설명 설정
        Text descriptionText = card.transform.Find("Description").GetComponent<Text>();
        descriptionText.text = cardAsset.Description;

        // 카드 태그 설정
        Text tagsText = card.transform.Find("Tags").GetComponent<Text>();
        tagsText.text = cardAsset.Tags;

        // 카드 희귀도 설정 (필요한 경우 색상 변경 등)
        // RarityOptions rarity = cardAsset.Rarity;

        // 기타 필요한 데이터 설정
        // 예: SwordAttack, MagicAttack, ShieldAttack 등

        // 추가적으로 카드의 각종 능력치나 속성을 설정할 수 있음
    }
}
