using TMPro;
using UnityEngine;

public class LanguageChanger : MonoBehaviour
{
    private TMP_Text _tmpText;
    [SerializeField] private string key;
    public void Start()
    {
        TryGetComponent(out _tmpText);
        LanguageControl.onLanguageChange += UpdateTextLanguage;
        _tmpText.text = LanguageControl.GetTranslate(key);
    }
    public void OnDestroy()
    {
        LanguageControl.onLanguageChange -= UpdateTextLanguage;
    }
    public void UpdateTextLanguage()
    {
        if (_tmpText != null)
        {
            _tmpText.text = LanguageControl.GetTranslate(key);
        }
    }

}
