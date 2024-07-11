using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public GameObject cardPrefab; // ī���� �⺻ prefab
    public Transform handTransform; // ī�带 ��ġ�� ��ġ (��: �� �� ��ġ)

    public void CreateCard(CardAsset cardAsset)
    {
        // ī�� prefab �ν��Ͻ�ȭ
        GameObject card = Instantiate(cardPrefab, handTransform);

        // ī�� �̹��� ����
        Image cardImage = card.transform.Find("CardImage").GetComponent<Image>();
        cardImage.sprite = cardAsset.CardImage;

        // ī�� ���� ����
        Text descriptionText = card.transform.Find("Description").GetComponent<Text>();
        descriptionText.text = cardAsset.Description;

        // ī�� �±� ����
        Text tagsText = card.transform.Find("Tags").GetComponent<Text>();
        tagsText.text = cardAsset.Tags;

        // ī�� ��͵� ���� (�ʿ��� ��� ���� ���� ��)
        // RarityOptions rarity = cardAsset.Rarity;

        // ��Ÿ �ʿ��� ������ ����
        // ��: SwordAttack, MagicAttack, ShieldAttack ��

        // �߰������� ī���� ���� �ɷ�ġ�� �Ӽ��� ������ �� ����
    }
}
