using Photon.Pun;
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
        returnToLobbyButton = GameObject.Find("Button - ReturnLobby").GetComponent<Button>();   
    }
    void Start()
    {
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        returnToLobbyButton.onClick.AddListener(ReturnToLobby);
        returnToLobbyButton.gameObject.SetActive(false);
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
        returnToLobbyButton.gameObject.SetActive(true);
        yield return new WaitForSeconds(displayTime);
    }

    private void ReturnToLobby()
    {

        if (PhotonNetwork.IsMasterClient)
        {
            returnToLobbyButton.interactable = true;
            PhotonNetwork.LoadLevel("04.Lobby Scene"); // �κ� ���� �̸����� ����
        }
        else
        {
            returnToLobbyButton.interactable = false;
        }
    }
}
