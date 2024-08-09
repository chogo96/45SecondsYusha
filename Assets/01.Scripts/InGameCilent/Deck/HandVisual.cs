using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.EventSystems;

public class HandVisual : MonoBehaviour
{
    public enum PlayerPosition { Player1, Player2, Player3, Player4 }
    public PlayerPosition owner;
    public bool TakeCardsOpenly = true;

    [Header("Transform References")]
    public Transform DrawPreviewSpot;
    public Transform DeckTransform;
    public Transform OtherCardDrawSourceTransform;
    public Transform PlayPreviewSpot;

    public Transform player1Left;
    public Transform player1Right;
    public Transform player2Left;
    public Transform player2Right;
    public Transform player3Left;
    public Transform player3Right;
    public Transform player4Left;
    public Transform player4Right;

    public Transform cardGatheringSpot; // 카드가 모이는 위치

    public List<GameObject> CardsInHand = new List<GameObject>();
    private bool isFillingHand = false; // 손패를 채우는 중인지 확인하는 변수


    public void AddCard(GameObject card)
    {
        CardsInHand.Insert(0, card);
        card.transform.SetParent(this.transform);

        //// 드래그 기능을 카드에 추가합니다.
        //card.AddComponent<Draggable>();
        //card.GetComponent<Draggable>().HowToStart = Draggable.StartDragBehavior.OnMouseDown;
        //card.GetComponent<Draggable>().HowToEnd = Draggable.EndDragBehavior.OnMouseUp;//// 드래그 기능을 카드에 추가합니다.
        card.AddComponent<CardDraggable>();
        card.GetComponent<CardDraggable>().HowToStart = CardDraggable.StartDragBehavior.OnMouseDown;
        card.GetComponent<CardDraggable>().HowToEnd = CardDraggable.EndDragBehavior.OnMouseUp;

        // 손패의 카드 개수가 4장 이하일 때 코루틴을 시작합니다.
        if (CardsInHand.Count <= 4 && !isFillingHand)
        {
            StartCoroutine(FillHandCoroutine());
        }

        // 카드 배치 즉시 수행
        PlaceCardsOnNewSlots();
    }

    public void RemoveCard(GameObject card)
    {
        CardsInHand.Remove(card);

        // 손패의 카드 개수가 4장 이하일 때 코루틴을 시작합니다.
        if (CardsInHand.Count <= 4 && !isFillingHand)
        {
            StartCoroutine(FillHandCoroutine());
        }

        // 카드 배치를 애니메이션 완료 후 수행합니다.
        Sequence s = DOTween.Sequence();
        s.AppendInterval(0.3f); // 애니메이션 대기 시간
        s.OnComplete(() => PlaceCardsOnNewSlots());
    }

    public void RemoveCardAtIndex(int index)
    {
        CardsInHand.RemoveAt(index);
        PlaceCardsOnNewSlots();

        // 손패의 카드 개수가 4장 이하일 때 코루틴을 시작합니다.
        if (CardsInHand.Count <= 4 && !isFillingHand)
        {
            StartCoroutine(FillHandCoroutine());
        }
    }

    public GameObject GetCardAtIndex(int index)
    {
        return CardsInHand[index];
    }
    public void PlaceCardsOnNewSlots()
    {
        var originCardPRSs = RoundAlignment(GetLeftTransform(), GetRightTransform(), CardsInHand.Count, 0.5f, Vector3.one * 1.9f);

        for (int i = 0; i < CardsInHand.Count; i++)
        {
            var card = CardsInHand[i];
            var targetPRS = originCardPRSs[i];

            // 부모 객체를 기준으로 상대적인 위치를 설정합니다.
            Vector3 relativePosition = this.transform.InverseTransformPoint(targetPRS.position);

            card.transform.DOLocalMove(relativePosition, 0.3f).SetEase(Ease.InOutQuad);
            card.transform.DORotateQuaternion(targetPRS.rotation, 0.3f).SetEase(Ease.InOutQuad);

            WhereIsTheCardOrCreature w = card.GetComponent<WhereIsTheCardOrCreature>();
            w.Slot = i;
            w.SetHandSortingOrder();
        }
    }

    public Vector3 GetCardPositionAtIndex(int index)
    {
        var originCardPRSs = RoundAlignment(GetLeftTransform(), GetRightTransform(), CardsInHand.Count, 0.5f, Vector3.one * 1.9f);
        if (index >= 0 && index < originCardPRSs.Count)
        {
            return originCardPRSs[index].position;
        }
        return Vector3.zero; // 유효하지 않은 인덱스에 대해서는 기본값 반환
    }

