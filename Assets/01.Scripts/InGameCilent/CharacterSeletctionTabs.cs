using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterSelectionTabs : MonoBehaviour
{
    public List<CharacterFilterTab> Tabs = new List<CharacterFilterTab>();
    public CharacterFilterTab ClassTab;
    public CharacterFilterTab NeutralTabWhenCollectionBrowsing;
    private int currentIndex = 0;

    public void SelectTab(CharacterFilterTab tab, bool instant)
    {
        int newIndex = Tabs.IndexOf(tab);

        if (newIndex == currentIndex)
            return;

        currentIndex = newIndex;

        // �ٸ� ��
        foreach (CharacterFilterTab Tab in Tabs)
        {
            if (Tab != tab)
                Tab.Deselect(instant);
        }
        // �츮�� �� ���� �����մϴ�.
        tab.Select(instant);
        DeckBuildingScreen.instance.CollectionBrowser.Asset = tab.Asset;
        DeckBuildingScreen.instance.CollectionBrowser.IncludeAllCharacters = tab.showAllCharacters;
    }

    public void SetClassOnClassTab(CharacterAsset asset)
    {
        ClassTab.Asset = asset;
        ClassTab.GetComponentInChildren<Text>().text = asset.name;
    }
}
