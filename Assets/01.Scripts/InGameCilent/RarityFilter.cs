using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RarityFilter : MonoBehaviour
{

    public Image[] Rarities;
    public Color32 HighlightedColor = Color.white;
    public Color32 UnactiveColor = Color.gray;

    private int _currentIndex = -1;

    void Start()
    {
        RemoveAllFilters();
        _currentIndex = -1;
        DeckBuildingScreen.instance.CollectionBrowser.Rarity= (RarityOptions)_currentIndex;
    }

    /// <summary>
    /// 레어도 버튼 클릭시 호출할 함수임
    /// </summary>
    /// <param name="index"></param>
    public void PressRarity(int index)
    {
        RemoveAllFilters();
        if (index != _currentIndex)
        {
            CardCollection.instance.GetCardsWithRarity((RarityOptions)_currentIndex);
           _currentIndex = index;
            Rarities[index].color = HighlightedColor;
        }
        else
        {
            CardCollection.instance.GetCards(true, true, true, (RarityOptions)_currentIndex);
            _currentIndex = -1;
        }

        DeckBuildingScreen.instance.CollectionBrowser.Rarity= (RarityOptions)_currentIndex;
    }

    public void RemoveAllFilters()
    {
        foreach (Image image in Rarities)
            image.color = UnactiveColor;
    }
}