    public int GetMaxSlots()
    {
        return 99; // 예를 들어, 최대 손패 슬롯의 수를 99으로 가정
    }

    GameObject CreateACardAtPosition(CardAsset cardAsset, Vector3 position, Vector3 eulerAngles)
    {
        GameObject card = null;

        if (cardAsset.TypeOfCard == TypesOfCards.Attacks)
        {
            if (GlobalSettings.instance.CreatureCardPrefab == null)
            {
                Debug.LogError("CreatureCardPrefab is not assigned in GlobalSettings.");
            }
            card = Instantiate(GlobalSettings.instance.CreatureCardPrefab, position, Quaternion.Euler(eulerAngles));
        }
        else
        {
            if (cardAsset.Targets == TargetingOptions.Nothing)
            {
                if (GlobalSettings.instance.NoTargetSpellCardPrefab == null)
                {
                    Debug.LogError("NoTargetSpellCardPrefab is not assigned in GlobalSettings.");
                }
                card = Instantiate(GlobalSettings.instance.NoTargetSpellCardPrefab, position, Quaternion.Euler(eulerAngles));
            }
            else
            {
                if (GlobalSettings.instance.TargetedSpellCardPrefab == null)
                {
                    Debug.LogError("TargetedSpellCardPrefab is not assigned in GlobalSettings.");
                }
                card = Instantiate(GlobalSettings.instance.TargetedSpellCardPrefab, position, Quaternion.Euler(eulerAngles));
                DragSpellOnTarget dragSpell = card.GetComponentInChildren<DragSpellOnTarget>();
                dragSpell.Targets = cardAsset.Targets;
            }
        }

        if (card != null)
        {
            OneCardManager manager = card.GetComponent<OneCardManager>();
            manager.cardAsset = cardAsset;
            manager.ReadCardFromAsset();
        }
        else
        {
            Debug.LogError("Failed to create card: " + cardAsset.name);
        }

        return card;
    }
    public void GivePlayerACard(CardAsset c, int UniqueID, bool fast = false, bool fromDeck = true)
    {
        GameObject card;
        if (fromDeck)
            card = CreateACardAtPosition(c, DeckTransform.position, new Vector3(0f, -179f, 0f));
        else
            card = CreateACardAtPosition(c, OtherCardDrawSourceTransform.position, new Vector3(0f, -179f, 0f));

        // 플레이어 위치에 따라 태그를 설정합니다.
        string tag = "";
        switch (owner)
        {
            case PlayerPosition.Player1:
                tag = "1Card";
                break;
            case PlayerPosition.Player2:
                tag = "2Card";
                break;
            case PlayerPosition.Player3:
                tag = "3Card";
                break;
            case PlayerPosition.Player4:
                tag = "4Card";
                break;
            default:
                Debug.LogError("Invalid player position!");
                break;
        }

        if (!string.IsNullOrEmpty(tag))
        {
            foreach (Transform t in card.GetComponentsInChildren<Transform>())
                t.tag = tag;
        }
        else
        {
            Debug.LogError("Tag name is null or empty!");
        }

        // 카드 위치 설정 및 애니메이션 실행
        AddCard(card);

        WhereIsTheCardOrCreature w = card.GetComponent<WhereIsTheCardOrCreature>();
        w.BringToFront();
        w.Slot = 0;
        w.VisualState = VisualStates.Transition;

        IDHolder id = card.AddComponent<IDHolder>();
        id.UniqueID = UniqueID;

        // 새로 추가된 카드의 목표 위치
        Vector3 targetPosition = GetCardPositionAtIndex(0);
        card.transform.position = targetPosition;  // 위치를 바로 설정
        card.transform.rotation = Quaternion.identity;  // 회전을 바로 설정

        // 바로 정렬 수행
        PlaceCardsOnNewSlots();

        // 카드 상태 변경
        ChangeLastCardStatusToInHand(card, w);
    }


    void ChangeLastCardStatusToInHand(GameObject card, WhereIsTheCardOrCreature w)
    {
        switch (owner)
        {
            case PlayerPosition.Player1:
                w.VisualState = VisualStates.LowHand;
                break;
            case PlayerPosition.Player2:
                w.VisualState = VisualStates.TopHand;
                break;
            case PlayerPosition.Player3:
                w.VisualState = VisualStates.LeftHand;
                break;
            case PlayerPosition.Player4:
                w.VisualState = VisualStates.RightHand;
                break;
        }

        w.SetHandSortingOrder();
        Command.CommandExecutionComplete();
    }

