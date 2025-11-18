using UnityEngine;

public class MeteorBorder : MonoBehaviour
{
    [SerializeField] private float _distance = 20f;
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<Meteor>().MoveToStart(_distance);
    }
}
