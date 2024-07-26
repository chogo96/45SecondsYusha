using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class InGameShop: MonoBehaviourPunCallbacks
{
    public Button button1;
    public Button button2;
    private EnemySpawner _enemySpawner;
    void Start()
    {
        _enemySpawner = FindObjectOfType<EnemySpawner>();
        button1.onClick.AddListener(OnAddTimeButton);
        button2.onClick.AddListener(OnAddCardButton);
    }

    void OnAddTimeButton()
    {
        // 버튼 1 클릭 시 처리할 로직
        Debug.Log("시간이 더해짐");

        photonView.RPC("TimePlusMinus", RpcTarget.All, 10);

        if (_enemySpawner != null)
        {
            _enemySpawner.OnShopButtonPressed();
        }
        Destroy(gameObject); // 상점 UI 제거
    }

    void OnAddCardButton()
    {
        // 버튼2 클릭 시 처리할 로직
        Debug.Log("카드가 더해짐");
        if (_enemySpawner != null)
        {
            
            _enemySpawner.OnShopButtonPressed();
        }
        Destroy(gameObject); // 상점 UI 제거
    }
}
