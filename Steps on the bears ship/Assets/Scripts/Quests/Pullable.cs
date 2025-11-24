using ActionDatabase;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pullable : MonoBehaviour,IAction
{
    public string id;

    //сохранить:
    [HideInInspector] public Vector_Clear position;
    //дальше не сохранять

    [HideInInspector, JsonIgnore] public static List<Pullable> pullables = new List<Pullable>();

    private AudioSource _audioSource;
    [SerializeField, JsonIgnore] private AudioDictionary _audioDictionary;
    [SerializeField, JsonIgnore] private Vector3 _allowedDirection;
    [SerializeField, JsonIgnore] private Vector3 _pullableZone;
    [SerializeField, JsonIgnore] private Vector3 _startPositon;
    [SerializeField, JsonIgnore] private float _speedIntake = 1f;
    [SerializeField, JsonIgnore] private float _pitchMultiplier;
    public void Awake()
    {
        pullables.Add(this);
        Load();
        _audioSource = GetComponent<AudioSource>(); 
        pullables.Add(this);
        SaveLoadControl.SaveEvent += Save;
    }
    public void StartEvent()
    {
        StartCoroutine(Pull());
    }
    public IEnumerator Pull()
    {
        PlayerControl.Instance.speedMultiplier = _speedIntake;
        Vector3 previous = PlayerControl.Instance.transform.position;
        yield return new WaitForEndOfFrame();
        _audioSource.clip = _audioDictionary.Find("Pull");
        _audioSource.Play();
        _audioSource.loop = true;
        while (ActionsExecutor.actionExecuting)
        {
            Vector3 direction = Vector3.zero;
            direction.x = _allowedDirection.x * (previous.x - PlayerControl.Instance.transform.position.x);
            direction.y = _allowedDirection.y * (previous.y - PlayerControl.Instance.transform.position.y);
            direction.z = _allowedDirection.z * (previous.z - PlayerControl.Instance.transform.position.z);
            if ((_startPositon.x + _pullableZone.x < transform.position.x && direction.x < 0) || (_startPositon.x - _pullableZone.x > transform.position.x && direction.x > 0))
            {
                direction.x *= 0;
            }
            if ((_startPositon.y + _pullableZone.y < transform.position.y && direction.y < 0) || (_startPositon.y - _pullableZone.y > transform.position.y && direction.y > 0))
            {
                direction.y *= 0;
            }
            if ((_startPositon.z + _pullableZone.z < transform.position.z && direction.z < 0) || (_startPositon.z - _pullableZone.z > transform.position.z && direction.z > 0))
            {
                direction.z *= 0;
            }
            transform.position -= direction;
            float pitch = Vector3.Distance(previous, PlayerControl.Instance.transform.position);
            _audioSource.pitch = pitch < 0.5f? 0 : 1f;
            previous = PlayerControl.Instance.transform.position;
            yield return new WaitForEndOfFrame();
        }
        _audioSource.loop = false;
        _audioSource.pitch = 1f;
        _audioSource.Stop();
        PlayerControl.Instance.speedMultiplier = 1f;
        yield break;
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
