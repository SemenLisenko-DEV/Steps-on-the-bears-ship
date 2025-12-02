using ActionDatabase;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using TMPro;
public class SaveLoadControl : MonoBehaviour
{
    public struct GameData
    {
        public string name;
        public string date;
        public string screenPath;
        public string hint;
        public Player player;
        public List<ItemData> items;
        public List<ItemHandlerData> handlers;
        public List<AnimationActivatorData> animationActivators;
        public List<PullableData> pullable;
        public List<Trigger> triggers;
        public List<ValveData> valves;
        public List<QuestData> quests;
        public T GetData<T>(ref List<T> list, string key) where T : DataObject
        {
            if (list == null) { list = new List<T>(); return null; }
            foreach (var i in list)
            {
                if (Equals(i.id,key))
                {
                    return i;
                }

            }
            return null;
        }
        public void Remove<T>(ref List<T> list, string key) where T : DataObject
        {
            if(list == null) { list = new List<T>(); return; }   
            List<T> buffer = new List<T>();
            foreach (var i in list)
            {
                if (Equals(i.id, key))
                {
                    buffer.Add(i);
                }
            }
            foreach (var i in buffer)
            {
                list.Remove(i);
            }
        }
        public void CopyData(GameData data)
        {
            hint = data.hint;
            player = data.player;
            items = data.items;
            handlers = data.handlers;
            animationActivators = data.animationActivators;
            pullable = data.pullable;
            triggers = data.triggers;
            valves = data.valves;
            quests = data.quests;
        }
    }
    public static SaveLoadControl Instance;
    public static GameData gameData = new GameData();
    public static Settings settings = new Settings();
    public static string folder = "/Saves";
    public static string fileName = "";
    public static bool blockSaving = false;
    public static Action SaveEvent;
    [SerializeField,Header("Нужные объекты в меню")] private GameObject _allertMenu;
    [SerializeField] private GameObject _continueButton;
    [SerializeField] private TMP_Text _continueButtonText;
    [SerializeField] private TMP_InputField _nameField;
    [SerializeField, Header("Нужные объекты в игре")] private GameObject _saveUI;
    [SerializeField] private GameObject _saving;
    [SerializeField] private GameObject _allertSave;
    [SerializeField] private GameObject _allertExit;
    public void Awake()
    {
        Instance = this;
        blockSaving = false;
        LoadSettings();
    }
    private void Start()
    {
        SetContinueButton();
        SavesList.OnSubstrateDelete += SetContinueButton;
        LanguageControl.onLanguageChange += SetContinueButton;
    }
    public void SetContinueButton()
    {
        if (_continueButton != null)
        {
            if (File.Exists(Application.streamingAssetsPath + folder + "/" + settings.lastSave))
            {
                _continueButton.SetActive(true);
                _continueButtonText.text = LanguageControl.GetTranslate("MainMenu_Continue", new Pair<string, string>("{name}", settings.Find(settings.lastSave).name));
            }
            else
            {
                _continueButton.SetActive(false);
            }
        }
    }
    private void Update()
    {
        if(_allertExit != null)
        {
            _allertExit.SetActive(blockSaving);
        }
    }
    public static void SaveData()
    {
        if(blockSaving) { return; }
        try
        {
            string path = Application.streamingAssetsPath + folder + "/" + fileName;
            string json = JsonConvert.SerializeObject(gameData);
            File.WriteAllText(path, json);
        }
        catch
        {
            gameData = new GameData();
        }
    }
    public static void LoadData()
    {
        blockSaving = false;
        Time.timeScale = 1.0f;
        try
        {
            string path = Application.streamingAssetsPath + folder + "/" + fileName;
            string json = File.ReadAllText(path);
            gameData = JsonConvert.DeserializeObject<GameData>(json);
        }
        catch
        {
            gameData = new GameData();
        }
    }
    public void TryStartNewGame()
    {
        if (File.Exists(Application.streamingAssetsPath + folder + "/autoSave.json"))
        {
            _allertMenu.SetActive(true);
        }
        else
        {
            StartNewGame();
        }
    }
    public void StartNewGame()
    {
        gameData = new GameData();
        CreateAutoSave();
        LoadGame();
        SceneManager.LoadScene("Game");
    }
    public void DontStartNewGame()
    {
        _allertMenu.SetActive(false);
    }
    public void ContinueGame()
    {
        fileName = settings.lastSave;
        LoadGame();
    }
    public void SaveGame()
    {
        if (blockSaving)
        { 
            StartCoroutine(ShowSaveAllertUI());
            return;
        }
        settings.lastSave = fileName;
        StartCoroutine(ShowSaveUI());
        MakeScreenshot(gameData.screenPath);
        SaveEvent.Invoke();
        SaveData();
        SaveSettings();
    }
    public static void LoadGame()
    {
        SaveSettings();
        LoadData();
        SceneManager.LoadScene("Game");
    }
    public static void ReloadGame()
    {
        SaveSettings();
        LoadData();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void AutoSave()
    {
        if(blockSaving)
        {
            StartCoroutine(ShowSaveAllertUI());
            return;
        }
        SaveEvent.Invoke();
        StartCoroutine(ShowSaveUI());
        MakeScreenshot("autoSaveScreen.png");
        string path = Application.streamingAssetsPath + folder + "/" + "autoSave.json";
        if (!File.Exists(path))
        {
            CreateAutoSave();
        }
        GameData buffer =  new GameData();
        buffer.CopyData(gameData);
        buffer.name = "{AutoSaveName}";
        buffer.date = DateTime.Now.ToString();
        buffer.screenPath = "autoSaveScreen.png";
        string json = JsonConvert.SerializeObject(buffer);
        try
        {
            File.WriteAllText(path, json);
        }
        catch(Exception ex)
        {
            Debug.LogWarning(ex);
        }
    }
    public void ExitToMain()
    {
        blockSaving = false;
        Time.timeScale = 1.0f;
        settings.lastSave = fileName;
        SaveSettings();
        SceneManager.LoadScene("Main menu");
    }
    public void TrySaveAndQuit()
    {
        blockSaving = false;
        Time.timeScale = 1.0f;
        settings.lastSave = fileName;
        SaveSettings();
        Application.Quit();
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
        if(settings.savesLinks == null) { settings.savesLinks = new List<Save>(0); }
    }
    public IEnumerator ShowSaveUI()
    {
        if (_saveUI == null || _saving == null || _saveUI.activeSelf) { yield break; }
        _saveUI.SetActive(true);
        _saving.SetActive(true);
        yield return new WaitForSecondsRealtime(2.5f);
        _saveUI.SetActive(false);
        _saving.SetActive(false) ;
    }
    public IEnumerator ShowSaveAllertUI()
    {
        if (_saveUI == null || _allertSave == null || _saveUI.activeSelf) { yield break; }
        _allertSave.SetActive(true);
        _saveUI.SetActive(true);;
        yield return new WaitForSecondsRealtime(2.5f);
        _allertSave.SetActive(false);
        _saveUI.SetActive(false);
    }
    public void MakeScreenshot(string screenShotName)
    {
        Texture2D screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        RenderTexture targetTexture = RenderTexture.GetTemporary(Screen.width, Screen.height, 24);
        Camera.main.targetTexture = targetTexture;
        Camera.main.Render();
        RenderTexture.active = targetTexture;
        screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenshot.Apply();
        byte[] bytes = screenshot.EncodeToPNG();
        try
        {
            File.WriteAllBytes(Application.streamingAssetsPath + "/Screenshots/" + screenShotName, bytes);
        }
        catch(Exception e)
        {
            Debug.LogWarning("Something bad with screenshot: " + e.Message);
        }
        Destroy(screenshot);
        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(targetTexture);
   
    }
    public void CreateSavePlayer()
    {
        Debug.Log("Created player save");
        string file = "playerSave" + settings.playerSavesCount + ".json";
        string path = Application.streamingAssetsPath + folder + "/" + file;
        File.Create(path).Close();
        GameData buffer = new GameData();
        buffer.name = _nameField.text;
        buffer.date = DateTime.Now.ToString();
        buffer.screenPath = "playerSave" + settings.playerSavesCount + "Screen.png";
        string json = JsonConvert.SerializeObject(buffer);
        File.WriteAllText(path, json);
        Save save = new Save();
        save.name = _nameField.text;
        save.date = DateTime.Now.ToString();
        save.fileName = file;
        save.spritePath = buffer.screenPath;
        settings.savesLinks.Add(save);
        fileName = file;
        SaveSettings();
        SavesList.Refresh();
        settings.playerSavesCount += 1;

    }
    public void CreateAutoSave()
    {
        Debug.Log("Created auto save");
        foreach (Save i in settings.savesLinks)
        {
            if (Equals(i, "autoSave.json"))
            {
                DeleteSave(i.fileName);
                break;
            }
        }
        string path = Application.streamingAssetsPath + folder + "/" + "autoSave.json";
        if (!File.Exists(path))
        {
            File.Create(path).Close();
        }
        GameData buffer = new GameData();
        buffer.name = "{AutoSaveName}";
        buffer.date = DateTime.Now.ToString();
        buffer.screenPath = "autoSaveScreen.png";
        string json = JsonConvert.SerializeObject(buffer);
        File.WriteAllText(path, json);
        Save save = new Save();
        save.name = "{AutoSaveName}";
        save.date = DateTime.Now.ToString();
        save.fileName = "autoSave.json";
        save.spritePath = buffer.screenPath;
        settings.savesLinks.Add(save);
        fileName = "autoSave.json";
        SaveSettings();
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            SavesList.Refresh();
        }
    }
    public static void DeleteSave(string saveFileName)
    {
        Save saveData = new Save();
        saveData = settings.Find(saveFileName);
        settings.savesLinks.Remove(saveData);
        File.Delete(Application.streamingAssetsPath + folder + "/" + saveFileName);
        if (File.Exists(Application.streamingAssetsPath + "/Screenshots/" + saveData.spritePath))
        {
            File.Delete(Application.streamingAssetsPath + "/Screenshots/" + saveData.spritePath);
        }
        if (settings.lastSave == saveFileName) { settings.lastSave = ""; }
        if (fileName == saveFileName) { fileName = ""; }
        settings.savesLinks.Remove(saveData);
        SaveSettings();
    }
    public void JustLoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }
    public void JustExit()
    {
        Application.Quit();
    }
    private void OnDestroy()
    {
        SavesList.OnSubstrateDelete -= SetContinueButton;
        LanguageControl.onLanguageChange -= SetContinueButton;
    }
}
