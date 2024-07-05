using Photon.Pun;
using UnityEngine;

public class PlayerSetup : MonoBehaviourPun
{
    private GameObject playerCamera;

    private void Awake()
    {
        // ��Ȯ�� ��θ� ����Ͽ� ī�޶� ã���ϴ�.
        playerCamera = transform.Find("Camera")?.gameObject;

        if (playerCamera == null)
        {
            Debug.LogError("PlayerSetup: Camera not found. Make sure the Camera is a child of the Player prefab and named correctly.");
        }
    }

    void Start()
    {
        if (photonView.IsMine)
        {
            // �ڽ��� �÷��̾�� ī�޶� Ȱ��ȭ
            playerCamera.SetActive(true);

            // ī�޶��� ������ ���� (��� �÷��̾��� ī�޶� (0, 1, 0) ������ �ٶ󺸰� ����)
            playerCamera.transform.LookAt(new Vector3(0, 1, 0));
        }
        else
        {
            // �ٸ� �÷��̾��� ī�޶� ��Ȱ��ȭ
            playerCamera.SetActive(false);
        }
    }
}
