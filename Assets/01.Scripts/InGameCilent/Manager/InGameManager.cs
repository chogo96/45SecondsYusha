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
        _enemyUIManager = FindObjectOfType<EnemyUIManager>(); // EnemyUIManager를 찾습니다.
    }

    public void SetCurrentEnemy(Enemy enemy)
    {
        _currentEnemy = enemy;
        Utils.Log($"SetCurrentEnemy: {_currentEnemy}");
        UpdateEnemyUI(); // 새로운 적 설정 시 UI 업데이트
    }

    public void ResetValues()
    {
        Sword = 0;
        Magic = 0;
        Shield = 0;
        UpdateEnemyUI(); // 값 리셋 시 UI 업데이트
    }

    // 적의 UI를 업데이트하는 메서드
    private void UpdateEnemyUI()
    {
        if (_currentEnemy != null && _enemyUIManager != null)
        {
            _enemyUIManager.UpdateUI(_currentEnemy.requiredSword, _currentEnemy.requiredMagic, _currentEnemy.requiredShield);
        }
    }

    // Sword 값을 변경하는 메서드 (예시)
    public void ChangeSwordValue(int newValue)
    {
        Sword = newValue;
        UpdateEnemyUI(); // Sword 값 변경 시 UI 업데이트
    }

    // Magic 값을 변경하는 메서드 (예시)
    public void ChangeMagicValue(int newValue)
    {
        Magic = newValue;
        UpdateEnemyUI(); // Magic 값 변경 시 UI 업데이트
    }

    // Shield 값을 변경하는 메서드 (예시)
    public void ChangeShieldValue(int newValue)
    {
        Shield = newValue;
        UpdateEnemyUI(); // Shield 값 변경 시 UI 업데이트
    }
}
