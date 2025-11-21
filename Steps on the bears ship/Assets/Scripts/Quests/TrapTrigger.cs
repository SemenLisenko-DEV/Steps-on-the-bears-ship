using ActionDatabase;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class TrapTrigger : MonoBehaviour
{
    [SerializeField] private GameObject[] _quests;
    private bool _triggerMultiply = false;
    private bool _triggered = false;
    private AudioSource _audioSource;
    public void OnTriggerEnter(Collider other)
    {
        
        if(!_triggered || _triggerMultiply)
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.Play();
            _triggered = true;
            for (int i = 0; i < _quests.Length;i++)
            {
                _quests[i].GetComponent<IQuest>().StartQuest();
            }
        }
    }
}
