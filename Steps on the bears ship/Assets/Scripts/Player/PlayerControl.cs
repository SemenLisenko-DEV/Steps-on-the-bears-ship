using ActionDatabase;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    public static Action<Vector3> OnMove;
    public static PlayerControl Instance { get; private set; }

    [HideInInspector] public string floorType = "default";
    [SerializeField] private Transform _camera;
    [SerializeField] private AudioDictionary _audioDictionary;
    [SerializeField] private LayerMask _obstacleMask;

    public Transform flashlightPosition;
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
    [SerializeField] private float _maxStamina = 100f;
    [SerializeField] private Animator _cameraAnimator;
    [SerializeField] private float _fallTime = 1;
    [HideInInspector] public float stamina = 100f;
    public float moveSpeed = 5.0f;
    private bool _runBreaked = false;
    private Vector3 _yDirection = Vector3.up;

    [HideInInspector] public float speedMultiplier = 1f;
    [HideInInspector] public string itemId = "";
    private bool _canStand = true;
    private AudioSource _audioSource;
    [SerializeField] private AudioSource _voiceSource;
    [HideInInspector] public CharacterController controller;
    private float _coughTimer = 0f;
    private float _crutchTimer = 0;
    private void Awake()
    {
        Instance = this;
        stamina = _maxHealth;
        Load();
        _regeneration = _maxRegeneration;
        _health = _maxHealth;
        _audioSource = GetComponent<AudioSource>();
        controller = GetComponent<CharacterController>();
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
        _health += _health + Time.deltaTime * _maxRegeneration > _maxHealth ? 0 : Time.deltaTime * _maxRegeneration;

        float axisFactor = 0;
        axisFactor = Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0 ? 1 : 0;
        axisFactor = Input.GetAxis("Horizontal") != 0 && Input.GetAxis("Vertical") != 0 ? 0.65f : axisFactor;

        if (Input.GetButton("Run") && !_runBreaked)
        {
            axisFactor *= 1f + Input.GetAxis("Run") / 3;
            stamina -= Time.deltaTime * speedMultiplier * moveSpeed;
            _runBreaked = stamina < 0.05f;
            if (_runBreaked)
            {
                StartCoroutine(Fall());
            }
        }
        else
        {
            stamina += stamina < _maxStamina ? Time.deltaTime : 0;
        }

        _cameraAnimator.SetFloat("Speed", axisFactor * speedMultiplier * moveSpeed / _standSpeed);

        _canStand = !Physics.Raycast(transform.position + transform.up * controller.height / 2, transform.up, 2f, _obstacleMask);

        _audioSource.pitch = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0? ((moveSpeed * speedMultiplier) / _standSpeed) : 0;
        _audioSource.pitch *= Input.GetButton("Run") ? 1.2f : 1;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = (transform.forward * verticalInput + transform.right * horizontalInput) * moveSpeed * speedMultiplier * axisFactor + _yDirection * _gravity;

        controller.Move(moveDirection * Time.deltaTime);
        OnMove?.Invoke((transform.forward * verticalInput + transform.right * horizontalInput) * moveSpeed * speedMultiplier * axisFactor * Time.deltaTime);
    }
    public IEnumerator Fall()
    {
        float t = 0;
        while (t < _fallTime)
        {
            t += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        _runBreaked = false;
    }
    private IEnumerator Squat()
    {
        yield return new WaitUntil(() => Input.GetButtonDown("Squat"));

        moveSpeed = _squatSpeed;
        SaveLoadControl.blockSaving = true;
        while (controller.height >= _squatHeight)
        {
            controller.height -= Time.deltaTime * 8;
            _camera.localPosition = new Vector3(0, 0.5f + (controller.height - _squatHeight) / (_standHeight - _squatHeight), 0);
            yield return new WaitForEndOfFrame();
        }
        controller.height = _squatHeight;
        _camera.localPosition = new Vector3(0, 0.5f, 0);

        yield return new WaitWhile(() => Input.GetButton("Squat") || !_canStand);

        moveSpeed = _standSpeed;
        _yDirection = Vector3.up / _gravity * 3;

        while (controller.height <= _standHeight)
        {
            controller.height += Time.deltaTime * 8;
            _camera.localPosition = new Vector3(0, 0.5f + (controller.height - _squatHeight) / (_standHeight - _squatHeight), 0);
            yield return new WaitForEndOfFrame();
        }

        _camera.localPosition = new Vector3(0, 1.5f, 0);
        controller.height = _standHeight;
        _yDirection = Vector3.up;

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
        StartCoroutine(WaitForItemLoading());
    }
    public IEnumerator WaitForItemLoading()
    {
        yield return new WaitForEndOfFrame();
        itemId = SaveLoadControl.gameData.player.itemId;
        Debug.Log(itemId);
        Item item = Item.GetItem(itemId);
        if (item != null)
        {
            item.PickUp();
        }
    }
}
