using ActionDatabase;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private Image _staminaImage;
    [HideInInspector] public float stamina = 100f;
    public float moveSpeed = 5.0f;
    private bool _runBreaked = false;
    private float _staminaAlphaColor = 1;
    private Vector3 _yDirection = Vector3.up;

    [HideInInspector] public float speedMultiplier = 1f;
    [HideInInspector] public string itemId = "";
    private bool _canStand = true;

    private AudioSource _audioSource;
    [SerializeField] private AudioSource _voiceSource;

    [HideInInspector] public CharacterController controller;

    private float _coughTimer = 0f;

    private float _crutchTimer = 0;

    public List<string> cardTaken;

    [Header("Информация для взаимодействий")]
    public TMP_Text actionInfo;
    [Min(2.5f)]public float showTime = 5f;
    private void Awake()
    {
        Instance = this;
        _staminaImage.color = Color.blue;
        stamina = _maxStamina;
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

        if (Input.GetButton("Run") && !_runBreaked && axisFactor != 0)
        {
            axisFactor *= 1f + Input.GetAxis("Run") / 2.5f;
            stamina -= Time.deltaTime * speedMultiplier * moveSpeed;
            _runBreaked = stamina < 0.05f;
            if (_runBreaked)
            {
                StartCoroutine(Fall());
            }
        }
        else
        {
            stamina += stamina < _maxStamina ? Time.deltaTime * 5 : 0;
        }

        if (stamina >= _maxStamina)
        {
            _staminaAlphaColor = _staminaAlphaColor <= 0 ? 0 : _staminaAlphaColor - Time.deltaTime;
        }
        else
        {
            _staminaAlphaColor = _staminaAlphaColor >= 1 ? 1 : _staminaAlphaColor + Time.deltaTime * 5;
        }

        _staminaImage.color = new Color(_staminaImage.color.r, _staminaImage.color.g, _staminaImage.color.b, _staminaAlphaColor);
        _staminaImage.fillAmount = stamina / _maxStamina;

        _cameraAnimator.SetFloat("Speed", axisFactor * speedMultiplier * moveSpeed / _standSpeed);

        _canStand = !Physics.Raycast(transform.position + transform.up * controller.height / 2, transform.up, 2f, _obstacleMask);

        _audioSource.pitch = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0? ((moveSpeed * speedMultiplier) / _standSpeed) : 0;
        _audioSource.pitch *= Input.GetButton("Run") && !_runBreaked ? 1.2f : 1;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = (transform.forward * verticalInput + transform.right * horizontalInput) * moveSpeed * speedMultiplier * axisFactor + _yDirection * _gravity;

        controller.Move(moveDirection * Time.deltaTime);
        OnMove?.Invoke((transform.forward * verticalInput + transform.right * horizontalInput) * moveSpeed * speedMultiplier * axisFactor * Time.deltaTime);

    }
    private void LateUpdate()
    {
        if(transform.parent != null)
        {
            transform.position = new Vector3(transform.position.x, transform.parent.position.y, transform.position.z);
        }
    }
    public IEnumerator Fall()
    {
        _staminaImage.color = Color.red;
        yield return new WaitForSeconds(_fallTime);
        _staminaImage.color = Color.blue;
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
    public void Win()
    {
        StopAllCoroutines();
        SaveLoadControl.blockSaving = true;
        UICanvasController.instance.SetActiveCanvas("Win");
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

    public void ShowActionInfo(string text)
    {

    }
    public IEnumerator ShowActionInfo_Corutine(string text)
    {
        actionInfo.text = text;
        actionInfo.gameObject.SetActive(true);
        yield return new WaitForSeconds(showTime - 2.5f);
        float timer = 2.5f;
        while (timer > 0)
        {
            actionInfo.alpha = timer / 2.5f;
            timer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        actionInfo.gameObject.SetActive(false);
    }
    public void SetParent(GameObject parent)
    {
        transform.parent = parent.transform;
    }
    public void UnParentWithTime(float time)
    {
        StartCoroutine(UnParentWithTime_C(time));
    }
    public IEnumerator UnParentWithTime_C(float time)
    {
        yield return new WaitForSeconds(time);
        transform.parent = null;
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
        player.cardsTaken = cardTaken;
        SaveLoadControl.gameData.player = player;
    }
    public void Load()
    {
        if (SaveLoadControl.gameData.player == null) { return; }
        _health = SaveLoadControl.gameData.player.health;
        cardTaken = SaveLoadControl.gameData.player.cardsTaken;
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
