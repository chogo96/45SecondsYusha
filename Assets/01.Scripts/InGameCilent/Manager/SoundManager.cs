using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;  // 싱글턴 인스턴스

    [Header("BGM Settings")]
    private Dictionary<string, AudioClip> bgmClips; // BGM 클립을 관리할 사전
    [Range(0f, 1f)] public float bgmVolume = 1f;  // BGM 볼륨 조정
    private AudioSource bgmPlayer;

    [Header("SFX Settings")]
    public AudioClip[] sfxClips;
    [Range(0f, 1f)] public float sfxVolume = 1f;  // SFX 볼륨 조정
    public int channels = 5;
    private AudioSource[] sfxPlayers;
    private int channelIndex;

    public enum Sfx { DeckShuffle,CardGrab, CardUse,CardDraw, ButtonActivate , CardPackOpen , EnemyDown}

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // 씬 변경 시 오브젝트 유지
            Init();
            SceneManager.sceneLoaded += OnSceneLoaded;  // 씬 로드 시 이벤트 구독
        }
        else
        {
            Destroy(gameObject);  // 중복 인스턴스 방지
        }
    }

    private void Init()
    {
        // BGM 클립을 초기화
        bgmClips = new Dictionary<string, AudioClip>
        {
            { "01.MainScene", Resources.Load<AudioClip>("Sounds/MainLobby") },
            { "02.Lobby Scene", Resources.Load<AudioClip>("Sounds/MainLobby") },
            { "03.GamePlay Scene", Resources.Load<AudioClip>("Sounds/인게임비트bgm") },
            { "TimeRunningOut", Resources.Load<AudioClip>("Sounds/TimeRunningOut") }, // 시간 부족 상황에 맞는 BGM 추가
            { "Win", Resources.Load<AudioClip>("Sounds/게임 승리 사운드") } ,
            { "Lose", Resources.Load<AudioClip>("Sounds/게임 패배 사운드") } ,
        };

        // BGM 플레이어 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;

        // 효과음 플레이어 초기화
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
        // 씬이 로드될 때마다 호출되는 메서드
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
            Debug.LogWarning($"Situation '{situation}'에 해당하는 BGM이 없습니다.");
        }
    }

    public void PlayBgm(bool isPlay)
    {
        if (bgmPlayer.clip == null) return;  // BGM 클립이 없을 경우 안전장치
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
        // 여기에 BGM 효과 처리를 추가할 수 있습니다.
    }

    public void PlaySfx(Sfx sfx)
    {
        if (sfxClips.Length == 0) return;  // SFX 클립이 없을 경우 안전장치

        for (int index = 0; index < sfxPlayers.Length; index++)
        {
            int loopIndex = (index + channelIndex) % sfxPlayers.Length;  // 모듈러 연산으로 인덱스 순환

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

    // 특정 상황에서 BGM 변경하기 위한 메서드 추가
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
            Debug.LogWarning($"상황 '{situation}'에 맞는 BGM이 없습니다.");
        }
    }
}

