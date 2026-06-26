using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ReciveItemText : MonoBehaviour
{
    [SerializeField] private TMP_Text tmp_item;
    [SerializeField] private float durationScale = 0.7f;
    [SerializeField] private float durationWait = 2f;

    private Coroutine coroutine;

    public void ShowItemInfo (SpinItemInfo itemInfo)
    {
        tmp_item.text += $"<b>{itemInfo.Name}!</b>\n" +
            $"{itemInfo.Description}";
        if (coroutine != null)
            StopCoroutine(coroutine);
        coroutine = StartCoroutine(ScaleRoutine());
    }

    private IEnumerator ScaleRoutine()
    {
        transform.DOKill();
        transform.localScale = Vector3.zero;
        yield return transform.DOScale(1.15f, durationScale).WaitForCompletion();
        yield return transform.DOScale(1f, durationScale / 9f).WaitForCompletion();
        yield return new WaitForSeconds(durationWait);
        yield return transform.DOScale(0f, durationScale * 1.5f).WaitForCompletion();
    }
}
