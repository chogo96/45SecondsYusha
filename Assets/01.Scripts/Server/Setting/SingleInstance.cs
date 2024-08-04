using UnityEngine;
using System.Diagnostics;
using System.Linq;

public class SingleInstance : SingletonMonoBase<SingleInstance>
{
    protected override void Awake()
    {
        base.Awake();  // 반드시 호출되어야 함

        if (instance != this)
        {
            return;
        }

        string processName = Process.GetCurrentProcess().ProcessName;
        Process[] processes = Process.GetProcessesByName(processName);

        if (processes.Length > 1)
        {
            // 이미 실행 중인 인스턴스가 있음
            UnityEngine.Debug.Log("Application is already running.");
            Application.Quit();
        }
    }
}
