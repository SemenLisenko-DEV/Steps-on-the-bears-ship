using UnityEngine;

public class Meteor : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    public Vector3 _moveDirection;
    private void Update()
    {
        transform.position += _moveDirection * _speed * Time.deltaTime;
    }
    public void MoveToStart(float distance)
    {
        transform.position -= _moveDirection * distance;
    }
}
