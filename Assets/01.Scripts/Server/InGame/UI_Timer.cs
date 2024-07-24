using TMPro;
using UnityEngine;
using Photon.Pun;

public class UI_Timer : MonoBehaviour
{
    private TMP_Text _timerText; // UI �� ǥ�õ� Text.
    private float _time = 45f; // �ð�.
    private float _maxTime = 45f; // �ִ�ð�.

    private void Awake()
    {
        _timerText = transform.Find("Text (TMP) - Timer").GetComponent<TMP_Text>();
    }

    private void Update()
    {
        UiTimerUpdate();
    }

    /// <summary>
    /// Ÿ�̸� ����
    /// </summary>
    public void UiTimerUpdate()
    {
        _timerText.text = $"Time = {_time:F2}";
        _time -= Time.deltaTime;
        _time = Mathf.Max(0, _time); // �ð��� ������ ���� �ʵ��� ����.
    }



    /// <summary>
    /// �߰�����óġ, �������� �ð�����, ������� �� �ð�+ or �ð�- ��Ȳ�϶� ����ϴ� �Լ�.
    /// </summary>
    /// <param name="plusMinusTime"> ���� Ȥ�� ���ҵ� �ð�. </param>
    [PunRPC]
    public void TimePlusMinus(float plusMinusTime)
    {
        // �ð� �߰� ����
        _time += plusMinusTime;

        // 45�� �̻� ���ö󰡰� �����
        if (_time >= _maxTime) _time = _maxTime;
    }
}
