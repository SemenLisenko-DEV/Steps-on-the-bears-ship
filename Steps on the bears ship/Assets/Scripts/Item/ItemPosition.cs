using System;
using UnityEngine;

public class ItemPosition : MonoBehaviour
{
    public static Action ItemsDrops;
    public static Transform _transform;
    public static bool haveItem = false;
    public static Item item;
    public void Awake()
    {
        _transform = GetComponent<Transform>();
    }
    private void OnDestroy()
    {
        haveItem = false;
        item = null;
    }
}
