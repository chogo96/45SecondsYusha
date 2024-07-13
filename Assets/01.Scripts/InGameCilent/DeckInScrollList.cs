using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DeckInScrollList : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public Image AvatarImage;
    public Text NameText;
    public GameObject DeleteDeckButton;
    public DeckInfo savedDeckInfo;

    public void Awake()
    {
        DeleteDeckButton.SetActive(false);
    }

    public void EditThisDeck()
    {
        // �÷����� ���� ���� ��ȯ�ϰ�, �� ����� �����ʿ� ǥ���մϴ�.
        // ���� ���� �����:
        // 0) ȭ�� �����
        DeckBuildingScreen.instance.HideScreen();
        // 1) ������ ĳ���Ϳ� ���� �����Ǿ� �ְ� ������ �� �̸��� �ҷ������� �մϴ�.
        DeckBuildingScreen.instance.BuilderScript.BuildADeckFor(savedDeckInfo.Character);
        DeckBuildingScreen.instance.BuilderScript.DeckName.text = savedDeckInfo.DeckName;
        // 2) �� ���� �ִ� ������ ī���� ä��ϴ�.
        foreach (CardAsset asset in savedDeckInfo.Cards)
            DeckBuildingScreen.instance.BuilderScript.AddCard(asset);
        // 3) ���� ���� ���� DecksStorage���� �����մϴ�.
        DecksStorage.instance.AllDecks.Remove(savedDeckInfo);
        // 4) "�Ϸ�" ��ư�� ������ ����� ���� ���ο� ������ �߰��˴ϴ�.

        // ĳ���� Ŭ������ �����ϰ� ���� Ȱ��ȭ�մϴ�.
        DeckBuildingScreen.instance.TabsScript.SetClassOnClassTab(savedDeckInfo.Character);
        DeckBuildingScreen.instance.CollectionBrowser.ShowCollectionForDeckBuilding(savedDeckInfo.Character);
        // TODO: �� ���� �ε����� �����Ͽ� �� ����� ������ �̵����� �ʵ��� �ϰ� ���� ��ġ�� �߰��մϴ�.

        DeckBuildingScreen.instance.ShowScreenForDeckBuilding();
    }

    public void DeleteThisDeck()
    {
        // TODO: ���� �����Ұǰ���? â ����� ������
        DecksStorage.instance.AllDecks.Remove(savedDeckInfo);
        Destroy(gameObject);
    }

    public void ApplyInfo(DeckInfo deckInfo)
    {
        AvatarImage.sprite = deckInfo.Character.AvatarImage;
        NameText.text = deckInfo.DeckName;
        savedDeckInfo = deckInfo;
    }

    public void OnPointerEnter(PointerEventData data)
    {
        // �� ���� ��ư �����ֱ�
        DeleteDeckButton.SetActive(true);
    }

    public void OnPointerExit(PointerEventData data)
    {
        // �� ���� ��ư �����
        DeleteDeckButton.SetActive(false);
    }
}
