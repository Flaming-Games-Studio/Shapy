using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public static class ConfigSaveLoadSystem
{

    public static readonly string CONFIG_SAVE_FOLDER = Application.persistentDataPath + "/Save";
    public static ProgressConfigManager configTemp = null;
    private static readonly string FILE_NAME = "/PlayerProgress.json";


    public static void Init()
    {
        if (!Directory.Exists(CONFIG_SAVE_FOLDER))
        {
            Directory.CreateDirectory(CONFIG_SAVE_FOLDER);
        }

        if (File.Exists(CONFIG_SAVE_FOLDER + FILE_NAME) && Load() != null)
        {
            Debug.Log("Found a save file, loading!");
            configTemp = Load();
        }
       
    }
    
    public static void Save(string saveString)
    {
        string filePath = CONFIG_SAVE_FOLDER + FILE_NAME;
        if (!File.Exists(filePath))
        {
            Debug.Log("Configuration file don't exist!");
            Debug.Log("Creating a new file.");
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.Write(saveString);
                }
            }

            //File.Create(CONFIG_SAVE_FOLDER + FILE_NAME, 1024, FileOptions.RandomAccess);
            //File.WriteAllText(CONFIG_SAVE_FOLDER + FILE_NAME, saveString);
        }
        else
        {
            using (FileStream fd = new FileStream(filePath, FileMode.Open, FileAccess.Write, FileShare.None))
            {
                using (StreamWriter writer = new StreamWriter(fd))
                {
                    writer.Write(saveString);
                }
            }
            Debug.Log("Data saved!");
            //File.WriteAllText(CONFIG_SAVE_FOLDER + FILE_NAME, saveString);
        }
        
    }
    public static ProgressConfigManager Load()
    {
        string saveString = File.ReadAllText(CONFIG_SAVE_FOLDER + FILE_NAME);
        if (saveString == null || saveString == "")
        {
            Debug.Log("Found an empty file!");
            return null;
        }
        configTemp = JsonUtility.FromJson<ProgressConfigManager>(saveString);
        Debug.Log("Found a save file!");

        return configTemp;
    }
}
