using UnityEngine;

public class PlayerDeckVisual : MonoBehaviour
{
    public AreaPosition owner;
    public float HeightOfOneCard = 0.012f;

    private Deck deck;
    private HandVisual handVisual;

    private void Awake()
    {
        deck = FindObjectOfType<Deck>();
        handVisual = FindObjectOfType<HandVisual>(); // 손패를 시각화하는 컴포넌트를 찾습니다.
        UpdateDeckCount();
    }

    private int cardsInDeck = 0;
    public int CardsInDeck
    {
        get { return cardsInDeck; }
        set
        {
            cardsInDeck = value;
            transform.localPosition = new Vector3(0, 0, -HeightOfOneCard * value);
        }
    }

    public void UpdateDeckCount()
    {
        CardsInDeck = deck.GetCardCount();
    }
}