using ActionDatabase;
using System.Collections;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public static PlayerControl Instance { get; private set; }

    [HideInInspector] public string floorType = "default";
    [SerializeField] private Transform _camera;
    [SerializeField] private AudioDictionary _audioDictionary;
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private float _squatHeight = 1f;
    [SerializeField] private float _standHeight = 2f;
    [SerializeField] private float _squatSpeed = 2.5f;
    [SerializeField] private float _standSpeed = 5f;
    [SerializeField] private float _gravity = -9.81f;
    public float moveSpeed = 5.0f;

    [HideInInspector] public float speedMultiplier = 1f;
    private bool _canStand = true;
    private Vector3 yDirection;
    private AudioSource _audioSource;
    private CharacterController _controller;

    private void Start()
    {
        Instance = this;
        _audioSource = GetComponent<AudioSource>();
        _controller = GetComponent<CharacterController>();
        StartCoroutine(Squat());
    }

    private void Update()
    {
        if(Time.deltaTime == 0) { _audioSource.pitch = 0; return; }

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

        StartCoroutine(Squat());
    }
}
