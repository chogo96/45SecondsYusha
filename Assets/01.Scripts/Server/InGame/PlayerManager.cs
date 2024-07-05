using Photon.Pun;
using UnityEngine;

public class PlayerManager : MonoBehaviourPun
{
    public static PlayerManager LocalPlayerInstance;

    void Awake()
    {
        if (photonView.IsMine)
        {
            LocalPlayerInstance = this;
        }
    }

    void Start()
    {
        if (photonView.IsMine)
        {
            LocalPlayerInstance = this;
        }
    }
}
