using ActionDatabase;
using System.Collections;
using UnityEngine;

public class Valve : MonoBehaviour,IAction
{
    [SerializeField] private float _sensative = 1f;
    [SerializeField] private int _revolutCount = 1;
    [SerializeField] private GameObject _quest;
    private bool _complite = false;
    private float _angle = 0;
    public void StartEvent()
    {
        StartCoroutine(Pull());
    }
    public IEnumerator Pull()
    {
        if (_complite) { yield break; }
        while (ActionsExecutor.actionExecuting)
        {
            float mouseX = Mathf.Abs(Input.GetAxis("Mouse X"));
            float mouseY = Mathf.Abs(Input.GetAxis("Mouse Y"));
            _angle += (mouseX + mouseY) * _sensative;
            if(_angle / 360 >= _revolutCount)
            {
                _complite = true;
                break;
            }
            transform.Rotate(0, 0.0f, (mouseX + mouseY) * _sensative);
            yield return new WaitForEndOfFrame();
        }
        if (_angle / 360 >= _revolutCount && _quest != null)
        {
            _complite = true;
            if (_quest != null)
            {
                _quest.GetComponent<IQuest>().StartQuest();
            }
        }
        yield break;
    }
}
