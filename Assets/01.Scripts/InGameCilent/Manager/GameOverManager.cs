using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    public GameObject winPanel;
    public GameObject losePanel;
    public float displayTime = 5.0f; // �г��� ���̴� �ð�
    private Button returnToLobbyButton;
    private void Awake()
    {
        //Todo : ��ư ���� �� �ּ� ���� �ؼ� ����ϼ���
        //returnToLobbyButton = GameObject.Find("Button - ReturnLobby").GetComponent<Button>();   
    }
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
        SceneManager.LoadScene("04.Lobby Scene"); // �κ� ���� �̸����� ����
    }
}
