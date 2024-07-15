using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScripts : MonoBehaviour
{
    public int Sword;
    public int Magic;
    public int Shield;
    private Enemy currentEnemy;

    void Update()
    {
        // 예시: 플레이어가 카드를 낼 때마다 적의 죽는 조건을 검사
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // 검, 마법, 실드 수치 올리기
            Sword++;
            Debug.Log("검!");

            // 현재 적의 죽는 조건 검사
            currentEnemy?.CheckDeathCondition(Sword, Magic, Shield);
        }
        // 예시: 플레이어가 카드를 낼 때마다 적의 죽는 조건을 검사
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // 검, 마법, 실드 수치 올리기
            Magic++;
            Debug.Log("마법!");

            // 현재 적의 죽는 조건 검사
            currentEnemy?.CheckDeathCondition(Sword, Magic, Shield);
        }
        // 예시: 플레이어가 카드를 낼 때마다 적의 죽는 조건을 검사
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // 검, 마법, 실드 수치 올리기
            Shield++;
            Debug.Log("방패!");

            // 현재 적의 죽는 조건 검사
            currentEnemy?.CheckDeathCondition(Sword, Magic, Shield);
        }
    }

    public void SetCurrentEnemy(Enemy enemy)
    {
        currentEnemy = enemy;
    }

    public void ResetValues()
    {
        Sword = 0;
        Magic = 0;
        Shield = 0;
    }
}