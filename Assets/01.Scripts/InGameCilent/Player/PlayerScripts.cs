using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using System.Collections;
using System;
using static UnityEngine.Rendering.DebugUI;

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
    // 카드 플레이 딜레이를 주기 위한 변수
    private bool isPlayingCard = false;

    // 적이 살아있는지 여부 체크
    private bool isEnemyAlive = true;

    public static PlayerScripts[] Players;
    private BuffManager _buffManager;

    private PlayerSetManager playerSetManager;
    private int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

    private bool isDrawingCard = false;

    private GameObject _playerTransform;

    // 서버에서 사용할 int값
    public int _swordPoint;
    public int _magicPoint;
    public int _shieldPoint;

    // UI_PlayerCount 에 보낼 값
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

        // EnemySpawner에 자신을 등록
        RegisterWithEnemySpawner();

        // BuffManager 초기화
        GameObject buffManagerObject = GameObject.Find("BuffManager");
        if (buffManagerObject != null)
        {
            _buffManager = buffManagerObject.GetComponent<BuffManager>();
            if (_buffManager == null)
            {
                Utils.LogRed("BuffManager 컴포넌트를 찾을 수 없습니다.");
            }
        }
        else
        {
            Utils.LogRed("BuffManager 오브젝트를 찾을 수 없습니다.");
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
            Utils.LogRed("EnemySpawner를 찾을 수 없습니다.");
        }
    }

    void Start()
    {
        // 적이 죽었을 때 호출되는 이벤트 핸들러 등록
        Enemy.OnEnemyDeath -= OnEnemyDeath;  // 중복 등록 방지
        Enemy.OnEnemyDeath += OnEnemyDeath;
        Enemy.OnEnemySpawned -= OnNewEnemySpawned;  // 중복 등록 방지
        Enemy.OnEnemySpawned += OnNewEnemySpawned;

        #region 직업 스크립트를 AddComponent 하는곳
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
        // 이벤트 핸들러 등록 해제
        Enemy.OnEnemyDeath -= OnEnemyDeath;
        Enemy.OnEnemySpawned -= OnNewEnemySpawned;
        GameManager.AllPlayersSpawned -= C_FillHand;
    }

    void OnEnemyDeath()
    {
        isEnemyAlive = false; // 적이 죽으면 플래그를 false로 설정
    }

    void OnNewEnemySpawned(Enemy newEnemy)
    {
        SetCurrentEnemy(newEnemy);
        isEnemyAlive = true; // 새로운 적이 생성되면 플래그를 true로 설정
    }

    [PunRPC]
    public void RealTimeBossStatusCheck(int sword, int magic, int shield)
    {
        if (_currentEnemy == null)
        {
            Debug.LogWarning("현재 적이 null 상태입니다. RealTimeBossStatusCheck 호출을 무시합니다.");
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

        // 현재 적의 생존 조건 확인
        _currentEnemy?.CheckDeathCondition(_swordPoint, _magicPoint, _shieldPoint);
    }

    public void DrawACard(int n)
    {
        if (isDrawingCard || !isEnemyAlive) // 적이 죽었거나 새로운 적이 생성되기 전이면 카드 드로우를 일시 중지
        {
            return;
        }

        StartCoroutine(DrawCardsCoroutine(n));
    }

    private IEnumerator DrawCardsCoroutine(int n)
    {
        isDrawingCard = true;
        HandVisual handVisual = PArea.handVisual;  // 현재 플레이어의 HandVisual 참조

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

                    // 바로 정렬 수행
                    handVisual.PlaceCardsOnNewSlots();

                    // 카드 드로우 후 카드 수 갱신
                    UpdateCardCounts("Plus");

                    yield return new WaitForSeconds(0.1f); // 카드 드로우 사이에 짧은 대기 시간을 추가하여 동시에 많은 카드 드로우를 방지합니다.
                }
                else
                {
                    // 덱을 다 썼을 때 탈진 효과 넣는 곳
                    break;
                }
            }
        }

        isDrawingCard = false;
    }

    // 덱과 손패 카드 수를 업데이트하는 메서드
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
        HandVisual handVisual = PArea.handVisual;  // 현재 플레이어의 HandVisual 참조
        if (hand.CardsInHand.Count < handVisual.GetMaxSlots() && hand.CardsInHand.Count < 5)
        {
            CardLogic newCard = new CardLogic(cardAsset, this);
            newCard.owner = this;
            hand.CardsInHand.Insert(0, newCard);
            new DrawACardCommand(hand.CardsInHand[0], this, fromDeck: false).AddToQueue();
            UpdateCardCounts("Plus"); // 카드 수 업데이트
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
            StartCoroutine(PlayACardWithDelay(card, target)); // 코루틴 시작
        }
        else
        {
            Utils.LogRed("Card not found");
        }
    }

    private IEnumerator PlayACardWithDelay(CardLogic card, ICharacter target)
    {
        Utils.Log($"PlayACardWithDelay called for card: {card.cardAsset.name}");

        // 적이 죽었을 때 카드 플레이를 일시 중지
        while (isPlayingCard || !isEnemyAlive)
        {
            yield return null;
        }

        isPlayingCard = true; // 카드 플레이 시작

        // 카드 사용 후 카드 수 갱신
        UpdateCardCounts("Minus");

        if (BuffManager.instance.BlindDebuff)
        {
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                Utils.Log("카드가 실명 효과로 버려졌습니다!! ^^");

                // 카드 처리 로직을 이곳에서 처리
                if (card.cardAsset.IsVanishCard)
                {
                    VanishCard(card);
                }
                else
                {
                    DiscardCard(card);
                }

                isPlayingCard = false; // 카드 플레이 종료
                yield break;
            }
        }

        if (card != null && card.cardAsset != null)
        {
            _previousSword = InGameManager.instance.Sword;
            _previousMagic = InGameManager.instance.Magic;
            _previousShield = InGameManager.instance.Shield;

            // SwordAttack, MagicAttack, ShieldAttack, RandomAttack 값을 플레이어에게 반영
            Utils.Log($"Before: Sword={InGameManager.instance.Sword}, Magic={InGameManager.instance.Magic}, Shield={InGameManager.instance.Shield}");
            InGameManager.instance.Sword += card.cardAsset.SwordAttack;
            InGameManager.instance.Magic += card.cardAsset.MagicAttack;
            InGameManager.instance.Shield += card.cardAsset.ShieldAttack;
            Utils.Log($"After Attack: Sword={InGameManager.instance.Sword}, Magic={InGameManager.instance.Magic}, Shield={InGameManager.instance.Shield}");

            // 이 부분이 각 속성을 두 번 추가하고 있는지 확인하세요.
            Utils.Log($"Final Values: Sword={InGameManager.instance.Sword}, Magic={InGameManager.instance.Magic}, Shield={InGameManager.instance.Shield}");

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

            // 전체 플레이어의 모든 디버프 제거
            void RemoveAllDebuffs()
            {
                foreach (var player in Players)
                {
                    player._buffManager.RemoveBlindEffect();
                    player._buffManager.RemoveBleedEffect();
                    player._buffManager.RemoveConfusionEffect();
                }
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
                Utils.Log("ㅇㅇ 적 ui매니저 있음");
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

            // 적 객체가 null이 아닌 경우에만 RealTimeBossStatusCheck 호출
            if (_currentEnemy != null)
            {
                photonView.RPC("RealTimeBossStatusCheck", RpcTarget.All, InGameManager.instance.Sword, InGameManager.instance.Magic, InGameManager.instance.Shield);
            }
            _deck.ReturnRandomCardsFromDiscard(card.cardAsset.RandomRestoreDeck);
        }

        // 손패의 카드 개수가 4장 이하일 때 덱에서 카드를 채우는 로직 추가
        if (hand.CardsInHand.Count <= 4 && !isFillingHand)
        {
            StartCoroutine(FillHandCoroutine());
        }

        yield return new WaitForSeconds(0.5f); // 딜레이 추가
        isPlayingCard = false; // 카드 플레이 종료
    }

    private void VanishCard(CardLogic card)
    {
        hand.CardsInHand.Remove(card);
        _deck.VanishDeck.Add(card.cardAsset);
        Utils.Log($"Card {card.cardAsset.name} vanished.");
        UpdateCardCounts("Minus"); // 카드 수 업데이트
    }

    private void DiscardCard(CardLogic card)
    {
        hand.CardsInHand.Remove(card);
        _deck.DiscardDeck.Add(card.cardAsset);
        Utils.Log($"Card {card.cardAsset.name} discarded.");
        UpdateCardCounts("Minus"); // 카드 수 업데이트
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
            Utils.LogRed("BuffManager가 null입니다. ApplyBleedToPlayer를 실행할 수 없습니다.");
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
            Utils.LogRed("BuffManager가 null입니다. RemoveBleedFromPlayer를 실행할 수 없습니다.");
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
            Utils.LogRed("BuffManager가 null입니다. ApplyBlindToPlayer를 실행할 수 없습니다.");
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
            Utils.LogRed("BuffManager가 null입니다. RemoveBlindFromPlayer를 실행할 수 없습니다.");
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
            Utils.LogRed("BuffManager가 null입니다. ApplyConfusionToPlayer를 실행할 수 없습니다.");
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
            Utils.LogRed("BuffManager가 null입니다. RemoveConfusionEffect를 실행할 수 없습니다.");
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
        UpdateCardCounts("Update"); // 손패가 가득 찬 후에도 카드 수를 업데이트
    }
}
