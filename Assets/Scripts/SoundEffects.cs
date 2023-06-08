using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    public static SoundEffects Instance { get; set; }

    [SerializeField] AudioSource audioSource;

    public AudioClip saleSound;
    public AudioClip tipSound;
    public AudioClip ewSound;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.LogError("Another instance of SoundEffects already exists!");
        }
    }

    public void PlayClip(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
