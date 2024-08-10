using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyPunChat : MonoBehaviour
{
    private static DontDestroyPunChat instance;

    void Awake()
    {
        // �̹� �ν��Ͻ��� �����ϸ� �� ������Ʈ�� ����
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        // �ν��Ͻ��� �������� ������ �� ������Ʈ�� �ν��Ͻ��� �����ϰ� �������� �ʵ��� ����
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
