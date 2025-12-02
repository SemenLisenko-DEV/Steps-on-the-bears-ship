using UnityEngine;

public class DamageArea : MonoBehaviour
{
    [SerializeField] private float _damage;
    private void OnTriggerStay(Collider other)
    {
        PlayerControl playerControl;
        if(other.TryGetComponent(out playerControl))
        {
            playerControl.TakeDamage(_damage * Time.deltaTime);
        }
    }
}
