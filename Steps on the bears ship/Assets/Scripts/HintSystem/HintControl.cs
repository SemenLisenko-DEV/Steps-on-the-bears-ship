using ActionDatabase;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent (typeof(AudioSource))]
public class HintControl : MonoBehaviour
{
    public static HintControl Instance { get; private set; }

    [SerializeField] private TMP_Text _hintText;
    [SerializeField] private TMP_Text _hintNotifier;
    [SerializeField] private AudioDictionary _audioDictionary;
    private string _currentKey = "MishaRoom_Hint";
    private AudioSource _audioSource;
    public void Awake()
    {
        Instance = this;
        Load();
        UpdateHint();
        _audioSource = GetComponent<AudioSource>();
        LanguageControl.onLanguageChange += UpdateHint;
        SaveLoadControl.SaveEvent += Save;
    }
    public void SetHint(string key)
    {
        _audioSource.clip = _audioDictionary.Find("New hint");
        _audioSource.Play();
        StartCoroutine(Notify());
        _hintText.text = LanguageControl.GetTranslate(key);
        _currentKey = key;
    }
    public void UpdateHint()
    {
        _hintText.text = LanguageControl.GetTranslate(_currentKey);
    }
    public IEnumerator Notify()
    {
        _hintNotifier.alpha = 1;
        while (_hintNotifier.alpha > 0)
        {
            _hintNotifier.alpha -= Time.deltaTime * 0.5f;
            yield return new WaitForEndOfFrame();
        }
    }    
    private void OnDestroy()
    {
        LanguageControl.onLanguageChange -= UpdateHint;
        SaveLoadControl.SaveEvent -= Save;
    }
    public void Load()
    {
        _currentKey = SaveLoadControl.gameData.hint;
    }
    public void Save()
    {
        SaveLoadControl.gameData.hint = _currentKey;
    }
}
