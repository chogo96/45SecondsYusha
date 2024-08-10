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
        DisableAllCardInteractions(); // ī�� ��ȣ�ۿ� ��Ȱ��ȭ
        StartCoroutine(ShowPanel(winPanel));
        SoundManager.instance.ChangeBgm("Win");
    }

    public void DisplayLose()
    {
        DisableAllCardInteractions(); // ī�� ��ȣ�ۿ� ��Ȱ��ȭ
        StartCoroutine(ShowPanel(losePanel));
        SoundManager.instance.ChangeBgm("Lose");
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
            PhotonNetwork.LoadLevel("02.Lobby Scene"); // �κ� ���� �̸����� ����
        }
        else
        {
            returnToLobbyButton.interactable = false;
        }
    }

    private void DisableAllCardInteractions()
    {
        // ��� CardDraggable�� ���� ������Ʈ�� BoxCollider ��Ȱ��ȭ
        CardDraggable[] cards = FindObjectsOfType<CardDraggable>();
        foreach (CardDraggable card in cards)
        {
            BoxCollider cardCollider = card.GetComponent<BoxCollider>();
            if (cardCollider != null)
            {
                cardCollider.enabled = false;  // BoxCollider ��Ȱ��ȭ
            }
        }
    }
}
