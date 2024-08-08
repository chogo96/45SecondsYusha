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
        _enemyUIManager = FindObjectOfType<EnemyUIManager>(); // EnemyUIManager�� ã���ϴ�.
    }

    public void SetCurrentEnemy(Enemy enemy)
    {
        _currentEnemy = enemy;
        Utils.Log($"SetCurrentEnemy: {_currentEnemy}");
        UpdateEnemyUI(); // ���ο� �� ���� �� UI ������Ʈ
    }

    public void ResetValues()
    {
        Sword = 0;
        Magic = 0;
        Shield = 0;
        UpdateEnemyUI(); // �� ���� �� UI ������Ʈ
    }

    // ���� UI�� ������Ʈ�ϴ� �޼���
    private void UpdateEnemyUI()
    {
        if (_currentEnemy != null && _enemyUIManager != null)
        {
            _enemyUIManager.UpdateUI(_currentEnemy.requiredSword, _currentEnemy.requiredMagic, _currentEnemy.requiredShield);
        }
    }

    // Sword ���� �����ϴ� �޼��� (����)
    public void ChangeSwordValue(int newValue)
    {
        Sword = newValue;
        UpdateEnemyUI(); // Sword �� ���� �� UI ������Ʈ
    }

    // Magic ���� �����ϴ� �޼��� (����)
    public void ChangeMagicValue(int newValue)
    {
        Magic = newValue;
        UpdateEnemyUI(); // Magic �� ���� �� UI ������Ʈ
    }

    // Shield ���� �����ϴ� �޼��� (����)
    public void ChangeShieldValue(int newValue)
    {
        Shield = newValue;
        UpdateEnemyUI(); // Shield �� ���� �� UI ������Ʈ
    }
}
