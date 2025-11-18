using ActionDatabase;
using System.Collections;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour, IAction
{
    public ItemType itemType;
    [SerializeField] private AudioDictionary _audioClips;
    [HideInInspector] public Rigidbody rigidBody;
    private bool _isPickUp = false;

    [HideInInspector] public ItemHandler handler = null;
    public bool canPickUp = true;

    private IEnumerator _followPlayerCoroutine;
    [SerializeField] public AudioSource audioSource = null;
    public void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();
        _followPlayerCoroutine = FollowPlayer();
        rigidBody = GetComponent<Rigidbody>();
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
        audioSource.clip = _audioClips.find("PickUp");
        audioSource.Play();
        _isPickUp = true;
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
            float dist = Vector3.Distance(transform.position, ItemPosition._transform.position) + 50f;
            rigidBody.linearVelocity = -(transform.position - ItemPosition._transform.position) * dist * dist * Time.deltaTime;
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
        _isPickUp = false;
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
    private void OnDestroy()
    {
        if(_isPickUp)
        {
            ItemPosition.ItemsDrops -= Drop;
            ItemPosition.haveItem = false;
        }
    }
}
