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
    private PlayerDeckVisual playerDeckVisual;

    private bool isFillingHand = false;

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
        Debug.Log($"SetCurrentEnemy: {currentEnemy}");
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
        playerDeckVisual = FindObjectOfType<PlayerDeckVisual>();
    }

    void Start()
    {
        InitializePlayerDeck();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            DrawACard();
        }

        // 키보드 입력으로 카드 속성 값을 증가시키는 부분 제거
    }

    private void InitializePlayerDeck()
    {
        if (deck != null)
        {
            deck.ShuffleDeck();
        }
    }

    private IEnumerator DrawInitialCards()
    {
        for (int i = 0; i < 5; i++)
        {
            DrawACard(true);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void DrawACard(bool fast = false)
    {
        Debug.Log("DrawACard called");
        if (deck.Cards.Count > 0)
        {
            if (hand.CardsInHand.Count < PArea.handVisual.slots.Children.Length)
            {
                CardLogic newCard = new CardLogic(deck.Cards[0], this);
                hand.CardsInHand.Insert(0, newCard);
                deck.Cards.RemoveAt(0);
                new DrawACardCommand(hand.CardsInHand[0], this, fast, fromDeck: true).AddToQueue();
                playerDeckVisual.UpdateDeckCount();
            }
        }
        else
        {
            // 덱을 다 썼을 때 탈진 효과 넣는 곳
        }
    }
    //public void DrawACard(bool fast = false)
    //{
    //    if (deck.Cards.Count > 0)
    //    {
    //        if (hand.CardsInHand.Count < PArea.handVisual.slots.Children.Length)
    //        {
    //            CardLogic newCard = new CardLogic(deck.Cards[0], this);
    //            hand.CardsInHand.Insert(0, newCard);
    //            deck.Cards.RemoveAt(0);
    //            new DrawACardCommand(hand.CardsInHand[0], this, fast, fromDeck: true).AddToQueue();
    //            playerDeckVisual.UpdateDeckCount(); // 덱의 카드 개수를 업데이트
    //            Debug.Log("카드 호출");
    //        }
    //    }
    //    else
    //    {
    //        // 덱을 다 썼을 때 탈진 효과 넣는 곳
    //    }
    //}
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
            // 카드의 SwordAttack, MagicAttack, ShieldAttack 값을 플레이어에게 반영
            Sword += card.cardAsset.SwordAttack;
            Magic += card.cardAsset.MagicAttack;
            Shield += card.cardAsset.ShieldAttack;

            Debug.Log($"Sword: {Sword}, Magic: {Magic}, Shield: {Shield}");
            Debug.Log($"CurrentEnemy: {currentEnemy}");

            // 현재 적의 생존 조건 확인
            currentEnemy?.CheckDeathCondition(Sword, Magic, Shield);

            // 카드 처리 로직을 이곳에서 처리
            if (card.cardAsset.IsVanishCard)
            {
                VanishCard(card);
            }
            else
            {
                DiscardCard(card);
            }
        }

        // 손패의 카드 개수가 4장 이하일 때 덱에서 카드를 채우는 로직 추가
        if (hand.CardsInHand.Count <= 4 && !isFillingHand)
        {
            StartCoroutine(FillHandCoroutine());
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
        HeroPowerEffect.ActivateEffect(this, target);
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

    private IEnumerator FillHandCoroutine()
    {
        isFillingHand = true;
        while (hand.CardsInHand.Count < 5)
        {
            bool cardAdded = false;
            DrawACard();

            while (!cardAdded)
            {
                yield return new WaitForSeconds(0.1f);
                if (hand.CardsInHand.Count == PArea.handVisual.CardsInHand.Count)
                {
                    cardAdded = true;
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
        isFillingHand = false;
    }
}
