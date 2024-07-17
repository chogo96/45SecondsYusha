using UnityEngine;
using System.Collections.Generic;

public class Deck : MonoBehaviour
{
    public List<CardAsset> Cards = new List<CardAsset>();
    public List<CardAsset> VanishDeck = new List<CardAsset>();
    public List<CardAsset> DiscardDeck = new List<CardAsset>();

    void Awake()
    {
        Cards.Shuffle();
    }
}
