using UnityEngine;
using System.Collections;
using Photon.Realtime;

public class DrawACardCommand : Command
{

    private PlayerScripts p;
    private CardLogic cl;
    private bool fast;
    private bool fromDeck;

    public PlayerScripts player
    {
        get { return p; }
    }

    public DrawACardCommand(CardLogic cl, PlayerScripts p, bool fast, bool fromDeck)
    {
        this.cl = cl;
        this.p = p;
        this.fast = fast;
        this.fromDeck = fromDeck;
    }

    public override void StartCommandExecution()
    {
        p.PArea.PDeck.CardsInDeck--;
        p.PArea.handVisual.GivePlayerACard(cl.cardAsset, cl.UniqueCardID, fast, fromDeck);
    }
}
