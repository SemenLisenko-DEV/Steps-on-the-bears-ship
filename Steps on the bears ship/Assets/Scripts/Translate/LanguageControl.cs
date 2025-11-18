using System;
using TMPro;
using UnityEngine;

public class LanguageControl: MonoBehaviour
{
    public static Action onLanguageChange;
    private TMP_Dropdown _dropdown;
    public enum languages
    {
        Rus = 0,
        Eng = 1,
    }
    public void Start()
    {
        _dropdown = GetComponent<TMP_Dropdown>();
        _dropdown.value = PlayerPrefs.GetInt("language");
    }
    public void ChangeLanguage()
    {
        PlayerPrefs.SetInt("language", _dropdown.value);
        if(onLanguageChange != null )
        {
            onLanguageChange.Invoke();
        }
    }
}
