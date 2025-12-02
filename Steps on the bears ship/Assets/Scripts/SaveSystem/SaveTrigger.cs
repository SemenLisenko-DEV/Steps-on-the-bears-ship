using UnityEngine;
using UnityEngine.VFX;

public class SaveTrigger : MonoBehaviour
{
    [SerializeField] private bool _saved = false;
    private void OnTriggerEnter(Collider other)
    {
        if(!_saved)
        {
            bool buffer = SaveLoadControl.blockSaving;
            SaveLoadControl.blockSaving = false;
            _saved = true;
            SaveLoadControl.Instance.AutoSave();
            SaveLoadControl.blockSaving = buffer;
        }
    }
}
