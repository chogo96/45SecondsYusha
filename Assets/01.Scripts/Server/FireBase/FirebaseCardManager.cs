using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseCardManager : MonoBehaviour
{
    public FirebaseInit firebaseInit;
    public GameObject CardPrefab; // 카드 Prefab
    public Transform Content; // ScrollView의 Content GameObject

    private List<CardAsset> basicCards; // 기본 카드 리스트
    public Dictionary<CardAsset, int> viewCardss = new Dictionary<CardAsset, int>(); // 카드 및 수량
    public List<CardAsset> viewCards = new List<CardAsset>(); // 필터링된 카드 리스트

    public GameObject OneCharacterTabs; // 특정 캐릭터의 카드만 보여주는 탭
    public GameObject AllCharactersTabs; // 전체 캐릭터의 카드를 보여주는 탭

    private CharacterAsset selectedCharacter; // 선택된 캐릭터

    private void Awake()
    {
        LoadBasicCards(); // 기본 카드 로드

        if (firebaseInit == null)
        {
            Utils.LogRed("FirebaseInit 인스턴스를 찾을 수 없습니다.");
        }
        else
        {
            FirebaseInit.OnFirebaseInitialized += OnFirebaseInitialized;
            Utils.Log("FirebaseInit 인스턴스를 찾았습니다.");
        }
    }

    private void OnDestroy()
    {
        FirebaseInit.OnFirebaseInitialized -= OnFirebaseInitialized;
    }

    private void OnFirebaseInitialized()
    {
        Utils.Log("Firebase 초기화가 완료되었습니다.");
    }

    private void LoadBasicCards()
    {
        basicCards = new List<CardAsset>
        {
            // 기본 카드를 수동으로 추가하거나 리소스에서 로드합니다.
            Resources.Load<CardAsset>("GameAssets/Cards/격려"),
            Resources.Load<CardAsset>("GameAssets/Cards/될 대로 되라!"),
            Resources.Load<CardAsset>("GameAssets/Cards/머리!"),
            Resources.Load<CardAsset>("GameAssets/Cards/명검 [마가나츠]"),
            Resources.Load<CardAsset>("GameAssets/Cards/몽둥이질"),
            Resources.Load<CardAsset>("GameAssets/Cards/무모한 일격"),
            Resources.Load<CardAsset>("GameAssets/Cards/물약 섭취"),
            Resources.Load<CardAsset>("GameAssets/Cards/방어 자세"),
            Resources.Load<CardAsset>("GameAssets/Cards/보호"),
            Resources.Load<CardAsset>("GameAssets/Cards/붕대"),
            Resources.Load<CardAsset>("GameAssets/Cards/빈 물약통"),
            Resources.Load<CardAsset>("GameAssets/Cards/사각 방패"),
            Resources.Load<CardAsset>("GameAssets/Cards/숨고르기"),
            Resources.Load<CardAsset>("GameAssets/Cards/스크롤 해제!"),
            Resources.Load<CardAsset>("GameAssets/Cards/안약"),
            Resources.Load<CardAsset>("GameAssets/Cards/왕국 지원"),
            Resources.Load<CardAsset>("GameAssets/Cards/예언"),
            Resources.Load<CardAsset>("GameAssets/Cards/의료용 물자"),
            Resources.Load<CardAsset>("GameAssets/Cards/적에게 은총을!"),
            Resources.Load<CardAsset>("GameAssets/Cards/파이어볼 주문서"),
            Resources.Load<CardAsset>("GameAssets/Cards/하급 정화"),
            Resources.Load<CardAsset>("GameAssets/Cards/하급 치유"),
        };

        // 기본 카드를 viewCardss에 3장씩 추가
        foreach (var basicCard in basicCards)
        {
            viewCardss[basicCard] = 3;
        }
    }

    public void SaveCardData(Dictionary<CardAsset, int> cards, string userId)
    {
        if (firebaseInit == null || firebaseInit.firestore == null)
        {
            Utils.LogRed("firebaseInit 또는 firestore가 null입니다. SaveCardData를 호출할 수 없습니다.");
            return;
        }

        CollectionReference cardsRef = firebaseInit.firestore.Collection("users").Document(userId).Collection("cards");

        foreach (var cardEntry in cards)
        {
            var cardData = new Dictionary<string, object>
            {
                { "cardName", cardEntry.Key.CardScriptName },
                { "cardCount", cardEntry.Value },
                { "rarity", cardEntry.Key.Rarity }
            };
            cardsRef.Document(cardEntry.Key.CardScriptName).SetAsync(cardData);
        }
    }

    public async void LoadCardNames(string userId)
    {
        if (firebaseInit == null || firebaseInit.firestore == null)
        {
            Utils.LogRed("firebaseInit 또는 firestore가 null입니다. LoadCardNames를 호출할 수 없습니다.");
            return;
        }

        if (string.IsNullOrEmpty(userId))
        {
            Utils.LogRed("userId가 null이거나 빈 문자열입니다.");
            return;
        }

        try
        {
            CollectionReference cardsRef = firebaseInit.firestore.Collection("users").Document(userId).Collection("cards");
            QuerySnapshot snapshot = await cardsRef.GetSnapshotAsync();

            List<CardAsset> loadedCards = new List<CardAsset>();
            Dictionary<CardAsset, int> loadedCardss = new Dictionary<CardAsset, int>();

            foreach (var document in snapshot.Documents)
            {
                if (document.TryGetValue("cardName", out string cardName) && document.TryGetValue("cardCount", out int cardCount))
                {
                    CardAsset card = FindCardByName(cardName);
                    if (card != null && (selectedCharacter == null || card.CharacterAsset == selectedCharacter || card.CharacterAsset == null))
                    {
                        loadedCards.Add(card);
                        loadedCardss[card] = cardCount;
                    }
                }
            }

            // 기본 카드도 필터링하여 추가
            foreach (var basicCard in basicCards)
            {
                if (selectedCharacter == null || basicCard.CharacterAsset == selectedCharacter || basicCard.CharacterAsset == null)
                {
                    loadedCards.Add(basicCard);
                    if (!loadedCardss.ContainsKey(basicCard))
                    {
                        loadedCardss[basicCard] = 3;
                    }
                }
            }

            viewCardss = loadedCardss;
            viewCards.Clear();
            viewCards.AddRange(loadedCards);

            UpdateCardUI();
        }
        catch (System.Exception e)
        {
            Utils.LogRed("카드 데이터 로드 실패: " + e.Message);
        }
    }

    // 생성된 카드 UI를 모두 제거하는 메서드
    public void ClearCreatedCards()
    {
        foreach (Transform child in Content)
        {
            Destroy(child.gameObject);
        }
    }

    // 특정 카드의 수량을 반환하는 메서드
    public int GetCardCount(string userId, CardAsset card)
    {
        if (viewCardss.TryGetValue(card, out int count))
        {
            return count;
        }
        return 0; // 카드가 없으면 0 반환
    }

    private CardAsset FindCardByName(string cardName)
    {
        string path = $"GameAssets/Cards/{cardName}";
        CardAsset card = Resources.Load<CardAsset>(path);

        if (card == null)
        {
            Utils.LogRed($"카드 에셋을 찾을 수 없습니다: {path}");
        }
        else
        {
            Utils.Log($"카드 에셋 로드 성공: {path}");
        }

        return card;
    }

    public void SetSelectedCharacter(CharacterAsset character)
    {
        selectedCharacter = character;
        LoadCardNames(LoginManager.UserId);
    }

    private void UpdateCardUI()
    {
        foreach (Transform child in Content)
        {
            Destroy(child.gameObject);
        }

        foreach (var card in viewCards)
        {
            GameObject newCard = Instantiate(CardPrefab, Content);
            OneCardManager manager = newCard.GetComponent<OneCardManager>();
            manager.cardAsset = card;
            manager.ReadCardFromAsset();

            AddCardToDeck addCardComponent = newCard.GetComponent<AddCardToDeck>();
            addCardComponent.SetCardAsset(card);

            if (viewCardss.TryGetValue(card, out int cardCount))
            {
                addCardComponent.UpdateQuantity(cardCount);
            }
            else
            {
                addCardComponent.UpdateQuantity(0);
            }
        }
    }
}
