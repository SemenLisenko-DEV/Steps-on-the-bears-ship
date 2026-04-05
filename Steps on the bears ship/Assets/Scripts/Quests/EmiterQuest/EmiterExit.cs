using ActionDatabase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmiterExit : MonoBehaviour
{
    public LayerMask layerMask;
    public Emiter emiter;
    private GameObject _currentBeam;
    private GameObject beamObj;
    private EmiterEnter connectedEmiterEnter;
    public void Awake()
    {
        beamObj = Resources.Load<GameObject>("Prefabs/Beam");
    }
    public void DrawBeam()
    {
        DeleteBeam();
        RaycastHit hit;
        if ((emiter.takingBeam || emiter.allwaysCast) && Physics.Raycast(transform.position, transform.forward, out hit, 40f, layerMask))
        {
            Debug.Log("EMITER EXIT: " + emiter.id + " WAS HIT SOMETHING: " + hit.collider?.name);
            if (hit.collider != null)
            {
                if (hit.collider.TryGetComponent(out connectedEmiterEnter))
                {
                    connectedEmiterEnter.SetActive(true);
                    Debug.Log("EMITER: " + emiter.id + " CONNECTED TO ENTER: " + connectedEmiterEnter.emiter.id);
                }
                float size = hit.distance / 2;
                Vector3 position = transform.position + (hit.point - transform.position) / 2;
                _currentBeam = Instantiate(beamObj, position, transform.rotation);
                _currentBeam.transform.localScale = new Vector3(1, 1, size);
            }
        }
    }
    public void DeleteBeam()
    {
        if (_currentBeam != null) 
        {
            Destroy(_currentBeam);
        }
        if(connectedEmiterEnter != null) 
        {
            connectedEmiterEnter.SetActive(false);
            connectedEmiterEnter = null;
        }
    }
}
