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
        InitializePlayerDeck();
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
            // 덱을 다 썼을 때 탈진 효과 넣는 곳
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
        // 여기에 보스 상태Text 갱신 여부 적어주면됌

        Debug.Log($"Sword: {InGameManager.instance.Sword}, Magic: {InGameManager.instance.Magic}, Shield: {InGameManager.instance.Shield}");
        Debug.Log($"CurrentEnemy: {_currentEnemy}");

        // 현재 적의 생존 조건 확인
        _currentEnemy?.CheckDeathCondition(InGameManager.instance.Sword, InGameManager.instance.Magic, InGameManager.instance.Shield);
    }

    public void PlayACardFromHand(CardLogic card, ICharacter target)
    {
        if (BuffManager.instance.BlindDebuff)
        {
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                Debug.Log("카드가 실명 효과로 버려졌습니다!! ^^");

                // 카드 처리 로직을 이곳에서 처리
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

            // 카드의 SwordAttack, MagicAttack, ShieldAttack 값을 플레이어에게 반영
            InGameManager.instance.Sword += card.cardAsset.SwordAttack;
            InGameManager.instance.Magic += card.cardAsset.MagicAttack;
            InGameManager.instance.Shield += card.cardAsset.ShieldAttack;
            InGameManager.instance.RandomValue = card.cardAsset.RandomAttack;

            // 공격력 값을 배열에 저장
            int[] attackValues = { card.cardAsset.SwordAttack, card.cardAsset.MagicAttack, card.cardAsset.ShieldAttack };
            System.Random random = new System.Random();
            int index = random.Next(attackValues.Length);
            int randomAttackValue = attackValues[index];

            // 무작위로 선택된 공격력 값을 플레이어에게 반영
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
            if (card.cardAsset.RemoveDebuff == "실명")
            {
                BuffManager.instance.RemoveBlindEffect();
            }
            if (card.cardAsset.RemoveDebuff == "출혈")
            {
                BuffManager.instance.RemoveBleedEffect();
            }
            if (card.cardAsset.RemoveDebuff == "혼란")
            {
                BuffManager.instance.RemoveConfusionEffect();
            }
            photonView.RPC("RealTimeBossStatusCheck", RpcTarget.All, InGameManager.instance.Sword, InGameManager.instance.Magic, InGameManager.instance.Shield);

            #region RPC호출로_인한_주석처리
            // Debug.Log($"Sword: {Sword}, Magic: {Magic}, Shield: {Shield}");
            // Debug.Log($"CurrentEnemy: {currentEnemy}");

            // 현재 적의 생존 조건 확인
            // currentEnemy?.CheckDeathCondition(Sword, Magic, Shield);
            #endregion

            // 카드 처리 로직을 이곳에서 처리
            if (card.cardAsset.IsVanishCard)
            {
                VanishCard(card);
            }
            else
            {
                DiscardCard(card);
            }

            // 조건 확인 및 알파값 변경
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
            deck.ReturnRandomCardsFromDiscard(card.cardAsset.RandomRestoreDeck);
            // 2초 후에 추가 공격을 수행하는 코루틴 시작 (조건 확인)
            if (card.cardAsset.AdditionalSwordAttack > 0 ||
                card.cardAsset.AdditionalMagicAttack > 0 ||
                card.cardAsset.AdditionalShieldAttack > 0 ||
                card.cardAsset.AdditionalRandomAttack > 0)
            {
                StartCoroutine(PerformAdditionalAttack(card.cardAsset));
            }
        }

        // 손패의 카드 개수가 4장 이하일 때 덱에서 카드를 채우는 로직 추가
        if (hand.CardsInHand.Count <= 4 && !isFillingHand)
        {
            StartCoroutine(FillHandCoroutine());
        }
    }

    private IEnumerator PerformAdditionalAttack(CardAsset cardAsset)
    {
        // 2초 대기
        yield return new WaitForSeconds(2f);

        // 추가 공격력 반영
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
    // 특정 상황에서 출혈 디버프를 적용
    public void ApplyBleedToPlayer()
    {
        _buffManager.ApplyBleedEffect();
    }

    // 특정 상황에서 출혈 디버프를 제거
    public void RemoveBleedFromPlayer()
    {
        _buffManager.RemoveBleedEffect();
    }

    // 특정 상황에서 실명 디버프를 적용
    public void ApplyBlindToPlayer()
    {
        _buffManager.ApplyBlindEffect();
    }

    // 특정 상황에서 실명 디버프를 제거
    public void RemoveBlindFromPlayer()
    {
        _buffManager.RemoveBlindEffect();
    }
    // 특정 상황에서 실명 디버프를 적용
    public void ApplyConfusionToPlayer()
    {
        _buffManager.ApplyConfusionEffect();
    }
    // 특정 상황에서 실명 디버프를 제거
    public void RemoveConfusionToPlayer()
    {
        _buffManager.RemoveConfusionEffect();
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
