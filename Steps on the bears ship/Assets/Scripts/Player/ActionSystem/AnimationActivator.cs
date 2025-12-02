using ActionDatabase;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class AnimationActivator : MonoBehaviour, IAction,IQuest
{
    public string id;

    //сохранить:
    public bool blockByQuest;// отключает возможность активировать нажатием анимацию до выполения задачи
    [HideInInspector] public bool status = false;
    [HideInInspector] public int currentState;
    //дальше не сохранять

    [SerializeField] public static List<AnimationActivator> animationActivators = new List<AnimationActivator>();

    private bool _wait = false;
    public Animator _animator;
    [SerializeField] private float _waitTime;
    [SerializeField] private string boolName = "";
    [SerializeField] private AudioDictionary _audioDictionary;
    [SerializeField] private bool _disabledByQuest;// полностью отключает возможность активировать нажатием анимацию
    private AudioSource _audioSource;
    public void Awake()
    {
        animationActivators.Add(this);
        _audioSource = GetComponent<AudioSource>();
        Load();
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
            if(!status)
            {
                if (!_audioDictionary.IsEmpty())
                {
                    _audioSource.clip = _audioDictionary.Find("True");
                    _audioSource.Play();
                }
                _animator.SetBool(boolName, true);
                status = true;
            }
            else
            {
                if (!_audioDictionary.IsEmpty())
                {
                    _audioSource.clip = _audioDictionary.Find("False");
                    _audioSource.Play();
                }
                _animator.SetBool(boolName, false);
                status = false;
            }
        }
    }
    public void StartEvent()
    {
        if(_wait || blockByQuest || _disabledByQuest) { return; }
        if (!status)
        {
            if (!_audioDictionary.IsEmpty())
            {
                _audioSource.clip = _audioDictionary.Find("True");
                _audioSource.Play();
            }
            _animator.SetBool(boolName, true);
            status = true;
        }
        else
        {
            if (!_audioDictionary.IsEmpty())
            {
                _audioSource.clip = _audioDictionary.Find("False");
                _audioSource.Play();
            }
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
        currentState = _animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
        SaveLoadControl.gameData.Remove(ref SaveLoadControl.gameData.animationActivators, id);
        SaveLoadControl.gameData.animationActivators.Add(new AnimationActivatorData(this));
    }
    public void Load()
    {
        AnimationActivatorData animationActivator = SaveLoadControl.gameData.GetData(ref SaveLoadControl.gameData.animationActivators,id);
        if (animationActivator == null) { return; }
        blockByQuest = animationActivator.blockByQuest;
        status = animationActivator.status;
        currentState = animationActivator.currentState;
        _animator.SetBool(boolName, status);
        _animator.Play(currentState);
    }
}
