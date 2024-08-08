using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using System.Collections;
using System;
using static UnityEngine.Rendering.DebugUI;

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
    // ī�� �÷��� �����̸� �ֱ� ���� ����
    private bool isPlayingCard = false;

    // ���� ����ִ��� ���� üũ
    private bool isEnemyAlive = true;

    public static PlayerScripts[] Players;
    private BuffManager _buffManager;

    private PlayerSetManager playerSetManager;
    private int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

    private bool isDrawingCard = false;

    private GameObject _playerTransform;

    // �������� ����� int��
    public int _swordPoint;
    public int _magicPoint;
    public int _shieldPoint;

    // UI_PlayerCount �� ���� ��
    UI_PlayerCount ui_PlayerCount;
    private int _deckCardCount;
    private int _handCardCount;

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
        isEnemyAlive = _currentEnemy != null;

        if (_currentEnemy != null)
        {
            Utils.Log($"SetCurrentEnemy: {_currentEnemy}");
        }
        else
        {
            Utils.Log("SetCurrentEnemy: null");
        }
    }

    public void ResetValues()
    {
        Sword = 0;
        Magic = 0;
        Shield = 0;
    }

    void Awake()
    {
        playerSetManager = FindObjectOfType<PlayerSetManager>();
        _enemyUIManager = FindObjectOfType<EnemyUIManager>();
        Players = GameObject.FindObjectsOfType<PlayerScripts>();
        PlayerID = IDFactory.GetUniquePlayerID();
        _playerDeckVisual = FindObjectOfType<PlayerDeckVisual>();
        ui_PlayerCount = FindObjectOfType<UI_PlayerCount>();

        // EnemySpawner�� �ڽ��� ���
        RegisterWithEnemySpawner();

        // BuffManager �ʱ�ȭ
        GameObject buffManagerObject = GameObject.Find("BuffManager");
        if (buffManagerObject != null)
        {
            _buffManager = buffManagerObject.GetComponent<BuffManager>();
            if (_buffManager == null)
            {
                Utils.LogRed("BuffManager ������Ʈ�� ã�� �� �����ϴ�.");
            }
        }
        else
        {
            Utils.LogRed("BuffManager ������Ʈ�� ã�� �� �����ϴ�.");
        }

        _playerTransform = gameObject;

        LoadCharacterInfoFromDeck();

        GameManager.AllPlayersSpawned += C_FillHand;
    }

    private void LoadCharacterInfoFromDeck()
    {
        DeckInfo selectedDeck = DeckGameManager.instance.GetSelectedDeckInfo();

        if (selectedDeck != null)
        {
            charAsset = selectedDeck.Character;
        }
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
            Utils.LogRed("EnemySpawner�� ã�� �� �����ϴ�.");
        }
    }

    void Start()
    {
        // ���� �׾��� �� ȣ��Ǵ� �̺�Ʈ �ڵ鷯 ���
        Enemy.OnEnemyDeath -= OnEnemyDeath;  // �ߺ� ��� ����
        Enemy.OnEnemyDeath += OnEnemyDeath;
        Enemy.OnEnemySpawned -= OnNewEnemySpawned;  // �ߺ� ��� ����
        Enemy.OnEnemySpawned += OnNewEnemySpawned;

        #region ���� ��ũ��Ʈ�� AddComponent �ϴ°�
        if (charAsset.ClassName == "Attacker") // Attacker, Buffer, Healer, Tanker
        {
            // AttackerSkills attackerSkills = _playerTransform.AddComponent<AttackerSkills>();
            BufferSkills bufferSkills = _playerTransform.AddComponent<BufferSkills>();
        }
        else if (charAsset.ClassName == "Buffer")
        {
            BufferSkills bufferSkills = _playerTransform.AddComponent<BufferSkills>();
        }
        else if (charAsset.ClassName == "Healer")
        {
            // HealerSkills healerSkills = _playerTransform.AddComponent<HealerSkills>();
            BufferSkills bufferSkills = _playerTransform.AddComponent<BufferSkills>();
        }
        else // Tanker
        {
            // TankerSkills tankerSkills = _playerTransform.AddComponent<TankerSkills>();
            BufferSkills bufferSkills = _playerTransform.AddComponent<BufferSkills>();
        }
        #endregion
    }

    public void C_FillHand()
    {
        if (hand.CardsInHand.Count <= 4 && !isFillingHand)
        {
            StartCoroutine(FillHandCoroutine());
        }
    }

    void OnDestroy()
    {
        // �̺�Ʈ �ڵ鷯 ��� ����
        Enemy.OnEnemyDeath -= OnEnemyDeath;
        Enemy.OnEnemySpawned -= OnNewEnemySpawned;
        GameManager.AllPlayersSpawned -= C_FillHand;
    }

    void OnEnemyDeath()
    {
        isEnemyAlive = false; // ���� ������ �÷��׸� false�� ����
    }

    void OnNewEnemySpawned(Enemy newEnemy)
    {
        SetCurrentEnemy(newEnemy);
        isEnemyAlive = true; // ���ο� ���� �����Ǹ� �÷��׸� true�� ����
    }

    [PunRPC]
    public void RealTimeBossStatusCheck(int sword, int magic, int shield)
    {
        if (_currentEnemy == null)
        {
            Debug.LogWarning("���� ���� null �����Դϴ�. RealTimeBossStatusCheck ȣ���� �����մϴ�.");
            return;
        }

        _swordPoint = sword;
        _magicPoint = magic;
        _shieldPoint = shield;

        InGameManager.instance.Sword = _swordPoint;
        InGameManager.instance.Magic = _magicPoint;
        InGameManager.instance.Shield = _shieldPoint;

        int swordIncrement = _swordPoint - _previousSword;
        int magicIncrement = _magicPoint - _previousMagic;
        int shieldIncrement = _shieldPoint - _previousShield;

        _enemyUIManager.ChangeAlphaForIncrement(swordIncrement, _enemyUIManager.swordImageParent, Sword, _currentEnemy.requiredSword);
        _enemyUIManager.ChangeAlphaForIncrement(magicIncrement, _enemyUIManager.magicImageParent, Magic, _currentEnemy.requiredMagic);
        _enemyUIManager.ChangeAlphaForIncrement(shieldIncrement, _enemyUIManager.shieldImageParent, Shield, _currentEnemy.requiredShield);

        Utils.Log($"Sword: {InGameManager.instance.Sword}, Magic: {InGameManager.instance.Magic}, Shield: {InGameManager.instance.Shield}");
        Utils.Log($"CurrentEnemy: {_currentEnemy}");
        Utils.LogGreen($"{_swordPoint},{_magicPoint},{_shieldPoint},{swordIncrement},{magicIncrement},{shieldIncrement}");

        // ���� ���� ���� ���� Ȯ��
        _currentEnemy?.CheckDeathCondition(_swordPoint, _magicPoint, _shieldPoint);
    }

    public void DrawACard(int n)
    {
        if (isDrawingCard || !isEnemyAlive) // ���� �׾��ų� ���ο� ���� �����Ǳ� ���̸� ī�� ��ο츦 �Ͻ� ����
        {
            return;
        }

        StartCoroutine(DrawCardsCoroutine(n));
    }

    private IEnumerator DrawCardsCoroutine(int n)
    {
        isDrawingCard = true;
        HandVisual handVisual = PArea.handVisual;  // ���� �÷��̾��� HandVisual ����

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

                    // �ٷ� ���� ����
                    handVisual.PlaceCardsOnNewSlots();

                    // ī�� ��ο� �� ī�� �� ����
                    UpdateCardCounts("Plus");

                    yield return new WaitForSeconds(0.1f); // ī�� ��ο� ���̿� ª�� ��� �ð��� �߰��Ͽ� ���ÿ� ���� ī�� ��ο츦 �����մϴ�.
                }
                else
                {
                    // ���� �� ���� �� Ż�� ȿ�� �ִ� ��
                    break;
                }
            }
        }

        isDrawingCard = false;
    }

    // ���� ���� ī�� ���� ������Ʈ�ϴ� �޼���
    public void UpdateCardCounts(string plusMinus)
    {
        _deckCardCount = _deck.Cards.Count;
        _handCardCount = hand.CardsInHand.Count;
        ui_PlayerCount.HandCardCount(plusMinus, _deckCardCount, _handCardCount);
    }

    public void InitializePlayerDeck()
    {
        List<CardAsset> selectedDeckCards = DeckGameManager.instance.GetSelectedDeckCards();

        if (selectedDeckCards != null)
        {
            _deck.Cards = new List<CardAsset>(selectedDeckCards);
            _deck.ShuffleDeck();
        }
    }

    public void GetACardNotFromDeck(CardAsset cardAsset)
    {
        HandVisual handVisual = PArea.handVisual;  // ���� �÷��̾��� HandVisual ����
        if (hand.CardsInHand.Count < handVisual.GetMaxSlots() && hand.CardsInHand.Count < 5)
        {
            CardLogic newCard = new CardLogic(cardAsset, this);
            newCard.owner = this;
            hand.CardsInHand.Insert(0, newCard);
            new DrawACardCommand(hand.CardsInHand[0], this, fromDeck: false).AddToQueue();
            UpdateCardCounts("Plus"); // ī�� �� ������Ʈ
        }
    }

    public void PlayACardFromHand(int CardUniqueID, int TargetUniqueID)
    {
        Utils.Log($"PlayACardFromHand called with CardUniqueID: {CardUniqueID}, TargetUniqueID: {TargetUniqueID}");

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
            //PlayACardFromHand(card, target);
            StartCoroutine(PlayACardWithDelay(card, target)); // �ڷ�ƾ ����
        }
        else
        {
            Utils.LogRed("Card not found");
        }
    }

    private IEnumerator PlayACardWithDelay(CardLogic card, ICharacter target)
    {
        Utils.Log($"PlayACardWithDelay called for card: {card.cardAsset.name}");

        // ���� �׾��� �� ī�� �÷��̸� �Ͻ� ����
        while (isPlayingCard || !isEnemyAlive)
        {
            yield return null;
        }

        isPlayingCard = true; // ī�� �÷��� ����

        // ī�� ��� �� ī�� �� ����
        UpdateCardCounts("Minus");

        if (BuffManager.instance.BlindDebuff)
        {
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                Utils.Log("ī�尡 �Ǹ� ȿ���� ���������ϴ�!! ^^");

                // ī�� ó�� ������ �̰����� ó��
                if (card.cardAsset.IsVanishCard)
                {
                    VanishCard(card);
                }
                else
                {
                    DiscardCard(card);
                }

                isPlayingCard = false; // ī�� �÷��� ����
                yield break;
            }
        }

        if (card != null && card.cardAsset != null)
        {
            _previousSword = InGameManager.instance.Sword;
            _previousMagic = InGameManager.instance.Magic;
            _previousShield = InGameManager.instance.Shield;

            // SwordAttack, MagicAttack, ShieldAttack, RandomAttack ���� �÷��̾�� �ݿ�
            Utils.Log($"Before: Sword={InGameManager.instance.Sword}, Magic={InGameManager.instance.Magic}, Shield={InGameManager.instance.Shield}");
            InGameManager.instance.Sword += card.cardAsset.SwordAttack;
            InGameManager.instance.Magic += card.cardAsset.MagicAttack;
            InGameManager.instance.Shield += card.cardAsset.ShieldAttack;
            Utils.Log($"After Attack: Sword={InGameManager.instance.Sword}, Magic={InGameManager.instance.Magic}, Shield={InGameManager.instance.Shield}");

            // �� �κ��� �� �Ӽ��� �� �� �߰��ϰ� �ִ��� Ȯ���ϼ���.
            Utils.Log($"Final Values: Sword={InGameManager.instance.Sword}, Magic={InGameManager.instance.Magic}, Shield={InGameManager.instance.Shield}");

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

            // ��ü �÷��̾��� ��� ����� ����
            void RemoveAllDebuffs()
            {
                foreach (var player in Players)
                {
                    player._buffManager.RemoveBlindEffect();
                    player._buffManager.RemoveBleedEffect();
                    player._buffManager.RemoveConfusionEffect();
                }
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
                Utils.Log("���� �� ui�Ŵ��� ����");
                int swordIncrement = InGameManager.instance.Sword - _previousSword;
                int magicIncrement = InGameManager.instance.Magic - _previousMagic;
                int shieldIncrement = InGameManager.instance.Shield - _previousShield;

                Utils.Log($"Sword Increment: {swordIncrement}");
                Utils.Log($"Magic Increment: {magicIncrement}");
                Utils.Log($"Shield Increment: {shieldIncrement}");

                Utils.Log($"_enemyUIManager.swordImageParent: {_enemyUIManager.swordImageParent}");
                Utils.Log($"Sword: {Sword}");
                Utils.Log($"_currentEnemy.requiredSword: {_currentEnemy.requiredSword}");

                // _enemyUIManager.ChangeAlphaForIncrement(swordIncrement, _enemyUIManager.swordImageParent, Sword, _currentEnemy.requiredSword);
                // _enemyUIManager.ChangeAlphaForIncrement(magicIncrement, _enemyUIManager.magicImageParent, Magic, _currentEnemy.requiredMagic);
                // _enemyUIManager.ChangeAlphaForIncrement(shieldIncrement, _enemyUIManager.shieldImageParent, Shield, _currentEnemy.requiredShield);
            }

            // �� ��ü�� null�� �ƴ� ��쿡�� RealTimeBossStatusCheck ȣ��
            if (_currentEnemy != null)
            {
                photonView.RPC("RealTimeBossStatusCheck", RpcTarget.All, InGameManager.instance.Sword, InGameManager.instance.Magic, InGameManager.instance.Shield);
            }
            _deck.ReturnRandomCardsFromDiscard(card.cardAsset.RandomRestoreDeck);
        }

        // ������ ī�� ������ 4�� ������ �� ������ ī�带 ä��� ���� �߰�
        if (hand.CardsInHand.Count <= 4 && !isFillingHand)
        {
            StartCoroutine(FillHandCoroutine());
        }

        yield return new WaitForSeconds(0.5f); // ������ �߰�
        isPlayingCard = false; // ī�� �÷��� ����
    }

    private void VanishCard(CardLogic card)
    {
        hand.CardsInHand.Remove(card);
        _deck.VanishDeck.Add(card.cardAsset);
        Utils.Log($"Card {card.cardAsset.name} vanished.");
        UpdateCardCounts("Minus"); // ī�� �� ������Ʈ
    }

    private void DiscardCard(CardLogic card)
    {
        hand.CardsInHand.Remove(card);
        _deck.DiscardDeck.Add(card.cardAsset);
        Utils.Log($"Card {card.cardAsset.name} discarded.");
        UpdateCardCounts("Minus"); // ī�� �� ������Ʈ
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
            Utils.LogRed("BuffManager�� null�Դϴ�. ApplyBleedToPlayer�� ������ �� �����ϴ�.");
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
            Utils.LogRed("BuffManager�� null�Դϴ�. RemoveBleedFromPlayer�� ������ �� �����ϴ�.");
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
            Utils.LogRed("BuffManager�� null�Դϴ�. ApplyBlindToPlayer�� ������ �� �����ϴ�.");
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
            Utils.LogRed("BuffManager�� null�Դϴ�. RemoveBlindFromPlayer�� ������ �� �����ϴ�.");
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
            Utils.LogRed("BuffManager�� null�Դϴ�. ApplyConfusionToPlayer�� ������ �� �����ϴ�.");
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

                    Utils.Log($"Sword Increment: {swordIncrement}");
                    Utils.Log($"Magic Increment: {magicIncrement}");
                    Utils.Log($"Shield Increment: {shieldIncrement}");

                    _enemyUIManager.ChangeAlphaForIncrement(swordIncrement, _enemyUIManager.swordImageParent, Sword, _currentEnemy.requiredSword);
                    _enemyUIManager.ChangeAlphaForIncrement(magicIncrement, _enemyUIManager.magicImageParent, Magic, _currentEnemy.requiredMagic);
                    _enemyUIManager.ChangeAlphaForIncrement(shieldIncrement, _enemyUIManager.shieldImageParent, Shield, _currentEnemy.requiredShield);
                }
            }
        }
        else
        {
            Utils.LogRed("BuffManager�� null�Դϴ�. RemoveConfusionEffect�� ������ �� �����ϴ�.");
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
        UpdateCardCounts("Update"); // ���а� ���� �� �Ŀ��� ī�� ���� ������Ʈ
    }
}
