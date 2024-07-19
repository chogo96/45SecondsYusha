using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadDeckAndCharacterFromStaticClass : MonoBehaviour
{

    void Awake()
    {
        PlayerScripts player = GetComponent<PlayerScripts>();
        //if (BattleStartInfo.SelectedDeck != null)
        //{
        //    //if (BattleStartInfo.SelectedDeck.Character != null)
        //    //{
        //    //    player.charAsset = BattleStartInfo.SelectedDeck.Character;
        //    //}
        //    if (BattleStartInfo.SelectedDeck.Cards != null)
        //    {
        //        player.deck.Cards = new List<CardAsset>(BattleStartInfo.SelectedDeck.Cards);
        //    }
        //}

    }
}
