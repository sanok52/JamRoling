using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class SpinGamerView : MonoBehaviour 
{
    public string ID => gameObject.name;
    public TMP_Text textCount;
    public TMP_Text textCountMulty;

    [Space]
    public Animator animatorCharacter;
    public Animator animatorTunel;
    public Transform transformDead;

    [Space]
    public ParticleSystem[] fxRemont;
    public ScreenRemont screenRemont;
    public AudioSource[] remontSources;
    public static List<SpinGamerView> brokeList = new List<SpinGamerView>();

    public bool IsBroke { get; private set; }

    private void Start()
    {
        brokeList.Clear();
    }

    public void TryUpdateProgress(string id, int value)
    {
        if (id != ID)
            return;

        if(textCount != null)
            textCount.text = value.ToString();
    }

    public void TryUpdateMulty (int count)
    {
        if (textCountMulty != null)
            textCountMulty.text = count == 1 ? "" : $"x{count}";
    }

    public void Dead()
    {
        if (animatorTunel != null)
            animatorTunel.SetTrigger("Dead");

        DOVirtual.DelayedCall(0.7f, () =>
        {
            transformDead.DOMove(new Vector3(transformDead.position.x, - 34.8f, transformDead.position.z), 5f)
            .SetEase(Ease.Linear);
        });

        foreach (var item in remontSources)
            item.Stop();
    }

    public void WhellAnim()
    {
        if (animatorCharacter != null)
            animatorCharacter.SetTrigger("StartWell");
    }

    public void IdleAnim()
    {
        if (animatorCharacter != null)
            animatorCharacter.SetTrigger("Idle");
        SetSpeedAnim();
    }

    public void FixAnim()
    {
        if (animatorCharacter != null)
            animatorCharacter.SetTrigger("Fix");
        SetSpeedAnim();

        foreach (var item in fxRemont)        
            item.Play();

        StartCoroutine(PlaySoundBroke());

        if (screenRemont != null)
            screenRemont.StartRemont();

        IsBroke = true;
    }

    private IEnumerator PlaySoundBroke()
    {
        if (brokeList.Count > 1)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.7f, 2.5f));
            if (brokeList.Count > 1)
                yield break;
        }

        foreach (var item in remontSources)
            item.Play();

        brokeList.Add(this);
        yield return new WaitForSeconds(0.5f);
        brokeList.Remove(this);

    }

    public void EndFix()
    {
        foreach (var item in fxRemont)
            item.Stop();

        foreach (var item in remontSources)
            item.Stop();

        if (G.SpinGameFlow.IsGamePlay)
            WhellAnim();
        else
            IdleAnim();

        IsBroke = false;
    }

    public void SetSpeedAnim (float speed = 1f)
    {
        if (animatorCharacter != null)
            animatorCharacter.speed = speed;
    }

    public void Shtraf()
    {
        Shtraf(Color.red);
    }

    public void Shtraf(Color color)
    {
        if (textCount == null)
            return;

        textCount.DOKill(true);
        textCount.DOColor(color, 0.3f).SetLoops(3, LoopType.Yoyo).OnComplete(() =>
        {
            textCount.color = Color.yellow;
        }).SetAutoKill();
    }

    public void TriggerAnimation(string triggerAnim)
    {
        if (animatorCharacter != null)
            animatorCharacter.SetTrigger(triggerAnim);
    }
}
