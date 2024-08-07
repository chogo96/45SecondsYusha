using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;  // �̱��� �ν��Ͻ�

    [Header("BGM Settings")]
    private Dictionary<string, AudioClip> bgmClips; // BGM Ŭ���� ������ ����
    [Range(0f, 1f)] public float bgmVolume = 1f;  // BGM ���� ����
    private AudioSource bgmPlayer;

    [Header("SFX Settings")]
    public AudioClip[] sfxClips;
    [Range(0f, 1f)] public float sfxVolume = 1f;  // SFX ���� ����
    public int channels = 5;
    private AudioSource[] sfxPlayers;
    private int channelIndex;

    public enum Sfx { DeckShuffle,CardGrab, CardUse,CardDraw, ButtonActivate , CardPackOpen , EnemyDown}

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // �� ���� �� ������Ʈ ����
            Init();
            SceneManager.sceneLoaded += OnSceneLoaded;  // �� �ε� �� �̺�Ʈ ����
        }
        else
        {
            Destroy(gameObject);  // �ߺ� �ν��Ͻ� ����
        }
    }

    private void Init()
    {
        // BGM Ŭ���� �ʱ�ȭ
        bgmClips = new Dictionary<string, AudioClip>
        {
            { "01.MainScene", Resources.Load<AudioClip>("Sounds/MainLobby") },
            { "02.Lobby Scene", Resources.Load<AudioClip>("Sounds/MainLobby") },
            { "03.GamePlay Scene", Resources.Load<AudioClip>("Sounds/�ΰ��Ӻ�Ʈbgm") },
            { "TimeRunningOut", Resources.Load<AudioClip>("Sounds/TimeRunningOut") }, // �ð� ���� ��Ȳ�� �´� BGM �߰�
            { "Win", Resources.Load<AudioClip>("Sounds/���� �¸� ����") } ,
            { "Lose", Resources.Load<AudioClip>("Sounds/���� �й� ����") } ,
        };

        // BGM �÷��̾� �ʱ�ȭ
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;

        // ȿ���� �÷��̾� �ʱ�ȭ
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];

        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].volume = sfxVolume;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ���� �ε�� ������ ȣ��Ǵ� �޼���
        ChangeBgm(scene.name);
    }

    public void ChangeBgm(string situation)
    {
        if (bgmClips.ContainsKey(situation))
        {
            bgmPlayer.clip = bgmClips[situation];
            PlayBgm(true);
        }
        else
        {
            Debug.LogWarning($"Situation '{situation}'�� �ش��ϴ� BGM�� �����ϴ�.");
        }
    }

    public void PlayBgm(bool isPlay)
    {
        if (bgmPlayer.clip == null) return;  // BGM Ŭ���� ���� ��� ������ġ
        if (isPlay)
        {
            bgmPlayer.Play();
        }
        else
        {
            bgmPlayer.Stop();
        }
    }

    public void PlayEffectBgm(bool isPlay)
    {
        // ���⿡ BGM ȿ�� ó���� �߰��� �� �ֽ��ϴ�.
    }

    public void PlaySfx(Sfx sfx)
    {
        if (sfxClips.Length == 0) return;  // SFX Ŭ���� ���� ��� ������ġ

        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            int loopIndex = (index + channelIndex) % sfxPlayers.Length;  // ��ⷯ �������� �ε��� ��ȯ

            if (sfxPlayers[loopIndex].isPlaying)
            {
                continue;
            }

            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            sfxPlayers[loopIndex].Play();
            break;
        }
    }

    public void SetBgmVolume(float volume)
    {
        bgmVolume = Mathf.Clamp(volume, 0f, 1f);
        bgmPlayer.volume = bgmVolume;
    }

    public void SetSfxVolume(float volume)
    {
        sfxVolume = Mathf.Clamp(volume, 0f, 1f);
        foreach (var player in sfxPlayers)
        {
            player.volume = sfxVolume;
        }
    }

    // Ư�� ��Ȳ���� BGM �����ϱ� ���� �޼��� �߰�
    public void ChangeBgmForSituation(string situation)
    {
        if (bgmClips.ContainsKey(situation))
        {
            if (bgmPlayer.clip != bgmClips[situation])
            {
                bgmPlayer.clip = bgmClips[situation];
                bgmPlayer.Play();
            }
        }
        else
        {
            Debug.LogWarning($"��Ȳ '{situation}'�� �´� BGM�� �����ϴ�.");
        }
    }
}

