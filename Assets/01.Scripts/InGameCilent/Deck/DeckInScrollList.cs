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
        // 0) 화면 숨기기
        DeckBuildingScreen.instance.HideScreen();
        Tabs.SetActive(false);
        // 1) FirebaseCardManager를 통해 덱을 로드
        if (firebaseCardManager != null)
        {
            firebaseCardManager.SetSelectedCharacter(savedDeckInfo.Character);
        }
        else
        {
            Debug.LogError("FirebaseCardManager를 찾을 수 없습니다.");
            return;
        }

        // 2) 동일한 캐릭터와 덱 이름 설정
        DeckBuildingScreen.instance.BuilderScript.BuildADeckFor(savedDeckInfo.Character);
        DeckBuildingScreen.instance.BuilderScript.DeckName.text = savedDeckInfo.DeckName;

        // 3) 이 덱에 있던 동일한 카드들로 채우기
        foreach (CardAsset asset in savedDeckInfo.Cards)
        {
            DeckBuildingScreen.instance.BuilderScript.AddCard(asset);
        }

        // 4) 편집 중인 덱을 DecksStorage에서 삭제
        DecksStorage.instance.AllDecks.Remove(savedDeckInfo);

        // 5) 덱 빌딩 화면을 표시
        DeckBuildingScreen.instance.ShowScreenForDeckBuilding();
    }

    public void DeleteThisDeck()
    {
        // TODO: 정말 삭제할건가요? 창 만들면 좋겠음
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
        // 덱 삭제 버튼 보여주기
        DeleteDeckButton.SetActive(true);
    }

    public void OnPointerExit(PointerEventData data)
    {
        // 덱 삭제 버튼 숨기기
        DeleteDeckButton.SetActive(false);
    }
}
