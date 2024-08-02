using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectDeckButton : MonoBehaviour
{
    public Image AvatarImage;
    public TMP_Text NameText;
    private DeckInfo deckInfo;

    public void SetDeckInfo(DeckInfo info)
    {
        
        if(NameText == null)
        {
            return;
        }
        if (AvatarImage == null)
        {
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
            return;
        }

        DeckGameManager.instance.SetSelectedDeck(deckInfo.Cards);
        // 필요한 경우 추가 작업 수행
    }
}