using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[System.Serializable]
public class SaveObject
{
    public LevelRecord[] levels;
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
        autoplay = true;
        voiceVolume = 1f;
        musicVolume = 0.5f;
        soundVolume = 0.75f;
    }
}

[System.Serializable]
public class LevelRecord
{
    public string levelId;
    public bool unlocked;
    public float bestTime;
    public int healthLost;
    public int loot;

    public LevelRecord(string _levelId)
    {
        levelId = _levelId;
        unlocked = false;
        bestTime = -1;
        healthLost = -1;
        loot = -1;
        
    }
}

public static class SaveService
{
    public static SaveObject loadedSave;
    public static string path = Application.persistentDataPath + "/save_data.dat";

    public static void SaveData()
    {
        SaveData(loadedSave);
    }

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
        if (File.Exists(path) && false)
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
            SaveObject emptySave = CreateEmptySave();
            SaveData(emptySave);
            return emptySave;
        }
    }

    public static SaveObject CreateEmptySave()
    {
        SaveObject newSave = new SaveObject
        {
            options = new GameOptions(),
            levels = new LevelRecord[Globals.LEVEL_LIST.Count]
        };

        for (int i = 0; i < Globals.LEVEL_LIST.Count; ++i)
            newSave.levels[i] = new LevelRecord(Globals.LEVEL_LIST[i].levelId);
        newSave.levels[0].unlocked = true;

        return newSave;
    }

    public static int GetLevelIndex(string levelId)
    {
        for(int i = 0; i < loadedSave.levels.Length; ++i)
            if (loadedSave.levels[i].levelId == levelId)
                return i;
        
        return -1;
    }

    public static void UpdateLevelRecord(string levelId, int loot, float time, int healthLost)
    {
        int index = GetLevelIndex(levelId);

        if(loadedSave.levels[index].bestTime == -1)
        {
            loadedSave.levels[index].loot = loot;
            loadedSave.levels[index].bestTime = time;
            loadedSave.levels[index].healthLost = healthLost;
        }
        else
        {
            loadedSave.levels[index].loot = Mathf.Max(loadedSave.levels[index].loot, loot);
            loadedSave.levels[index].bestTime = Mathf.Min(loadedSave.levels[index].bestTime, time);
            loadedSave.levels[index].healthLost = Mathf.Min(loadedSave.levels[index].healthLost, healthLost);
        }

        SaveData(loadedSave);
    }
}
