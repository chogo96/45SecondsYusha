using UnityEngine;
public class Utils
{
    public static void Log(object message)
    {
#if UNITY_EDITOR
        Debug.Log(message);
#endif
    }

    public static void LogRed(object message)
    {
#if UNITY_EDITOR
        Debug.Log($"<color=red>{message}</color>");
#endif
    }

    public static void LogGreen(object message)
    {
#if UNITY_EDITOR
        Debug.Log($"<color=green>{message}</color>");
#endif
    }
}