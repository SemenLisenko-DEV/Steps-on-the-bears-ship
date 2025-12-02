using ActionDatabase;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour, IAction
{
    public string id;

    //сохранить:
    [HideInInspector] public bool isPickUp = false;
    public bool canPickUp = true;
    [HideInInspector] public string handlerId;
    [HideInInspector] public Vector_Clear position;
    [HideInInspector] public Vector_Clear rotation;
    //дальше не сохранять

    public static List<Item> items = new List<Item>();

    public ItemType itemType;
    [SerializeField] private AudioDictionary _audioClips;
    [HideInInspector] public Rigidbody rigidBody;

    [HideInInspector] public ItemHandler handler = null;

    private IEnumerator _followPlayerCoroutine;
    [SerializeField] public AudioSource audioSource = null;
    public void Awake()
    {
        items.Add(this);
        audioSource = GetComponent<AudioSource>();
        _followPlayerCoroutine = FollowPlayer();
        rigidBody = GetComponent<Rigidbody>();
        Load();
        SaveLoadControl.SaveEvent += Save;

    }
    public void StartEvent()
    {
        PickUp();
    }
    public void PickUp()
    {
        if (!canPickUp || ItemPosition.haveItem)
        {
            if (handler != null)
            {
                handler.ErrorSound();
            }
            return; 
        }
        if (handler != null) { handler.TakeOut(); }
        UnLock();
        audioSource.clip = _audioClips.Find("PickUp");
        audioSource.Play();
        isPickUp = true;
        rigidBody.useGravity = false;
        ItemPosition.item = this;
        ItemPosition.haveItem = true;
        ItemPosition.ItemsDrops += Drop;
        StartCoroutine(_followPlayerCoroutine);
    }
    public IEnumerator FollowPlayer()
    {
        while (true)
        {
            float speed = Vector3.Distance(transform.position, ItemPosition._transform.position) + 50f;
            speed = speed > 250? 250 : speed;
            rigidBody.linearVelocity = -(transform.position - ItemPosition._transform.position) * speed ;
            rigidBody.angularVelocity = new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z) * 10;
            yield return new WaitForEndOfFrame();
        }
    }
    public void Drop()
    {
        ItemPosition.ItemsDrops -= Drop;
        StopAllCoroutines();
        rigidBody.linearVelocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
        ItemPosition.haveItem = false;
        ItemPosition.item = null;
        isPickUp = false;
        rigidBody.useGravity = true;
    }
    public IEnumerator moveToLock(Transform target)
    {
        Drop();
        while (Vector3.Distance(transform.position, target.position) > 0.2f)
        {
            rigidBody.linearVelocity = -(transform.position - target.position) * 10;
            yield return new WaitForEndOfFrame();
        }
        transform.position = target.position;
        transform.rotation = target.rotation;
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;
    }
    public void Lock()
    {
        rigidBody.constraints = RigidbodyConstraints.FreezeAll;
    }
    public void UnLock()
    {
        rigidBody.constraints = RigidbodyConstraints.None;
    }
    public static Item GetItem(string id)
    {
        foreach(Item i in items)
        {
            if(Equals(i.id,id))
            {
                return i;
            }
        }
        return null;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(isPickUp || handler != null) { return; }
        Noise.MakeNoise(transform.position, 20f, _audioClips.Find("Hit"));
    }
    private void OnDestroy()
    {
        if(isPickUp)
        {
            ItemPosition.ItemsDrops -= Drop;
            ItemPosition.haveItem = false;
        }
        SaveLoadControl.SaveEvent -= Save;
        items.Remove(this);
    }
    public void Save()
    {
        if (Equals(id, "")) { return; }
        handlerId = handler?.id;
        position = new Vector_Clear(transform.position);
        rotation = new Vector_Clear(transform.rotation);
        SaveLoadControl.gameData.Remove(ref SaveLoadControl.gameData.items,id);
        SaveLoadControl.gameData.items.Add(new ItemData(this));
    }
    public void Load()
    {
        ItemData item = SaveLoadControl.gameData.GetData(ref SaveLoadControl.gameData.items,id);
        if(item == null) { return; }
        isPickUp = item.isPickUp;
        canPickUp = item.canPickUp;
        handlerId = item.handlerId;
        transform.position = item.position.ToVector3();
        transform.rotation = item.rotation.ToQuaternion();
        if (handlerId != "") { handler = ItemHandler.GetItemHandler(handlerId); }
        if (!canPickUp && handler != null) { StartCoroutine(moveToLock(handler.transform)); }
    }
}
