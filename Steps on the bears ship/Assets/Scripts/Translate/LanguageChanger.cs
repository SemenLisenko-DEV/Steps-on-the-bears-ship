using TMPro;
using UnityEngine;

public class LanguageChanger : MonoBehaviour
{
    private TMP_Text _tmpText;
    [SerializeField] private string _key;
    [SerializeField] private int _maxRandomVariation = 0;
    public int variation = 0;
    public void Start()
    {
        TryGetComponent(out _tmpText);
        LanguageControl.onLanguageChange += UpdateTextLanguage;
        UpdateTextLanguage();
    }
    public void OnDestroy()
    {
        LanguageControl.onLanguageChange -= UpdateTextLanguage;
    }
    public void UpdateTextLanguage()
    {
        if (_tmpText != null)
        {
            if (variation != 0)
            {
                _tmpText.text = LanguageControl.GetTranslate(_key, variation);
            }
            else
            {
                int r = Random.Range(0, _maxRandomVariation);
                _tmpText.text = LanguageControl.GetTranslate(_key, r);
            }
        }
    }
    private void OnDisable()
    {
        UpdateTextLanguage();
    }
}
