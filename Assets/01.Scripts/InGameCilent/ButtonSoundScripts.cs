using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonSoundManager : MonoBehaviour
{
    public static ButtonSoundManager instance;
    public SoundManager.Sfx ButtonActivate = SoundManager.Sfx.ButtonActivate; // ����� ��ư Ŭ�� ����

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // ���� ����Ǿ �� ������Ʈ ����
            SceneManager.sceneLoaded += OnSceneLoaded; // �� �ε� �̺�Ʈ ����
            AddSoundToAllButtonsInScene(); // ù ��° ���� ��ư�� ���� �߰�
        }
        else
        {
            Destroy(gameObject); // �ߺ� ����: �̹� �ν��Ͻ��� �����ϸ� ���� ������ ������Ʈ�� �ı�
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // �̺�Ʈ ���� ����
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AddSoundToAllButtonsInScene(); // �� �ε� �� ��ư�� ���� �߰�
    }

    private void AddSoundToAllButtonsInScene()
    {
        // ���� ���� ��� ��ư ã��
        Button[] buttons = FindObjectsOfType<Button>();

        foreach (Button button in buttons)
        {
            // �̹� �����ʰ� �߰��Ǿ� �ִ��� Ȯ���ϰ� �ߺ� �߰� ����
            button.onClick.RemoveListener(PlayButtonSound);
            button.onClick.AddListener(PlayButtonSound);
        }
    }

    private void PlayButtonSound()
    {
        // ���� �Ŵ����� ���� ȿ���� ���
        SoundManager.instance.PlaySfx(ButtonActivate);
    }
}