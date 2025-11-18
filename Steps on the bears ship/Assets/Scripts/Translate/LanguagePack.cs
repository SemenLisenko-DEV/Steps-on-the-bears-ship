using TMPro;
using UnityEngine;

public class LanguagePack : MonoBehaviour
{
    [SerializeField] private TMP_Text _tmpText;
    public string[] texts;
    public void Start()
    {
        TryGetComponent(out _tmpText);
        LanguageControl.onLanguageChange += UpdateTextLanguage;
        _tmpText.text = texts[PlayerPrefs.GetInt("language")];
    }
    public void OnDestroy()
    {
        LanguageControl.onLanguageChange -= UpdateTextLanguage;
    }
    public void UpdateTextLanguage()
    {
        if (_tmpText != null)
        {
            _tmpText.text = texts[PlayerPrefs.GetInt("language")];
        }
    }
}
