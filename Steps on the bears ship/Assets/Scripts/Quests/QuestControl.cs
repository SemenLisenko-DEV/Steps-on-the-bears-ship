using ActionDatabase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestControl : MonoBehaviour,IAction,IQuest
{
    public string id;

    //сохранить:
    public bool actionCanExecute = false;
    public bool rechageBlock = false;
    public int questCount = 0;
    [HideInInspector] public bool complited = false;
    //дальше не сохранять

    public ExecuteType executeType;
    public bool allwaysExecuteAction = false;
    public bool multiComplite;
    public static List<QuestControl> questControls = new List<QuestControl>();
    public UnityEvent onComplite;
    public UnityEvent onDecomplite;
    [SerializeField] private int _questTargetCount = 1;
    [SerializeField] private float _reChargeTime = 1;
    [SerializeField] private AudioDictionary _audioDictionary;
    private AudioSource _audioSource;
    public void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        questControls.Add(this);
        Load();
        SaveLoadControl.SaveEvent += Save;
    }
    public void ReCharge()
    {
        StartCoroutine(Charging());
    }
    public IEnumerator Charging()
    {
        rechageBlock = true;
        yield return new WaitForSeconds(_reChargeTime);
        rechageBlock = false;
    }
    public void StartEvent()
    {
        if(rechageBlock)
        {
            if (_audioSource != null)
            {
                _audioSource.clip = _audioDictionary.Find("Block");
                _audioSource.Play();
            }
            return;
        }
        if(!actionCanExecute && !allwaysExecuteAction) { Debug.Log("QUEST CONTROL: " + id + " BLOCKED BY ACTIONCANEXECUTE: " + actionCanExecute); ; return; }
        if (!complited)
        {
            Debug.Log("QUEST CONTROL: " + id + " WAS EXECUTED BY ACTION SYSTEM. ENABLE");
            questCount++;
            switch (executeType)
            {
                case ExecuteType.all:
                    if (questCount >= _questTargetCount)
                    {
                        complited = true;
                        onComplite.Invoke();
                    }
                    break;
                case ExecuteType.anything:
                    complited = true;
                    onComplite.Invoke();
                    break;
                case ExecuteType.allways:
                    complited = true;
                    onComplite.Invoke();
                    break;
            }
        }
        else
        {
            Debug.Log("QUEST CONTROL: " + id + " WAS EXECUTED BY ACTION SYSTEM. DISABLE");
            questCount--;
            switch (executeType)
            {
                case ExecuteType.all:
                    if (questCount >= _questTargetCount && complited)
                    {
                        complited = false;
                        onDecomplite.Invoke();
                    }
                    break;
                case ExecuteType.anything:
                    if (questCount <= 0)
                    {
                        complited = false;
                        onDecomplite.Invoke();
                    }
                    break;
                case ExecuteType.allways:
                    complited = false;
                    onDecomplite.Invoke();
                    break;
            }
        }

        if (_audioSource != null)
        {
            _audioSource.clip = _audioDictionary.Find("Activate");
            _audioSource.Play();
        }
    }
    public void StartQuest()
    {
        questCount += complited? 0 : 1;
        switch (executeType)
        {
            case ExecuteType.all:

                if (questCount >= _questTargetCount && !complited)
                {
                    complited = true;
                    onComplite.Invoke();
                }
                else
                {
                    return;
                }
                break;
            case ExecuteType.anything:
                if (complited) { return; }
                complited = true;
                onComplite.Invoke();
                break;
            case ExecuteType.allways:
                complited = true;
                onComplite.Invoke();
                break;
        }
    }
    public void DisableQuest()
    {
        questCount -= questCount <= 0? 0 : 1;
        switch (executeType)
        {
            case ExecuteType.all:
                if (questCount >= _questTargetCount && complited)
                {
                    complited = false;
                    onDecomplite.Invoke();
                }
                break;
            case ExecuteType.anything:
                if (questCount <= 0)
                {
                    complited = false;
                    onDecomplite.Invoke();
                }
                break;
            case ExecuteType.allways:
                complited = false;
                onDecomplite.Invoke();
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
