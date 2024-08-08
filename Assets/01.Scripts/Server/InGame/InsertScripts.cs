using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Linq; // LINQ를 사용하기 위해 추가

public class InsertScripts : MonoBehaviour
{
    public static event Action OnScriptsInserted;

    private void Awake()
    {
        GameManager.AllPlayersSpawned += InsetScriptForNickNameObject;
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        GameManager.AllPlayersSpawned -= InsetScriptForNickNameObject;
    }

    void InsetScriptForNickNameObject()
    {
        // 추가할 스크립트 타입을 지정합니다.
        System.Type scriptType = typeof(PlayerScripts); // 여기에 원하는 스크립트 타입을 지정합니다.

        // PhotonNetwork.PlayerList를 사용하여 모든 플레이어를 탐색합니다.
        foreach (var player in PhotonNetwork.PlayerList)
        {
            // 각 플레이어의 닉네임을 사용하여 게임 오브젝트를 찾습니다.
            GameObject playerObject = GameObject.Find(player.NickName);
            if (playerObject != null)
            {
                // 플레이어 오브젝트에 스크립트 추가
                playerObject.AddComponent(scriptType);
            }

            PlayerScripts playerScripts = playerObject.GetComponent<PlayerScripts>();
            if (player.IsLocal)
            {
                GlobalSettings.instance.AssignLowPlayer(playerScripts);
            }
        }

        // 모든 작업이 완료되었음을 알리는 이벤트 호출
        OnScriptsInserted?.Invoke();
    }
}
