using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadDeckAndCharacterFromStaticClass : MonoBehaviour
{
    void Awake()
    {
        PlayerScripts player = GetComponent<PlayerScripts>();
    }
}
