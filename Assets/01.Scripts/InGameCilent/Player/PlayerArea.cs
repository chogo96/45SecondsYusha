using UnityEngine;
using System.Collections;

public enum AreaPosition { Top, Low, Left, Right }

public class PlayerArea : MonoBehaviour
{
    public AreaPosition owner;
    public bool ControlsON = true;
    public PlayerDeckVisual PDeck;
    public HandVisual handVisual;
    public PlayerPortraitVisual Portrait;
    public HeroPowerButton HeroPower;
    //public TableVisual tableVisual;
    public Transform PortraitPosition;
    public Transform InitialPortraitPosition;

    public bool AllowedToControlThisPlayer
    {
        get;
        set;
    }
}
