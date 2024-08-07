using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectDeckButton : MonoBehaviour
{
    public Image AvatarImage;
    public TMP_Text NameText;
    public DeckInfo deckInfo;
    private LobbyPlayer lobbyPlayer;

    private void Awake()
    {
        lobbyPlayer = FindObjectOfType<LobbyPlayer>();
    }

    public void SetDeckInfo(DeckInfo info)
    {
        
        if(NameText == null)
        {
            Utils.LogRed("�̸��̰� �����ϴ�");
            return;
        }
        if (AvatarImage == null)
        {
            Utils.LogRed("AvatarImage�� �����ϴ�");
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
            Utils.LogRed("deckInfo�� null�Դϴ�.");
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


        // ���� �÷��̾��� �̹��� ����
        Utils.LogRed(deckInfo.Character.AvatarImage.name);
        lobbyPlayer.SetPlayerImage(deckInfo.Character.AvatarImage.name);
    }
}
