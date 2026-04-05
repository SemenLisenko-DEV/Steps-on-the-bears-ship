using UnityEngine;

public class EmiterEnter : MonoBehaviour
{
    public Emiter emiter;
    public void SetActive(bool state)
    {
        emiter.takingBeam = state;
        emiter.DrawAllBeams();
    }
}
