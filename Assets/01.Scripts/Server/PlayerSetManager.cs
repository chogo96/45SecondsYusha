using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSetManager : MonoBehaviour
{
    private Image _playerImage;
    private Image _bleedDebuffImage;
    private Image _blindDebuffImage;
    private Image _confusionDebuffImage;
    private Image _deckImage;
    private TMP_Text _deckCountText;
    private Image _handImage;
    private TMP_Text _handCountText;

    private void Awake()
    {
        _playerImage = transform.Find("Player_Image").GetComponent<Image>();
        _bleedDebuffImage = transform.Find("BleedDebuffImage").GetComponent<Image>();
        _blindDebuffImage = transform.Find("BlindDebuffImage").GetComponent<Image>();
        _confusionDebuffImage = transform.Find("ConfusionDebuffImage").GetComponent<Image>();
        _deckImage = transform.Find("DeckImage").GetComponent<Image>();
        _deckCountText = transform.Find("DeckImage/DeckCountText (TMP)").GetComponent<TMP_Text>();
        _handImage = transform.Find("HandImage").GetComponent<Image>();
        _handCountText = transform.Find("HandImage/HandCountText (TMP)").GetComponent<TMP_Text>();
    }
}
