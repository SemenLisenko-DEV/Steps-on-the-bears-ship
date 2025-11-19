using System;
using UnityEngine;

namespace ActionDatabase
{
    public interface IAction
    {
        public void StartEvent();
    }
    public interface IQuest
    {
        public void StartQuest();

    }
    public enum ItemType
    {
        None,
        Trash,
        Fuse
    }
    [Serializable]
    public struct AudioDictionary
    {
        [Serializable]
        public struct AudioPair
        {
            public string key;
            public AudioClip value;
        }
        public AudioPair[] elements;
        public AudioClip Find(string key)
        {
            for (int i = 0; i < elements.Length; i++)
            {
                if (elements[i].key == key)
                {
                    if (elements[i].value == null)
                    {
                        Resources.Load<AudioClip>("Audio/MisingAudio");
                    }
                    return elements[i].value;
                }
            }
            return Resources.Load<AudioClip>("Audio/MisingAudio");
        }
    }
    [Serializable]
    public struct SpriteDictionary
    {
        [Serializable]
        public struct SpritePair
        {
            public string key;
            public Sprite value;
        }
        public SpritePair[] elements;
        public Sprite Find(string key)
        {
            for (int i = 0; i < elements.Length; i++)
            {
                if (elements[i].key == key)
                {
                    if (elements[i].value == null)
                    {
                        return Resources.Load<Sprite>("Spites/MissingSprite");
                    }
                    return elements[i].value;
                }
            }
            return Resources.Load<Sprite>("Spites/MissingSprite");
        }
    }
    public class Pair<T, U>
    {
        public Pair()
        {
        }

        public Pair(T first, U second)
        {
            First = first;
            Second = second;
        }

        public T First { get; set; }
        public U Second { get; set; }
    }
}
