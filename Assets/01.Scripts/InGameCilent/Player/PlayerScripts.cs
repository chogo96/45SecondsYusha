using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using static UnityEngine.Rendering.DebugUI;
using System.Collections;
using System;

public class PlayerScripts : MonoBehaviourPunCallbacks, ICharacter
{
    // ��� ���� ����
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
    public Deck _deck; // Deck1�� Deck ��ũ��Ʈ�� ����
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
        PlayerID = IDFactory.GetUniquePlayerID();
        _playerDeckVisual = FindObjectOfType<PlayerDeckVisual>();
        // EnemySpawner�� �ڽ��� ���
        RegisterWithEnemySpawner();
        // BuffManager �ʱ�ȭ
        GameObject buffManagerObject = GameObject.Find("BuffManager");
        if (buffManagerObject != null)
        {
            _buffManager = buffManagerObject.GetComponent<BuffManager>();
            if (_buffManager == null)
            {
                Debug.LogError("BuffManager ������Ʈ�� ã�� �� �����ϴ�.");
            }
        }
        else
        {
            Debug.LogError("BuffManager ������Ʈ�� ã�� �� �����ϴ�.");
        }

        // Deck1 ������Ʈ�� ã�Ƽ� Deck ��ũ��Ʈ�� �����մϴ�.
        GameObject deckObject = GameObject.Find("Deck1");
        if (deckObject != null)
        {
            _deck = deckObject.GetComponent<Deck>();
            if (_deck == null)
            {
                Debug.LogError("Deck1 ������Ʈ���� Deck ������Ʈ�� ã�� �� �����ϴ�.");
            }
        }
        else
        {
            Debug.LogError("Deck1 ������Ʈ�� ã�� �� �����ϴ�.");
        }

