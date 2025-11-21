using ActionDatabase;
using System.Collections;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class SmoothConnector : MonoBehaviour
{
    private AudioSource _audioSource;
    private bool _isPlaying;
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    private IEnumerator AudioFadeIn(AudioClip clip, float speed = 1f)
    {
        _audioSource.clip = clip;
        _audioSource.Play();
        _audioSource.volume = 0f;
        while (_audioSource.volume < 1)
        {
            _audioSource.volume += Time.deltaTime * speed;
            yield return new WaitForEndOfFrame();
        }
        yield break;
    }
    private IEnumerator AudioFadeOut(AudioClip clip, float speed = 1f)
    {
        while (_audioSource.volume > 0)
        {
            _audioSource.volume -= Time.deltaTime * speed;
            yield return new WaitForEndOfFrame();
        }
        _audioSource.Stop();
        if (clip != null)
        {
            StartCoroutine(AudioFadeIn(clip, speed));
        }
        yield break;
    }
    public void SetAudio(AudioClip clip, float speed = 1f)
    {
        StopAllCoroutines();
        if (_isPlaying)
        {
            StartCoroutine(AudioFadeOut(clip, speed));
        }
        else
        {
            StartCoroutine(AudioFadeIn(clip, speed));
            _isPlaying = true;
        }
    }
    public void Stop()
    {
        _isPlaying = false;
        StartCoroutine(AudioFadeOut(null, 1f));
    }
}
