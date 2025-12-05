using ActionDatabase;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class SaveTrigger : MonoBehaviour, IQuest
{
    [SerializeField] private bool _saved = false;
    private void OnTriggerStay(Collider other)
    {
        if(!_saved && !SaveLoadControl.blockSaving)
        {
            bool buffer = SaveLoadControl.blockSaving;
            SaveLoadControl.blockSaving = false;
            _saved = true;
            SaveLoadControl.Instance.AutoSave();
            SaveLoadControl.blockSaving = buffer;
        }
    }
    public void StartQuest()
    {
        StartCoroutine(SaveWithTime());
    }
    public IEnumerator SaveWithTime()
    {
        yield return new WaitForEndOfFrame();
        bool buffer = SaveLoadControl.blockSaving;
        SaveLoadControl.blockSaving = false;
        _saved = true;
        SaveLoadControl.Instance.AutoSave();
        SaveLoadControl.blockSaving = buffer;
    }
    public void DisableQuest()
    {

    }
}
