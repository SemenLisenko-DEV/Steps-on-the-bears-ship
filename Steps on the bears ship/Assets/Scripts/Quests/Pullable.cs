using ActionDatabase;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class Pullable : MonoBehaviour,IAction
{
    public string id;

    //сохранить:
    [HideInInspector] public Vector_Clear position;
    //дальше не сохранять

    [HideInInspector] public static List<Pullable> pullables = new List<Pullable>();

    private AudioSource _audioSource;
    [SerializeField] private AudioDictionary _audioDictionary;
    [SerializeField] private Vector3 _allowedDirection;
    [SerializeField] private Vector3 _pullableZone;
    [SerializeField] private Vector3 _startPositon;
    [SerializeField] private float _speedIntake = 1f;
    [SerializeField] private float _pitchMultiplier;

    private Rigidbody _rb;
    public void Awake()
    {
        pullables.Add(this);
        _rb = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>(); 
        pullables.Add(this);
        Load();
        SaveLoadControl.SaveEvent += Save;
    }
    public void StartEvent()
    {
        StartCoroutine(Pull());
    }
    public IEnumerator Pull()
    {
        PlayerControl.OnMove += Move;
        _audioSource.clip = _audioDictionary.Find("Pull");
        _audioSource.loop = true;
        _audioSource.Play();
        while (ActionsExecutor.actionExecuting)
        {
            PlayerControl.Instance.speedMultiplier = _speedIntake;
            yield return new WaitForFixedUpdate();
        }
        PlayerControl.OnMove -= Move;
        _rb.linearVelocity = Vector3.zero;
        _audioSource.loop = false;
        _audioSource.pitch = 1f;
        _audioSource.Stop();
        ActionsExecutor.stopExecuting = true;
        PlayerControl.Instance.speedMultiplier = 1f;
        yield break;
    }
    public void Move(Vector3 direction)
    {
        _rb.position += direction;
    }
    private void OnDestroy()
    {
        pullables.Remove(this);
        SaveLoadControl.SaveEvent -= Save;
    }
    public static Pullable GetPullable(string id)
    {
        foreach (Pullable i in pullables)
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
        position = new Vector_Clear(transform.position);
        SaveLoadControl.gameData.Remove(ref SaveLoadControl.gameData.pullable, id);
        SaveLoadControl.gameData.pullable.Add(new PullableData(this));
    }
    public void Load()
    {
        PullableData pullable = SaveLoadControl.gameData.GetData(ref SaveLoadControl.gameData.pullable,id);
        if (pullable == null) { return; }
        transform.position = pullable.position.ToVector3();
    }
}
