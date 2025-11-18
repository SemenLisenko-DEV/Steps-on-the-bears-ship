using UnityEngine;

public class StageTrigger : MonoBehaviour
{
    public GameObject stage;
    private void OnTriggerEnter(Collider other)
    {
        stage.SetActive(true);
    }
    private void OnTriggerExit(Collider other)
    {
        stage.SetActive(false);
    }
}
