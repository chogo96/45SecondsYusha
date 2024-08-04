using UnityEngine;
using System.Diagnostics;
using System.Linq;

public class SingleInstance : SingletonMonoBase<SingleInstance>
{
    protected override void Awake()
    {
        base.Awake();  // �ݵ�� ȣ��Ǿ�� ��

        if (instance != this)
        {
            return;
        }

        string processName = Process.GetCurrentProcess().ProcessName;
        Process[] processes = Process.GetProcessesByName(processName);

        if (processes.Length > 1)
        {
            // �̹� ���� ���� �ν��Ͻ��� ����
            UnityEngine.Debug.Log("Application is already running.");
            Application.Quit();
        }
    }
}
