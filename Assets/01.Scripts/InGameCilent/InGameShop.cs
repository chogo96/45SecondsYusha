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
        // ��ư 1 Ŭ�� �� ó���� ����
        Debug.Log("�ð��� ������");

        photonView.RPC("TimePlusMinus", RpcTarget.All, 10);

        if (_enemySpawner != null)
        {
            _enemySpawner.OnShopButtonPressed();
        }
        Destroy(gameObject); // ���� UI ����
    }

    void OnAddCardButton()
    {
        // ��ư2 Ŭ�� �� ó���� ����
        Debug.Log("ī�尡 ������");
        if (_enemySpawner != null)
        {
            
            _enemySpawner.OnShopButtonPressed();
        }
        Destroy(gameObject); // ���� UI ����
    }
}
