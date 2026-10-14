using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class SoundInteract : MonoBehaviour
{
    public AudioClip audioClip;
    public AudioSource audio;

    [ProButton]
    void PlayAudio()
    {
        if (!audio.isPlaying)
            audio.PlayOneShot(audioClip);
    }
}