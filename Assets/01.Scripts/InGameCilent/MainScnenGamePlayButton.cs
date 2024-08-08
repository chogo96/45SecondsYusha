using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainScnenGamePlayButton : MonoBehaviour
    {
    private Button _gamestart;
    private void Awake()
    {
        _gamestart = transform.Find("BackGround/PlayButton/PlayButton").GetComponent<Button>();
    }

    private void Start()
    {
        _gamestart.onClick.AddListener(GoToLobby);
    }

    public void GoToLobby()
        {
            SceneManager.LoadScene("02.Lobby Scene");
        }
    }

