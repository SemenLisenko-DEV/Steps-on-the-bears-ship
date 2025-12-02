using ActionDatabase;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class SavesList : MonoBehaviour
{
    public static Action OnSubstrateDelete;
    public static SavesList Instance;
    public static string folder = "/Saves";
    public static Transform substrateParent;
    public static List<Substrate> substrateList;
    [SerializeField] private Transform _substrateParentLink;
    [SerializeField] private TMP_InputField _nameField;

    public void Awake()
    {
        Instance = this;
        substrateParent = _substrateParentLink;
        substrateList = new List<Substrate>();
        Refresh();
    }
    public static Substrate CreateSaveSubstrate(Save save)
    {
        Substrate saveSubstrate = Instantiate(Resources.Load<GameObject>("Prefabs/Saves/Substrate"),substrateParent).GetComponent<Substrate>();
        saveSubstrate.data = save;
        substrateList.Add(saveSubstrate);
        return saveSubstrate;
    }
    public static void Refresh()
    {
        if (SaveLoadControl.settings.savesLinks == null) { return; }
        foreach (Save i in SaveLoadControl.settings.savesLinks)
        {
            if (!Contains(i))
            {
                CreateSaveSubstrate(i);
            }
        }
    }
    public static bool Contains(Save save)
    {
        foreach(Substrate s in substrateList)
        {
            if(s.data.fileName == save.fileName)
            {
                return true;
            }
        }
        return false;
    }
    public static void DeleteSubstrate(Substrate substrate)
    {
        substrateList.Remove(substrate);
        Destroy(substrate.gameObject);
    }
    public static void DeleteSave(Substrate substrate)
    {
        SaveLoadControl.DeleteSave(substrate.data.fileName);
        substrateList.Remove(substrate);
        Destroy(substrate.gameObject);
        OnSubstrateDelete.Invoke();
    }
}

