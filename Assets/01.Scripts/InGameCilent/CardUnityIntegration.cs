using UnityEngine;
using UnityEditor;
static class CardUnityIntegration
{
    [MenuItem("Asset/Create/CardAsset")]
    public static void CreateScriptableObject() => ScriptableObjectUtility.CreateAsset<CardAsset>();

    [MenuItem("Asset/Create/CharacterAsset")]
    public static void CreateScrptableObject() => ScriptableObjectUtility.CreateAsset<CharacterAsset>();
}