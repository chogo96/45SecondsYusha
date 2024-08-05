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
            Debug.LogError("이름이가 없습니다");
            return;
        }
        if (AvatarImage == null)
        {
            Debug.LogError("AvatarImage가 없습니다");
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
            Debug.LogError("deckInfo가 null입니다.");
            return;
        }

        DeckGameManager.instance.SetSelectedDeck(deckInfo.Cards);
        // 필요한 경우 추가 작업 수행
        DeckGameManager.instance.SetSelectedDeckInfo(deckInfo);
        // ScreenContent 오브젝트 비활성화
        Transform currentTransform = transform;

        Transform screenContent = transform.parent?.parent?.parent; // 부모의 부모의 부모 찾기

        if (screenContent != null && screenContent.name == "ScreenContent")
        {
            screenContent.gameObject.SetActive(false);
        }
    }
}