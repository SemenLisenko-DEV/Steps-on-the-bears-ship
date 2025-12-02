using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        UICanvasController.instance.SetActiveCanvas("Win");
    }
}
