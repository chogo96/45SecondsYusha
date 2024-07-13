using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAssetsByName : MonoBehaviour
{

    public static CharacterAssetsByName instance;
    private CharacterAsset[] allCharacterAssets;
    private Dictionary<string, CharacterAsset> AllCharactersDictionary = new Dictionary<string, CharacterAsset>();

    void Awake()
    {
        instance = this;
        allCharacterAssets = Resources.LoadAll<CharacterAsset>("");

        foreach (CharacterAsset characterAsset in allCharacterAssets)
            if (!AllCharactersDictionary.ContainsKey(characterAsset.name))
                AllCharactersDictionary.Add(characterAsset.name, characterAsset);
    }

    public CharacterAsset GetCharacterByName(string name)
    {
        if (AllCharactersDictionary.ContainsKey(name))
            return AllCharactersDictionary[name];
        else
            return null;
    }
}
