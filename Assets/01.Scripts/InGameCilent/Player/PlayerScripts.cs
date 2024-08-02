using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using System.Collections;
using static UnityEngine.Rendering.DebugUI;
using System;

// The PlayerScripts class handles player actions, interactions, and state management.
public class PlayerScripts : MonoBehaviourPunCallbacks, ICharacter
{
    // Member variables
    public int Sword;
    public int Magic;
    public int Shield;
    public int RandomValue;
    private Enemy _currentEnemy;
    public int PlayerID;
    public CharacterAsset charAsset;
    public PlayerArea PArea;
    public SpellEffect HeroPowerEffect;
    public bool usedHeroPowerThisGame = false;

    private int _previousSword;
    private int _previousMagic;
    private int _previousShield;
    public Deck _deck; // Reference to the Deck script on Deck1
    public Hand hand;
    public Table table;
    private PlayerDeckVisual _playerDeckVisual;
    private EnemyUIManager _enemyUIManager;
    private bool isFillingHand = false;
    private bool isPlayingCard = false; // For delaying card play
    private bool isEnemyAlive = true;   // Check if enemy is alive

    public static PlayerScripts[] Players;
    private BuffManager _buffManager;
    private PlayerSetManager playerSetManager;
    private int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
    private bool isDrawingCard = false;
    private GameObject _playerTransform;

    // Server-side int values
    public int _swordPoint;
    public int _magicPoint;
    public int _shieldPoint;

    // Player ID property
    public int ID
    {
        get { return PlayerID; }
    }

    // Reference to the other player
    public PlayerScripts otherPlayer
    {
        get
        {
            return Players[0] == this ? Players[1] : Players[0];
        }
    }

    // Delegate declaration
    public delegate void VoidWithNoArguments();

    // Set the current enemy
    public void SetCurrentEnemy(Enemy enemy)
    {
        _currentEnemy = enemy;
        isEnemyAlive = _currentEnemy != null;

        // Log the current enemy status
        if (_currentEnemy != null)
        {
            Debug.Log($"SetCurrentEnemy: {_currentEnemy}");
        }
    }

    // Reset player values
    public void ResetValues()
    {
        Sword = 0;
        Magic = 0;
        Shield = 0;
    }

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        playerSetManager = FindObjectOfType<PlayerSetManager>();
        _enemyUIManager = FindObjectOfType<EnemyUIManager>();
        Players = GameObject.FindObjectsOfType<PlayerScripts>();
        PlayerID = IDFactory.GetUniquePlayerID();
        _playerDeckVisual = FindObjectOfType<PlayerDeckVisual>();

        RegisterWithEnemySpawner(); // Register with EnemySpawner

        // Initialize BuffManager
        GameObject buffManagerObject = GameObject.Find("BuffManager");
        if (buffManagerObject != null)
        {
            _buffManager = buffManagerObject.GetComponent<BuffManager>();
        }

        // Reference the Deck script on Deck1
        GameObject deckObject = GameObject.Find("Deck1");
        if (deckObject != null)
        {
            _deck = deckObject.GetComponent<Deck>();
        }

        _playerTransform = gameObject;

