using Newtonsoft.Json;
using System.IO;

public static class SaveDataManager
{
    public static string SaveDataToJSon(SaveGameData data)
    {
        return JsonConvert.SerializeObject(data);
    }

    public static void SaveJSonToDirectory(string jsonString,  string directory)
    {
        File.WriteAllText(directory, jsonString);
    }
}
