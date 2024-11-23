using Newtonsoft.Json;
using System.IO;
using UnityEngine;

public static class SaveDataManager
{
#if UNITY_EDITOR
    public static readonly string SaveDataDirectory = Application.persistentDataPath + "/Data/Save";
#else
    public static readonly string SaveDataDirectory = Application.dataPath + "/Data/Save";
#endif
    public static readonly string MainSaveFileDirectory = "/MainSaveData.txt";

    public static string SaveDataToJSon(SaveGameData data)
    {
        return JsonConvert.SerializeObject(data, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        });
    }

    public static void SaveJSonToDirectory(string jsonString,  string directory, string fileName)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (!File.Exists(directory + fileName))
        {
            File.Create(directory + fileName);
        }
        
        File.WriteAllText(directory + fileName, jsonString);
    }

    public static string ReadFile(string fileDirectory)
    {
        if (!File.Exists(fileDirectory))
        {
            throw new System.Exception("Directory Does Not Exist.");
        }

        return File.ReadAllText(fileDirectory);
    }
}
