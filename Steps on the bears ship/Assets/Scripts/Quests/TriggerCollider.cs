using ActionDatabase;
using UnityEngine;
using UnityEngine.Events;
public class TriggerCollider : MonoBehaviour
{
    public string id;
    [SerializeField] private bool _multiply = false;
    public UnityEvent action;
    private bool _triggered = false;
    public void Awake()
    {
        Load();
        SaveLoadControl.SaveEvent += Save;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!_triggered || _multiply)
        {
            _triggered = true;
            action.Invoke();
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
        if (triggerData == null) { return; }
        _triggered = triggerData.triggered;
    }
}
