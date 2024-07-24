using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using UnityEditor;
using UnityEngine;

public class GoogleSheetsToCardAsset : MonoBehaviour
{
    static readonly string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
    static readonly string ApplicationName = "TeamFullLeaf";
    static readonly string SpreadsheetId = "1y2MHqFZtHYUSrzjdj9Kb86w0CDpF1rraiYgcKS-av8o";
    static readonly string sheet = "Sheet1";
    static SheetsService service;

    public List<CardAsset> cardAssets = new List<CardAsset>();

    public void Initialize()
    {
        GoogleCredential credential;

        string credentialsPath = Path.Combine(Application.dataPath, "Resources/teamfullleafcard-2fbd6d7473cb.json");
        Debug.Log("Loading credentials from: " + credentialsPath);

        using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
        }

        Debug.Log("Credentials loaded successfully.");

        service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });

        Debug.Log("Sheets service initialized.");
        StartCoroutine(ReadEntries());
    }

    IEnumerator ReadEntries()
    {
        Debug.Log("Requesting data from Google Sheets...");
        var range = $"{sheet}!A2:Y65"; // 데이터 범위 설정
        SpreadsheetsResource.ValuesResource.GetRequest request =
                service.Spreadsheets.Values.Get(SpreadsheetId, range);

        ValueRange response = request.Execute();
        IList<IList<object>> values = response.Values;

        if (values != null && values.Count > 0)
        {
            Debug.Log($"Values found: {values.Count}");
            foreach (var row in values)
            {
                // 데이터가 충분하지 않으면 건너뜀
                if (row.Count < 25) // 필요한 최소 열 개수
                {
                    Debug.LogWarning("Skipping row due to insufficient data: " + row.Count);
                    continue;
                }

                try
                {
                    CardAsset card = ScriptableObject.CreateInstance<CardAsset>();

                    card.CardScriptName = GetValue(row, 0);
                    card.Tags = GetValue(row, 1);
                    card.Rarity = ParseEnum<RarityOptions>(GetValue(row, 2));
                    card.TokenCard = ParseBool(GetValue(row, 4));
                    card.LimitOfThisCardInDeck = ParseInt(GetValue(row, 24));
                    card.IsVanishCard = ParseBool(GetValue(row, 21));
                    card.TypeOfCard = ParseEnum<TypesOfCards>(GetValue(row, 3));
                    card.SwordAttack = ParseInt(GetValue(row, 5));
                    card.MagicAttack = ParseInt(GetValue(row, 7));
                    card.ShieldAttack = ParseInt(GetValue(row, 8));
                    card.RandomAttack = ParseInt(GetValue(row, 9));
                    card.Description = GetValue(row, 23);
                    card.Targets = ParseEnum<TargetingOptions>(GetValue(row, 19));

                    // 이미지 파일 이름을 가져와서 Resources 폴더에서 로드
                    string imageName = GetValue(row, 13);
                    card.CardImage = Resources.Load<Sprite>($"CardImages/{imageName}");

                    if (card.CardImage == null)
                    {
                        Debug.LogWarning($"Image not found: {imageName}");
                    }
                    // 캐릭터 에셋 로드
                    string characterType = GetValue(row, 1);
                    card.CharacterAsset = GetCharacterAsset(characterType);

                    if (card.CharacterAsset == null)
                    {
                        card.CharacterAsset = null;
                    }
                    cardAssets.Add(card);

                    // ScriptableObject로 저장
                    SaveCardAsset(card);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Error processing row: " + ex.Message);
                }
            }

            // 카드 리스트를 디버그 출력
            foreach (var card in cardAssets)
            {
                Debug.Log($"{card.CardScriptName}, {card.Description}, {card.Rarity}");
            }
        }
        else
        {
            Debug.Log("No data found.");
        }

        yield return null;
    }

    string GetValue(IList<object> row, int index)
    {
        if (index >= row.Count)
        {
            Debug.LogWarning($"Index {index} is out of range for row with {row.Count} columns.");
            return string.Empty;
        }
        return row[index]?.ToString() ?? string.Empty;
    }

    bool ParseBool(string value)
    {
        if (string.IsNullOrEmpty(value)) return false;

        string strValue = value.ToLower();
        if (strValue == "true" || strValue == "yes" || strValue == "1")
            return true;
        if (strValue == "false" || strValue == "no" || strValue == "0")
            return false;

        Debug.LogWarning($"Unable to parse bool value: {value}");
        return false;
    }

    int ParseInt(string value)
    {
        int result;
        if (int.TryParse(value, out result))
        {
            return result;
        }
        else
        {
            Debug.LogWarning($"Unable to parse int value: {value}");
            return 0;
        }
    }

    T ParseEnum<T>(string value) where T : struct
    {
        T result;
        if (Enum.TryParse(value, true, out result))
        {
            return result;
        }
        else
        {
            Debug.LogWarning($"Unable to parse enum value: {value}");
            return default(T);
        }
    }
    CharacterAsset GetCharacterAsset(string characterType)
    {
        switch (characterType.ToLower())
        {
            case "attacker":
                return Resources.Load<CharacterAsset>("GameAssets/Character/AttackerClass");
            case "buffer":
                return Resources.Load<CharacterAsset>("GameAssets/Character/BufferClass");
            case "healer":
                return Resources.Load<CharacterAsset>("GameAssets/Character/HealerClass");
            case "tanker":
                return Resources.Load<CharacterAsset>("GameAssets/Character/TankerClass");
            case "neutral":
                return null;
            default:
                Debug.Log($"Unknown character type:");
                return null;
        }
    }
    void SaveCardAsset(CardAsset card)
    {
        string path = "Assets/CardAssets"; // 저장할 경로
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath($"{path}/{card.CardScriptName}.asset");

        AssetDatabase.CreateAsset(card, assetPathAndName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
