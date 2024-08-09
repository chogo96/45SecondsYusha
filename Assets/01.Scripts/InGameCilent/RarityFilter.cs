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
    private FirebaseCardManager firebaseCardManager;

    void Start()
    {
        firebaseCardManager = FindObjectOfType<FirebaseCardManager>();
        RemoveAllFilters();
        _currentIndex = -1;
        UpdateRarityFilter(_currentIndex);
    }

    /// <summary>
    /// ��� ��ư Ŭ���� ȣ���� �Լ���
    /// </summary>
    /// <param name="index"></param>
    public void PressRarity(int index)
    {
        RemoveAllFilters();
        if (index != _currentIndex)
        {
            _currentIndex = index;
            Rarities[index].color = HighlightedColor;
        }
        else
        {
            _currentIndex = -1;
        }

        UpdateRarityFilter(_currentIndex);
    }

    private void UpdateRarityFilter(int rarityIndex)
    {
        if (firebaseCardManager != null)
        {
            //firebaseCardManager.FilterByRarity((RarityOptions)rarityIndex);
        }
        else
        {
            Debug.LogError("FirebaseCardManager�� ã�� �� �����ϴ�.");
        }
    }

    public void RemoveAllFilters()
    {
        foreach (Image image in Rarities)
            image.color = UnactiveColor;
    }
}
