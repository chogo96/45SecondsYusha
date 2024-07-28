using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DecksStorage : MonoBehaviour
{

    public static DecksStorage instance;
    public List<DeckInfo> AllDecks { get; set; }

    private bool alreadyLoadedDecks = false;

    void Awake()
    {
        AllDecks = new List<DeckInfo>();
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (!alreadyLoadedDecks)
        {
            LoadDecksFromPlayerPrefs();
            alreadyLoadedDecks = true;
        }
    }

    void LoadDecksFromPlayerPrefs()
    {
        List<DeckInfo> DecksFound = new List<DeckInfo>();
        // playerPrefx에서 카드 정보 읽어옴
        for (int i = 0; i < 9; i++)
        {
            string deckListKey = "Deck" + i.ToString();
            string characterKey = "DeckHero" + i.ToString();
            string deckNameKey = "DeckName" + i.ToString();
            string[] DeckAsCardNames = PlayerPrefsX.GetStringArray(deckListKey);

            Debug.Log("Has character key: " + PlayerPrefs.HasKey(characterKey));
            Debug.Log("Has Deckname key: " + PlayerPrefs.HasKey(deckNameKey));
            Debug.Log("Length of DeckAsCardNames: " + DeckAsCardNames.Length);

            if (DeckAsCardNames.Length > 0 && PlayerPrefs.HasKey(characterKey) && PlayerPrefs.HasKey(deckNameKey))
            {
                string characterName = PlayerPrefs.GetString(characterKey);
                string deckName = PlayerPrefs.GetString(deckNameKey);

                List<CardAsset> deckList = new List<CardAsset>();
                foreach (string name in DeckAsCardNames)
                {
                    deckList.Add(CardCollection.instance.GetCardAssetByName(name));
                }

                DecksFound.Add(new DeckInfo(deckList, deckName, CharacterAssetsByName.instance.GetCharacterByName(characterName)));
            }
        }

        AllDecks = DecksFound;
    }

    public void SaveDecksIntoPlayerPrefs()
    {
        for (int i = 0; i < 9; i++)
        {
            string characterKey = "DeckHero" + i.ToString();
            string deckNameKey = "DeckName" + i.ToString();

            if (PlayerPrefs.HasKey(characterKey))
            {
                PlayerPrefs.DeleteKey(characterKey);
            }

            if (PlayerPrefs.HasKey(deckNameKey))
            {
                PlayerPrefs.DeleteKey(deckNameKey);
            }
        }

        for (int i = 0; i < AllDecks.Count; i++)
        {
            string deckListKey = "Deck" + i.ToString();
            string characterKey = "DeckHero" + i.ToString();
            string deckNameKey = "DeckName" + i.ToString();

            List<string> cardNamesList = new List<string>();
            foreach (CardAsset a in AllDecks[i].Cards)
                cardNamesList.Add(a.name);

            string[] cardNamesArray = cardNamesList.ToArray();

            PlayerPrefsX.SetStringArray(deckListKey, cardNamesArray);
            PlayerPrefs.SetString(deckNameKey, AllDecks[i].DeckName);
            PlayerPrefs.SetString(characterKey, AllDecks[i].Character.name);
        }
    }

    void OnApplicationQuit()
    {
        SaveDecksIntoPlayerPrefs();
    }
}


[System.Serializable]
public class DeckInfo
{
    public string DeckName;
    public CharacterAsset Character;
    public List<CardAsset> Cards;
    public int AmountOfCardsInDeck = 30;


    public DeckInfo(List<CardAsset> cards, string deckName, CharacterAsset charAsset)
    {
        Cards = new List<CardAsset>(cards);
        Character = charAsset;
        DeckName = deckName;
    }

    public bool IsComplete()
    {
        return (AmountOfCardsInDeck == Cards.Count);
    }

    public int NumberOfThisCardInDeck (CardAsset asset)
    {
        int count = 0;
        foreach (CardAsset ca in Cards)
        {
            if (ca == asset)
                count++;
        }
        return count;
    }
}

