using ActionDatabase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Valve : MonoBehaviour,IAction
{
    public string id;

    //сохранить:
    [HideInInspector] public float angle = 0;
    [HideInInspector] public bool complited = false;
    [HideInInspector] public Vector_Clear rotation;
    //дальше не сохранять

    public static List<Valve> valves = new List<Valve>();

    [SerializeField] private float _sensative = 1f;
    [SerializeField] private int _revolutCount = 1;
    [SerializeField] private GameObject _quest;
    [SerializeField] private UnityEvent _onComplite;
    [SerializeField] private AudioDictionary _audioDictionary;
    private float _angle = 0;
    public void Awake()
    {
        valves.Add(this);
        Load();
        SaveLoadControl.SaveEvent += Save;
    }
    public void StartEvent()
    {
        StartCoroutine(Pull());
    }
    public IEnumerator Pull()
    {
        if (complited) { yield break; }
        while (ActionsExecutor.actionExecuting)
        {
            float mouseX = Mathf.Abs(Input.GetAxis("Mouse X"));
            float mouseY = Mathf.Abs(Input.GetAxis("Mouse Y"));
            _angle += (mouseX + mouseY) * _sensative;
            if(_angle / 360 >= _revolutCount)
            {
                complited = true;
                break;
            }
            transform.Rotate(0, 0.0f, (mouseX + mouseY) * _sensative);
            yield return new WaitForEndOfFrame();
        }
        if (_angle / 360 >= _revolutCount && _quest != null)
        {
            complited = true;
            _onComplite.Invoke();
            Noise.MakeNoise(transform.position, 20f,_audioDictionary.Find("complite"));
        }
        yield break;
    }
    private void OnDestroy()
    {
        valves.Remove(this);
        SaveLoadControl.SaveEvent -= Save;
    }
    public static Valve GetValve(string id)
    {
        foreach (Valve i in valves)
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
        rotation = new Vector_Clear(transform.rotation);
        SaveLoadControl.gameData.Remove(ref SaveLoadControl.gameData.valves, id);
        SaveLoadControl.gameData.valves.Add(new ValveData(this));
    }
    public void Load()
    {
        ValveData valve = SaveLoadControl.gameData.GetData(ref SaveLoadControl.gameData.valves, id);
        if (valve == null) { return; }
        transform.rotation = valve.rotation.ToQuaternion();
        angle = valve.angle;
        complited = valve.complited;
    }
}
