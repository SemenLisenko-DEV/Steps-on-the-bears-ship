using ActionDatabase;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Translates", menuName = "Scriptable Objects/Translates")]
public class Translates : ScriptableObject
{
    [Serializable]
    public struct TranslateVariation
    {
        public string[] variations;
    }
    [Serializable]
    public struct KeyTranslate
    {
        public string key;
        public TranslateVariation[] translates;
    }
    public KeyTranslate[] translations;
    public string GetTranslate(string key, int translateVariation = 0)
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
        return translations[keyId].translates[(int)SaveLoadControl.settings.language].variations[translateVariation];
    }
}
