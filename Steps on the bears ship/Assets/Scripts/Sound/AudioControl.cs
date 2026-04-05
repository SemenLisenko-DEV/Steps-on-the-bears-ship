using UnityEngine;
[RequireComponent (typeof(AudioSource))]
public class AudioControl : MonoBehaviour
{
    private AudioSource _audioSource;
    public void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    private void Update()
    {
        if(Time.deltaTime == 0) { _audioSource.Pause(); } else { _audioSource.UnPause(); }
    }
}
