using TMPro;
using UnityEngine;
using Photon.Pun;

public class UI_Timer : MonoBehaviourPunCallbacks
{
    private TMP_Text _timerText; // UI 에 표시될 Text.
    private float _time = 45f; // 시간.
    private float _maxTime = 45f; // 최대시간.
    GameOverManager _gameOverManager;
    private void Awake()
    {
        _timerText = transform.Find("Text (TMP) - Timer").GetComponent<TMP_Text>();
        _gameOverManager = FindObjectOfType<GameOverManager>();
    }

    private void Update()
    {
        UiTimerUpdate();
    }

    /// <summary>
    /// 타이머 로직
    /// </summary>
    public void UiTimerUpdate()
    {
        _timerText.text = $"Time = {_time:F2}";
        _time -= Time.deltaTime;
        _time = Mathf.Max(0, _time); // 시간이 음수가 되지 않도록 보정.
        //시간 초가 지나간다면 패배 판넬 켬
        //if (_gameOverManager != null)
        //{
        //    if (_time == 0f)
        //    {
        //        _gameOverManager.DisplayLose();
        //    }
        //}
    }



    /// <summary>
    /// 중간보스처치, 상점에서 시간구매, 보스기믹 등 시간+ or 시간- 상황일때 사용하는 함수.
    /// </summary>
    /// <param name="plusMinusTime"> 증가 혹은 감소될 시간. </param>
    [PunRPC]
    public void TimePlusMinus(float plusMinusTime)
    {
        // 시간 추가 감소
        _time += plusMinusTime;

        // 45초 이상 못올라가게 만들기
        if (_time >= _maxTime) _time = _maxTime;
    }
}
