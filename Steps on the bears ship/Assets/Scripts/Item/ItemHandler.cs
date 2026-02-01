using ActionDatabase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemHandler : MonoBehaviour,IAction
{
    public string id;

    //сохранить:
    [HideInInspector] public bool isTriggered = false;
    [HideInInspector] public string itemId = string.Empty;
    //дальше не сохранять

    public static List<ItemHandler> handlers = new List<ItemHandler>();

    [SerializeField] public AudioDictionary audioDictionary;
    [SerializeField] private bool canTakeOut = false;
    [SerializeField] private bool _triggerOnTakeOut = false;
    [SerializeField] public Transform _itemPosition;
    public ItemType allowedType;
    public Item item;

    [SerializeField,Header("Quest system")] private UnityEvent _questTakeIn;
    [SerializeField] private UnityEvent _questTakeOut;
    [SerializeField] private bool _triggerMultiply;
    [HideInInspector] public AudioSource audioSource;

    public void Awake()
    {
        handlers.Add(this);
        audioSource = GetComponent<AudioSource>();
        Load();
        SaveLoadControl.SaveEvent += Save;
    }
    public void StartEvent()
    {
        if (ItemPosition.haveItem && item == null) 
        {
            if (ItemPosition.item.itemType == allowedType && (!isTriggered || _triggerMultiply))
            {
                item = ItemPosition.item;
                StartCoroutine(item.MoveToLock(this));
                item.canPickUp = canTakeOut;
                item.handler = this;
                if(_questTakeIn != null && (!isTriggered || _triggerMultiply))
                {
                    isTriggered = true;
                    _questTakeIn.Invoke();
                }
            }
            else
            {
                ErrorSound();
            }
        }
    }
    public void ErrorSound()
    {
        audioSource.clip = audioDictionary.Find("Error");
        audioSource.Play();
    }
    public void TakeOut()
    {
        item.handler = null;
        item = null;
        itemId = "";
        audioSource.clip = audioDictionary.Find("UnLock");
        audioSource.Play();
        if (_questTakeIn != null && (!isTriggered || _triggerMultiply) && _triggerOnTakeOut)
        {
            isTriggered = true;
            _questTakeIn.Invoke();
        }
        if(_questTakeOut != null && (!isTriggered || _triggerMultiply))
        {
            isTriggered = true;
            _questTakeOut.Invoke();
        }
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
        if (Equals(id, "")) { return; }
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
        isTriggered = handler.isTriggered;
        itemId = handler.itemId;
        StartCoroutine(WaitForItemLoading());
    }
    public IEnumerator WaitForItemLoading()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        item = Item.GetItem(itemId);
        if (item != null)
        {
            if(item.isFlying) 
            {
                StartCoroutine(item.MoveToLock(this));
            }
            else
            {
                item.transform.position = _itemPosition.transform.position;
                item.transform.rotation = _itemPosition.transform.rotation;
                item.rigidBody.constraints = RigidbodyConstraints.FreezeAll;
            }
            item.canPickUp = canTakeOut;
            item.handler = this;
        }
    }
}
