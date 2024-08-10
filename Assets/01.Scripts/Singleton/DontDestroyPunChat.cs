using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyPunChat : MonoBehaviour
{
    private static DontDestroyPunChat instance;

    void Awake()
    {
        // 이미 인스턴스가 존재하면 이 오브젝트를 삭제
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        // 인스턴스가 존재하지 않으면 이 오브젝트를 인스턴스로 설정하고 삭제되지 않도록 설정
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
