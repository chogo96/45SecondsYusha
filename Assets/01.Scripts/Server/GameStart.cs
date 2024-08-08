using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    private Button[] _startSceneButton;

    private void Awake()
    {
        _startSceneButton = new Button[4];
        // 버튼 찾기 및 배열에 추가
        for (int i = 0; i < _startSceneButton.Length; i++)
        {
            string buttonName = $"Button - {i + 1}";
            _startSceneButton[i] = transform.Find(buttonName).GetComponent<Button>();

            int index = i;
            _startSceneButton[i].onClick.AddListener(() => OnClickStartSceneButton(index));
        }
    }

    /// <summary>
    /// 스타트씬 버튼 클릭 이벤트 함수
    /// </summary>
    /// <param name="index">인덱스0 = 로비, 1 = 상점, 2 = 내카드, 3 = 게임종료</param>
    void OnClickStartSceneButton(int index)
    {
        switch (index)
        {
            case 0:
                SceneManager.LoadScene("02.Lobby Scene");
                break;
            case 1:
                SceneManager.LoadScene("02.Store Scene");
                break;
            case 2:
                SceneManager.LoadScene("03.Card Scene");
                break;
            case 3:
                Application.Quit();
                break;
        }
    }
}
