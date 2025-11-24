using System;
using TMPro;
using UnityEngine;
using ActionDatabase;
using static SaveLoadControl;
public class LanguageControl: MonoBehaviour
{
    public static Languages language;
    public static Action onLanguageChange;
    public static Translates translate;
    public Translates linkTranslate;
    [SerializeField]private TMP_Dropdown _dropdown;
    public void Awake()
    {
        translate = linkTranslate;
        LoadSettings();
        language = (Languages)settings.language;
        _dropdown.value = (int)settings.language;
    }
    public void ChangeLanguage()
    {
        language = (Languages)_dropdown.value;
        settings.language = language;
        if (onLanguageChange != null)
        {
            onLanguageChange.Invoke();
        }
        SaveSettings();
    }
    
    public static string GetTranslate(string key)
    {
        return translate.GetTranslate(key);
    }
    
}
