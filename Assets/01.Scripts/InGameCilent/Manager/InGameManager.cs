using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : MonoBehaviourPunCallbacks
{
    public int Sword;
    public int Magic;
    public int Shield;
    public int RandomValue;
    private Enemy _currentEnemy;
    private PlayerScripts[] _players;

    private int _previousSword;
    private int _previousMagic;
    private int _previousShield;

    private EnemyUIManager _enemyUIManager;

    public static InGameManager instance;

    private void Awake()
    {
        instance = this;

        _players = GameObject.FindObjectsOfType<PlayerScripts>();
    }

    public void SetCurrentEnemy(Enemy enemy)
    {
        _currentEnemy = enemy;
        Debug.Log($"SetCurrentEnemy: {_currentEnemy}");
    }

    public void ResetValues()
    {
        Sword = 0;
        Magic = 0;
        Shield = 0;
    }
}
