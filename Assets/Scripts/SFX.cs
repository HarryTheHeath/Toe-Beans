using UnityEngine;

public class SoundInteract : MonoBehaviour
{
    public AudioClip audioClip;
    AudioSource audio;

    void PlayAudio()
    {
        if (!audio.isPlaying)
            audio.Play();
    }   
}