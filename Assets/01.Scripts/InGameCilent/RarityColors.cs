
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RarityColors : MonoBehaviour
{
    public RarityColorData[] Data;
    public Dictionary<RarityOptions, Color32> ColorsDictionary = new Dictionary<RarityOptions, Color32>();

    public static RarityColors instance;

    void Awake()
    {
        // TODO: 스크립트가 있어도 다시 소환하는 버그가 있어서 이런 식으로 구현함  
        if (instance != this && instance != null)
            Destroy(instance.gameObject);
        instance = this;

        DontDestroyOnLoad(this);

        foreach (RarityColorData d in Data)
        {
            ColorsDictionary.Add(d.options, d.color);
        }
    }
}

[System.Serializable]
public class RarityColorData
{
    public RarityOptions options;
    public Color32 color = Color.white;
}


