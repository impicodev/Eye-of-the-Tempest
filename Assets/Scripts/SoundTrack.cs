using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundTrack : MonoBehaviour
{
    public static SoundTrack track;
    [System.NonSerialized]
    public AudioSource src;

    void Awake()
    {
        src = GetComponent<AudioSource>();
        if (track != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        track = this;
    }
}
