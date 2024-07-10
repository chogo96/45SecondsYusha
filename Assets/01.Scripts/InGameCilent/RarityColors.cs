//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class RarityColors : SingletonMonoBase<RarityColors>
//{
//    public RarityColorData[] Data;
//    public Dictionary<RarityOptions, Color32> ColorsDictionary = new Dictionary<RarityOptions, Color32>();

//    void Awake()
//    {
//        foreach (RarityColorData d in Data)
//        {
//            ColorsDictionary.Add(d.options, d.color);
//        }
//    }
//}

//[System.Serializable]
//public class RarityColorData
//{
//    public RarityOptions options;
//    public Color32 color = Color.white;
//}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RarityColors : MonoBehaviour
{
    public RarityColorData[] Data;
    public Dictionary<RarityOptions, Color32> ColorsDictionary = new Dictionary<RarityOptions, Color32>();

    public static RarityColors Instance;

    void Awake()
    {
        if (Instance != this && Instance != null)
            Destroy(Instance.gameObject);
        Instance = this;

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


