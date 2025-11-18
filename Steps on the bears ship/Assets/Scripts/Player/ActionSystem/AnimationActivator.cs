using ActionDatabase;
using System.Collections;
using UnityEngine;
public class AnimationActivator : MonoBehaviour, IAction
{
    private bool _wait = false;
    public Animator _animator;
    [SerializeField] private float _waitTime;
    [SerializeField]public string boolName = "";
    [SerializeField] private AudioDictionary _audioDictionary;
    private AudioSource _audioSource;

    private bool status = false;
    public void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    public void Update()
    {
        if(Time.timeScale == 0) { _audioSource.Pause(); } else { _audioSource.UnPause(); }

    }
    public void StartEvent()
    {
        if(_wait) { return; }
        if (!status)
        {
            _audioSource.clip = _audioDictionary.find("True");
            _audioSource.Play();
            _animator.SetBool(boolName, true);
            status = true;
        }
        else
        {
            _audioSource.clip = _audioDictionary.find("False");
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
}
