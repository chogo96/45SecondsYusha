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

    public void AssignLowPlayer(PlayerScripts lowPlayer)
    {
        LowPlayer = lowPlayer;
        Debug.Log("LowPlayer assigned successfully");
    }
}