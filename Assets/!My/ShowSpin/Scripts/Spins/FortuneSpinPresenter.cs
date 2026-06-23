using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FortuneSpinPresenter : MonoBehaviour, ITaggable
{
    [SerializeField] private SpinObserver spinObserver;
    [SerializeField] private Transform root;
    [SerializeField] private float offsetY = 15f;
    [SerializeField] private SpinHandle[] spinHandles;
    [SerializeField] private float minForce = 1f;
    [SerializeField] private float minForceHold = 5f;
    [SerializeField] private float coefForceDuration = 1f;
    [SerializeField] private float coefForce = 4f;
    [SerializeField] private AnimationCurve animationCurve;
    private bool isCanRule;

    [SerializeField] private List<string> tags;
    public List<string> Tags => tags;

    public event Action OnEndSpin;

    private Coroutine coroutine;

    private void Start()
    {
        spinObserver.OnSpin += SpinWork;
    }

    public void Restart()
    {
        gameObject.SetActive(true);

        transform.localPosition = new Vector3(transform.localPosition.x, offsetY, transform.localPosition.z);
        if (coroutine != null)
            StopCoroutine(coroutine);
        transform.DOKill(true);
        coroutine = StartCoroutine(AnimationRoutine());
        deltaNow = 0;
    }

    private IEnumerator AnimationRoutine()
    {
        SetCanRule(false);

        yield return transform.DOLocalMove(new Vector3(transform.localPosition.x, 0f, transform.localPosition.z), 0.65f)
            .SetEase(Ease.InSine).WaitForCompletion();
        yield return transform.DOLocalMove(new Vector3(transform.localPosition.x, 0.5f, transform.localPosition.z), 0.15f)
            .SetEase(Ease.OutSine).WaitForCompletion();
        yield return transform.DOLocalMove(new Vector3(transform.localPosition.x, 0f, transform.localPosition.z), 0.15f)
            .SetEase(Ease.InSine).WaitForCompletion();

        SetCanRule(true);
        deltaRotate = 0f;

        yield return new WaitWhile(() => deltaRotate == 0f);

        float wait = 0;
        while (wait < coefForceDuration)
        {
            float offsetX = deltaRotate * coefForce *
                animationCurve.Evaluate(wait / coefForceDuration) * Time.deltaTime;

            //Debug.Log($"{deltaRotate} * {coefForce} * {animationCurve.Evaluate(wait / coefForceDuration)} * {Time.deltaTime}");

            //Debug.Log($"offsetX {offsetX}");
            spinObserver.SpinMain.AddRotation(offsetX);

            yield return new WaitForEndOfFrame();
            wait += Time.deltaTime;
        }

        yield return transform.DOPunchScale(Vector3.one * 0.35f, 0.4f).WaitForCompletion();
        yield return new WaitForSeconds(3f);

        yield return transform.DOLocalMove(new Vector3(transform.localPosition.x, offsetY, transform.localPosition.z), 0.25f)
            .SetEase(Ease.InOutSine).WaitForCompletion();

        gameObject.SetActive(false);

        OnEndSpin?.Invoke();
    }

    private void SetCanRule(bool canRule)
    {
        foreach (var handle in spinHandles)
        {
            handle.enabled = canRule;
        }

        isCanRule = canRule;
    }

    float deltaRotate;
    float deltaNow;

    private void SpinWork(SpinObserver.SpinEventInfo info)
    {
        if (!isCanRule)
            return;

        if (Mathf.Abs(info.delta) <= minForce)
            return;

        deltaNow += info.delta * Time.deltaTime;
        if (Mathf.Abs(deltaNow) < minForceHold)
            return;

        deltaRotate = deltaNow / Time.deltaTime;
        SetCanRule(false);
    }
}
