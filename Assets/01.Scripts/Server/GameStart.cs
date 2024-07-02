using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    private Button _startButton;

    private void Awake()
    {
        _startButton = transform.Find("Button").GetComponent<Button>();
    }

    void Start()
    {
        _startButton.onClick.AddListener(OnClickStartButton);
    }

    void OnClickStartButton()
    {
        SceneManager.LoadScene("04.Lobby Scene");
    }
}
