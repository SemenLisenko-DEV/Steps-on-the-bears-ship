using ActionDatabase;
using UnityEngine;

public class ItemHandler : MonoBehaviour,IAction
{
    [SerializeField] private AudioDictionary _audioDictionary;
    [SerializeField] private bool canTakeOut = false;
    [SerializeField] private Transform _itemPosition;
    public ItemType allowedType;
    [SerializeField,Header("Quest system")] private GameObject _questTarget;
    [SerializeField] private bool _triggerMultiply;
    private bool _isTriggered = false;
    private AudioSource _audioSource;
    [HideInInspector] public Item item;
    public void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    public void StartEvent()
    {
        if (ItemPosition.haveItem && item == null) 
        {
            if (ItemPosition.item.itemType == allowedType)
            {
                item = ItemPosition.item;
                StartCoroutine(item.moveToLock(_itemPosition));
                item.canPickUp = canTakeOut;
                item.handler = this;
                _audioSource.clip = _audioDictionary.Find("Lock");
                _audioSource.Play();
                if(_questTarget != null && (!_isTriggered || _triggerMultiply))
                {
                    _isTriggered = true;
                    _questTarget.GetComponent<IQuest>().StartQuest();
                }
            }
            else
            {
                _audioSource.clip = _audioDictionary.Find("Error");
                _audioSource.Play();
            }
        }
    }
    public void ErrorSound()
    {
        _audioSource.clip = _audioDictionary.Find("Error");
        _audioSource.Play();
    }
    public void TakeOut()
    {
        item.handler = null;
        item = null;
        _audioSource.clip = _audioDictionary.Find("UnLock");
        _audioSource.Play();
    }
}
