using UnityEngine;
public class QuitOutTheGame : MonoBehaviour
{
    public void Quit()
    {
        Application.Quit();
    }

    public void OpenLink(string link)
    {
        Application.OpenURL(link);
    }
}
