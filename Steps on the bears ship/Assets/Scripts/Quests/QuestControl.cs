using ActionDatabase;
using System.Collections.Generic;
using UnityEngine;

public class QuestControl : MonoBehaviour,IAction,IQuest
{
    public string id;

    //сохранить:
    public bool actionCanExecute = false;
    public int questCount = 0;
    [HideInInspector] public bool complited = false;
    //дальше не сохранять

    public ExecuteType executeType;
    public static List<QuestControl> questControls = new List<QuestControl>();
    public GameObject[] quests;
    [SerializeField]private int _questTargetCount = 1;
    [SerializeField] private AudioDictionary _audioDictionary;
    private AudioSource _audioSource;
    public void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        questControls.Add(this);
        Load();
        SaveLoadControl.SaveEvent += Save;
    }
    public void StartEvent()
    {
        if (!actionCanExecute || complited) { return; }
        questCount++;
        
        if(_audioSource != null)
        {
            _audioSource.clip = _audioDictionary.Find("Activate");
            _audioSource.Play();
        }
        switch(executeType)
        {
            case ExecuteType.all:
                if (questCount >= _questTargetCount)
                {
                    complited = true;
                    foreach (var quest in quests)
                    {
                        quest.GetComponent<IQuest>().StartQuest();
                    }
                }
                break;
            case ExecuteType.anything:
                complited = true;
                foreach (var quest in quests)
                {
                    quest.GetComponent<IQuest>().StartQuest();
                }
                break;
        }
    }
    public void StartQuest()
    {
        if (complited) { return; }
        questCount++;
        switch (executeType)
        {
            case ExecuteType.all:
                if (questCount >= _questTargetCount)
                {
                    complited = true;
                    foreach (var quest in quests)
                    {
                        quest.GetComponent<IQuest>().StartQuest();
                    }
                }
                break;
            case ExecuteType.anything:
                complited = true;
                foreach (var quest in quests)
                {
                    quest.GetComponent<IQuest>().StartQuest();
                }
                break;
        }
    }
    private void OnDestroy()
    {
        questControls.Remove(this);
        SaveLoadControl.SaveEvent -= Save;
    }
    public static QuestControl GetValve(string id)
    {
        foreach (QuestControl i in questControls)
        {
            if (Equals(i.id, id))
            {
                return i;
            }
        }
        return null;
    }
    public void Save()
    {
        if (Equals(id, "")) { return; }
        SaveLoadControl.gameData.Remove(ref SaveLoadControl.gameData.quests, id);
        SaveLoadControl.gameData.quests.Add(new QuestData(this));
    }
    public void Load()
    {
        QuestData quest = SaveLoadControl.gameData.GetData(ref SaveLoadControl.gameData.quests, id);
        if (quest == null) { return; }
        actionCanExecute = quest.actionCanExecute;
        complited = quest.complited;
        questCount = quest.questCount;
    }
}
