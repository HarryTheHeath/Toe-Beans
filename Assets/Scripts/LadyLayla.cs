using com.cyborgAssets.inspectorButtonPro;
using System.Collections;
using UnityEngine;

public class LadyLyla : MonoBehaviour
{
    [Header("Tags")]
    [SerializeField] private string myName;
    [SerializeField] private string action;


    [Header("Dialogue")]
    [SerializeField] private AudioClip[] greetings;
    [SerializeField] private AudioClip[] goodbyes;
    [SerializeField] private AudioClip[] prints;
    [SerializeField] private AudioClip[] footprints;
    [SerializeField] private AudioClip[] lastPrint;
    [SerializeField] private AudioClip[] inspect;
    [SerializeField] private AudioClip[] present;
    [SerializeField] private AudioClip[] success;
    [SerializeField] private AudioClip[] take;
    [SerializeField] private AudioClip[] yes;
    [SerializeField] private AudioClip[] reactions;
    [SerializeField] private AudioClip humming;

    [Header("AudioSources")]
    public AudioSource voiceSource; 
    public AudioSource hummingSource;
    public AudioSource footsteps;

    [Header("Humming")]
    [SerializeField] private float humMinInterval = 10f;
    [SerializeField] private float humMaxInterval = 30f;
    [SerializeField] private bool humWhilePlayingVoice = false; // if false, humming waits until no voice is playing

    private int _lastInt;
    private int _lastFootstepIndex = -1;


    [ProButton] public void PlayGreeting() => PlayRandomArray(greetings);
    [ProButton] public void PlayGoodbye() => PlayRandomArray(goodbyes);
    [ProButton] public void PlayFootstepReaction() => PlayRandomArray(footprints);
    [ProButton] public void PlayLastPrintReaction() => PlayRandomArray(lastPrint);
    [ProButton] public void PlayPrints() => PlayRandomArray(prints);
    [ProButton] public void PlayPresent() => PlayRandomArray(present);
    [ProButton] public void PlayInspect() => PlayRandomArray(inspect);
    [ProButton] public void PlaySuccess() => PlayRandomArray(success);
    [ProButton] public void PlayTake() => PlayRandomArray(take);
    [ProButton] public void PlayYes() => PlayRandomArray(yes);
    [ProButton] public void PlayReaction() => PlayRandomArray(reactions);

    public void Hum()
    {
        hummingSource.Stop();
        hummingSource.PlayOneShot(humming);
    }


    private void Start()
    {
        if (humming != null && hummingSource != null)
            StartCoroutine(HumLoop());
    }

    public void PlayRandomArray(AudioClip[] array) 
    {
        PlayAudio(ReturnRandomClip(array));
    }

    public bool AudioPlaying()
    {
        return voiceSource.isPlaying;
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

    private IEnumerator HumLoop()
    {
        while (true)
        {
            float wait = Random.Range(humMinInterval, humMaxInterval);
            yield return new WaitForSeconds(wait);

            if (!humWhilePlayingVoice)
            {
                if (voiceSource != null && voiceSource.isPlaying)
                    continue;
            }

            if (hummingSource != null && humming != null)
                hummingSource.PlayOneShot(humming);
        }
    }

    public bool TryPlayFootstep()
    {
        if (footprints == null || footprints.Length == 0) return false;
        if (footsteps == null) return false;

        // Avoid overlapping footsteps
        if (footsteps.isPlaying) return false;

        int idx = Random.Range(0, footprints.Length);
        if (footprints.Length > 1)
        {
            int attempts = 0;
            while (idx == _lastFootstepIndex && attempts < 8)
            {
                idx = Random.Range(0, footprints.Length);
                attempts++;
            }
            if (idx == _lastFootstepIndex)
                idx = (idx + 1) % footprints.Length;
        }

        _lastFootstepIndex = idx;
        footsteps.PlayOneShot(footprints[idx]);
        return true;
    }


    [ProButton]
    public void PlayAudio(AudioClip dialogueLine)
    {
        if (!voiceSource.isPlaying)
            voiceSource.PlayOneShot(dialogueLine);
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