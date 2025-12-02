using ActionDatabase;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class TrapTrigger : MonoBehaviour
{
    public string id;

    //сохранить:
    public bool triggered = false;
    //дальше не сохранять

    [SerializeField] public static List<TrapTrigger> trapTriggers = new List<TrapTrigger>();

    [SerializeField] private GameObject[] _quests;
    private bool _triggerMultiply = false;
    private AudioSource _audioSource;
    public void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        trapTriggers.Add(this);
        SaveLoadControl.SaveEvent += Save;
        Load();
    }
    public void OnTriggerEnter(Collider other)
    {
        
        if(!triggered || _triggerMultiply)
        {
            _audioSource.Play();
            triggered = true;
            SaveLoadControl.blockSaving = true; 
            for (int i = 0; i < _quests.Length;i++)
            {
                _quests[i].GetComponent<IQuest>().StartQuest();
            }
        }
    }
    public void OnDestroy()
    {
        trapTriggers.Remove(this);
        SaveLoadControl.SaveEvent -= Save;
    }
    public void Save()
    {
        if (Equals(id, "")) { return; }
        SaveLoadControl.gameData.Remove(ref SaveLoadControl.gameData.triggers, id);
        SaveLoadControl.gameData.triggers.Add(new Trigger(triggered, id));
    }
    public void Load()
    {
        Trigger trigger = SaveLoadControl.gameData.GetData(ref SaveLoadControl.gameData.triggers, id);
        if(trigger == null) { return; }
        triggered = trigger.triggered;
    }
}
