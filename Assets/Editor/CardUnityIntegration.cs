using UnityEngine;
using UnityEditor;

static class CardUnityIntegration
{
    [MenuItem("Assets/Create/CardAsset")]
    public static void CreateCardScriptableObject() => ScriptableObjectUtility.CreateAsset<CardAsset>();

    [MenuItem("Assets/Create/CharacterAsset")]
    public static void CreateCharacterScriptableObject() => ScriptableObjectUtility.CreateAsset<CharacterAsset>();

    [MenuItem("Assets/Create/EnemyAsset")]
    public static void CreateEnemyScriptableObjects() => ScriptableObjectUtility.CreateAsset<EnemyData>();

    [MenuItem("Tools/Import Card Data From Google Sheets")]
    public static void ImportCardDataFromGoogleSheets()
    {
        GoogleSheetsToCardAsset importer = new GameObject("GoogleSheetsImporter").AddComponent<GoogleSheetsToCardAsset>();
        importer.Initialize();
    }
}
