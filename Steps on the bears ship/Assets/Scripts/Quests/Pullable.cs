using ActionDatabase;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Pullable : MonoBehaviour,IAction
{
    [SerializeField] private GameObject questTarget;
    [SerializeField] private Transform crutch;
    [SerializeField] private float _angle = 90f;
    public void StartEvent()
    {
        StartCoroutine(Pull());
    }
    public IEnumerator Pull()
    {
        yield return new WaitForEndOfFrame();
        while (ActionsExecutor.actionExecuting)
        {
            transform.LookAt(ActionsExecutor.executerLastHit.point);
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0, 0);
            yield return new WaitForEndOfFrame();
        }
        if (transform.localEulerAngles.x > _angle - 5f || transform.localEulerAngles.x < _angle + 5f)
        {
            ExecuteQuest();
        }    
        yield break;
    }
    public void ExecuteQuest()
    {
        questTarget.GetComponent<IQuest>().StartQuest();
    }
}
