using UnityEngine;
using System.Collections;
using System;
using static UnityEngine.Rendering.DebugUI;

public class PlayerScripts : MonoBehaviour, ICharacter
{
    public int Sword;
    public int Magic;
    public int Shield;
    private Enemy currentEnemy;
    public int PlayerID;
    public CharacterAsset charAsset;
    public PlayerArea PArea;
    public SpellEffect HeroPowerEffect;
    public bool usedHeroPowerThisGame = false;

    public Deck deck;
    public Hand hand;
    public Table table;

    public static PlayerScripts[] Players;

    public int ID
    {
        get { return PlayerID; }
    }

    public PlayerScripts otherPlayer
    {
        get
        {
            if (Players[0] == this)
                return Players[1];
            else
                return Players[0];
        }
    }

    public delegate void VoidWithNoArguments();

    public void SetCurrentEnemy(Enemy enemy)
    {
        currentEnemy = enemy;
    }

    public void ResetValues()
    {
        Sword = 0;
        Magic = 0;
        Shield = 0;
    }

    void Awake()
    {
        Players = GameObject.FindObjectsOfType<PlayerScripts>();
        PlayerID = IDFactory.GetUniqueID();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
            DrawACard();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Sword++;
            Debug.Log("��!");
            currentEnemy?.CheckDeathCondition(Sword, Magic, Shield);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Magic++;
            Debug.Log("����!");
            currentEnemy?.CheckDeathCondition(Sword, Magic, Shield);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Shield++;
            Debug.Log("����!");
            currentEnemy?.CheckDeathCondition(Sword, Magic, Shield);
        }
    }

    public void DrawACard(bool fast = false)
    {
        if (deck.Cards.Count > 0)
        {
            if (hand.CardsInHand.Count < PArea.handVisual.slots.Children.Length)
            {
                CardLogic newCard = new CardLogic(deck.Cards[0], this);
                hand.CardsInHand.Insert(0, newCard);
                deck.Cards.RemoveAt(0);
                new DrawACardCommand(hand.CardsInHand[0], this, fast, fromDeck: true).AddToQueue();
            }
        }
        else
        {
            //���� �ٽ����� Ż�� ȿ�� �ִ� ��
        }
    }

    public void GetACardNotFromDeck(CardAsset cardAsset)
    {
        if (hand.CardsInHand.Count < PArea.handVisual.slots.Children.Length)
        {
            CardLogic newCard = new CardLogic(cardAsset, this);
            newCard.owner = this;
            hand.CardsInHand.Insert(0, newCard);
            new DrawACardCommand(hand.CardsInHand[0], this, fast: true, fromDeck: false).AddToQueue();
        }
    }

    public void PlayACardFromHand(int CardUniqueID, int TargetUniqueID)
    {
        if (CardLogic.CardsCreatedThisGame.TryGetValue(CardUniqueID, out CardLogic card))
        {
            ICharacter target = null;

            if (TargetUniqueID >= 0)
            {
                if (TargetUniqueID == ID)
                {
                    target = this;
                }
                else if (TargetUniqueID == otherPlayer.ID)
                {
                    target = otherPlayer;
                }
                else if (CreatureLogic.CreaturesCreatedThisGame.TryGetValue(TargetUniqueID, out CreatureLogic creature))
                {
                    target = creature;
                }
            }

            PlayACardFromHand(card, target);
        }
        else
        {
            Debug.LogError("Card not found");
        }
    }

    public void PlayACardFromHand(CardLogic card, ICharacter target)
    {
        if (card != null && card.cardAsset != null)
        {
            if (!string.IsNullOrEmpty(card.cardAsset.CardScriptName))
            {
                SpellEffect effect = Activator.CreateInstance(Type.GetType(card.cardAsset.CardScriptName)) as SpellEffect;
                effect?.ActivateEffect(this, target);
            }

            if (card.cardAsset.IsVanishCard)
            {
                VanishCard(card);
            }
            else
            {
                DiscardCard(card);
            }
        }
    }

    private void VanishCard(CardLogic card)
    {
        hand.CardsInHand.Remove(card);
        deck.VanishDeck.Add(card.cardAsset);
        Debug.Log($"Card {card.cardAsset.name} vanished.");
    }

    private void DiscardCard(CardLogic card)
    {
        hand.CardsInHand.Remove(card);
        deck.DiscardDeck.Add(card.cardAsset);
        Debug.Log($"Card {card.cardAsset.name} discarded.");
    }

    public void Die()
    {
        PArea.ControlsON = false;
        otherPlayer.PArea.ControlsON = false;
        TurnManager.instance.StopTheTimer();
        new GameOverCommand(this).AddToQueue();
    }

    public void UseHeroPower()
    {
        ICharacter target = null;
        usedHeroPowerThisGame = true;
        HeroPowerEffect.ActivateEffect(this,target);

    }

    public void LoadCharacterInfoFromAsset()
    {
        PArea.Portrait.charAsset = charAsset;
        PArea.Portrait.ApplyLookFromAsset();

        if (!string.IsNullOrEmpty(charAsset.HeroPowerName))
        {
            HeroPowerEffect = Activator.CreateInstance(Type.GetType(charAsset.HeroPowerName)) as SpellEffect;
        }
        else
        {
            Debug.LogWarning("Check hero power name for character " + charAsset.ClassName);
        }
    }
}
