using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class MrMori : MonoBehaviour, IInteractable
{
    [Header("Tags")]
    [SerializeField] private string myName;
    [SerializeField] private string action;


    [Header("Dialogue")]
    [SerializeField] private AudioClip[] greetings;
    [SerializeField] private AudioClip[] goodbyes;
    [SerializeField] private AudioClip[] accepts;
    [SerializeField] private AudioClip[] yes;
    [SerializeField] private AudioClip[] prints;
    [SerializeField] private AudioClip onkey;


    private int _lastInt;
    public AudioSource audioSource;

    [ProButton]
    public void Interact()
    {
        PlayAudio(ReturnRandomClip(greetings));
    }

    public void PlayRandomArray(AudioClip[] array) 
    {
        PlayAudio(ReturnRandomClip(array));
    }

    public bool AudioPlaying()
    {
        return audioSource.isPlaying;
    }

    public AudioClip ReturnRandomClip(AudioClip[] audioClip)
    {
        int idx = Random.Range(0, audioClip.Length);
        if (audioClip.Length > 1)
        {
            while (idx == _lastInt)
                idx = Random.Range(0, audioClip.Length);
        }

        _lastInt = idx;
        return audioClip[idx];
    }

    [ProButton]
    public void PlayAudio(AudioClip dialogueLine)
    {
        if (!audioSource.isPlaying)
            audioSource.PlayOneShot(dialogueLine);
    }

    public bool hasTriggered()
    {
        return AudioPlaying();
    }

    public string promptTag()
    {
        return myName;
    }

    public string promptAction()
    {
        return action;
    }

}
