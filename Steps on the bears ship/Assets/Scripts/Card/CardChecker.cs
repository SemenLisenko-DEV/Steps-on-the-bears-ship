using ActionDatabase;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(AudioSource))]
public class CardChecker : MonoBehaviour,IAction
{
    public string id;

    //сохранить:
    public bool state = false;
    public bool block = false;
    //дальше не сохранять

    [SerializeField] private string _cardTier;
    [SerializeField] private bool _multiCompite = false;
    [SerializeField] private AudioDictionary _audioDictionary;
    [SerializeField] private UnityEvent _onComplite;
    [SerializeField] private UnityEvent _onDecomplite;
    [SerializeField] private UnityEvent _onOutOfAcces;

    private AudioSource _audioSource;



    public void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        Load();
        SaveLoadControl.SaveEvent += Save;
    }
    public void StartEvent()
    {
        if (block) { return; }
        Debug.Log("Card Executing...");
        if(PlayerControl.Instance.cardTaken.Contains(_cardTier) && (!state || _multiCompite))
        {
            Debug.Log("Card Verifed...");
            _audioSource.clip = _audioDictionary.Find("accessVerifed");
            _audioSource.Play();
            state = !state;
            if(state)
            {
                _onComplite.Invoke();
                Debug.Log("COMPLITE CARD QUEST: " + id);
            }
            else
            {
                _onDecomplite.Invoke();
                Debug.Log("DECOMPLITE CARD QUEST: " + id);
            }
        }
        else
        {
            Debug.Log("Card Access Denied!");
            _onOutOfAcces.Invoke();
            _audioSource.clip = _audioDictionary.Find("accessDenied");
            _audioSource.Play();
        }
    }
    public void SetBlock(bool f)
    {
        block = f;
    }
    public void SetState(bool f)
    {
        state = f;
    }
    public void OnDestroy()
    {
        SaveLoadControl.SaveEvent -= Save;
    }
    public void Save()
    {
        SaveLoadControl.gameData.Remove(ref SaveLoadControl.gameData.checkerDatas, id);
        SaveLoadControl.gameData.checkerDatas.Add(new CardCheckerData(this));
    }
    public void Load()
    {
        CardCheckerData data = SaveLoadControl.gameData.GetData(ref SaveLoadControl.gameData.checkerDatas, id);
        if (data == null) { return; }
        state = data.state;
        block = data.blocked;
    }
}
