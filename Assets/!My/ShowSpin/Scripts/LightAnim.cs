using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LightAnim : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform trUp;

    private void Start()
    {
        StartCoroutine(AnimRoutine());
    }

    private IEnumerator AnimRoutine()
    {
        animator.gameObject.SetActive(true);
        animator.SetTrigger("Light");
        yield return new WaitForSeconds(5f);
        trUp.DOMove(trUp.position + (Vector3.up * 12), 25f).SetEase(Ease.InSine);
        yield return new WaitForSeconds(20f);

        CanvasGroup canvasGroup = GameObject.Find("BlackScreen").GetComponent<CanvasGroup>();
        Image image = canvasGroup.GetComponent<Image>();
        image.color = Color.white;

        yield return canvasGroup.DOFade(1f, 4f).WaitForCompletion();
        yield return image.DOColor(Color.black, 2f).WaitForCompletion();
    }
}
