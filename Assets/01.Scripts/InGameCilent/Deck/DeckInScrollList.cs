using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

public class DeckInScrollList : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image AvatarImage;
    public TMP_Text NameText;
    public GameObject DeleteDeckButton;
    public DeckInfo savedDeckInfo;
    private FirebaseCardManager firebaseCardManager;
    private GameObject Tabs;

    public void Awake()
    {
        DeleteDeckButton.SetActive(false);
        firebaseCardManager = FindObjectOfType<FirebaseCardManager>();
        Tabs = GameObject.Find("Tabs");
    }

    public void EditThisDeck()
    {
        // 0) ȭ�� �����
        DeckBuildingScreen.instance.HideScreen();
        Tabs.SetActive(false);
        // 1) FirebaseCardManager�� ���� ���� �ε�
        if (firebaseCardManager != null)
        {
            firebaseCardManager.SetSelectedCharacter(savedDeckInfo.Character);
        }
        else
        {
            Debug.LogError("FirebaseCardManager�� ã�� �� �����ϴ�.");
            return;
        }

        // 2) ������ ĳ���Ϳ� �� �̸� ����
        DeckBuildingScreen.instance.BuilderScript.BuildADeckFor(savedDeckInfo.Character);
        DeckBuildingScreen.instance.BuilderScript.DeckName.text = savedDeckInfo.DeckName;

        // 3) �� ���� �ִ� ������ ī���� ä���
        foreach (CardAsset asset in savedDeckInfo.Cards)
        {
            DeckBuildingScreen.instance.BuilderScript.AddCard(asset);
        }

        // 4) ���� ���� ���� DecksStorage���� ����
        DecksStorage.instance.AllDecks.Remove(savedDeckInfo);

        // 5) �� ���� ȭ���� ǥ��
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
