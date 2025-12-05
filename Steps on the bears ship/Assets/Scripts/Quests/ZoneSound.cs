using ActionDatabase;
using System.Collections;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class ZoneSound : MonoBehaviour, IQuest
{
    private AudioSource _audioSource;
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    public void StartQuest()
    {
        if(_audioSource.isPlaying)
        {
            StartCoroutine(SoundLower());
        }
        else
        {
            _audioSource.Play();
        }
    }
    public void DisableQuest()
    {

    }
    public IEnumerator SoundLower() 
    {
        while (_audioSource.volume > 0)
        {
            _audioSource.volume -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        _audioSource.Stop();
        yield break;
    }
}
