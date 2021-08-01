using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[System.Serializable]
public class SaveObject
{
    public LevelStats[] levels;
    public GameOptions options;
}

[System.Serializable]
public class GameOptions
{
    public bool parallax;
    public bool autoplay;
    public float voiceVolume;
    public float musicVolume;
    public float soundVolume;

    public GameOptions()
    {
        parallax = true;
        voiceVolume = 1f;
        musicVolume = 0.5f;
        soundVolume = 0.75f;
    }
}

[System.Serializable]
public class LevelStats
{
    public bool unlocked;
    public float bestTime;
    public int healthLost;
    public int moneyCollected;

    public LevelStats()
    {
        unlocked = false;
        bestTime = -1;
        healthLost = -1;
        moneyCollected = -1;
    }
}

public static class SaveService
{
    public static SaveObject loadedSave;
    public static string path = Application.persistentDataPath + "/save_data.dat";

    public static void SaveData(SaveObject saveObject)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        FileStream stream = new FileStream(path, FileMode.Create);

        // PlayerData data = new PlayerData(player);
        formatter.Serialize(stream, saveObject);
        stream.Close();
    }

    public static SaveObject LoadData()
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveObject data = formatter.Deserialize(stream) as SaveObject;
            stream.Close();
            return data;
        }
        else
        {
            Debug.Log("No save data found");
            return CreateEmptySave();
        }
    }

    public static SaveObject CreateEmptySave()
    {
        SaveObject newSave = new SaveObject
        {
            options = new GameOptions(),
            levels = new LevelStats[Globals.NUM_LEVELS]
        };

        for (int i = 0; i < Globals.NUM_LEVELS; ++i)
            newSave.levels[i] = new LevelStats();
        newSave.levels[0].unlocked = true;

        return newSave;
    }
}
