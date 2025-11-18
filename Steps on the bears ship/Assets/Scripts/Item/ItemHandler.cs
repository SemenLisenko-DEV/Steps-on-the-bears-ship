using ActionDatabase;
using UnityEngine;

public class ItemHandler : MonoBehaviour,IAction
{
    [SerializeField] private AudioDictionary _audioDictionary;
    [SerializeField] private bool canTakeOut = false;
    [SerializeField] private Transform _itemPosition;
    private AudioSource _audioSource;
    [HideInInspector] public Item item;
    public ItemType allowedType;
    public void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    public void StartEvent()
    {
        if (ItemPosition.haveItem && item == null) 
        {
            if (ItemPosition.item.itemType == allowedType )
            {
                item = ItemPosition.item;
                StartCoroutine(item.moveToLock(_itemPosition));
                item.canPickUp = canTakeOut;
                item.handler = this;
                _audioSource.clip = _audioDictionary.find("Lock");
                _audioSource.Play();
            }
            else
            {
                _audioSource.clip = _audioDictionary.find("Error");
                _audioSource.Play();
            }
        }
    }
    public void ErrorSound()
    {
        _audioSource.clip = _audioDictionary.find("Error");
        _audioSource.Play();
    }
    public void TakeOut()
    {
        item.handler = null;
        item = null;
        _audioSource.clip = _audioDictionary.find("UnLock");
        _audioSource.Play();
    }
}
