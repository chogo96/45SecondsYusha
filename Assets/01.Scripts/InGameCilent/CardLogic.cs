using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class CardLogic : IIdentifiable, IComparable<CardLogic>
{
    public PlayerScripts owner;
    public int UniqueCardID;
    public CardAsset cardAsset;

    public static Dictionary<int, CardLogic> CardsCreatedThisGame = new Dictionary<int, CardLogic>();

    public int ID
    {
        get { return UniqueCardID; }
    }

    public bool CanBePlayed
    {
        get
        {
            bool fieldNotFull = true;
            return fieldNotFull;
        }
    }

    public CardLogic(CardAsset cardAsset, PlayerScripts owner)
    {
        this.owner = owner;
        this.cardAsset = cardAsset;
        UniqueCardID = IDFactory.GetUniqueID();
        CardsCreatedThisGame.Add(UniqueCardID, this);
    }

    public int CompareTo(CardLogic other)
    {
        return this.cardAsset.name.CompareTo(other.cardAsset.name);
    }
}
