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
    
    public static string GetTranslate(string key,int r = 0)
    {
        return translate.GetTranslate(key,r);
    }
    public static string GetTranslate(string key, Pair<string, string> placeholder, int r = 0)
    {
        string s = GetTranslate(key, r);
        if (placeholder.Second.Contains("{") && placeholder.Second.Contains("}"))
        {
            placeholder.Second = GetTranslate(placeholder.Second.Substring(1, placeholder.Second.Length - 2));
        }
        s = s.Replace(placeholder.First, placeholder.Second);
        return s;
    }
    public static string GetTranslate(string key, Pair<string,string>[] placeholder, int r = 0)
    {
        string s = GetTranslate(key,r);
        for (int i = 0; i < placeholder.Length; i++)
        {
            if (placeholder[i].Second.Contains("{") && placeholder[i].Second.Contains("}"))
            {
                placeholder[i].Second = GetTranslate(placeholder[i].Second.Substring(1, placeholder[i].Second.Length - 2));
            }
            s = s.Replace(placeholder[i].First, placeholder[i].Second);
        }
        return s;
    }
}
