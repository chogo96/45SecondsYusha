using UnityEngine;
using UnityEditor;
static class CardUnityIntegration
{
    [MenuItem("Asset/Create/CardAsset")]
    public static void CreateCardScriptableObject() => ScriptableObjectUtility.CreateAsset<CardAsset>();

    [MenuItem("Asset/Create/CharacterAsset")]
    public static void CreateCharacterScriptableObject() => ScriptableObjectUtility.CreateAsset<CharacterAsset>();

    [MenuItem("Asset/Create/EnemyAsset")]
    public static void CreateEnemyScriptableObjects() => ScriptableObjectUtility.CreateAsset<EnemyData>();
}