using System;
using System.Collections.Generic;
using UnityEngine;

public enum Language { RU, EN }

public static class DictorSpeachManager
{
    public static Dictionary<string, List<SpinDictorSpeech>> Speeches = new Dictionary<string, List<SpinDictorSpeech>>();
    public static Dictionary<string, List<SpinVictorinQuest>> VictotinData = new Dictionary<string, List<SpinVictorinQuest>>();

    public const string fileSpeechName = "SpinText";
    public const string fileVictorinName = "SpinText2";

    public static Language language
    {
        get
        {
#if UNITY_EDITOR
            return Language.RU;
#endif
            return Language.EN;
        }
    }

    private static Dictionary<string, string> Varibles = new Dictionary<string, string>()
    {
        { "PlayerName", "N-451" }
    };

    public static void LoadFiles()
    {
        Speeches.Clear();
        VictotinData.Clear();

        LoadSpeechesFromTSV(fileSpeechName);
        LoadVictorinFromTSV(fileVictorinName);

        foreach (var speech in Speeches)
        {
            PocketRandomazer.CreatePocket("sp_" + speech.Key, speech.Value.ToArray());
        }

        foreach (var victorin in VictotinData)
        {
            PocketRandomazer.CreatePocket("vc_" + victorin.Key, victorin.Value.ToArray());
        }
    }

    // --- «агрузка реплик (TSV) ---
    private static void LoadSpeechesFromTSV(string fileName)
    {
        TextAsset csvFile = Resources.Load<TextAsset>(fileName);
        if (csvFile == null)
        {
            Debug.LogError($"TSV file '{fileName}' not found in Resources!");
            return;
        }

        string[] lines = csvFile.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length == 0) return;

        // ѕерва€ строка Ч заголовок (пропускаем)
        // ќжидаемый пор€док: ID;Category;RU;ENG  (но с табул€цией)
        int idIdx = 0, catIdx = 1, ruIdx = 2, engIdx = 3, animIdx = 4;

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] fields = line.Split('\t'); // разделитель Ч табул€ци€
            if (fields.Length < 4) continue;

            string id = fields[idIdx].Trim();
            string category = fields[catIdx].Trim();
            string textRU = fields[ruIdx].Trim();
            string textENG = fields[engIdx].Trim();
            string animID = fields[animIdx].Trim();

            var speech = new SpinDictorSpeech
            {
                ID = id,
                textRU = textRU,
                textENG = textENG,
                AnimID = animID
            };

            if (!Speeches.ContainsKey(category))
                Speeches[category] = new List<SpinDictorSpeech>();
            Speeches[category].Add(speech);
        }
    }

    // --- «агрузка викторины (TSV) ---
    private static void LoadVictorinFromTSV(string fileName)
    {
        TextAsset csvFile = Resources.Load<TextAsset>(fileName);
        if (csvFile == null)
        {
            Debug.LogError($"TSV file '{fileName}' not found in Resources!");
            return;
        }

        string[] lines = csvFile.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length == 0) return;

        // «аголовок: ID;Category;RU_Text;EN_Text;CorrectRU;CorrectEN;Wrong1RU;Wrong1EN;Wrong2RU;Wrong2EN;Wrong3RU;Wrong3EN
        int idIdx = 0, catIdx = 1, ruTextIdx = 2, enTextIdx = 3,
            corrRUIdx = 4, corrENIdx = 5,
            w1RUIdx = 6, w1ENIdx = 7,
            w2RUIdx = 8, w2ENIdx = 9,
            w3RUIdx = 10, w3ENIdx = 11;

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] fields = line.Split('\t'); // разделитель Ч табул€ци€
            if (fields.Length < 12) continue;

            string id = fields[idIdx].Trim();
            string category = fields[catIdx].Trim();

            var quest = new SpinVictorinQuest
            {
                ID = id,
                textRU = fields[ruTextIdx].Trim(),
                textEN = fields[enTextIdx].Trim(),
                rightRU = fields[corrRUIdx].Trim(),
                rightEN = fields[corrENIdx].Trim(),
                anser1RU = fields[w1RUIdx].Trim(),
                anser1EN = fields[w1ENIdx].Trim(),
                anser2RU = fields[w2RUIdx].Trim(),
                anser2EN = fields[w2ENIdx].Trim(),
                anser3RU = fields[w3RUIdx].Trim(),
                anser3EN = fields[w3ENIdx].Trim()
            };

            if (!VictotinData.ContainsKey(category))
                VictotinData[category] = new List<SpinVictorinQuest>();
            VictotinData[category].Add(quest);
        }
    }

    // --- ѕубличные методы ---
    public static SpinDictorSpeech GetRandomSpeech(string category)
    {
        // убрал лишние Debug.Log, чтобы не засор€ть консоль
        return PocketRandomazer.GetRandomElement<SpinDictorSpeech>("sp_" + category);
    }

    public static SpinVictorinQuest GetRandomVictorin(string category)
    {
        return PocketRandomazer.GetRandomElement<SpinVictorinQuest>("vc_" + category);
    }

    public static string GetVariable(string id) => Varibles[id];
    public static void SetVariable(string id, string value)
    {
        if (Varibles.ContainsKey(id) == false)
            Varibles.Add(id, value);
        else
            Varibles[id] = value;
    }

    public static string ReplaceVariable(string text)
    {
        foreach (var variable in Varibles)
        {
            text = text.Replace("[" + variable.Key + "]", variable.Value);
        }
        return text;
    }
}

// ---  лассы данных (без изменений) ---
[Serializable]
public class SpinDictorSpeech
{
    public string ID;
    public string textRU;
    public string textENG;
    public string AnimID;

    public string GetText(Language language)
    {
        switch (language)
        {
            case Language.RU: return DictorSpeachManager.ReplaceVariable(textRU);
            case Language.EN: return DictorSpeachManager.ReplaceVariable(textENG);
            default: return "not implemented";
        }
    }
}

[Serializable]
public class SpinVictorinQuest
{
    public string ID;
    public string textRU;
    public string textEN;
    public string rightRU;
    public string rightEN;
    public string anser1RU;
    public string anser1EN;
    public string anser2RU;
    public string anser2EN;
    public string anser3RU;
    public string anser3EN;

    public string GetText(Language language)
    {
        switch (language)
        {
            case Language.RU: return textRU;
            case Language.EN: return textEN;
            default: return "not implemented";
        }
    }

    public string GetRightText(Language language)
    {
        switch (language)
        {
            case Language.RU: return rightRU;
            case Language.EN: return rightEN;
            default: return "not implemented";
        }
    }

    public string[] GetWrongAnswers(Language language)
    {
        if (language == Language.RU)
            return new[] { anser1RU, anser2RU, anser3RU };
        else
            return new[] { anser1EN, anser2EN, anser3EN };
    }
}