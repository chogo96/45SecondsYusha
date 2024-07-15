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
        // ����: �÷��̾ ī�带 �� ������ ���� �״� ������ �˻�
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // ��, ����, �ǵ� ��ġ �ø���
            Sword++;
            Debug.Log("��!");

            // ���� ���� �״� ���� �˻�
            currentEnemy?.CheckDeathCondition(Sword, Magic, Shield);
        }
        // ����: �÷��̾ ī�带 �� ������ ���� �״� ������ �˻�
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // ��, ����, �ǵ� ��ġ �ø���
            Magic++;
            Debug.Log("����!");

            // ���� ���� �״� ���� �˻�
            currentEnemy?.CheckDeathCondition(Sword, Magic, Shield);
        }
        // ����: �÷��̾ ī�带 �� ������ ���� �״� ������ �˻�
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // ��, ����, �ǵ� ��ġ �ø���
            Shield++;
            Debug.Log("����!");

            // ���� ���� �״� ���� �˻�
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