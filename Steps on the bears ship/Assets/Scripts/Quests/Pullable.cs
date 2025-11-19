using ActionDatabase;
using System.Collections;
using UnityEngine;

public class Pullable : MonoBehaviour,IAction
{
    private AudioSource _audioSource;
    [SerializeField] private AudioDictionary _audioDictionary;
    [SerializeField] private Vector3 _allowedDirection;
    [SerializeField] private Vector3 _pullableZone;
    [SerializeField] private Vector3 _startPositon;
    [SerializeField] private float _speedIntake = 1f;
    [SerializeField] private float _pitchMultiplier;
    public void Start()
    {
        _audioSource = GetComponent<AudioSource>(); 
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

}