        InitializePlayerDeck();
    }

    private void RegisterWithEnemySpawner()
    {
        EnemySpawner enemySpawner = FindObjectOfType<EnemySpawner>();
        if (enemySpawner != null)
        {
            enemySpawner.RegisterPlayer(this);
        }
        else
        {
            Debug.LogError("EnemySpawner�� ã�� �� �����ϴ�.");
        }
    }

    void Start()
    {
        if (hand.CardsInHand.Count <= 4 && !isFillingHand)
        {
            StartCoroutine(FillHandCoroutine());
        }
    }

    void Update()
    {
    }

    private void InitializePlayerDeck()
    {
        List<CardAsset> selectedDeckCards = DeckGameManager.instance.GetSelectedDeckCards();

        if (selectedDeckCards != null)
        {
            _deck.Cards = new List<CardAsset>(selectedDeckCards);
            _deck.ShuffleDeck();
        }
    }

    private IEnumerator DrawInitialCards()
    {
        for (int i = 0; i < 5; i++)
        {
            DrawACard(1);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void DrawACard(int n)
    {
        if (_deck.Cards.Count > 0)
        {
            HandVisual handVisual = PArea.handVisual;  // ���� �÷��̾��� HandVisual ����
            if (hand.CardsInHand.Count < handVisual.GetMaxSlots())
            {
                CardLogic newCard = new CardLogic(_deck.Cards[0], this);
                hand.CardsInHand.Insert(0, newCard);
                _deck.Cards.RemoveAt(0);
                new DrawACardCommand(hand.CardsInHand[0], this, fromDeck: true).AddToQueue();
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
        HandVisual handVisual = PArea.handVisual;  // ���� �÷��̾��� HandVisual ����
        if (hand.CardsInHand.Count < handVisual.GetMaxSlots())
        {
            CardLogic newCard = new CardLogic(cardAsset, this);
            newCard.owner = this;
            hand.CardsInHand.Insert(0, newCard);
            new DrawACardCommand(hand.CardsInHand[0], this, fromDeck: false).AddToQueue();
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
        Debug.Log($"Sword: {InGameManager.instance.Sword}, Magic: {InGameManager.instance.Magic}, Shield: {InGameManager.instance.Shield}");
        Debug.Log($"CurrentEnemy: {_currentEnemy}");

        // ���� ���� ���� ���� Ȯ��
        _currentEnemy?.CheckDeathCondition(InGameManager.instance.Sword, InGameManager.instance.Magic, InGameManager.instance.Shield);
    }

    public void PlayACardFromHand(CardLogic card, ICharacter target)
    {
        if (BuffManager.instance.BlindDebuff)
        {
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                Debug.Log("ī�尡 �Ǹ� ȿ���� ���������ϴ�!! ^^");

                // ī�� ó�� ������ �̰����� ó��
                if (card.cardAsset.IsVanishCard)
                {
                    VanishCard(card);
                }
                else
                {
                    DiscardCard(card);
                }
                return;
            }
        }

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

            void RemoveDebuff(string debuff)
            {
                switch (debuff)
                {
                    case "�Ǹ�":
                        BuffManager.instance.RemoveBlindEffect();
                        break;
                    case "����":
                        BuffManager.instance.RemoveBleedEffect();
                        break;
                    case "ȥ��":
                        BuffManager.instance.RemoveConfusionEffect();
                        break;
                    case "����":
                        RemoveRandomDebuff();
                        break;
                    case "���":
                        RemoveAllDebuffs();
                        break;
                    case "������":
                        RemoveAllDebuffs();
                        break;
                }
            }

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

            void RemoveAllDebuffs()
            {
                BuffManager.instance.RemoveBlindEffect();
                BuffManager.instance.RemoveBleedEffect();
                BuffManager.instance.RemoveConfusionEffect();
            }

            // ����� ���� �Լ� ȣ��
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

            photonView.RPC("RealTimeBossStatusCheck", RpcTarget.All, InGameManager.instance.Sword, InGameManager.instance.Magic, InGameManager.instance.Shield);

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

            _deck.ReturnRandomCardsFromDiscard(card.cardAsset.RandomRestoreDeck);

            // 2�� �Ŀ� �߰� ������ �����ϴ� �ڷ�ƾ ���� (���� Ȯ��)
            if (card.cardAsset.AdditionalSwordAttack > 0 ||
                card.cardAsset.AdditionalMagicAttack > 0 ||
                card.cardAsset.AdditionalShieldAttack > 0 ||
                card.cardAsset.AdditionalRandomAttack > 0)
            {
                StartCoroutine(PerformAdditionalAttack(card.cardAsset));
            }
        }

        // ������ ī�� ������ 4�� ������ �� ������ ī�带 ä��� ���� �߰�
        if (hand.CardsInHand.Count <= 4 && !isFillingHand)
        {
            StartCoroutine(FillHandCoroutine());
        }
    }

    private IEnumerator PerformAdditionalAttack(CardAsset cardAsset)
    {
        // 2�� ���
        yield return new WaitForSeconds(2f);

        // �߰� ���ݷ� �ݿ�
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

        photonView.RPC("RealTimeBossStatusCheck", RpcTarget.All, InGameManager.instance.Sword, InGameManager.instance.Magic, InGameManager.instance.Shield);
    }

    private void VanishCard(CardLogic card)
    {
        hand.CardsInHand.Remove(card);
        _deck.VanishDeck.Add(card.cardAsset);
        Debug.Log($"Card {card.cardAsset.name} vanished.");
    }

    private void DiscardCard(CardLogic card)
    {
        hand.CardsInHand.Remove(card);
        _deck.DiscardDeck.Add(card.cardAsset);
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
        if (_buffManager != null)
        {
            _buffManager.ApplyBleedEffect();
        }
        else
        {
            Debug.LogError("BuffManager�� null�Դϴ�. ApplyBleedToPlayer�� ������ �� �����ϴ�.");
        }
    }

    // Ư�� ��Ȳ���� ���� ������� ����
    public void RemoveBleedFromPlayer()
    {
        if (_buffManager != null)
        {
            _buffManager.RemoveBleedEffect();
        }
        else
        {
            Debug.LogError("BuffManager�� null�Դϴ�. RemoveBleedFromPlayer�� ������ �� �����ϴ�.");
        }
    }

    // Ư�� ��Ȳ���� �Ǹ� ������� ����
    public void ApplyBlindToPlayer()
    {
        if (_buffManager != null)
        {
            _buffManager.ApplyBlindEffect();
        }
        else
        {
            Debug.LogError("BuffManager�� null�Դϴ�. ApplyBlindToPlayer�� ������ �� �����ϴ�.");
        }
    }

    // Ư�� ��Ȳ���� �Ǹ� ������� ����
    public void RemoveBlindFromPlayer()
    {
        if (_buffManager != null)
        {
            _buffManager.RemoveBlindEffect();
        }
        else
        {
            Debug.LogError("BuffManager�� null�Դϴ�. RemoveBlindFromPlayer�� ������ �� �����ϴ�.");
        }
    }

    // Ư�� ��Ȳ���� ȥ�� ������� ����
    public void ApplyConfusionToPlayer()
    {
        if (_buffManager != null)
        {
            _buffManager.ApplyConfusionEffect();
        }
        else
        {
            Debug.LogError("BuffManager�� null�Դϴ�. ApplyConfusionToPlayer�� ������ �� �����ϴ�.");
        }
    }

    // Ư�� ��Ȳ���� ȥ�� ������� ����
    public void RemoveConfusionEffect()
    {
        if (_buffManager != null)
        {
            _buffManager.RemoveConfusionEffect();
            if (_buffManager.ConfusionDebuff)
            {
                // EnemyUIManager�� UpdateUI �޼ҵ� ȣ��
                if (_enemyUIManager != null)
                {
                    // �ʿ��� �Ķ���͸� �Ѱ���
                    _enemyUIManager.UpdateUI(_currentEnemy.requiredSword, _currentEnemy.requiredMagic, _currentEnemy.requiredShield);

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
        }
        else
        {
            Debug.LogError("BuffManager�� null�Դϴ�. RemoveConfusionEffect�� ������ �� �����ϴ�.");
        }
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
