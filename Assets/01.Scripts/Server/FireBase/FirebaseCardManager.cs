using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseCardManager : MonoBehaviour
{
    public FirebaseInit firebaseInit;
    private List<CardAsset> _allCards; // 프로젝트 내 모든 카드 데이터의 리스트

    public GameObject CreatureMenuPrefab;
    public List<CardAsset> viewCards = new List<CardAsset>(); // 프로젝트 내 모든 카드 데이터의 리스트

    public Dictionary<CardAsset, int> viewCardss = new Dictionary<CardAsset, int>();


    public Transform[] Slots;
    CollectionBrowser collectionBrowser = new CollectionBrowser();




    private void Awake()
    {
        if (firebaseInit == null)
        {
            Debug.LogError("FirebaseInit 인스턴스를 찾을 수 없습니다.");
        }
        else
        {
            FirebaseInit.OnFirebaseInitialized += OnFirebaseInitialized;
            Debug.Log("FirebaseInit 인스턴스를 찾았습니다.");
        }
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        FirebaseInit.OnFirebaseInitialized -= OnFirebaseInitialized;
    }
    private void OnFirebaseInitialized()
    {
        // Firebase 초기화가 완료되었을 때 호출되는 메서드
        Debug.Log("Firebase 초기화가 완료되었습니다.");
    }

    /// <summary>
    /// 카드 이름만 Firestore에 저장
    /// </summary>
    /// <param name="cards">뽑은 카드들</param>
    /// <param name="userEmail">유저 이메일</param>
    public void SaveCardData(Dictionary<CardAsset, int> cards, string userEmail)
    {

        if (firebaseInit == null || firebaseInit.firestore == null)
        {
            Debug.LogError("firebaseInit 또는 firestore가 null입니다. SaveCardData를 호출할 수 없습니다.");
            return;
        }


        CollectionReference cardsRef = firebaseInit.firestore.Collection("users").Document(userEmail).Collection("cards");

        foreach (var cardEntry in cards)
        {
            CardAsset card = cardEntry.Key;
            int cardCount = cardEntry.Value;

            Dictionary<string, object> cardData = new Dictionary<string, object>
            {
                { "cardName", card.CardScriptName },
                { "cardCount", cardCount },
                { "rarity", card.Rarity }
            };
            cardsRef.Document(card.CardScriptName).SetAsync(cardData);
        }
    }


    /// <summary>
    /// Firestore에서 카드 이름을 불러와 리스트로 변환
    /// </summary>
    /// <param name="userEmail">유저 이메일</param>
    public async void LoadCardNames(string userEmail)
    {
        if (firebaseInit == null || firebaseInit.firestore == null)
        {
            Debug.LogError("firebaseInit 또는 firestore가 null입니다. LoadCardNames를 호출할 수 없습니다.");
            return;
        }

        if (string.IsNullOrEmpty(userEmail))
        {
            Debug.LogError("userEmail이 null이거나 빈 문자열입니다.");
            return;
        }

        Debug.Log("LoadCardNames 호출됨");
        Debug.Log($"사용자 이메일: {userEmail}");

        CollectionReference cardsRef = firebaseInit.firestore.Collection("users").Document(userEmail).Collection("cards");
        Debug.Log($"Firestore 경로: users/{userEmail}/cards");

        try
        {
            QuerySnapshot snapshot = await cardsRef.GetSnapshotAsync();
            Debug.Log("카드 데이터 로드 완료");
            List<CardAsset> loadedCards = new List<CardAsset>();
            Dictionary<CardAsset,int> loadedCardss = new Dictionary<CardAsset, int>();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                Debug.Log($"문서 ID: {document.Id}");
                if (document.TryGetValue("cardName", out string cardName) && document.TryGetValue("cardCount", out int cardCount))
                {
                    Debug.Log($"카드 이름: {cardName}");
                    Debug.Log($"카드 수량: {cardCount}");
                    CardAsset card = FindCardByName(cardName);
                    if (card != null)
                    {
                        loadedCards.Add(card);
                        loadedCardss.Add(card, cardCount);
                        
                    }
                    else
                    {
                        Debug.LogError($"카드 이름을 찾을 수 없습니다: {cardName}");
                    }
                }
                else
                {
                    Debug.LogError("카드 이름을 가져오지 못했습니다.");
                }
            }
            viewCardss = loadedCardss;

            Debug.Log($"로드된 카드 수: {loadedCards.Count}");

            // 중복 추가 방지: viewCards 초기화
            viewCards.Clear();

            foreach (CardAsset card in loadedCards)
            {
                if (!viewCards.Contains(card))
                {
                    viewCards.Add(card);
                }
            }
            


            // 이전에 생성된 카드 UI 제거
            foreach (GameObject cardObject in collectionBrowser.CreatedCards)
            {
                Destroy(cardObject);
            }
            collectionBrowser.CreatedCards.Clear();


            


            for (int i = 0; i < viewCards.Count; i++)
            {
                GameObject newMenuCard;

                if (viewCards[i].TypeOfCard == TypesOfCards.Attacks)
                {
                    newMenuCard = Instantiate(CreatureMenuPrefab, Slots[i].position, Quaternion.identity);
                }
                else
                {
                    newMenuCard = Instantiate(CreatureMenuPrefab, Slots[i].position, Quaternion.identity);
                }

                newMenuCard.transform.SetParent(this.transform);

                collectionBrowser.CreatedCards.Add(newMenuCard);

                OneCardManager manager = newMenuCard.GetComponent<OneCardManager>();
                manager.cardAsset = viewCards[i];
                manager.ReadCardFromAsset();

                AddCardToDeck addCardComponent = newMenuCard.GetComponent<AddCardToDeck>();
                addCardComponent.SetCardAsset(viewCards[i]);
                addCardComponent.UpdateQuantity();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("카드 데이터 로드 실패: " + e.Message);
        }
    }

    private CardAsset FindCardByName(string cardName)
    {
        string path = $"GameAssets/Cards/{cardName}";
        CardAsset card = Resources.Load<CardAsset>(path);

        if (card == null)
        {
            Debug.LogError($"카드 에셋을 찾을 수 없습니다: {path}");
        }
        else
        {
            Debug.Log($"카드 에셋 로드 성공: {path}");
        }

        return card;
    }
}
