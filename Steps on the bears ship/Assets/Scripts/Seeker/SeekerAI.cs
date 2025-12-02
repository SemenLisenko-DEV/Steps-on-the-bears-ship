using ActionDatabase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
[RequireComponent(typeof(NavMeshAgent))]
public class SeekerAI : MonoBehaviour,IQuest
{
    public static SeekerAI currentSeeker;
    
    [Serializable]
    private struct PathPoint
    {
        public int priority;
        public Transform point;
    }
    private bool _active = false;
    private bool _chasing = false;
    private NavMeshAgent _agent;

    [SerializeField] private bool _canSee = true;
    [SerializeField] private bool _canHear = true;
    [SerializeField] private float _flairDistance = 4;
    [SerializeField] private float _hearDistance = 30;
    [SerializeField] private AudioSource _stepsSource;
    [SerializeField] private AudioSource _anotherSounds;
    [SerializeField] private float _angleOfView = 90f;
    [SerializeField] private float _speedSeek = 3.5f;
    [SerializeField] private float _speedChase = 7f;
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private AudioDictionary _audioDictionary;
    [SerializeField] private PathPoint[] _path;
    [SerializeField] private SmoothConnector _smoothConnector;
    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _speedSeek;
    }
    public void StartQuest()
    {
        if(!_active)
        {
            _stepsSource.clip = _audioDictionary.Find("Walk");
            _stepsSource.Play();
            _smoothConnector.SetAudio(_audioDictionary.Find("Seek"),0.05f);
            _active = true;
            currentSeeker = this;
            if(_canHear)
            {
                Noise.OnMakeNoise += OnNoiseHear;
            }
            StartCoroutine(Seek());
            StartCoroutine(Chase());
        }
        else
        {
            _active = false;
            StopAllCoroutines();
            _smoothConnector.Stop();
        }
    }
    private Vector3 GetRandomPoint(int minPriority = 0,int maxPriority = int.MaxValue)
    {
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < _path.Length; i++)
        {
            if(_path[i].priority >= minPriority && _path[i].priority <= maxPriority)
            {
                points.Add(_path[i].point.position);
            }
        }
        return points[UnityEngine.Random.Range(0, points.Count)];
    }
    private bool TargetInFieldOfView(Vector3 target)
    {
        Vector3 directionToTarget = (target - transform.position).normalized;
        if (!Physics.Linecast(transform.position, target,_obstacleMask) && Vector3.Angle(transform.forward, directionToTarget) < _angleOfView / 2)
        {
            return _canSee;
        }
        return false;
    }
    private bool TargetNear(Vector3 target)
    {
        if (!Physics.Linecast(transform.position, target, _obstacleMask) && Vector3.Distance(target, transform.position) < _flairDistance)
        {
            return true;
        }
        return false;
    }
    public void OnNoiseHear(Vector3 position)
    {
        if(Vector3.Distance(position, transform.position) > _hearDistance) { return; }
        _anotherSounds.clip = _audioDictionary.Find("Distract");
        _anotherSounds.Play();
        StopCoroutine(Seek());
        StartCoroutine(CheckPosition(position));
    }

    private IEnumerator Seek()
    {
        if (_agent.speed == _speedChase) { yield break; }
        _agent.isStopped = false;
        Vector3 currentPoint = GetRandomPoint();
        _agent.SetDestination(currentPoint);
        yield return new WaitUntil(() => Vector3.Distance(transform.position, currentPoint) < 1f);
        if (_agent.speed == _speedChase) { yield break; }
        _agent.isStopped = true;
        yield return new WaitForSeconds(3f);
        if (_agent.speed == _speedChase) { yield break; }
        StartCoroutine(Seek());
    }
    public IEnumerator CheckPosition(Vector3 position)
    {
        if (_chasing) { yield break; }
        StopCoroutine(Seek());
        _agent.isStopped = false;
        _agent.SetDestination(position);
        _agent.speed = _speedChase;
        yield return new WaitUntil(() => Vector3.Distance(transform.position, position) < 3f);
        if (_chasing) { yield break; }
        _agent.isStopped = true;
        yield return new WaitForSeconds(1f);
        if (_chasing) { yield break; }
        _agent.speed = _speedSeek;
        StartCoroutine(Seek());
    }
    private IEnumerator Chase()
    {
        yield return new WaitUntil(() => TargetInFieldOfView(PlayerControl.Instance.transform.position)  || TargetNear(PlayerControl.Instance.transform.position));
        _smoothConnector.SetAudio(_audioDictionary.Find("Chase"), 1.5f);
        Debug.Log("Start chase!");
        StopCoroutine(Seek());
        _agent.isStopped = false;
        _chasing = true;
        float timer = 5f;
        _agent.speed = _speedChase;
        bool inView = TargetInFieldOfView(PlayerControl.Instance.transform.position) || TargetNear(PlayerControl.Instance.transform.position);
        Vector3 lastPosition = PlayerControl.Instance.transform.position;
        while (timer > 0)
        {
            inView = TargetInFieldOfView(PlayerControl.Instance.transform.position) || TargetNear(PlayerControl.Instance.transform.position);
            if (timer > 4.5f)
            {
                _agent.SetDestination(PlayerControl.Instance.transform.position);
                lastPosition = PlayerControl.Instance.transform.position;
            }
            else
            {
                _agent.SetDestination(lastPosition);
            }
            timer = inView ? 5f : timer - Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        _agent.speed = _speedSeek;
        _chasing = false;
        _smoothConnector.SetAudio(_audioDictionary.Find("Seek"),0.2f);
        StartCoroutine(Seek());
        StartCoroutine(Chase());
    }
    private void Update()
    {
        if (_active)
        {
            SaveLoadControl.blockSaving = true;
        }
        if (Time.timeScale == 0) { _stepsSource.Pause(); } else { _stepsSource.UnPause(); }
        if (_agent.speed == _speedChase)
        {
            _stepsSource.pitch = 1.5f;
        }
        else if (_agent.isStopped)
        {
            _stepsSource.pitch = 0f;
        }
        else
        {
            _stepsSource.pitch = 1f;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        PlayerControl playerControl;
        if(other.TryGetComponent(out playerControl))
        {
            playerControl.TakeDamage(13f);
        }
    }
    private void OnDestroy()
    {
        Noise.OnMakeNoise -= OnNoiseHear;
    }
}
