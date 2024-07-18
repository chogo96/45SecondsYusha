using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;

public class GlobalSettings : MonoBehaviour
{
    [Header("Players")]
    public PlayerScripts TopPlayer;
    public PlayerScripts LowPlayer;
    [Header("Colors")]
    public Color32 CardBodyStandardColor;
    public Color32 CardRibbonsStandardColor;
    public Color32 CardGlowColor;
    [Header("Numbers and Values")]
    public float CardPreviewTime = 1f;
    public float CardTransitionTime = 1f;
    public float CardPreviewTimeFast = 0.2f;
    public float CardTransitionTimeFast = 0.5f;
    [Header("Prefabs and Assets")]
    public GameObject NoTargetSpellCardPrefab;
    public GameObject TargetedSpellCardPrefab;
    public GameObject CreatureCardPrefab;
    public GameObject CreaturePrefab;
    public GameObject DamageEffectPrefab;
    public GameObject ExplosionPrefab;
    [Header("Other")]
    public GameObject GameOverPanel;
    public Dictionary<AreaPosition, PlayerScripts> Players = new Dictionary<AreaPosition, PlayerScripts>();
    public static GlobalSettings instance;

    private void Awake()
    {
        if (instance != this && instance != null)
            Destroy(instance.gameObject);
        instance = this;
    }
    //void Awake()
    //{
    //    Players.Add(AreaPosition.Top, TopPlayer);
    //    Players.Add(AreaPosition.Low, LowPlayer);
    //}

    //public bool CanControlThisPlayer(AreaPosition owner)
    //{
    //    bool PlayersTurn = (TurnManager.Instance.whoseTurn == Players[owner]);
    //    bool NotDrawingAnyCards = !Command.CardDrawPending();
    //    return Players[owner].PArea.AllowedToControlThisPlayer && Players[owner].PArea.ControlsON && PlayersTurn && NotDrawingAnyCards;
    //}

    //public bool CanControlThisPlayer(Player ownerPlayer)
    //{
    //    bool PlayersTurn = (TurnManager.instance.whoseTurn == ownerPlayer);
    //    return ownerPlayer.HasRejoined;
    //    //bool NotDrawingAnyCards = !Command.CardDrawPending();
    //    //return ownerPlayer.PArea.AllowedToControlThisPlayer && ownerPlayer.PArea.ControlsON && PlayersTurn && NotDrawingAnyCards;
    //}
}
