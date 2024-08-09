using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeywordInputField : MonoBehaviour
{

    public TMP_InputField iField;

    void Awake()
    {
        //iField = GetComponent<TMP_InputField>();
    }

    public void Clear()
    {
        iField.text = "";
        //DeckBuildingScreen.instance.CollectionBrowser.Keyword = iField.text;
    }

    public void EnterSubmit()
    {
        //DeckBuildingScreen.instance.CollectionBrowser.Keyword = iField.text;
    }
}
