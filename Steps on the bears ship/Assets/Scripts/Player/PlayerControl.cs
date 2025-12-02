using ActionDatabase;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    public static PlayerControl Instance { get; private set; }

    [HideInInspector] public string floorType = "default";
    [SerializeField] private Transform _camera;
    [SerializeField] private AudioDictionary _audioDictionary;
    [SerializeField] private LayerMask _obstacleMask;

    [SerializeField, Header("Здоровье")] private float _maxRegeneration = 5f;
    private float _regeneration;
    [SerializeField] private float _maxHealth = 100f;
    private float _health;
    [SerializeField] private Image _image;
    [SerializeField, Header("Передвижение")] private float _squatHeight = 1f;
    [SerializeField] private float _standHeight = 2f;
    [SerializeField] private float _squatSpeed = 2.5f;
    [SerializeField] private float _standSpeed = 5f;
    [SerializeField] private float _gravity = -9.81f;
    public float moveSpeed = 5.0f;

    [HideInInspector] public float speedMultiplier = 1f;
    [HideInInspector] public string itemId = "";
    private bool _canStand = true;
    private Vector3 yDirection;
    private Vector3 pos;
    private AudioSource _audioSource;
    [SerializeField] private AudioSource _voiceSource;
    private CharacterController _controller;
    private float _coughTimer = 0f;
    private float _crutchTimer = 0;
    private void Awake()
    {
        Instance = this;
        Load();
        _regeneration = _maxRegeneration;
        _health = _maxHealth;
        _audioSource = GetComponent<AudioSource>();
        _controller = GetComponent<CharacterController>();
        StartCoroutine(Squat());
        SaveLoadControl.SaveEvent += Save;
    }
    private void Update()
    {
        _crutchTimer += _crutchTimer > 5 ? 0 : 1;
        if (Time.timeScale == 0 || _crutchTimer < 5) { _audioSource.pitch = 0; _voiceSource.Pause(); return; } else { _voiceSource.UnPause(); }

        if (_health / _maxHealth < 0.75f && _coughTimer < 0)
        {
            _coughTimer = 5f;
            _voiceSource.clip = _audioDictionary.Find("Damaged");
            _voiceSource.Play();
        }
        else
        {
            _coughTimer -= _coughTimer < 0 ? 0 : Time.deltaTime;
        }

        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 1 - _health / _maxHealth);
        _health += _health + Time.deltaTime * _maxRegeneration > _maxHealth? 0 : Time.deltaTime * _maxRegeneration;

        _canStand = !Physics.Raycast(transform.position + transform.up * _controller.height / 2, transform.up, 2f, _obstacleMask);

        _audioSource.pitch = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0? ((moveSpeed * speedMultiplier) / _standSpeed) : 0;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;

        _controller.Move(moveDirection * (moveSpeed * speedMultiplier) * Time.deltaTime + yDirection * Time.deltaTime);

    }
    private IEnumerator Squat()
    {
        yDirection = transform.up * _gravity;
        yield return new WaitUntil(() => Input.GetButtonDown("Squat"));

        moveSpeed = _squatSpeed;
        SaveLoadControl.blockSaving = true;
        while (_controller.height > _squatHeight)
        {
            _controller.height -= Time.deltaTime * moveSpeed * 5;
            _camera.localPosition -= new Vector3(0, Time.deltaTime * moveSpeed * 5 * 0.25f, 0);
            yield return new WaitForEndOfFrame();
        }
        _controller.height = _squatHeight;
        _camera.localPosition = new Vector3(0, 0.5f, 0);

        yield return new WaitWhile(() => Input.GetButton("Squat") || !_canStand);

        moveSpeed = _standSpeed;
        yDirection = transform.up;

        while (_controller.height < _standHeight)
        {
            yDirection = transform.up * _gravity;
            _camera.localPosition += new Vector3(0, Time.deltaTime * moveSpeed * 5 * 0.25f, 0);
            _controller.height += Time.deltaTime * moveSpeed * 5;
            yield return new WaitForEndOfFrame();
        }
        _camera.localPosition = new Vector3(0, 1.25f, 0);
        yDirection = transform.up * _gravity;
        _controller.height = _standHeight;
        SaveLoadControl.blockSaving = false;
        StartCoroutine(Squat());

    }
    public void TakeDamage(float damage)
    {
        if(Time.deltaTime == 0) { return; }
        _health -= damage;
        StartCoroutine(DisableRegeneration());
        if (_health < 0)
        {
            Lose();
        }
    }
    public void Lose()
    {
        StopAllCoroutines();
        SaveLoadControl.blockSaving = true;
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 1);
        UICanvasController.instance.SetActiveCanvas("Lose");
        UICanvasController.instance.blockChange = true;
    }
    public IEnumerator DisableRegeneration()
    {
        if(_regeneration != _maxRegeneration) { yield break; }
        _regeneration = 0f;
        speedMultiplier = 0.25f;
        yield return new WaitForSeconds(1f);
        speedMultiplier = 1f;
        _regeneration = _maxRegeneration;
    }
    public void OnDestroy()
    {
        SaveLoadControl.SaveEvent -= Save;
    }
    public void Save()
    {
        Player player = new Player();
        player.health = _health;
        player.position = new Vector_Clear(transform.position);
        player.rotation = new Vector_Clear(transform.rotation);
        player.itemId = ItemPosition.item != null? ItemPosition.item.id : "";
        SaveLoadControl.gameData.player = player;
    }
    public void Load()
    {
        if (SaveLoadControl.gameData.player == null) { return; }
        _health = SaveLoadControl.gameData.player.health;
        transform.position = SaveLoadControl.gameData.player.position.ToVector3();
        transform.rotation = SaveLoadControl.gameData.player.rotation.ToQuaternion();
        itemId = SaveLoadControl.gameData.player.itemId;
        Item item = Item.GetItem(itemId);
        if (item != null) 
        {
            item.StartEvent();
        }
    }
}
