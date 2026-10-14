using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class SFX : MonoBehaviour
{
    public AudioClip audioClip;
    public AudioSource audio;

    [ProButton]
    public void PlayAudio()
    {
        if (!audio.isPlaying)
            audio.PlayOneShot(audioClip);
    }
}