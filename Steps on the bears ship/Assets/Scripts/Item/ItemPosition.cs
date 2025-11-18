using System;
using UnityEngine;

public class ItemPosition : MonoBehaviour
{
    public static Action ItemsDrops;
    public static Transform _transform;
    public static Transform itemSpawn;
    public static bool haveItem = false;
    public static Item item;

    [SerializeField] private Transform itemSpawnLocal;
    public void Start()
    {
        _transform = GetComponent<Transform>();
        itemSpawn = itemSpawnLocal;
    }
}
