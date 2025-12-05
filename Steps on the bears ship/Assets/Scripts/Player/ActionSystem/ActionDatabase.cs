using System;
using System.Collections.Generic;
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
        public void DisableQuest();
    }
    [Serializable]
    public enum ExecuteType
    {
        all = 0,
        anything = 1,

    }
    [Serializable]
    public enum Languages
    {
        Rus = 0,
        Eng = 1,
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
        public bool IsEmpty()
        {
            return elements.Length == 0;
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
    [Serializable]
    public struct Save
    {
        public string name;
        public string date;
        public string fileName;
        public string spritePath;
    }
    [Serializable]
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
    [Serializable]
    public class Settings
    {
        public Languages language = Languages.Eng;
        public float volume = 1f;
        public int playerSavesCount = 0;
        public List<Save> savesLinks;
        public string lastSave;
        public Save Find(string fileName)
        {
            foreach (Save s in savesLinks)
            {
                if(s.fileName == fileName)
                {
                    return s;
                }
            }
            return new Save();
        }
    }
    [Serializable]
    public class Vector_Clear
    {
        public Vector_Clear() 
        {
        }
        public Vector_Clear(float x, float y, float z)
        {
            this.x = x; 
            this.y = y;
            this.z = z;
        }
        public Vector_Clear(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
        public Vector_Clear(Vector3 vector3)
        {
            this.x = vector3.x;
            this.y = vector3.y;
            this.z = vector3.z;
        }
        public Vector_Clear(Quaternion quaternion)
        {
            this.x = quaternion.x;
            this.y = quaternion.y;
            this.z = quaternion.z;
            this.w = quaternion.w;
        }
        public Vector3 ToVector3()
        {
            Vector3 vector3 = new Vector3();
            vector3.x = this.x;
            vector3.y = this.y;
            vector3.z = this.z;
            return vector3;
        }
        public Quaternion ToQuaternion()
        {
            Quaternion quaternion = new Quaternion();
            quaternion.x = this.x;
            quaternion.y = this.y;
            quaternion.z = this.z;
            quaternion.w = this.w;
            return quaternion;
        }
        public float x;
        public float y;
        public float z;
        public float w;
    }
    [Serializable]
    public class Player
    {
        public Vector_Clear position;
        public Vector_Clear rotation;
        public float health;
        public string itemId;
    }
    [Serializable]
    public class DataObject
    {
        public string id;
    }
    [Serializable]
    public class ItemData : DataObject 
    {
        public ItemData() 
        {
        }
        public ItemData(Item item)
        {
            isPickUp = item.isPickUp;
            canPickUp = item.canPickUp;
            handlerId = item.handlerId;
            position = item.position;
            rotation = item.rotation;
            id = item.id;
        }
        public bool isPickUp;
        public bool canPickUp;
        public string handlerId;
        public Vector_Clear position;
        public Vector_Clear rotation;
    }
    [Serializable]
    public class ItemHandlerData : DataObject
    {
        public ItemHandlerData()
        {
        }
        public ItemHandlerData(ItemHandler handler)
        {
            isTriggered = handler.isTriggered;
            id = handler.id;
        }
        public bool isTriggered = false;
        public string itemId;
    }
    [Serializable]
    public class PullableData : DataObject
    {
        public PullableData()
        {
        }
        public PullableData(Pullable pullable)
        {
            position = pullable.position;
            id = pullable.id;
        }
        public Vector_Clear position;
    }
    [Serializable]
    public class AnimationActivatorData : DataObject
    {
        public AnimationActivatorData()
        {
        }
        public AnimationActivatorData(AnimationActivator animationActivator)
        {
            blockByQuest = animationActivator.blockByQuest;
            status = animationActivator.status;
            currentState = animationActivator.currentState;
            id = animationActivator.id;
        }
        public bool blockByQuest;
        public bool status;
        public int currentState;
    }
    [Serializable]
    public class Trigger : DataObject
    {
        public Trigger()
        {
        }
        public Trigger(bool state, string id)
        {
            triggered = state;
            this.id = id; 
        }
        public bool triggered = false;
    }
    [Serializable]
    public class ValveData : DataObject
    {
        public ValveData()
        {
        }
        public ValveData(Valve valve)
        {
            this.rotation = valve.rotation;
            this.complited = valve.complited;
            this.angle = valve.angle;
            id = valve.id;
        }
        public bool complited;
        public float angle;
        public Vector_Clear rotation;
    }
    [Serializable]
    public class QuestData : DataObject
    {
        public QuestData()
        {
        }
        public QuestData(QuestControl questControl)
        {
            actionCanExecute = questControl.actionCanExecute;
            complited = questControl.complited;
            questCount = questControl.questCount;
            id = questControl.id;
        }
        public int questCount;
        public bool actionCanExecute;
        public bool complited = false;
    }
}
