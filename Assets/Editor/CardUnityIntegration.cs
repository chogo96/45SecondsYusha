using UnityEngine;
using UnityEditor;
static class CardUnityIntegration
{
    [MenuItem("Asset/Create/CardAsset")]
    public static void CreateScriptableObject() => ScriptableObjectUtility.CreateAsset<CardAsset>();
}