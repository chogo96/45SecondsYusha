using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewDeckScript : MonoBehaviour
{
    public void MakeANewDeck()
    {
        DeckBuildingScreen.instance.HideScreen();
        CharacterSelectionScreen.instance.ShowScreen();
    }
}
