using UnityEngine;
using System;
using System.Collections;

public class NPCBobbing : MonoBehaviour
{
    [SerializeField] public Transform transformToAffect;
    [SerializeField] private SquashAndStretchAxis axisToAffect = SquashAndStretchAxis.y;
    [SerializeField] private float animationDuration = 0.25f;
    [SerializeField] private bool canBeOverwritten;
    [SerializeField] private bool playOnStart;

    [Flags]
    public enum SquashAndStretchAxis
    {
        None = 0,
        x = 1,
        y = 2,
        z = 4
    }

    [SerializeField] private float initialScale = 1f;
    [SerializeField] private float maximumScale = 1.3f;
    [SerializeField] private bool resetToInitialScaleAfterAnimation = true;
    [SerializeField] private bool reverseAnimationCurveAfterPlayeing;
    private bool _isReversed;

    [SerializeField] private AnimationCurve squashAndStretchCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));

    [SerializeField] private bool looping;
    [SerializeField] private float loopingDelay = 1f;

    private Coroutine _squashAndStretchCoroutine;
    private WaitForSeconds _loopingDelayWaitForSeconds;
    private Vector3 _initialScaleVector;

    private bool affectX => (axisToAffect & SquashAndStretchAxis.x) != 0;
    private bool affectY => (axisToAffect & SquashAndStretchAxis.y) != 0;
    private bool affectZ => (axisToAffect & SquashAndStretchAxis.z) != 0;

    private void Awake()
    {
        if (transformToAffect == null)
        {
            transformToAffect = transform;
        }

        _initialScaleVector = transformToAffect.localScale;
        _loopingDelayWaitForSeconds = new WaitForSeconds(loopingDelay);
    }

    private void Start()
    {
        if (playOnStart)
        {
            CheckForAndStartCoroutine();
        }
    }

    public void PlaySquashAndStretch()
    {
        if (looping && !canBeOverwritten)
        {
            return;
        }

        CheckForAndStartCoroutine();
    }

    private void CheckForAndStartCoroutine()
    {
        if (_squashAndStretchCoroutine != null)
        {
            StopCoroutine(_squashAndStretchCoroutine);
            //if ()
        }

        _squashAndStretchCoroutine = StartCoroutine(SquashAndStretchEffect());

    }

    private IEnumerator SquashAndStretchEffect()
    {
        do
        {
            if (reverseAnimationCurveAfterPlayeing)
            {
                _isReversed = !_isReversed;
            }

            float elapsedTime = 0;
            Vector3 originalScale = _initialScaleVector;
            Vector3 modifiedScale = originalScale;

            while (elapsedTime < animationDuration)
            {
                elapsedTime += Time.deltaTime;

                float curvePosition = elapsedTime / animationDuration;

                if (_isReversed)
                {
                    curvePosition = 1 - (elapsedTime / animationDuration);
                }
                else
                {
                    curvePosition = (elapsedTime / animationDuration);
                }

                float curveValue = squashAndStretchCurve.Evaluate(curvePosition);
                float remappedValue = initialScale + (curveValue * (maximumScale - initialScale));

                float minimumThreshold = 0.0001f;
                if (Mathf.Abs(remappedValue) < minimumThreshold)
                {
                    remappedValue = minimumThreshold;
                }

                if (affectX)
                {
                    modifiedScale.x = originalScale.x * remappedValue;
                }
                else
                {
                    modifiedScale.x = originalScale.x / remappedValue;
                }

                if (affectY)
                {
                    modifiedScale.y = originalScale.y * remappedValue;
                }
                else
                {
                    modifiedScale.y = originalScale.y / remappedValue;
                }

                if (affectZ)
                {
                    modifiedScale.z = originalScale.z * remappedValue;
                }
                else
                {
                    modifiedScale.z = originalScale.z / remappedValue;
                }

                transformToAffect.localScale = modifiedScale;

                yield return null;
            }

            if (resetToInitialScaleAfterAnimation)
            {
                transformToAffect.localScale = originalScale;
            }

            if (looping)
            {
                yield return _loopingDelayWaitForSeconds;
            }
        }
        while (looping);
    }

    public void SetLooping(bool shouldLoop)
    {
        looping = shouldLoop;
    }

}