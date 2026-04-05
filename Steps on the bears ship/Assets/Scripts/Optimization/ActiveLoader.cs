using ActionDatabase;
using System.Collections;
using UnityEngine;
public class ActiveLoader : MonoBehaviour
{
    public string id;

    //cохранять:
    public bool state = true;
    //дальше не сохранять
    public void Awake()
    {
        Load();
        SaveLoadControl.SaveEvent += Save;
    }
    public void SetActive(bool active)  
    { 
        state = active;
        gameObject.SetActive(active);
    }
    public void Active(float time)
    {
        state = true;
        StartCoroutine(Active_C(time));
    }
    public IEnumerator Active_C(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(true);
    }
    public void Disable(float time)
    {
        state = false;
        StartCoroutine(Disable_C(time));
    }
    public IEnumerator Disable_C(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        SaveLoadControl.SaveEvent -= Save;
    }
    public void Save()
    {
        if (Equals(id, "")) { return; }
        state = gameObject.activeSelf;
        SaveLoadControl.gameData.Remove(ref SaveLoadControl.gameData.activeLoaderDatas, id);
        SaveLoadControl.gameData.activeLoaderDatas.Add(new ActiveLoaderData(this));
    }
    public void Load()
    {
        ActiveLoaderData activeLoader = SaveLoadControl.gameData.GetData(ref SaveLoadControl.gameData.activeLoaderDatas, id);
        if (activeLoader == null) { gameObject.SetActive(state); return;}
        state = activeLoader.state;
        gameObject.SetActive(state);
    }
}

