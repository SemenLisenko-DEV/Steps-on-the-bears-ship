using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public static CameraControl Instance { get; private set; }

    public Transform crutch;
    [HideInInspector]public bool block = false;
    public float sensitivity = 10.0f;
    public float maxYAngle = 80.0f;

    private float rotationX = 0.0f;
    private void Start()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Time.timeScale == 0 || block)
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        transform.parent.parent.Rotate(Vector3.up * mouseX * sensitivity);

        rotationX -= mouseY * sensitivity;
        rotationX = rotationX < -maxYAngle ? -maxYAngle : rotationX;
        rotationX = rotationX > maxYAngle ? maxYAngle : rotationX;
        transform.localRotation = Quaternion.Euler(rotationX, 0.0f, 0.0f);
    }
    public void LookAt(Vector3 target)
    {
        crutch.position = transform.position;
        crutch.LookAt(target);
        transform.localRotation = Quaternion.Euler(crutch.localEulerAngles.x, 0.0f, 0.0f);
        transform.parent.parent.localRotation = Quaternion.Euler(0.0f, crutch.localEulerAngles.y, 0.0f);
    }
}
