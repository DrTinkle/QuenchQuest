using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientAudio : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] float fadeInTime = 1.0f;
    [SerializeField] float maxVolume = 1.0f;

    void Start()
    {
        StartCoroutine(FadeIn(audioSource, fadeInTime, maxVolume));
    }

    IEnumerator FadeIn(AudioSource audioSource, float FadeTime, float maxVolume)
    {
        float startVolume = 0.0f;
        audioSource.volume = startVolume;
        audioSource.Play();

        while (audioSource.volume < maxVolume)
        {
            audioSource.volume += Time.deltaTime / FadeTime;
            yield return null;
        }

        audioSource.volume = maxVolume;
    }

}
