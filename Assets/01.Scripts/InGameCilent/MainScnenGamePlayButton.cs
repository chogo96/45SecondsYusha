using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainScnenGamePlayButton : MonoBehaviour
    {
    private Button _gamestart;
    private Button _gameQuit;
    private void Awake()
    {
        _gamestart = transform.Find("BackGround/PlayButton/PlayButton").GetComponent<Button>();
        _gameQuit = transform.Find("BackGround/QuitButton/QuitButton").GetComponent<Button>();
    }

    private void Start()
    {
        _gamestart.onClick.AddListener(GoToLobby);
        _gameQuit.onClick.AddListener(QuitGame);
    }

    public void GoToLobby()
    {
        SceneManager.LoadScene("02.Lobby Scene");
    }
    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }
}