        InitializePlayerDeck(); // Initialize the player's deck
    }

    // Register player with EnemySpawner
    private void RegisterWithEnemySpawner()
    {
        EnemySpawner enemySpawner = FindObjectOfType<EnemySpawner>();
        if (enemySpawner != null)
        {
            enemySpawner.RegisterPlayer(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (hand.CardsInHand.Count <= 4 && !isFillingHand)
        {
            StartCoroutine(FillHandCoroutine());
        }

        // Register event handlers for enemy actions
        Enemy.OnEnemyDeath += OnEnemyDeath;
        Enemy.OnEnemySpawned += OnNewEnemySpawned;

        #region AddComponent for Character Scripts
        if (charAsset.ClassName == "Attacker") // Attacker, Buffer, Healer, Tanker
        {
            _playerTransform.AddComponent<BufferSkills>();
        }
        else if (charAsset.ClassName == "Buffer")
        {
            _playerTransform.AddComponent<BufferSkills>();
        }
        else if (charAsset.ClassName == "Healer")
        {
            _playerTransform.AddComponent<BufferSkills>();
        }
        else // Tanker
        {
            _playerTransform.AddComponent<BufferSkills>();
        }
        #endregion
    }

    // OnDestroy is called when the script is destroyed
    void OnDestroy()
    {
        // Unregister event handlers
        Enemy.OnEnemyDeath -= OnEnemyDeath;
        Enemy.OnEnemySpawned -= OnNewEnemySpawned;
    }

    // Handle enemy death
    void OnEnemyDeath()
    {
        isEnemyAlive = false; // Set flag to false when enemy dies
    }

    // Handle new enemy spawn
    void OnNewEnemySpawned(Enemy newEnemy)
    {
        SetCurrentEnemy(newEnemy);
        isEnemyAlive = true; // Set flag to true when new enemy spawns
    }

    // Real-time boss status check with Photon RPC
    [PunRPC]
    public void RealTimeBossStatusCheck(int sword, int magic, int shield)
    {
        if (_currentEnemy == null)
        {
            return; // Skip if current enemy is null
        }

        _swordPoint += sword;
        _magicPoint += magic;
        _shieldPoint += shield;


        InGameManager.instance.Sword = _swordPoint;
        InGameManager.instance.Magic = _magicPoint;
        InGameManager.instance.Shield = _shieldPoint;

        int swordIncrement = _swordPoint - _previousSword;
        int magicIncrement = _magicPoint - _previousMagic;
        int shieldIncrement = _shieldPoint - _previousShield;

        _enemyUIManager.ChangeAlphaForIncrement(swordIncrement, _enemyUIManager.swordImageParent, Sword, _currentEnemy.requiredSword);
        _enemyUIManager.ChangeAlphaForIncrement(magicIncrement, _enemyUIManager.magicImageParent, Magic, _currentEnemy.requiredMagic);
        _enemyUIManager.ChangeAlphaForIncrement(shieldIncrement, _enemyUIManager.shieldImageParent, Shield, _currentEnemy.requiredShield);

        // Check if the enemy meets the death condition
        _currentEnemy?.CheckDeathCondition(_swordPoint, _magicPoint, _shieldPoint);
    }

    // Draw a card from the deck
    public void DrawACard(int n)
    {
        // Pause card draw if enemy is dead or new enemy is spawning
        if (isDrawingCard || !isEnemyAlive)
        {
            return;
        }

        playerSetManager.photonView.RPC("HandCardCount", RpcTarget.All, actorNumber, "Plus");
        StartCoroutine(DrawCardsCoroutine(n));
    }

    // Coroutine to handle drawing cards
    private IEnumerator DrawCardsCoroutine(int n)
    {
        // photonView.RPC("RealTimeBossStatusCheck", RpcTarget.All, InGameManager.instance.Sword, InGameManager.instance.Magic, InGameManager.instance.Shield);
        isDrawingCard = true;
        HandVisual handVisual = PArea.handVisual;  // Reference to current player's HandVisual

        for (int i = 0; i < n; i++)
        {
            if (hand.CardsInHand.Count < handVisual.GetMaxSlots() && hand.CardsInHand.Count < 5)
            {
                if (_deck.Cards.Count > 0)
                {
                    CardLogic newCard = new CardLogic(_deck.Cards[0], this);
                    hand.CardsInHand.Insert(0, newCard);
                    _deck.Cards.RemoveAt(0);
                    new DrawACardCommand(hand.CardsInHand[0], this, fromDeck: true).AddToQueue();
                    _playerDeckVisual.UpdateDeckCount();

                    // Immediately arrange cards
                    handVisual.PlaceCardsOnNewSlots();

                    yield return new WaitForSeconds(0.1f); // Short delay to prevent simultaneous card draws
                }
                else
                {
                    // Implement fatigue effect if deck is empty
                    break;
                }
            }
        }

        isDrawingCard = false;
    }

    // Initialize player's deck
    private void InitializePlayerDeck()
    {
        List<CardAsset> selectedDeckCards = DeckGameManager.instance.GetSelectedDeckCards();

        if (selectedDeckCards != null)
        {
            _deck.Cards = new List<CardAsset>(selectedDeckCards);
            _deck.ShuffleDeck();
        }
    }

    // Obtain a card not from the deck
    public void GetACardNotFromDeck(CardAsset cardAsset)
    {
        HandVisual handVisual = PArea.handVisual;  // Reference to current player's HandVisual
        if (hand.CardsInHand.Count < handVisual.GetMaxSlots() && hand.CardsInHand.Count < 5)
        {
            CardLogic newCard = new CardLogic(cardAsset, this);
            newCard.owner = this;
            hand.CardsInHand.Insert(0, newCard);
            new DrawACardCommand(hand.CardsInHand[0], this, fromDeck: false).AddToQueue();
        }
    }

    // Play a card from the hand
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
            StartCoroutine(PlayACardWithDelay(card, target)); // Start coroutine for card play with delay
        }
    }

    // Coroutine to handle card play with delay
    private IEnumerator PlayACardWithDelay(CardLogic card, ICharacter target)
    {
        // Pause card play if enemy is dead
        while (isPlayingCard || !isEnemyAlive)
        {
            yield return null;
        }

        isPlayingCard = true; // Start card play

        playerSetManager.photonView.RPC("HandCardCount", RpcTarget.All, actorNumber, "Minus");

        if (BuffManager.instance.BlindDebuff)
        {
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                // Discard card due to blindness effect
                if (card.cardAsset.IsVanishCard)
                {
                    VanishCard(card);
                }
                else
                {
                    DiscardCard(card);
                }

                isPlayingCard = false; // End card play
                yield break;
            }
        }

        if (card != null && card.cardAsset != null)
        {
            _previousSword = InGameManager.instance.Sword;
            _previousMagic = InGameManager.instance.Magic;
            _previousShield = InGameManager.instance.Shield;

            // Apply card's attack values to player
            InGameManager.instance.Sword += card.cardAsset.SwordAttack;
            InGameManager.instance.Magic += card.cardAsset.MagicAttack;
            InGameManager.instance.Shield += card.cardAsset.ShieldAttack;

            // Store attack values in an array
            int[] attackValues = { card.cardAsset.SwordAttack, card.cardAsset.MagicAttack, card.cardAsset.ShieldAttack };
            System.Random random = new System.Random();
            int index = random.Next(attackValues.Length);
            int randomAttackValue = attackValues[index];

            // Apply randomly selected attack value to player
            switch (index)
            {
                case 0:
                    InGameManager.instance.Sword += randomAttackValue;
                    break;
                case 1:
                    InGameManager.instance.Magic += randomAttackValue;
                    break;
                case 2:
                    InGameManager.instance.Shield += randomAttackValue;
                    break;
            }

            // Function to remove debuffs
            void RemoveDebuff(string debuff)
            {
                switch (debuff)
                {
                    case "½Ç¸í":
                        BuffManager.instance.RemoveBlindEffect();
                        break;
                    case "ÃâÇ÷":
                        BuffManager.instance.RemoveBleedEffect();
                        break;
                    case "È¥¶õ":
                        BuffManager.instance.RemoveConfusionEffect();
                        break;
                    case "·£´ý":
                        RemoveRandomDebuff();
                        break;
                    case "¸ðµç":
                    case "¸ðµç¹öÇÁ":
                        RemoveAllDebuffs();
                        break;
                }
            }

            // Function to remove a random debuff
            void RemoveRandomDebuff()
            {
                int randomDebuffIndex = UnityEngine.Random.Range(0, 3);
                switch (randomDebuffIndex)
                {
                    case 0:
                        BuffManager.instance.RemoveBlindEffect();
                        break;
                    case 1:
                        BuffManager.instance.RemoveBleedEffect();
                        break;
                    case 2:
                        BuffManager.instance.RemoveConfusionEffect();
                        break;
                }
            }

            // Remove all debuffs from all players
            void RemoveAllDebuffs()
            {
                foreach (var player in Players)
                {
                    player._buffManager.RemoveBlindEffect();
                    player._buffManager.RemoveBleedEffect();
                    player._buffManager.RemoveConfusionEffect();
                }
            }

            // Call debuff removal function
            RemoveDebuff(card.cardAsset.RemoveDebuff);

            if (card.cardAsset.DiscardFromDeck > 0)
            {
                _deck.DiscardRandomCards(card.cardAsset.DiscardFromDeck);
            }
            if (card.cardAsset.DrawFromDeck > 0)
            {
                for (int i = 0; i < card.cardAsset.DrawFromDeck; i++)
                {
                    DrawACard(1);
                }
            }

            // Handle card processing
            if (card.cardAsset.IsVanishCard)
            {
                VanishCard(card);
            }
            else
            {
                DiscardCard(card);
            }

            // Check conditions and change alpha values
            if (_enemyUIManager != null)
            {
                int swordIncrement = InGameManager.instance.Sword - _previousSword;
                int magicIncrement = InGameManager.instance.Magic - _previousMagic;
                int shieldIncrement = InGameManager.instance.Shield - _previousShield;

                _enemyUIManager.ChangeAlphaForIncrement(swordIncrement, _enemyUIManager.swordImageParent, Sword, _currentEnemy.requiredSword);
                _enemyUIManager.ChangeAlphaForIncrement(magicIncrement, _enemyUIManager.magicImageParent, Magic, _currentEnemy.requiredMagic);
                _enemyUIManager.ChangeAlphaForIncrement(shieldIncrement, _enemyUIManager.shieldImageParent, Shield, _currentEnemy.requiredShield);
            }

            // Call RealTimeBossStatusCheck if enemy object is not null
            if (_currentEnemy != null)
            {
                photonView.RPC("RealTimeBossStatusCheck", RpcTarget.All, InGameManager.instance.Sword, InGameManager.instance.Magic, InGameManager.instance.Shield);
            }
            _deck.ReturnRandomCardsFromDiscard(card.cardAsset.RandomRestoreDeck);

            // Start coroutine for additional attack after 2 seconds (check conditions)
            //if (card.cardAsset.AdditionalSwordAttack > 0 ||
            //    card.cardAsset.AdditionalMagicAttack > 0 ||
            //    card.cardAsset.AdditionalShieldAttack > 0 ||
            //    card.cardAsset.AdditionalRandomAttack > 0)
            //{
            //    StartCoroutine(PerformAdditionalAttack(card.cardAsset));

            //    // Call RealTimeBossStatusCheck if enemy object is not null
            //    if (_currentEnemy != null)
            //    {
            //        photonView.RPC("RealTimeBossStatusCheck", RpcTarget.All, InGameManager.instance.Sword, InGameManager.instance.Magic, InGameManager.instance.Shield);
            //    }
            //}
        }

        // Add logic to fill deck with cards if card count is 4 or less
        if (hand.CardsInHand.Count <= 4 && !isFillingHand)
        {
            playerSetManager.photonView.RPC("HandCardCount", RpcTarget.All, actorNumber, "Plus");
            StartCoroutine(FillHandCoroutine());
        }

        yield return new WaitForSeconds(0.5f); // Add delay
        isPlayingCard = false; // End card play
    }

    // Coroutine to handle additional attack
    private IEnumerator PerformAdditionalAttack(CardAsset cardAsset)
    {
        // Wait for 2 seconds
        yield return new WaitForSeconds(2f);

        // Apply additional attack
        if (cardAsset.AdditionalSwordAttack > 0)
        {
            InGameManager.instance.Sword += cardAsset.AdditionalSwordAttack;
        }
        if (cardAsset.AdditionalMagicAttack > 0)
        {
            InGameManager.instance.Magic += cardAsset.AdditionalMagicAttack;
        }
        if (cardAsset.AdditionalShieldAttack > 0)
        {
            InGameManager.instance.Shield += cardAsset.AdditionalShieldAttack;
        }
        if (cardAsset.AdditionalRandomAttack > 0)
        {
            int[] additionalAttackValues = { cardAsset.AdditionalSwordAttack, cardAsset.AdditionalMagicAttack, cardAsset.AdditionalShieldAttack };
            System.Random random = new System.Random();
            int randomIndex = random.Next(additionalAttackValues.Length);
            int additionalRandomAttackValue = additionalAttackValues[randomIndex];

            if (randomIndex == 0)
            {
                InGameManager.instance.Sword += additionalRandomAttackValue;
            }
            else if (randomIndex == 1)
            {
                InGameManager.instance.Magic += additionalRandomAttackValue;
            }
            else if (randomIndex == 2)
            {
                InGameManager.instance.Shield += additionalRandomAttackValue;
            }
        }
    }

    // Vanish the card
    private void VanishCard(CardLogic card)
    {
        hand.CardsInHand.Remove(card);
        _deck.VanishDeck.Add(card.cardAsset);
    }

    // Discard the card
    private void DiscardCard(CardLogic card)
    {
        hand.CardsInHand.Remove(card);
        _deck.DiscardDeck.Add(card.cardAsset);
    }

    // Use hero power
    public void UseHeroPower()
    {
        ICharacter target = null;
        usedHeroPowerThisGame = true;
        HeroPowerEffect.ActivateEffect(this, target);
    }

    // Apply bleed debuff
    public void ApplyBleedToPlayer()
    {
        _buffManager?.ApplyBleedEffect();
    }

    // Remove bleed debuff
    public void RemoveBleedFromPlayer()
    {
        _buffManager?.RemoveBleedEffect();
    }

    // Apply blind debuff
    public void ApplyBlindToPlayer()
    {
        _buffManager?.ApplyBlindEffect();
    }

    // Remove blind debuff
    public void RemoveBlindFromPlayer()
    {
        _buffManager?.RemoveBlindEffect();
    }

    // Apply confusion debuff
    public void ApplyConfusionToPlayer()
    {
        _buffManager?.ApplyConfusionEffect();
    }

    // Remove confusion debuff
    public void RemoveConfusionEffect()
    {
        if (_buffManager != null)
        {
            _buffManager.RemoveConfusionEffect();
            if (_buffManager.ConfusionDebuff)
            {
                _enemyUIManager?.UpdateUI(_currentEnemy.requiredSword, _currentEnemy.requiredMagic, _currentEnemy.requiredShield);

                int swordIncrement = InGameManager.instance.Sword - _previousSword;
                int magicIncrement = InGameManager.instance.Magic - _previousMagic;
                int shieldIncrement = InGameManager.instance.Shield - _previousShield;

                _enemyUIManager?.ChangeAlphaForIncrement(swordIncrement, _enemyUIManager.swordImageParent, Sword, _currentEnemy.requiredSword);
                _enemyUIManager?.ChangeAlphaForIncrement(magicIncrement, _enemyUIManager.magicImageParent, Magic, _currentEnemy.requiredMagic);
                _enemyUIManager?.ChangeAlphaForIncrement(shieldIncrement, _enemyUIManager.shieldImageParent, Shield, _currentEnemy.requiredShield);
            }
        }
    }

    // Load character information from asset
    public void LoadCharacterInfoFromAsset()
    {
        PArea.Portrait.charAsset = charAsset;
        PArea.Portrait.ApplyLookFromAsset();

        if (!string.IsNullOrEmpty(charAsset.HeroPowerName))
        {
            HeroPowerEffect = Activator.CreateInstance(Type.GetType(charAsset.HeroPowerName)) as SpellEffect;
        }
    }

    // Coroutine to fill the hand
    private IEnumerator FillHandCoroutine()
    {
        isFillingHand = true;
        while (hand.CardsInHand.Count < 5)
        {
            bool cardAdded = false;
            DrawACard(1);

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