    public void PlayASpellFromHand(int CardID)
    {
        GameObject card = IDHolder.GetGameObjectWithID(CardID);
        PlayASpellFromHand(card);
    }

    public void PlayASpellFromHand(GameObject CardVisual)
    {
        Command.CommandExecutionComplete();
        CardVisual.GetComponent<WhereIsTheCardOrCreature>().VisualState = VisualStates.Transition;
        RemoveCard(CardVisual);
        CardVisual.transform.SetParent(null);

        Sequence s = DOTween.Sequence();
        s.Append(CardVisual.transform.DOMove(PlayPreviewSpot.position, 1f));
        s.Insert(0f, CardVisual.transform.DORotate(Vector3.zero, 1f));
        s.AppendInterval(2f);
        s.OnComplete(() =>
        {
            Destroy(CardVisual);
        });
    }

    // 손패를 5장으로 채우는 코루틴
    private IEnumerator FillHandCoroutine()
    {
        isFillingHand = true;
        PlayerScripts playerScript = FindObjectOfType<PlayerScripts>(); // PlayerScripts를 찾습니다.

        while (CardsInHand.Count < 5)
        {
            yield return new WaitForSeconds(0.5f);

            // PlayerScripts의 DrawACard 메서드를 호출합니다.
            playerScript.DrawACard(1);

            // Wait until the card is added to the hand
            bool cardAdded = false;
            while (!cardAdded)
            {
                yield return new WaitForSeconds(0.1f); // Check every 0.1 seconds
                if (CardsInHand.Count == playerScript.hand.CardsInHand.Count)
                {
                    cardAdded = true;
                }
            }
        }
        isFillingHand = false;
    }

    List<PRS> RoundAlignment(Transform leftTr, Transform rightTr, int objCount, float height, Vector3 scale)
    {
        float[] objLerps = new float[objCount];
        List<PRS> results = new List<PRS>(objCount);

        switch (objCount)
        {
            case 1: objLerps = new float[] { 0.5f }; break;
            case 2: objLerps = new float[] { 0.27f, 0.73f }; break;
            case 3: objLerps = new float[] { 0.1f, 0.5f, 0.9f }; break;
            default:
                float interval = 1f / (objCount - 1);
                for (int i = 0; i < objCount; i++)
                    objLerps[i] = interval * i;
                break;
        }

        for (int i = 0; i < objCount; i++)
        {
            var targetPos = Vector3.Lerp(leftTr.position, rightTr.position, objLerps[i]);
            var targetRot = Quaternion.identity;
            if (objCount >= 4)
            {
                float curve = Mathf.Sqrt(Mathf.Pow(height, 2) - Mathf.Pow(objLerps[i] - 0.5f, 2));
                curve = height >= 0 ? curve : -curve;
                targetPos.y += curve;
                targetRot = Quaternion.Slerp(leftTr.rotation, rightTr.rotation, objLerps[i]);
            }
            results.Add(new PRS(targetPos, targetRot, scale));
        }
        return results;
    }

    private Transform GetLeftTransform()
    {
        switch (owner)
        {
            case PlayerPosition.Player1:
                return player1Left;
            case PlayerPosition.Player2:
                return player2Left;
            case PlayerPosition.Player3:
                return player3Left;
            case PlayerPosition.Player4:
                return player4Left;
            default:
                Debug.LogError("Invalid player position in GetLeftTransform!");
                return player1Left;
        }
    }

    private Transform GetRightTransform()
    {
        switch (owner)
        {
            case PlayerPosition.Player1:
                return player1Right;
            case PlayerPosition.Player2:
                return player2Right;
            case PlayerPosition.Player3:
                return player3Right;
            case PlayerPosition.Player4:
                return player4Right;
            default:
                Debug.LogError("Invalid player position in GetRightTransform!");
                return player1Right;
        }
    }
}

public class PRS
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public PRS(Vector3 pos, Quaternion rot, Vector3 scl)
    {
        position = pos;
        rotation = rot;
        scale = scl;
    }
}

[SerializeField]
public enum PlayerPosition
{
    Player1,
    Player2,
    Player3,
    Player4
}