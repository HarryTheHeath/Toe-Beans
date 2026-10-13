using com.cyborgAssets.inspectorButtonPro;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[AddComponentMenu("Tweens/Raise On Y")]
public class Yraiser : MonoBehaviour
{
    [Header("Source / Target")]
    public bool captureStartOnStart;
    public bool setAtStartOnStart = true;
    public bool toggleSR;

    public GameObject target;
    public SpriteRenderer sr;
    public Vector3 startPosition;

    public Vector3 endPosition;

    [Header("Behaviour")]
    public bool useLocalPosition = true;

    public bool yOnly = true;

    public bool playOnStart = false;

    public float startDelay = 0f;

    [Header("Timing & Curve")]
    public float duration = 1f;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    public bool useUnscaledTime = false;

    [Header("Events")]
    public UnityEvent onComplete;

    private Coroutine _running;
    private Vector3 _capturedStartPos;

    private void Start()
    {
        if (captureStartOnStart)
            _capturedStartPos = useLocalPosition ? target.transform.localPosition : target.transform.position;
        else
            _capturedStartPos = startPosition;

        if (setAtStartOnStart)
        {
            if (useLocalPosition)
                target.transform.localPosition = startPosition;
            else
                target.transform.position = startPosition;
        }

        if (toggleSR && sr != null)
            sr.enabled = false;

        if (playOnStart)
            StartRaise();
    }


    [ProButton]
    public void StartRaise()
    {
        if (sr != null)
            sr.enabled = true;

        if (captureStartOnStart)
            _capturedStartPos = useLocalPosition ? target.transform.localPosition : target.transform.position;

        if (_running != null) StopCoroutine(_running);
        _running = StartCoroutine(RaiseCoroutine(_capturedStartPos, endPosition, false, false));
    }

    public void StartRaiseFrom(Vector3 from, Vector3 to)
    {
        if (_running != null) StopCoroutine(_running);
        _running = StartCoroutine(RaiseCoroutine(from, to, false, false));
    }

    [ProButton]
    public void ResetToStart()
    {
        Vector3 currentPos = useLocalPosition ? target.transform.localPosition : target.transform.position;
        Vector3 targetStartPos = _capturedStartPos;

        if (sr != null)
            sr.enabled = true;

        if (_running != null) StopCoroutine(_running);
        _running = StartCoroutine(RaiseCoroutine(currentPos, targetStartPos, true, toggleSR));
    }

    private IEnumerator RaiseCoroutine(Vector3 from, Vector3 to, bool reverseCurve, bool disableAtEnd)
    {
        if (startDelay > 0f)
        {
            if (useUnscaledTime)
                yield return new WaitForSecondsRealtime(startDelay);
            else
                yield return new WaitForSeconds(startDelay);
        }

        float elapsed = 0f;
        float effectiveDuration = Mathf.Max(1e-6f, duration);

        while (elapsed < effectiveDuration)
        {
            float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            elapsed += dt;
            float t = Mathf.Clamp01(elapsed / effectiveDuration);
            float eval = reverseCurve ? 1f - curve.Evaluate(1f - t) : curve.Evaluate(t);

            Vector3 pos;
            if (yOnly)
            {
                float y = Mathf.LerpUnclamped(from.y, to.y, eval);
                pos = new Vector3(from.x, y, from.z);
            }
            else
            {
                pos = Vector3.LerpUnclamped(from, to, eval);
            }

            ApplyPosition(pos);
            yield return null;
        }

        ApplyPosition(yOnly ? new Vector3(from.x, to.y, from.z) : to);

        _running = null;

        if (disableAtEnd && sr != null)
            sr.enabled = false;

        onComplete?.Invoke();
    }

    private void ApplyPosition(Vector3 pos)
    {
        if (useLocalPosition)
            target.transform.localPosition = pos;
        else
            target.transform.position = pos;
    }

    [ProButton]
    private void ApplyStart()
    {
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            if (captureStartOnStart)
                _capturedStartPos = useLocalPosition ? target.transform.localPosition : target.transform.position;
        }
    }
}