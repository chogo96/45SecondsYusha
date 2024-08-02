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
    public float displayTime = 5.0f; // 패널이 보이는 시간
    private Button returnToLobbyButton;
    private void Awake()
    {
        //Todo : 버튼 제작 후 주석 해제 해서 사용하세요
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
            PhotonNetwork.LoadLevel("04.Lobby Scene"); // 로비 씬의 이름으로 변경
        }
        else
        {
            returnToLobbyButton.interactable = false;
        }
    }
}
