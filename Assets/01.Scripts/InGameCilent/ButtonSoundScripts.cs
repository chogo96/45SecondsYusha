using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonSoundManager : MonoBehaviour
{
    public static ButtonSoundManager instance;
    public SoundManager.Sfx ButtonActivate = SoundManager.Sfx.ButtonActivate; // 사용할 버튼 클릭 사운드

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 변경되어도 이 오브젝트 유지
            SceneManager.sceneLoaded += OnSceneLoaded; // 씬 로드 이벤트 구독
            AddSoundToAllButtonsInScene(); // 첫 번째 씬의 버튼에 사운드 추가
        }
        else
        {
            Destroy(gameObject); // 중복 방지: 이미 인스턴스가 존재하면 새로 생성된 오브젝트를 파괴
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // 이벤트 구독 해제
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AddSoundToAllButtonsInScene(); // 씬 로드 시 버튼에 사운드 추가
    }

    private void AddSoundToAllButtonsInScene()
    {
        // 현재 씬의 모든 버튼 찾기
        Button[] buttons = FindObjectsOfType<Button>();

        foreach (Button button in buttons)
        {
            // 이미 리스너가 추가되어 있는지 확인하고 중복 추가 방지
            button.onClick.RemoveListener(PlayButtonSound);
            button.onClick.AddListener(PlayButtonSound);
        }
    }

    private void PlayButtonSound()
    {
        // 사운드 매니저를 통해 효과음 재생
        SoundManager.instance.PlaySfx(ButtonActivate);
    }
}