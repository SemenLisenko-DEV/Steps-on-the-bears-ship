using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EmitersFamily : MonoBehaviour
{
    [HideInInspector] public List<Emiter> emiters;
    [HideInInspector] public int endCount = 0;
    [HideInInspector] public int activeEnd = 0;
    public UnityEvent onComplite;
    public void DeleteAllBeamsOfEmiters(bool disableOnTime)
    {
        foreach(Emiter emiter in emiters)
        {
            emiter.DeleteAllBeams();
            emiter.disabled = true;
            emiter.disabledTime = disableOnTime;
        }
    }
}
