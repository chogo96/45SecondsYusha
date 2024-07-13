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
        // 컬렉션을 편집 모드로 전환하고, 덱 목록을 오른쪽에 표시합니다.
        // 가장 쉬운 방법은:
        // 0) 화면 숨기기
        DeckBuildingScreen.instance.HideScreen();
        // 1) 동일한 캐릭터에 대해 설정되어 있고 동일한 덱 이름을 불러오도록 합니다.
        DeckBuildingScreen.instance.BuilderScript.BuildADeckFor(savedDeckInfo.Character);
        DeckBuildingScreen.instance.BuilderScript.DeckName.text = savedDeckInfo.DeckName;
        // 2) 이 덱에 있던 동일한 카드들로 채웁니다.
        foreach (CardAsset asset in savedDeckInfo.Cards)
            DeckBuildingScreen.instance.BuilderScript.AddCard(asset);
        // 3) 편집 중인 덱을 DecksStorage에서 삭제합니다.
        DecksStorage.instance.AllDecks.Remove(savedDeckInfo);
        // 4) "완료" 버튼을 누르면 변경된 덱이 새로운 덱으로 추가됩니다.

        // 캐릭터 클래스를 적용하고 탭을 활성화합니다.
        DeckBuildingScreen.instance.TabsScript.SetClassOnClassTab(savedDeckInfo.Character);
        DeckBuildingScreen.instance.CollectionBrowser.ShowCollectionForDeckBuilding(savedDeckInfo.Character);
        // TODO: 이 덱의 인덱스를 저장하여 덱 목록의 끝으로 이동하지 않도록 하고 같은 위치에 추가합니다.

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
