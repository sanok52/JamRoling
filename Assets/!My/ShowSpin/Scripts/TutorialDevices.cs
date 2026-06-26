using DG.Tweening;
using System.Collections;
using UnityEngine;

public class TutorialDevices : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        StartCoroutine(WaitToTutorial());
    }

    private IEnumerator WaitToTutorial()
    {
        yield return new WaitUntil(() => G.GamerManager.SpinGamers["Player"].IsBroke);
        canvasGroup.DOFade(1f, 2f);
        yield return new WaitUntil(() => !G.GamerManager.SpinGamers["Player"].IsBroke);
        gameObject.SetActive(false);
    }
}
