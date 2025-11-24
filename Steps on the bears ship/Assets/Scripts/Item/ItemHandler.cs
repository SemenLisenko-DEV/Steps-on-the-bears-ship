using ActionDatabase;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemHandler : MonoBehaviour,IAction
{
    public string id;

    //сохранить:
    [HideInInspector] public bool isTriggered = false;
    [HideInInspector] public string itemId = string.Empty;
    //дальше не сохранять

    public static List<ItemHandler> handlers = new List<ItemHandler>();

    [SerializeField] private AudioDictionary _audioDictionary;
    [SerializeField] private bool canTakeOut = false;
    [SerializeField] private Transform _itemPosition;
    public ItemType allowedType;

    [SerializeField,Header("Quest system")] private GameObject _questTarget;
    [SerializeField] private bool _triggerMultiply;
    private AudioSource _audioSource;

    [HideInInspector, JsonIgnore] public Item item;
    public void Awake()
    {
        handlers.Add(this);
        Load();
        _audioSource = GetComponent<AudioSource>();
        SaveLoadControl.SaveEvent += Save;
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
                if(_questTarget != null && (!isTriggered || _triggerMultiply))
                {
                    isTriggered = true;
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
    private void OnDestroy()
    {
        handlers.Remove(this);
        SaveLoadControl.SaveEvent -= Save;
    }
    public static ItemHandler GetItemHandler(string id)
    {
        foreach (ItemHandler i in handlers)
        {
            if (Equals(i.id, id))
            {
                return i;
            }
        }
        return null;
    }
    public void Save()
    {
        if (item != null)
        {
            itemId = item.id;
        }
        SaveLoadControl.gameData.Remove(ref SaveLoadControl.gameData.handlers, id);
        SaveLoadControl.gameData.handlers.Add(new ItemHandlerData(this));
    }
    public void Load()
    {
        ItemHandlerData handler = SaveLoadControl.gameData.GetData(ref SaveLoadControl.gameData.handlers, id);
        if (handler == null) { return; }
        item = Item.GetItem(handler.itemId);
        if(item != null)
        {
            item.handler = this;
        }
        isTriggered = handler.isTriggered;
    }
}
