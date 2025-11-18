using ActionDatabase;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(Camera))]
public class ActionsExecutor : MonoBehaviour
{
    public static bool actionExecuting = false;
    public static RaycastHit executerLastHit;

    private Camera _camera;
    public LayerMask actionMask;

    [SerializeField] private float _maxRange = 2f;
    [SerializeField] private SpriteDictionary _spriteDictionary;
    [SerializeField] private AudioDictionary _audioDictionary;
    [SerializeField] private float _maxChargeTime = 2f;
    [SerializeField] private Image _chargeDisplay;
    [SerializeField] private Image _aim;

    private AudioSource _audioSource;
    private Pair<int, Sprite> _currentAim = new Pair<int, Sprite>();
    void Start()
    {
        SetAim(0, _spriteDictionary.find("defaultAim"));
        _audioSource = GetComponent<AudioSource>();
        _camera = GetComponent<Camera>();
        StartCoroutine(RayExecute());
        StartCoroutine(Throw());
    }
    public void FixedUpdate()
    {
        RaycastHit hit;
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit,_maxRange, actionMask);
        if (hit.collider != null)
        {
            IAction tip;
            if (hit.transform.TryGetComponent(out tip))
            {

                SetAim(1, _spriteDictionary.find("bearHand"));
            }
        }
        SetAim(0, _spriteDictionary.find("defaultAim"));
    }
    private void LateUpdate()
    {
        _aim.sprite = _currentAim.Second;
        _currentAim.First = -1;
    }
    public void SetAim(int priority, Sprite aim)
    {
        if(priority > _currentAim.First)
        {
            _currentAim.First = priority;
            _currentAim.Second = aim;
        }
    }
    public IEnumerator RayExecute()
    {
        yield return new WaitUntil(() => Input.GetButtonDown("MouseLeft"));

        if (Time.timeScale == 0)
        {
            yield return new WaitUntil(() => Input.GetButtonUp("MouseLeft"));
            StartCoroutine(RayExecute());
            yield break;
        }
        RaycastHit hit;
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit,_maxRange,actionMask);
        if(hit.collider != null)
        {
            IAction action;
            if (hit.transform.TryGetComponent(out action))
            {
                actionExecuting = true;
                action.StartEvent();
            }
        }
        while(Input.GetButton("MouseLeft") && Physics.Raycast(ray,out executerLastHit, _maxRange, actionMask))
        {
            ray = _camera.ScreenPointToRay(Input.mousePosition);
            actionExecuting = true;
            SetAim(3, _spriteDictionary.find("bearHand_Close"));
            yield return new WaitForEndOfFrame();
        }
        actionExecuting = false;
        StartCoroutine(RayExecute());
    }
    public IEnumerator Throw()
    {
        yield return new WaitUntil(() => Input.GetButtonDown("MouseRight") && ItemPosition.haveItem);
        _audioSource.volume = 1.0f;
        if (Time.timeScale == 0)
        {
            StartCoroutine(Throw());
            yield break;
        }

        _audioSource.clip = _audioDictionary.find("Charge");
        _audioSource.Play();
        float time = 0;
        while (Input.GetButton("MouseRight"))
        {
            time += time < _maxChargeTime ? Time.deltaTime : 0;
            _chargeDisplay.fillAmount = time / _maxChargeTime;
            _audioSource.volume = time / _maxChargeTime;
            yield return new WaitForEndOfFrame();
        }
        _audioSource.Stop();

        _audioSource.clip = _audioDictionary.find("Throw");
        _audioSource.Play();

        _chargeDisplay.fillAmount = 0f;

        Item item = ItemPosition.item;
        item.Drop();
        item.rigidBody.AddForce(transform.forward * time * 50f);

        StartCoroutine(Throw());
    }
}
