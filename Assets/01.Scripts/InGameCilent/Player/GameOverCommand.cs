using UnityEngine;
using System.Collections;
using Photon.Realtime;

public class GameOverCommand : Command
{

    private PlayerScripts looser;

    public GameOverCommand(PlayerScripts looser)
    {
        this.looser = looser;
    }

    public override void StartCommandExecution()
    {
        looser.PArea.Portrait.Explode();
    }
}
