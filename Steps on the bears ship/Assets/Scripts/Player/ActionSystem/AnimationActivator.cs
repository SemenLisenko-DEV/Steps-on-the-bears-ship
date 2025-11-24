using ActionDatabase;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AnimationActivator : MonoBehaviour, IAction,IQuest
{
    public string id;

    //сохранить:
    public bool blockByQuest;// отключает возможность активировать нажатием анимацию до выполения задачи
    [HideInInspector] public bool status = false;
    //дальше не сохранять

    [SerializeField, JsonIgnore] public static List<AnimationActivator> animationActivators = new List<AnimationActivator>();

    [JsonIgnore] private bool _wait = false;
    [JsonIgnore] public Animator _animator;
    [SerializeField, JsonIgnore] private float _waitTime;
    [SerializeField, JsonIgnore] private string boolName = "";
    [SerializeField, JsonIgnore] private AudioDictionary _audioDictionary;
    [SerializeField, JsonIgnore] private bool _disabledByQuest;// полностью отключает возможность активировать нажатием анимацию
    [JsonIgnore] private AudioSource _audioSource;
    public void Awake()
    {
        animationActivators.Add(this);
        Load();
        _audioSource = GetComponent<AudioSource>();
        SaveLoadControl.SaveEvent += Save;
    }
    public void Update()
    {
        if(Time.timeScale == 0) { _audioSource.Pause(); } else { _audioSource.UnPause(); }

    }
    public void StartQuest()
    {
        if (blockByQuest)
        {
            blockByQuest = false;
        }
        if(_disabledByQuest)
        {
            _audioSource.clip = _audioDictionary.Find("True");
            _audioSource.Play();
            _animator.SetBool(boolName, true);
        }
    }
    public void StartEvent()
    {
        if(_wait || blockByQuest || _disabledByQuest) { return; }
        if (!status)
        {
            _audioSource.clip = _audioDictionary.Find("True");
            _audioSource.Play();
            _animator.SetBool(boolName, true);
            status = true;
        }
        else
        {
            _audioSource.clip = _audioDictionary.Find("False");
            _audioSource.Play();
            _animator.SetBool(boolName, false);
            status = false;
        }
        StartCoroutine(Wait(_waitTime));
    }
    public IEnumerator Wait(float time)
    {
        _wait = true;
        float t = 0;
        while (t < time)
        {
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        _wait = false;
    }
    private void OnDestroy()
    {
        animationActivators.Remove(this);
        SaveLoadControl.SaveEvent -= Save;
    }
    public static AnimationActivator GetAnimationActivator(string id)
    {
        foreach (AnimationActivator i in animationActivators)
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
        if(Equals(id,"")) { return; }
        SaveLoadControl.gameData.Remove(ref SaveLoadControl.gameData.animationActivators, id);
        SaveLoadControl.gameData.animationActivators.Add(new AnimationActivatorData(this));
    }
    public void Load()
    {
        AnimationActivatorData animationActivator = SaveLoadControl.gameData.GetData(ref SaveLoadControl.gameData.animationActivators,id);
        if (animationActivator == null) { return; }
        blockByQuest = animationActivator.blockByQuest;
        status = animationActivator.status;
        _animator.SetBool(boolName, status);
    }
}
