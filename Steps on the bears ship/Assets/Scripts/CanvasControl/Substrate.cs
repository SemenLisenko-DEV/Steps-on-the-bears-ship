using ActionDatabase;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Substrate : MonoBehaviour
{
    public string key;
    public TMP_Text description;
    public Image saveScreenshot;
    public Save data;
    public Sprite _screenShot;
    public void Start()
    {
        SetDescription();
        try
        {
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(File.ReadAllBytes(Application.streamingAssetsPath + "/Screenshots/" + data.spritePath));
            _screenShot = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            saveScreenshot.sprite = _screenShot;
        }
        catch
        {
            saveScreenshot.sprite = Resources.Load<Sprite>("Sprite/DefaultScreen");
        }
        LanguageControl.onLanguageChange += SetDescription;
    }
    public void SetDescription()
    {
        List<Pair<string, string>> pair = new List<Pair<string, string>>(0);
        pair.Add(new Pair<string, string>("{name}", data.name));
        pair.Add(new Pair<string, string>("{time}", data.date));
        description.text = LanguageControl.GetTranslate(key, pair.ToArray());
    }
    public void SetCurrentSave()
    {
        SaveLoadControl.fileName = data.fileName;
        SaveLoadControl.LoadGame();
    }
    public void Delete()
    {
        SavesList.DeleteSave(this);
    }
    private void OnDestroy()
    {
        LanguageControl.onLanguageChange -= SetDescription;
    }
}
