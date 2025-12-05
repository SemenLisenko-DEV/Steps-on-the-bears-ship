using ActionDatabase;
using UnityEngine;
public class HintTrigger : MonoBehaviour
{
    public string id;
    public string key;
    private bool _triggered = false;
    public void Awake()
    {
        Load();
        SaveLoadControl.SaveEvent += Save;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(!_triggered)
        {
            _triggered = true;
            HintControl.Instance.SetHint(key);
        }
    }
    public void OnDestroy()
    {
        SaveLoadControl.SaveEvent -= Save;
    }
    public void Save()
    {
        SaveLoadControl.gameData.Remove(ref SaveLoadControl.gameData.triggers, id);
        SaveLoadControl.gameData.triggers.Add(new Trigger(_triggered, id));
    }
    public void Load()
    {
        Trigger triggerData = SaveLoadControl.gameData.GetData(ref SaveLoadControl.gameData.triggers, id);
        if(triggerData == null) { return; }
        _triggered = triggerData.triggered;
    }
}
