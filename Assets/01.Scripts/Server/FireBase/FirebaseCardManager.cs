using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseCardManager : MonoBehaviour
{
    private FirebaseInit firebaseInit;
    public List<CardAsset> allCards; // 프로젝트 내 모든 카드 데이터의 리스트

    private void Awake()
    {
        FirebaseInit.OnFirebaseInitialized += FirebaseGetcomponent;
    }


    private void FirebaseGetcomponent()
    {
        firebaseInit = FindObjectOfType<FirebaseInit>();
        if (firebaseInit == null)
        {
            Debug.LogError("FirebaseInit 인스턴스를 찾을 수 없습니다.");
        }
        else
        {
            Debug.Log("FirebaseInit 인스턴스를 찾았습니다.");
        }

    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        FirebaseInit.OnFirebaseInitialized -= FirebaseGetcomponent;
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
                { "cardCount", cardCount }
            };
            cardsRef.Document(card.CardScriptName).SetAsync(cardData);
        }
    }


    /// <summary>
    /// Firestore에서 카드 이름을 불러와 리스트로 변환
    /// </summary>
    /// <param name="userEmail">유저 이메일</param>
    public void LoadCardNames(string userEmail)
    {
        CollectionReference cardsRef = firebaseInit.firestore.Collection("users").Document(userEmail).Collection("cards");
        cardsRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                QuerySnapshot snapshot = task.Result;
                List<CardAsset> loadedCards = new List<CardAsset>();

                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    if (document.TryGetValue("cardName", out string cardName))
                    {
                        CardAsset card = FindCardByName(cardName);
                        if (card != null)
                        {
                            loadedCards.Add(card);
                        }
                    }
                }

                // 카드 리스트가 준비완료 => 여기는 이제 덱리스트 보는곳에서 보이게 수정하면 될듯
                foreach (CardAsset card in loadedCards)
                {
                    Debug.Log($"Card: {card.CardScriptName}, SwordAttack: {card.SwordAttack}, MagicAttack: {card.MagicAttack}, ShieldAttack: {card.ShieldAttack}");
                }
            }
        });
    }

    private CardAsset FindCardByName(string cardName)
    {
        return allCards.Find(card => card.CardScriptName == cardName);
    }
}
