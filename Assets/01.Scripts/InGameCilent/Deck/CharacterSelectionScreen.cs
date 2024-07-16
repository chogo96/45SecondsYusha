using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionScreen : MonoBehaviour
{

    public GameObject ScreenContent;
    public static CharacterSelectionScreen instance;
    public HeroInfoPanel HeroPanel;
    public PortraitMenu[] AllPortraits;

    // Use this for initialization
    void Awake()
    {
        instance = this;
        HideScreen();
    }

    public void ShowScreen()
    {
        ScreenContent.SetActive(true);
        foreach (PortraitMenu p in AllPortraits)
            p.Deselect();
        HeroPanel.SelectCharacter(null);
    }

    public void HideScreen()
    {
        ScreenContent.SetActive(false);
    }

}
