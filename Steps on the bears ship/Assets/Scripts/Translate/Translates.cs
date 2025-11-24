using ActionDatabase;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Translates", menuName = "Scriptable Objects/Translates")]
public class Translates : ScriptableObject
{
    [Serializable]
    public struct keyTranslate
    {
        public string key;
        public string[] translates;
    }
    public keyTranslate[] translations;
    public string GetTranslate(string key)
    {
        int keyId = 0;
        for (int i = 0; i < translations.Length; i++)
        {
            if (translations[i].key == key)
            {
                keyId = i;
                break;
            }
        }
        return translations[keyId].translates[(int)SaveLoadControl.settings.language];
    }
    public string GetTranslatePlaceHolder(string key, Pair<string,string> placeHolder)
    {

        return "";
    }
}
