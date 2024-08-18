using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class DeckIcon : MonoBehaviour
{
    public TMP_Text DeckNameText;
    public GameObject DeckNotCompleteObject;
    private PlayerPortraitVisual portrait;
    private float InitialScale;
    private float TargetScale = 1.3f;
    private bool selected = false;

    public DeckInfo DeckInformation { get; set; }

    void Awake()
    {
        portrait = GetComponent<PlayerPortraitVisual>();
        InitialScale = transform.localScale.x;
    }

    public void ApplyLookToIcon(DeckInfo info)
    {
        DeckInformation = info;

        DeckNotCompleteObject.SetActive(!info.IsComplete());

        portrait.charAsset = info.Character;
        portrait.ApplyLookFromAsset();
        DeckNameText.text = info.DeckName;
    }

    void OnMouseDown()
    {
        if (!selected)
        {
            selected = true;
            if (DeckInformation.IsComplete())
                transform.DOScale(TargetScale, 0.5f);

            DeckSelectionScreen.instance.HeroPanelDeckSelection.SelectDeck(this);
            DeckIcon[] allPortraitButtons = GameObject.FindObjectsOfType<DeckIcon>();
            foreach (DeckIcon m in allPortraitButtons)
                if (m != this)
                    m.Deselect();
        }
        else
        {
            Deselect();
            DeckSelectionScreen.instance.HeroPanelDeckSelection.SelectDeck(null);
        }
    }

    public void Deselect()
    {
        transform.DOScale(InitialScale, 0.5f);
        selected = false;
    }

    public void InstantDeselect()
    {
        transform.localScale = new Vector3(InitialScale, InitialScale, InitialScale);
        selected = false;
    }
}
