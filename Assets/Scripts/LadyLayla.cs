using com.cyborgAssets.inspectorButtonPro;
using UnityEngine;

public class LadyLyla : MonoBehaviour
{
    [Header("Tags")]
    [SerializeField] private string myName;
    [SerializeField] private string action;


    [Header("Dialogue")]
    [SerializeField] private AudioClip[] greetings;
    [SerializeField] private AudioClip[] goodbyes;
    [SerializeField] private AudioClip[] footprints;
    [SerializeField] private AudioClip[] lastPrint;
    [SerializeField] private AudioClip[] inspect;
    [SerializeField] private AudioClip[] present;
    [SerializeField] private AudioClip[] success;
    [SerializeField] private AudioClip[] take;
    [SerializeField] private AudioClip[] yes;
    [SerializeField] private AudioClip[] reactions;
    [SerializeField] private AudioClip humming;


    private int _lastInt;
    public AudioSource audioSource;

    [ProButton] public void PlayGreeting() => PlayRandomArray(greetings);
    [ProButton] public void PlayGoodbye() => PlayRandomArray(goodbyes);
    [ProButton] public void PlayFootstepReaction() => PlayRandomArray(footprints);
    [ProButton] public void PlayLastPrintReaction() => PlayRandomArray(lastPrint);
    [ProButton] public void PlayPresent() => PlayRandomArray(present);
    [ProButton] public void PlayInspect() => PlayRandomArray(inspect);
    [ProButton] public void PlaySucces() => PlayRandomArray(success);
    [ProButton] public void PlayTake() => PlayRandomArray(take);
    [ProButton] public void PlayYes() => PlayRandomArray(yes);
    [ProButton] public void PlayReaction() => PlayRandomArray(reactions);

    public void Hum()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(humming);
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