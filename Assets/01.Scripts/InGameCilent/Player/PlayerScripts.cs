using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using static UnityEngine.Rendering.DebugUI;
using System.Collections;
using System;

public class PlayerScripts : MonoBehaviourPunCallbacks, ICharacter
{
    // 멤버 변수 선언
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
    public Deck _deck; // Deck1의 Deck 스크립트를 참조
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
        // EnemySpawner에 자신을 등록
        RegisterWithEnemySpawner();
        // BuffManager 초기화
        GameObject buffManagerObject = GameObject.Find("BuffManager");
        if (buffManagerObject != null)
        {
            _buffManager = buffManagerObject.GetComponent<BuffManager>();
            if (_buffManager == null)
            {
                Debug.LogError("BuffManager 컴포넌트를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("BuffManager 오브젝트를 찾을 수 없습니다.");
        }

        // Deck1 오브젝트를 찾아서 Deck 스크립트를 참조합니다.
        GameObject deckObject = GameObject.Find("Deck1");
        if (deckObject != null)
        {
            _deck = deckObject.GetComponent<Deck>();
            if (_deck == null)
            {
                Debug.LogError("Deck1 오브젝트에서 Deck 컴포넌트를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("Deck1 오브젝트를 찾을 수 없습니다.");
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
            Debug.LogError("EnemySpawner를 찾을 수 없습니다.");
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
            HandVisual handVisual = PArea.handVisual;  // 현재 플레이어의 HandVisual 참조
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
            // 덱을 다 썼을 때 탈진 효과 넣는 곳
        }
    }

    public void GetACardNotFromDeck(CardAsset cardAsset)
    {
        HandVisual handVisual = PArea.handVisual;  // 현재 플레이어의 HandVisual 참조
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
                    case "실명":
                        BuffManager.instance.RemoveBlindEffect();
                        break;
                    case "출혈":
                        BuffManager.instance.RemoveBleedEffect();
                        break;
                    case "혼란":
                        BuffManager.instance.RemoveConfusionEffect();
                        break;
                    case "랜덤":
                        RemoveRandomDebuff();
                        break;
                    case "모든":
                        RemoveAllDebuffs();
                        break;
                    case "모든버프":
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

            // 디버프 제거 함수 호출
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

    // 특정 상황에서 출혈 디버프를 적용
    public void ApplyBleedToPlayer()
    {
        if (_buffManager != null)
        {
            _buffManager.ApplyBleedEffect();
        }
        else
        {
            Debug.LogError("BuffManager가 null입니다. ApplyBleedToPlayer를 실행할 수 없습니다.");
        }
    }

    // 특정 상황에서 출혈 디버프를 제거
    public void RemoveBleedFromPlayer()
    {
        if (_buffManager != null)
        {
            _buffManager.RemoveBleedEffect();
        }
        else
        {
            Debug.LogError("BuffManager가 null입니다. RemoveBleedFromPlayer를 실행할 수 없습니다.");
        }
    }

    // 특정 상황에서 실명 디버프를 적용
    public void ApplyBlindToPlayer()
    {
        if (_buffManager != null)
        {
            _buffManager.ApplyBlindEffect();
        }
        else
        {
            Debug.LogError("BuffManager가 null입니다. ApplyBlindToPlayer를 실행할 수 없습니다.");
        }
    }

    // 특정 상황에서 실명 디버프를 제거
    public void RemoveBlindFromPlayer()
    {
        if (_buffManager != null)
        {
            _buffManager.RemoveBlindEffect();
        }
        else
        {
            Debug.LogError("BuffManager가 null입니다. RemoveBlindFromPlayer를 실행할 수 없습니다.");
        }
    }

    // 특정 상황에서 혼란 디버프를 적용
    public void ApplyConfusionToPlayer()
    {
        if (_buffManager != null)
        {
            _buffManager.ApplyConfusionEffect();
        }
        else
        {
            Debug.LogError("BuffManager가 null입니다. ApplyConfusionToPlayer를 실행할 수 없습니다.");
        }
    }

    // 특정 상황에서 혼란 디버프를 제거
    public void RemoveConfusionEffect()
    {
        if (_buffManager != null)
        {
            _buffManager.RemoveConfusionEffect();
            if (_buffManager.ConfusionDebuff)
            {
                // EnemyUIManager의 UpdateUI 메소드 호출
                if (_enemyUIManager != null)
                {
                    // 필요한 파라미터를 넘겨줌
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
            Debug.LogError("BuffManager가 null입니다. RemoveConfusionEffect를 실행할 수 없습니다.");
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
