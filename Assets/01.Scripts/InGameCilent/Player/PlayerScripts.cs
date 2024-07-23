using UnityEngine;
using System.Collections;
using System;
using static UnityEngine.Rendering.DebugUI;
using Photon.Pun;
using Photon.Realtime;
using static UnityEngine.GraphicsBuffer;

public class PlayerScripts : MonoBehaviourPunCallbacks, ICharacter
{
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
    public Deck deck;
    public Hand hand;
    public Table table;
    private PlayerDeckVisual _playerDeckVisual;
    private EnemyUIManager _enemyUIManager;
    private bool isFillingHand = false;

    public static PlayerScripts[] Players;
    private BuffManager _buffManager;
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
        _currentEnemy = enemy;
        Debug.Log($"SetCurrentEnemy: {_currentEnemy}");
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
        _playerDeckVisual = FindObjectOfType<PlayerDeckVisual>();
        _buffManager = GameObject.Find("BuffManager").GetComponent<BuffManager>();
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

        // Ű���� �Է����� ī�� �Ӽ� ���� ������Ű�� �κ� ����
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
        if (deck.Cards.Count > 0)
        {
            if (hand.CardsInHand.Count < PArea.handVisual.slots.Children.Length)
            {
                CardLogic newCard = new CardLogic(deck.Cards[0], this);
                hand.CardsInHand.Insert(0, newCard);
                deck.Cards.RemoveAt(0);
                new DrawACardCommand(hand.CardsInHand[0], this, fast, fromDeck: true).AddToQueue();
                _playerDeckVisual.UpdateDeckCount();
            }
        }
        else
        {
            // ���� �� ���� �� Ż�� ȿ�� �ִ� ��
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


    [PunRPC]
    public void RealTimeBossStatusCheck(int sword, int magic, int shield)
    {
        // ���⿡ ���� ����Text ���� ���� �����ָ��

        Debug.Log($"Sword: {InGameManager.instance.Sword}, Magic: {InGameManager.instance.Magic}, Shield: {InGameManager.instance.Shield}");
        Debug.Log($"CurrentEnemy: {_currentEnemy}");

        // ���� ���� ���� ���� Ȯ��
        _currentEnemy?.CheckDeathCondition(InGameManager.instance.Sword, InGameManager.instance.Magic, InGameManager.instance.Shield);
    }

    public void PlayACardFromHand(CardLogic card, ICharacter target)
    {
        if (card != null && card.cardAsset != null)
        {
            _previousSword = InGameManager.instance.Sword;
            _previousMagic = InGameManager.instance.Magic;
            _previousShield = InGameManager.instance.Shield;

            // ī���� SwordAttack, MagicAttack, ShieldAttack ���� �÷��̾�� �ݿ�
            InGameManager.instance.Sword += card.cardAsset.SwordAttack;
            InGameManager.instance.Magic += card.cardAsset.MagicAttack;
            InGameManager.instance.Shield += card.cardAsset.ShieldAttack;
            InGameManager.instance.RandomValue = card.cardAsset.RandomAttack;

            // ���ݷ� ���� �迭�� ����
            int[] attackValues = { card.cardAsset.SwordAttack, card.cardAsset.MagicAttack, card.cardAsset.ShieldAttack };
            System.Random random = new System.Random();
            int index = random.Next(attackValues.Length);
            int randomAttackValue = attackValues[index];

            // �������� ���õ� ���ݷ� ���� �÷��̾�� �ݿ�
            if (index == 0)
            {
                InGameManager.instance.Sword += randomAttackValue;
            }
            else if (index == 1)
            {
                InGameManager.instance.Magic += randomAttackValue;
            }
            else if (index == 2)
            {
                InGameManager.instance.Shield += randomAttackValue;
            }

            photonView.RPC("RealTimeBossStatusCheck", RpcTarget.All, InGameManager.instance.Sword, InGameManager.instance.Magic, InGameManager.instance.Shield);

            #region RPCȣ���_����_�ּ�ó��
            // Debug.Log($"Sword: {Sword}, Magic: {Magic}, Shield: {Shield}");
            // Debug.Log($"CurrentEnemy: {currentEnemy}");

            // ���� ���� ���� ���� Ȯ��
            // currentEnemy?.CheckDeathCondition(Sword, Magic, Shield);
            #endregion

            // ī�� ó�� ������ �̰����� ó��
            if (card.cardAsset.IsVanishCard)
            {
                VanishCard(card);
            }
            else
            {
                DiscardCard(card);
            }

            // ���� Ȯ�� �� ���İ� ����
             _enemyUIManager = FindObjectOfType<EnemyUIManager>();
            if (_enemyUIManager != null)
            {
                int swordIncrement = InGameManager.instance.Sword - _previousSword;
                int magicIncrement = InGameManager.instance.Magic - _previousMagic;
                int shieldIncrement = InGameManager.instance.Shield - _previousShield;

                Debug.Log($"Sword Increment: {swordIncrement}");
                Debug.Log($"Magic Increment: {magicIncrement}");
                Debug.Log($"Shield Increment: {shieldIncrement}");

                _enemyUIManager.ChangeAlphaForIncrement(swordIncrement, _enemyUIManager.swordImageParent, Sword, _currentEnemy.requiredSword);
                _enemyUIManager.ChangeAlphaForIncrement(magicIncrement, _enemyUIManager.magicImageParent, Magic, _currentEnemy.requiredMagic);
                _enemyUIManager.ChangeAlphaForIncrement(shieldIncrement, _enemyUIManager.shieldImageParent, Shield, _currentEnemy.requiredShield);
            }
        }

        // ������ ī�� ������ 4�� ������ �� ������ ī�带 ä��� ���� �߰�
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
    public void UseHeroPower()
    {
        ICharacter target = null;
        usedHeroPowerThisGame = true;
        HeroPowerEffect.ActivateEffect(this, target);
    }
    // Ư�� ��Ȳ���� ���� ������� ����
    public void ApplyBleedToPlayer()
    {
        _buffManager.ApplyBleedEffect();
    }

    // Ư�� ��Ȳ���� ���� ������� ����
    public void RemoveBleedFromPlayer()
    {
        _buffManager.RemoveBleedEffect();
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
