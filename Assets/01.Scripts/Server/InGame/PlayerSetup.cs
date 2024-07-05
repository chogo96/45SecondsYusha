using Photon.Pun;
using UnityEngine;

public class PlayerSetup : MonoBehaviourPun
{
    private GameObject playerCamera;

    private void Awake()
    {
        // 정확한 경로를 사용하여 카메라를 찾습니다.
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
            // 자신의 플레이어에만 카메라 활성화
            playerCamera.SetActive(true);

            // 카메라의 방향을 설정 (모든 플레이어의 카메라가 (0, 1, 0) 방향을 바라보게 설정)
            playerCamera.transform.LookAt(new Vector3(0, 1, 0));
        }
        else
        {
            // 다른 플레이어의 카메라 비활성화
            playerCamera.SetActive(false);
        }
    }
}
