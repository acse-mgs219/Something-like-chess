using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    AudioSource audioSource;
    public AudioClip moveSound;
    public AudioClip checkSound;
    public AudioClip checkmateSound;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Play(AudioClip audio)
    {
        audioSource.PlayOneShot(audio);
    }
}
