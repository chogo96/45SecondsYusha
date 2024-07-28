using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    public GameObject winPanel;
    public GameObject losePanel;
    public float displayTime = 5.0f; // 패널이 보이는 시간
    public Button returnToLobbyButton;

    void Start()
    {
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        returnToLobbyButton.onClick.AddListener(ReturnToLobby);
    }

    public void DisplayWin()
    {
        StartCoroutine(ShowPanel(winPanel));
    }

    public void DisplayLose()
    {
        StartCoroutine(ShowPanel(losePanel));
    }

    private IEnumerator ShowPanel(GameObject panel)
    {
        panel.SetActive(true);
        yield return new WaitForSeconds(displayTime);
        panel.SetActive(false);
        returnToLobbyButton.gameObject.SetActive(true);
    }

    private void ReturnToLobby()
    {
        SceneManager.LoadScene("LobbyScene"); // 로비 씬의 이름으로 변경
    }
}
