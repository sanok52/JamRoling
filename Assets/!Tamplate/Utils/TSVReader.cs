using System.Collections.Generic;
using UnityEngine;

public static class TSVReader
{
    /// <summary>
    /// Читает TSV-файл (разделитель — табуляция) из папки Resources.
    /// </summary>
    /// <param name="path">Имя файла без расширения (например, "SpinText")</param>
    /// <returns>Словарь: имя столбца -> массив строк для этого столбца (без заголовка)</returns>
    public static Dictionary<string, string[]> ReadTxtFromResources(string path)
    {
        // Загружаем текстовый ассет из Resources
        TextAsset textAsset = Resources.Load<TextAsset>(path);
        if (textAsset == null)
        {
            Debug.LogError($"TSVReader: Файл '{path}' не найден в Resources!");
            return new Dictionary<string, string[]>();
        }

        string content = textAsset.text;
        string[] lines = content.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length == 0)
        {
            Debug.LogError($"TSVReader: Файл '{path}' пуст.");
            return new Dictionary<string, string[]>();
        }

        // Первая строка — заголовки столбцов
        string[] headers = lines[0].Split('\t');

        // Инициализируем словарь: для каждого столбца создаём список строк
        Dictionary<string, List<string>> columnData = new Dictionary<string, List<string>>();
        foreach (string header in headers)
        {
            columnData[header.Trim()] = new List<string>();
        }

        // Обрабатываем строки данных (начиная со второй)
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line))
                continue;

            string[] fields = line.Split('\t');
            // Если число полей меньше числа заголовков, дополняем пустыми строками
            if (fields.Length < headers.Length)
            {
                System.Array.Resize(ref fields, headers.Length);
            }

            for (int j = 0; j < headers.Length && j < fields.Length; j++)
            {
                string header = headers[j].Trim();
                string value = fields[j].Trim();
                columnData[header].Add(value);
            }
        }

        // Преобразуем списки в массивы
        Dictionary<string, string[]> result = new Dictionary<string, string[]>();
        foreach (var kvp in columnData)
        {
            result[kvp.Key] = kvp.Value.ToArray();
        }

        return result;
    }

    public static string FindInTable(Dictionary<string, string[]> table, string column, string idValue)
    {
        // 1. Определяем столбец с ID (если есть "ID", иначе берём первый)
        string idColumn = table.ContainsKey("ID") ? "ID" : GetFirstKey(table);

        // 2. Находим индекс строки с нужным ID
        int index = -1;
        string[] ids = table[idColumn];
        for (int i = 0; i < ids.Length; i++)
        {
            if (ids[i] == idValue)
            {
                index = i;
                break;
            }
        }

        // 3. Если найден, возвращаем значение из запрошенного столбца
        if (index != -1 && table.ContainsKey(column))
            return table[column][index];
        else
            return null;
    }

    private static string GetFirstKey(Dictionary<string, string[]> dict)
    {
        foreach (string key in dict.Keys)
            return key;
        return null;
    }

    public static string[] GetIdLine(Dictionary<string, string[]> table, string idValue)
    {
        if (table == null || table.Count == 0)
            return null;

        // 1. Определяем столбец-идентификатор
        string idColumn = table.ContainsKey("ID") ? "ID" : GetFirstKey(table);

        // 2. Ищем индекс строки с нужным ID
        int index = -1;
        string[] ids = table[idColumn];
        for (int i = 0; i < ids.Length; i++)
        {
            if (ids[i] == idValue)
            {
                index = i;
                break;
            }
        }

        if (index == -1)
            return null;

        // 3. Собираем значения из всех столбцов
        // Используем порядок ключей, который соответствует порядку столбцов в файле
        string[] result = new string[table.Count];
        int colIdx = 0;
        foreach (var kvp in table)
        {
            result[colIdx] = (index < kvp.Value.Length) ? kvp.Value[index] : null;
            colIdx++;
        }

        return result;
    }

    public static Dictionary<string, string> GetIdLineAsDict(Dictionary<string, string[]> table, string idValue)
    {
        string[] line = GetIdLine(table, idValue);
        if (line == null) return null;

        var dict = new Dictionary<string, string>();
        int i = 0;
        foreach (string key in table.Keys)
        {
            dict[key] = line[i++];
        }
        return dict;
    }
}