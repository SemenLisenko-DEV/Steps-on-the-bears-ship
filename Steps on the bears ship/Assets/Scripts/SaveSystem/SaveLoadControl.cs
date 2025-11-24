using ActionDatabase;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;
public class SaveLoadControl : MonoBehaviour
{
    public struct GameData
    {
        public Player player;
        public List<ItemData> items;
        public List<ItemHandlerData> handlers;
        public List<AnimationActivatorData> animationActivators;
        public List<PullableData> pullable;
        public T GetData<T>(ref List<T> list, string key) where T : DataObject
        {
            if (list == null) { list = new List<T>(); return null; }
            foreach (var i in list)
            {
                Debug.Log(i.id + ' ' + key);
                if (Equals(i.id,key))
                {
                    return i;
                }

            }
            return null;
        }
        public void Remove<T>(ref List<T> list, string key) where T : DataObject
        {
            if(list == null || list.Count == 0) { list = new List<T>(); return; }   
            foreach (var i in list)
            {
                if (Equals(i.id, key))
                {
                    list.Remove(i);
                }
            }
        }
    }
    public static GameData gameData = new GameData();
    public static Settings settings = new Settings();
    public static string folder = "/Saves";
    public static string filePath = "/Save.json";
    public static bool blockSaving = false;
    public static Action SaveEvent;
    [SerializeField] private GameObject _allertMenu;
    public void Awake()
    {
        LoadSettings(); 
    }
    public static void SaveData()
    {
        string path = Application.streamingAssetsPath + folder + filePath;
        string json = JsonConvert.SerializeObject(gameData);
        File.WriteAllText(path, json);
    }
    public static void LoadData()
    {
        string path = Application.streamingAssetsPath + folder + filePath;
        string json = File.ReadAllText(path);
        gameData = JsonConvert.DeserializeObject<GameData>(json);
    }
    public void TryStartNewGame()
    {
        if (File.ReadAllText(Application.streamingAssetsPath + folder + "/Save.json") != "")
        {
            _allertMenu.SetActive(true);
        }
    }
    public void StartNewGame()
    {
        gameData = new GameData();
        SaveData();
        LoadData();
        SceneManager.LoadScene("Game");
    }
    public void DontStartNewGame()
    {
        _allertMenu.SetActive(false);
    }
    public void SaveGame()
    {
        if (blockSaving) { return; }
        SaveEvent.Invoke();
        SaveData();
    }
    public void LoadGame()
    {
        LoadData();
        SceneManager.LoadScene("Game");
    }
    public void ExitToMain()
    {
        SaveGame();
        SceneManager.LoadScene("Main menu");
        Time.timeScale = 1.0f;
    }
    public static void SaveSettings()
    {
        string path = Application.streamingAssetsPath + folder + "/Settings.json";
        string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
        File.WriteAllText(path, json);
    }
    public static void LoadSettings()
    {
        string path = Application.streamingAssetsPath + folder + "/Settings.json";
        string json = File.ReadAllText(path);
        settings = JsonConvert.DeserializeObject<Settings>(json);
    }
}
