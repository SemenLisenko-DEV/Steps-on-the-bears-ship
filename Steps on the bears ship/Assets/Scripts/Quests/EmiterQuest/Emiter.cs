using ActionDatabase;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class Emiter : MonoBehaviour,IAction
{
    public string id = "_Emiter_";

    //сохранить:
    [HideInInspector] public bool disabled = false;
    [HideInInspector] public bool takingBeam = false;
    [HideInInspector] public bool complited = false;
    [HideInInspector] public int currentRotation = 0;
    [HideInInspector] public Vector_Clear color;
    [HideInInspector] public Vector_Clear rotation;
    //дальше не сохранять 
    public bool allwaysCast = false;
    public bool isEnd = false;
    public float[] rotations;
    [SerializeField] private EmiterExit[] _emiterExits;
    public EmitersFamily emitersFamily;
    private bool wait = false;
    public AudioDictionary audioDictionary;
    [HideInInspector] public AudioSource audioSource;
    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (isEnd)
        {
            emitersFamily.endCount++;
        }
        emitersFamily.emiters.Add(this);
        Load();
        SaveLoadControl.SaveEvent += Save;
        if (disabled)
        {
            DeleteAllBeams();
            return;
        }
        if (allwaysCast)
        {
            StartCoroutine(InvokeWithTime(0.2f));
        }
    }
    public void StartEvent()
    {
        if(wait || complited)
        {
            Debug.Log(complited + " " + wait);
            audioSource.clip = audioDictionary.Find("OutOffPermision");
            audioSource.Play();
            return;
        }
        audioSource.clip = audioDictionary.Find("Rotate");
        audioSource.Play();
        RotateToNext();
    }
    public void RotateToNext()
    {
        RotateTo(currentRotation + 1);
    }
    public void RotateToPrevious()
    {
        RotateTo(currentRotation - 1);
    }
    public void RotateTo(int index)
    {
        for (int i = 0; i < emitersFamily.emiters.Count; i++)
        {
            emitersFamily.emiters[i].StartCoroutine(Wait(1));
        }
        currentRotation = index;
        if (index >= rotations.Length)
        {
            currentRotation = 0;
        }
        if (index < 0)
        {
            currentRotation = rotations.Length - 1;
        }
        for (int i = 0; i < emitersFamily.emiters.Count; i++)
        {
            emitersFamily.emiters[i].DeleteAllBeams();
        }
        transform.localRotation = Quaternion.Euler(rotations[currentRotation], 0, 0);

        StartCoroutine(InvokeWithTime(1));
    }
    public void InvokeReset(float time)
    {
        if (disabled) {  return; }
        StartCoroutine(InvokeWithTime(time));
    }
    public IEnumerator InvokeWithTime(float time)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(time);
        ResetAllBeams();
    }
    public void DrawAllBeams()
    {
        if(disabled) { return; }

        CheckComplite();

        if (isEnd) { return; }
        Debug.Log(id + " WAS DRAW ALL BEAMS");
        for (int i = 0;i < _emiterExits.Length; i++)
        {
            _emiterExits[i].DrawBeam();
        }
    }
    public void DeleteAllBeams()
    {
        for (int i = 0; i < _emiterExits.Length; i++)
        {
            _emiterExits[i].DeleteBeam();
        }
    }
    public void CheckComplite()
    {
        if (complited || !isEnd) { return; }
        if(takingBeam)
        {
            emitersFamily.activeEnd++;
        }
        if (emitersFamily.endCount == emitersFamily.activeEnd)
        {
            complited = true;
            emitersFamily.onComplite.Invoke();
            audioSource.clip = audioDictionary.Find("OnComplite");
            audioSource.Play();
            foreach (Emiter emiter in emitersFamily.emiters)
            {
                emiter.complited = true;
            }
        }
    }
    private void ResetAllBeams()
    {
        emitersFamily.activeEnd = 0;
        for (int i = 0; i < emitersFamily.emiters.Count; i++)
        {
            emitersFamily.emiters[i].takingBeam = false;
            emitersFamily.emiters[i].DeleteAllBeams();
        }
        for (int i = 0; i < emitersFamily.emiters.Count; i++)
        {
            if(emitersFamily.emiters[i].allwaysCast)
            {
                emitersFamily.emiters[i].DrawAllBeams();
            }
        }
    }
    public IEnumerator Wait(float time)
    {
        wait = true;
        yield return new WaitForSeconds(time);
        wait = false;
    }
    private void OnDestroy()
    {
        SaveLoadControl.SaveEvent -= Save;
    }
    public void Save()
    {
        if (Equals(id, "")) { return; }
        rotation = new Vector_Clear(transform.rotation);
        SaveLoadControl.gameData.Remove(ref SaveLoadControl.gameData.emiterDatas, id);
        SaveLoadControl.gameData.emiterDatas.Add(new EmiterData(this));
    }
    public void Load()
    {
        EmiterData emiter = SaveLoadControl.gameData.GetData(ref SaveLoadControl.gameData.emiterDatas, id);
        if (emiter == null) { return; }
        color = emiter.color;
        complited = emiter.complited;
        takingBeam = emiter.takingBeam;
        transform.rotation = emiter.rotation.ToQuaternion();
        currentRotation = emiter.currentRotation;
        disabled = emiter.disabled;
        if(isEnd && takingBeam)
        {
            emitersFamily.activeEnd++;
        }
    }
}
