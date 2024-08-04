using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectDeckButton : MonoBehaviour
{
    public Image AvatarImage;
    public TMP_Text NameText;
    public DeckInfo deckInfo;

    public void SetDeckInfo(DeckInfo info)
    {
        
        if(NameText == null)
        {
            Debug.LogError("�̸��̰� �����ϴ�");
            return;
        }
        if (AvatarImage == null)
        {
            Debug.LogError("AvatarImage�� �����ϴ�");
            return;
        }
        deckInfo = info;
        AvatarImage.sprite = deckInfo.Character.AvatarImage;
        NameText.text = deckInfo.DeckName;
    }

    public void SelectDeck()
    {
        if (deckInfo == null)
        {
            Debug.LogError("deckInfo�� null�Դϴ�.");
            return;
        }

        DeckGameManager.instance.SetSelectedDeck(deckInfo.Cards);
        // �ʿ��� ��� �߰� �۾� ����
        DeckGameManager.instance.SetSelectedDeckInfo(deckInfo);
        // ScreenContent ������Ʈ ��Ȱ��ȭ
        Transform currentTransform = transform;

        Transform screenContent = transform.parent?.parent?.parent; // �θ��� �θ��� �θ� ã��

        if (screenContent != null && screenContent.name == "ScreenContent")
        {
            screenContent.gameObject.SetActive(false);
        }
    }
}