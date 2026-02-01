using ActionDatabase;
using System.Collections;
using UnityEngine;

public class Flashlight : MonoBehaviour, IAction
{
    public string id = "FlashLight_MK";

    //сохранять
    public bool isActive = false;
    //дальше не сохранять

    public bool isPickUp = false;
    public float maxBright = 15f;
    public float minBright = 10f;
    [SerializeField]private AudioDictionary _audioDictionary;
    private CapsuleCollider _capsuleCollider;
    private Rigidbody _rb;
    private AudioSource _audioSource;
    public void Awake()
    {
        SaveLoadControl.SaveEvent += Save;
        _capsuleCollider = GetComponent<CapsuleCollider>();
        _rb = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
    }
    public void StartEvent()
    {
        Get();
    }
    public void Get()
    {
        isPickUp = true;
        transform.parent = PlayerControl.Instance.flashlightPosition;
        transform.position = PlayerControl.Instance.flashlightPosition.position;
        transform.rotation = PlayerControl.Instance.flashlightPosition.rotation;
        _capsuleCollider.enabled = false;
        _rb.constraints = RigidbodyConstraints.FreezeAll;
        StartCoroutine(WorkCorutina());
        StartCoroutine(DropChecker());
    } 
    public void Drop()
    {
        isPickUp = false;
        _capsuleCollider.enabled = true;
        _rb.constraints = RigidbodyConstraints.None;
        transform.parent = null;
        StopAllCoroutines();
    }
    public IEnumerator WorkCorutina()
    {
        yield return new WaitUntil(() => Input.GetButtonDown("FlashLight") || isActive);
        yield return new WaitForEndOfFrame();

        while (!Input.GetButtonDown("FlashLight"))
        {
            //float bright = UnityEngine.Random.Range(minBright,maxBright);
            //int inc = UnityEngine.Random.Range(0,100);
            //if(inc < 5f)
            //{
            //    br
            //}
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitUntil(() => Input.GetButtonUp("FlashLight"));
        StartCoroutine(WorkCorutina());
    }
    public IEnumerator DropChecker()
    {
        yield return new WaitUntil(() => Input.GetButtonDown("FlashLight_Drop"));
        Drop();
        yield return new WaitUntil(() => Input.GetButtonUp("FlashLight_Drop"));
    }
    private void OnCollisionEnter(Collision collision)
    {
        Noise.MakeNoise(transform.position, 20f, _audioDictionary.Find("Hit"));
    }
    private void OnDestroy()
    {
        SaveLoadControl.SaveEvent -= Save;
    }
    public void Save()
    {
        if(isPickUp)
        {
            SaveLoadControl.gameData.player.flashlightId = id;
        }
        SaveLoadControl.gameData.flashLights.Add(new FlashLightData(this));
    }
    public void Load()
    {
        FlashLightData flashLight = SaveLoadControl.gameData.GetData(ref SaveLoadControl.gameData.flashLights, id);
        transform.position = flashLight.position.ToVector3();
        transform.rotation = flashLight.rotation.ToQuaternion();
        isActive = flashLight.isActive;
        if (SaveLoadControl.gameData.player.flashlightId == id)
        {
            Get();
        }
    }
}
