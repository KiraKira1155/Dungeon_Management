using DG.Tweening.Plugins.Core.PathCore;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class SaveDataManager
{  
    private static string filepath;
    private static string fileName = "SaveData.json";
    public static bool initGame;

    public static void DoAwake()
    {
        filepath = Application.dataPath + "/" + fileName;
        initGame = !SaveFileCheck();
    }

    public static SaveData Load()
    {
        var fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        var reader = new StreamReader(fs, Encoding.UTF8);
        string json = reader.ReadToEnd();
        fs.Close();
        reader.Close();
        var data = JsonUtility.FromJson<SaveData>(json);
        return data;
    }

    public static void Save(SaveData data)
    {
        CreateFile(filepath);

        var writer = new StreamWriter(filepath, false);
        string jsonstr = JsonUtility.ToJson(data);
        writer.Write(jsonstr);
        writer.Flush();
        writer.Close();
    }

    private static void CreateFile(string path)
    {
        File.Create(path).Close();
    }

    public static bool SaveFileCheck()
    {
        if (File.Exists(filepath))
            return true;

        return false;
    }
}
