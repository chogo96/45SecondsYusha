using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.EventSystems;

public class HandVisual : MonoBehaviour
{
    public AreaPosition owner;
    public bool TakeCardsOpenly = true;
    public SameDistanceChildren slots;

    [Header("Transform References")]
    public Transform DrawPreviewSpot;
    public Transform DeckTransform;
    public Transform OtherCardDrawSourceTransform;
    public Transform PlayPreviewSpot;

    public List<GameObject> CardsInHand = new List<GameObject>();
    private bool isFillingHand = false; // ���и� ä��� ������ Ȯ���ϴ� ����

    public void AddCard(GameObject card)
    {
        CardsInHand.Insert(0, card);
        card.transform.SetParent(slots.transform);
        PlaceCardsOnNewSlots();
        UpdatePlacementOfSlots();

        // �巡�� ����� ī�忡 �߰��մϴ�.
        card.AddComponent<Draggable>();
        card.GetComponent<Draggable>().HowToStart = Draggable.StartDragBehavior.OnMouseDown;
        card.GetComponent<Draggable>().HowToEnd = Draggable.EndDragBehavior.OnMouseUp;

        // ������ ī�� ������ 4�� ������ �� �ڷ�ƾ�� �����մϴ�.
        if (CardsInHand.Count <= 4 && !isFillingHand)
        {
            StartCoroutine(FillHandCoroutine());
        }
    }

    public void RemoveCard(GameObject card)
    {
        CardsInHand.Remove(card);
        PlaceCardsOnNewSlots();
        UpdatePlacementOfSlots();

        // ������ ī�� ������ 4�� ������ �� �ڷ�ƾ�� �����մϴ�.
        if (CardsInHand.Count <= 4 && !isFillingHand)
        {
            StartCoroutine(FillHandCoroutine());
        }
    }

    public void RemoveCardAtIndex(int index)
    {
        CardsInHand.RemoveAt(index);
        PlaceCardsOnNewSlots();
        UpdatePlacementOfSlots();

        // ������ ī�� ������ 4�� ������ �� �ڷ�ƾ�� �����մϴ�.
        if (CardsInHand.Count <= 4 && !isFillingHand)
        {
            StartCoroutine(FillHandCoroutine());
        }
    }

    public GameObject GetCardAtIndex(int index)
    {
        return CardsInHand[index];
    }

    void UpdatePlacementOfSlots()
    {
        float posX;
        if (CardsInHand.Count > 0)
            posX = (slots.Children[0].transform.localPosition.x - slots.Children[CardsInHand.Count - 1].transform.localPosition.x) / 2f;
        else
            posX = 0f;

        slots.gameObject.transform.DOLocalMoveX(posX, 0.3f);
    }

    public void PlaceCardsOnNewSlots()
    {
        foreach (GameObject g in CardsInHand)
        {
            g.transform.DOLocalMoveX(slots.Children[CardsInHand.IndexOf(g)].transform.localPosition.x, 0.3f);
            WhereIsTheCardOrCreature w = g.GetComponent<WhereIsTheCardOrCreature>();
            w.Slot = CardsInHand.IndexOf(g);
            w.SetHandSortingOrder();
        }
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

        foreach (Transform t in card.GetComponentsInChildren<Transform>())
            t.tag = owner.ToString() + "Card";
        AddCard(card);

        WhereIsTheCardOrCreature w = card.GetComponent<WhereIsTheCardOrCreature>();
        w.BringToFront();
        w.Slot = 0;
        w.VisualState = VisualStates.Transition;

        IDHolder id = card.AddComponent<IDHolder>();
        id.UniqueID = UniqueID;

        Sequence s = DOTween.Sequence();
        if (!fast)
        {
            s.Append(card.transform.DOMove(DrawPreviewSpot.position, GlobalSettings.instance.CardTransitionTime));
            if (TakeCardsOpenly)
                s.Insert(0f, card.transform.DOLocalRotate(Vector3.zero, GlobalSettings.instance.CardTransitionTime));
            else
                s.Insert(0f, card.transform.DOLocalRotate(new Vector3(0f, -179f, 0f), GlobalSettings.instance.CardTransitionTime));
            s.AppendInterval(GlobalSettings.instance.CardPreviewTime);
            s.Append(card.transform.DOLocalMove(slots.Children[0].transform.localPosition, GlobalSettings.instance.CardTransitionTime));
        }
        else
        {
            s.Append(card.transform.DOLocalMove(slots.Children[0].transform.localPosition, GlobalSettings.instance.CardTransitionTimeFast));
            if (TakeCardsOpenly)
                s.Insert(0f, card.transform.DOLocalRotate(Vector3.zero, GlobalSettings.instance.CardTransitionTimeFast));
            else
                s.Insert(0f, card.transform.DOLocalRotate(new Vector3(0f, -179f, 0f), GlobalSettings.instance.CardTransitionTimeFast));
        }

        s.OnComplete(() => ChangeLastCardStatusToInHand(card, w));
    }

    void ChangeLastCardStatusToInHand(GameObject card, WhereIsTheCardOrCreature w)
    {
        if (owner == AreaPosition.Low)
            w.VisualState = VisualStates.LowHand;
        else
            w.VisualState = VisualStates.TopHand;

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

    // ���и� 5������ ä��� �ڷ�ƾ
    private IEnumerator FillHandCoroutine()
    {
        isFillingHand = true;
        PlayerScripts playerScript = FindObjectOfType<PlayerScripts>(); // PlayerScripts�� ã���ϴ�.

        while (CardsInHand.Count < 5)
        {
            yield return new WaitForSeconds(0.5f);

            // PlayerScripts�� DrawACard �޼��带 ȣ���մϴ�.
            playerScript.DrawACard();

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
}
