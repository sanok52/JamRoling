using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinUpObjectAnimation : MonoBehaviour, ITaggable
{
    [SerializeField] private Transform root;
    [SerializeField] private float offsetY = 15f;

    [SerializeField] private List<string> tags;
    public List<string> Tags => tags;

    public event Action OnAnimationEnd;

    private void Start()
    {
    }

    public void AnimationDown()
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(AnimationDownRoutine());
    }

    public void AnimationUp()
    {
        StopAllCoroutines();
        gameObject.SetActive(true);
        StartCoroutine(AnimationUpRoutine());
    }

    public IEnumerator AnimationDownRoutine()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, offsetY, transform.localPosition.z);

        yield return transform.DOLocalMove(new Vector3(transform.localPosition.x, 0f, transform.localPosition.z), 0.65f)
            .SetEase(Ease.InSine).WaitForCompletion();
        yield return transform.DOLocalMove(new Vector3(transform.localPosition.x, 0.5f, transform.localPosition.z), 0.15f)
            .SetEase(Ease.OutSine).WaitForCompletion();
        yield return transform.DOLocalMove(new Vector3(transform.localPosition.x, 0f, transform.localPosition.z), 0.15f)
            .SetEase(Ease.InSine).WaitForCompletion();

        yield return transform.DOPunchScale(Vector3.one * 0.35f, 0.4f).WaitForCompletion();
        yield return new WaitForSeconds(3f);

        OnAnimationEnd?.Invoke();
    }

    public IEnumerator AnimationUpRoutine()
    {
        yield return transform.DOLocalMove(new Vector3(transform.localPosition.x, offsetY, transform.localPosition.z), 0.25f)
            .SetEase(Ease.InOutSine).WaitForCompletion();
        gameObject.SetActive(false);
    }
}