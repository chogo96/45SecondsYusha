using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterSelectionTabs : MonoBehaviour
{
    public List<CharacterFilterTab> Tabs = new List<CharacterFilterTab>();
    public CharacterFilterTab ClassTab;
    public CharacterFilterTab NeutralTabWhenCollectionBrowsing;
    private int currentIndex = 0;

    public void SelectTab(CharacterFilterTab tab, bool instant)
    {
        if (tab == null || !Tabs.Contains(tab))
        {
            Debug.LogWarning("Invalid tab selected.");
            return;
        }

        int newIndex = Tabs.IndexOf(tab);

        if (newIndex == currentIndex)
            return;

        currentIndex = newIndex;

        // 다른 탭 비활성화
        foreach (CharacterFilterTab Tab in Tabs)
        {
            if (Tab != tab)
                Tab.Deselect(instant);
        }

        // 선택된 탭 활성화
        tab.Select(instant);

        if (DeckBuildingScreen.instance != null)
        {
            if (DeckBuildingScreen.instance.CollectionBrowser != null)
            {
                DeckBuildingScreen.instance.CollectionBrowser.Asset = tab.Asset;
                DeckBuildingScreen.instance.CollectionBrowser.IncludeAllCharacters = tab.showAllCharacters;

                FirebaseCardManager firebaseCardManager = FindObjectOfType<FirebaseCardManager>();
                if (firebaseCardManager != null)
                {
                    firebaseCardManager.SetSelectedCharacter(tab.Asset);
                }
                else
                {
                    Debug.LogWarning("FirebaseCardManager instance is null.");
                }
            }
            else
            {
                Debug.LogWarning("CollectionBrowser instance is null.");
            }
        }
        else
        {
            Debug.LogWarning("DeckBuildingScreen instance is null.");
        }
    }

    public void SetClassOnClassTab(CharacterAsset asset)
    {
        if (ClassTab == null || asset == null)
        {
            Debug.LogWarning("ClassTab or asset is null.");
            return;
        }

        ClassTab.Asset = asset;
        TMP_Text textComponent = ClassTab.GetComponentInChildren<TMP_Text>();

        if (textComponent != null)
        {
            textComponent.text = asset.name;
        }
        else
        {
            Debug.LogWarning("TMP_Text component is not found in ClassTab.");
        }
    }
}
