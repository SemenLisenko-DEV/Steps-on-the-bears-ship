using ActionDatabase;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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
    }
    public void StartQuest()
    {
        if(!_active)
        {
            _smoothConnector.SetAudio(_audioDictionary.Find("Seek"),0.05f);
            _active = true;
            currentSeeker = this;
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
            return true;
        }
        return false;
    }
    private bool TargetNear(Vector3 target)
    {
        if (!Physics.Linecast(transform.position, target, _obstacleMask) && Vector3.Distance(target, transform.position) < 8)
        {
            return true;
        }
        return false;
    }
    private IEnumerator Seek()
    {
        Vector3 currentPoint = GetRandomPoint();
        _agent.SetDestination(currentPoint);
        Debug.Log("Seek");
        yield return new WaitUntil(() => Vector3.Distance(transform.position, currentPoint) < 1f);
        yield return new WaitForSeconds(3f);
        StartCoroutine(Seek());
    }
    public IEnumerator CheckPosition(Vector3 soundPosition)
    {
        if (_chasing) { yield break; }
        _anotherSounds.clip = _audioDictionary.Find("Distract");
        _anotherSounds.Play();
        _agent.SetDestination(soundPosition);
        while (TargetInFieldOfView(soundPosition))
        {
            yield return new WaitForEndOfFrame();
        }
    }
    private IEnumerator Chase()
    {
        yield return new WaitUntil(() => TargetInFieldOfView(PlayerControl.Instance.transform.position) || TargetNear(PlayerControl.Instance.transform.position));
        _smoothConnector.SetAudio(_audioDictionary.Find("Chase"), 1.5f);
        Debug.Log("Start chase!");
        StopCoroutine(Seek());
        _chasing = true;
        float timer = 3.5f;
        _agent.speed = _speedChase;
        bool inView = TargetInFieldOfView(PlayerControl.Instance.transform.position) || TargetNear(PlayerControl.Instance.transform.position);
        Vector3 lastPosition = PlayerControl.Instance.transform.position;
        while (timer > 0)
        {
            inView = TargetInFieldOfView(PlayerControl.Instance.transform.position) || TargetNear(PlayerControl.Instance.transform.position);
            if (inView)
            {
                _agent.SetDestination(PlayerControl.Instance.transform.position);
                lastPosition = PlayerControl.Instance.transform.position;
            }
            else
            {
                _agent.SetDestination(lastPosition);
            }
            timer = inView ? 3.5f : timer - Time.deltaTime;
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
        //Debug.Log("In view:" + (TargetInFieldOfView(PlayerControl.Instance.transform.position) || TargetNear(PlayerControl.Instance.transform.position)));
    }
}
