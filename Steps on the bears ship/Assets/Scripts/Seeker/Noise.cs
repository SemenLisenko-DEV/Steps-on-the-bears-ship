using System;
using UnityEngine;

public class Noise : MonoBehaviour
{
    public delegate void NoiseAction(Vector3 position);
    public static NoiseAction OnMakeNoise;
    public static void MakeNoise(Vector3 position, float distance, AudioClip clip)
    {
        AudioSource source = Instantiate(Resources.Load<GameObject>("Prefabs/Noise"), position, Quaternion.identity).GetComponent<AudioSource>();
        source.clip = clip;
        source.maxDistance = distance;
        source.Play();
        OnMakeNoise?.Invoke(position);
        Destroy(source.gameObject,5f);
    }
}
