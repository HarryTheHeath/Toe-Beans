using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class SFX : MonoBehaviour
{
    public AudioClip audioClip;
    public AudioSource audioSource;

    [ProButton]
    public void PlayAudio()
    {
        if (!audioSource.isPlaying)
            audioSource.PlayOneShot(audioClip);
    }
}