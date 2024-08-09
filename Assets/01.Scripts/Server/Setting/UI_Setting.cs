using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UI_Setting : MonoBehaviour
{
    private SoundManager soundManager;
    private Scrollbar _bgmSoundBar;
    private Scrollbar _sfxSoundBar;
    private GameObject _settingPanel;

    private Button _setting;
    private Button _cancel;
    private Button _save;

    private string _bgmSaveKey = "BGMScrollbarValue";
    private string _sfxSaveKey = "SFXScrollbarValue";

    private float _currentBGMValue; // 현재 BGM 스크롤바에 따른 값
    private float _currentSFXValue; // 현재 SFX 스크롤바에 따른 값

    private float _savedBGMValue;
    private float _savedSFXValue;

    // 해상도 및 전체화면 관련 변수
    private TMP_Dropdown _resolutionDropdown; // 해상도 드롭다운
    private Toggle _fullscreenToggle; // 전체화면 선택 토글

    private Resolution[] _resolutions; // 사용 가능한 해상도 목록
    private bool _isFullscreen; // 현재 전체화면 상태
    private string _resolutionSaveKey = "ResolutionIndex";
    private string _fullscreenSaveKey = "Fullscreen";

    private int _savedResolutionIndex; // 현재 해상도 인덱스
    private bool _savedFullscreenState; // 현재 전체화면 상태

    public GameObject SettingPanel // 프로퍼티
    {
        get { return _settingPanel; }
        private set { _settingPanel = value; } // 외부에서 수정하지 못하도록 private set 사용
    }

    private void Awake()
    {
        soundManager = FindObjectOfType<SoundManager>();

        _bgmSoundBar = transform.Find("Panel - SettingPopUp/Sound/Scrollbar - BGMSound").GetComponent<Scrollbar>();
        _sfxSoundBar = transform.Find("Panel - SettingPopUp/Sound/Scrollbar - SFXSound").GetComponent<Scrollbar>();
        _cancel = transform.Find("Panel - SettingPopUp/Buttons/Button - Cancel").GetComponent<Button>();
        _save = transform.Find("Panel - SettingPopUp/Buttons/Button - Save").GetComponent<Button>();
        _setting = transform.Find("Button - Setting").GetComponent<Button>();
        _settingPanel = transform.Find("Panel - SettingPopUp").gameObject;
        _resolutionDropdown = transform.Find("Panel - SettingPopUp/Resolution/Dropdown - Resolution").GetComponent<TMP_Dropdown>();
        _fullscreenToggle = transform.Find("Panel - SettingPopUp/Resolution/Toggle - FullScreen").GetComponent<Toggle>();
    }

    private void Start()
    {
        _setting.onClick.AddListener(OnCLickSettingButton);
        _cancel.onClick.AddListener(() => OnClickUiPopUpButtons(1));
        _save.onClick.AddListener(() => OnClickUiPopUpButtons(0));
        _settingPanel.SetActive(false);

        _savedBGMValue = PlayerPrefs.GetFloat(_bgmSaveKey, 0.5f);
        _savedSFXValue = PlayerPrefs.GetFloat(_sfxSaveKey, 0.5f);

        _bgmSoundBar.value = _savedBGMValue;
        _sfxSoundBar.value = _savedSFXValue;

        // 해상도 초기화
        InitializeResolutionSettings();
    }

    private void InitializeResolutionSettings()
    {
        // 사용 가능한 해상도 리스트 가져오기
        _resolutions = Screen.resolutions;

        // 드롭다운 초기화
        _resolutionDropdown.ClearOptions();

        // 드롭다운에 표시할 해상도 옵션 리스트 만들기
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        int defaultResolutionIndex = -1; // FHD 해상도의 인덱스를 저장할 변수

        for (int i = 0; i < _resolutions.Length; i++)
        {
            string option = _resolutions[i].width + " x " + _resolutions[i].height;
            options.Add(option);

            // 현재 해상도 찾기
            if (_resolutions[i].width == Screen.currentResolution.width &&
                _resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }

            // FHD (1920x1080) 해상도 찾기
            if (_resolutions[i].width == 1920 && _resolutions[i].height == 1080)
            {
                defaultResolutionIndex = i;
            }
        }

        _resolutionDropdown.AddOptions(options);

        // 저장된 해상도와 전체화면 설정 불러오기
        _savedResolutionIndex = PlayerPrefs.GetInt(_resolutionSaveKey, defaultResolutionIndex != -1 ? defaultResolutionIndex : currentResolutionIndex);
        _savedFullscreenState = PlayerPrefs.GetInt(_fullscreenSaveKey, 1) == 1;

        // 해상도와 전체화면 설정 적용
        _resolutionDropdown.value = _savedResolutionIndex;
        _fullscreenToggle.isOn = _savedFullscreenState;
        SetResolution(_savedResolutionIndex);
        SetFullscreen(_savedFullscreenState);

        // 이벤트 리스너 추가
        _resolutionDropdown.onValueChanged.AddListener(delegate { SetResolution(_resolutionDropdown.value); });
        _fullscreenToggle.onValueChanged.AddListener(delegate { SetFullscreen(_fullscreenToggle.isOn); });
    }

    private void SetResolution(int resolutionIndex)
    {
        // 선택한 해상도를 적용
        Resolution resolution = _resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, _isFullscreen);
    }

    private void SetFullscreen(bool fullscreen)
    {
        _isFullscreen = fullscreen;
        Resolution resolution = _resolutions[_resolutionDropdown.value];
        Screen.SetResolution(resolution.width, resolution.height, fullscreen);
    }

    public void OnCLickSettingButton()
    {
        _settingPanel.SetActive(true);
    }

    public void OnClickUiPopUpButtons(int num)
    {
        switch (num)
        {
            case 0: // 저장 버튼
                // BGM 사운드 조절 내용
                _currentBGMValue = Mathf.Lerp(0f, 1f, _bgmSoundBar.value);
                soundManager.bgmVolume = _currentBGMValue;
                PlayerPrefs.SetFloat(_bgmSaveKey, _bgmSoundBar.value);

                // SFX 사운드 조절 내용
                _currentSFXValue = Mathf.Lerp(0f, 1f, _sfxSoundBar.value);
                soundManager.sfxVolume = _currentSFXValue;
                PlayerPrefs.SetFloat(_sfxSaveKey, _sfxSoundBar.value);

                // 해상도 및 전체화면 설정 저장
                PlayerPrefs.SetInt(_resolutionSaveKey, _resolutionDropdown.value);
                PlayerPrefs.SetInt(_fullscreenSaveKey, _fullscreenToggle.isOn ? 1 : 0);
                PlayerPrefs.Save();

                _settingPanel.SetActive(false);
                break;

            case 1: // 취소 버튼
                // 설정을 저장하지 않고 이전 상태로 되돌림
                _bgmSoundBar.value = _savedBGMValue;
                _sfxSoundBar.value = _savedSFXValue;

                // 해상도 및 전체화면 설정 되돌리기
                _resolutionDropdown.value = _savedResolutionIndex;
                _fullscreenToggle.isOn = _savedFullscreenState;
                SetResolution(_savedResolutionIndex);
                SetFullscreen(_savedFullscreenState);

                _settingPanel.SetActive(false);
                break;
        }
    }
}