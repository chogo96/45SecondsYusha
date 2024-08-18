using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PortraitMenu : MonoBehaviour
{

    public CharacterAsset asset;
    public PlayerPortraitVisual portrait;
    private float InitialScale;
    private float TargetScale = 1.3f;
    private bool selected = false;

    void Awake()
    {
        portrait.ApplyLookFromAsset();
        InitialScale = transform.localScale.x;
    }

    void OnMouseDown()
    {
        // 애니메이션 재생
        if (!selected)
        {
            selected = true;
            transform.DOScale(TargetScale, 0.5f);
            CharacterSelectionScreen.instance.HeroPanel.SelectCharacter(this);
            PortraitMenu[] allPortraitButtons = GameObject.FindObjectsOfType<PortraitMenu>();
            foreach (PortraitMenu m in allPortraitButtons)
                if (m != this)
                    m.Deselect();
        }
        else
        {
            Deselect();
            CharacterSelectionScreen.instance.HeroPanel.SelectCharacter(null);
        }
    }

    public void Deselect()
    {
        transform.DOScale(InitialScale, 0.5f);
        selected = false;
    }
}
