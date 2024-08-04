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
        // ��ư ã�� �� �迭�� �߰�
        for (int i = 0; i < _startSceneButton.Length; i++)
        {
            string buttonName = $"Button - {i + 1}";
            _startSceneButton[i] = transform.Find(buttonName).GetComponent<Button>();

            int index = i;
            _startSceneButton[i].onClick.AddListener(() => OnClickStartSceneButton(index));
        }
    }

    /// <summary>
    /// ��ŸƮ�� ��ư Ŭ�� �̺�Ʈ �Լ�
    /// </summary>
    /// <param name="index">�ε���0 = �κ�, 1 = ����, 2 = ��ī��, 3 = ��������</param>
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
