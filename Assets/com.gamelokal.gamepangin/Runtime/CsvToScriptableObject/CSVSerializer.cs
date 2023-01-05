using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gamepangin
{
    public static class CsvSerializer
{
    public static T[] Deserialize<T>(string text)
    {
        return (T[])CreateArray(typeof(T), ParseCsv(text));
    }

    public static T[] Deserialize<T>(List<string[]> rows)
    {
        return (T[])CreateArray(typeof(T), rows);
    }

    public static T DeserializeIdValue<T>(string text, int columnId = 0, int columnValue = 1)
    {
        return (T)CreateIdValue(typeof(T), ParseCsv(text), columnId, columnValue);
    }

    public static T DeserializeIdValue<T>(List<string[]> rows, int columnId = 0, int columnValue = 1)
    {
        return (T)CreateIdValue(typeof(T), rows, columnId, columnValue);
    }

    private static object CreateArray(Type type, List<string[]> rows)
    {
        var arrayValue = Array.CreateInstance(type, rows.Count - 1);
        var table = new Dictionary<string, int>();

        for (int i = 0; i < rows[0].Length; i++)
        {
            string id = rows[0][i];
            string id2 = "";
            foreach (var c in id)
            {
                if (c is >= 'a' and <= 'z' or >= '0' and <= '9')
                    id2 += c.ToString();
                else if (c is >= 'A' and <= 'Z')
                    id2 += ((char)(c - 'A' + 'a')).ToString();
            }

            table.Add(id, i);
            if (!table.ContainsKey(id2))
                table.Add(id2, i);
        }

        for (int i = 1; i < rows.Count; i++)
        {
            var rowData = Create(rows[i], table, type);
            arrayValue.SetValue(rowData, i-1);
        }
        return arrayValue;
    }

    static object Create(string[] cols, Dictionary<string, int> table, Type type)
    {
        object v = Activator.CreateInstance(type);

        FieldInfo[] fieldInfo = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (FieldInfo tmp in fieldInfo)
        {
            if (table.ContainsKey(tmp.Name))
            {
                int idx = table[tmp.Name];
                if (idx < cols.Length)
                    SetValue(v, tmp, cols[idx]);
            }
        }
        return v;
    }

    static void SetValue(object v, FieldInfo fieldInfo, string value)
    {
        if (string.IsNullOrEmpty(value))
            return;

        if (fieldInfo.FieldType.IsArray)
        {
            Type elementType = fieldInfo.FieldType.GetElementType();
            if (elementType == null) return;
            
            string[] elem = value.Split(',');
            Array arrayValue = Array.CreateInstance(elementType, elem.Length);
            for (int i = 0; i < elem.Length; i++)
            {
                if (elementType == typeof(string))
                    arrayValue.SetValue(elem[i], i);
                else
                    arrayValue.SetValue(Convert.ChangeType(elem[i], elementType), i);
            }
            fieldInfo.SetValue(v, arrayValue);
        }
        else if (fieldInfo.FieldType.IsEnum)
            fieldInfo.SetValue(v, Enum.Parse(fieldInfo.FieldType, value));
        else if (value.IndexOf('.') != -1 &&
            (fieldInfo.FieldType == typeof(Int32) || fieldInfo.FieldType == typeof(Int64) || fieldInfo.FieldType == typeof(Int16)))
        {
            float f = (float)Convert.ChangeType(value, typeof(float));
            fieldInfo.SetValue(v, Convert.ChangeType(f, fieldInfo.FieldType));
        }
#if UNITY_EDITOR
        else if (fieldInfo.FieldType == typeof(Sprite))
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(value);
            fieldInfo.SetValue(v, sprite);
        }
#endif
        else if (fieldInfo.FieldType == typeof(string))
            fieldInfo.SetValue(v, value);
        else
            fieldInfo.SetValue(v, Convert.ChangeType(value, fieldInfo.FieldType));
    }

    static object CreateIdValue(Type type, List<string[]> rows, int columnId = 0, int columnValue = 1)
    {
        object v = Activator.CreateInstance(type);

        Dictionary<string, int> table = new Dictionary<string, int>();

        for (int i = 1; i < rows.Count; i++)
        {
            if (rows[i][columnId].Length > 0)
                table.Add(rows[i][columnId].TrimEnd(' '), i);
        }

        FieldInfo[] fieldInfo = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (FieldInfo tmp in fieldInfo)
        {
            if (table.ContainsKey(tmp.Name))
            {
                int idx = table[tmp.Name];
                if (rows[idx].Length > columnValue)
                    SetValue(v, tmp, rows[idx][columnValue]);
            }
            else
            {
                Debug.Log("Miss " + tmp.Name);
            }
        }
        return v;
    }

    static public List<string[]> ParseCsv(string text, char separator = ',')
    {
        List<string[]> lines = new List<string[]>();
        List<string> line = new List<string>();
        StringBuilder token = new StringBuilder();
        bool quotes = false;

        for (int i = 0; i < text.Length; i++)
        {
            if (quotes)
            {
                if ((text[i] == '\\' && i + 1 < text.Length && text[i + 1] == '\"') || (text[i] == '\"' && i + 1 < text.Length && text[i + 1] == '\"'))
                {
                    token.Append('\"');
                    i++;
                }
                else if (text[i] == '\\' && i + 1 < text.Length && text[i + 1] == 'n')
                {
                    token.Append('\n');
                    i++;
                }
                else if (text[i] == '\"')
                {
                    line.Add(token.ToString());
                    token = new StringBuilder();
                    quotes = false;
                    if (i + 1 < text.Length && text[i + 1] == separator)
                        i++;
                }
                else
                {
                    token.Append(text[i]);
                }
            }
            else if (text[i] == '\r' || text[i] == '\n')
            {
                if (token.Length > 0)
                {
                    line.Add(token.ToString());
                    token = new StringBuilder();
                }
                if (line.Count > 0)
                {
                    lines.Add(line.ToArray());
                    line.Clear();
                }
            }
            else if (text[i] == separator)
            {
                line.Add(token.ToString());
                token = new StringBuilder();
            }
            else if (text[i] == '\"')
            {
                quotes = true;
            }
            else
            {
                token.Append(text[i]);
            }
        }

        if (token.Length > 0)
        {
            line.Add(token.ToString());
        }
        if (line.Count > 0)
        {
            lines.Add(line.ToArray());
        }
        return lines;
    }
}
}