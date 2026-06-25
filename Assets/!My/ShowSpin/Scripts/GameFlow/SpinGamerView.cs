using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpinGamerView : MonoBehaviour 
{
    public string ID => gameObject.name;
    public TMP_Text textCount;

    [Space]
    public Animator animatorCharacter;
    public Animator animatorTunel;
    public Transform transformDead;

    public void TryUpdateProgress(string id, int value)
    {
        if (id != ID)
            return;

        if(textCount != null)
            textCount.text = value.ToString();
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
    }

    public void SetSpeedAnim (float speed = 1f)
    {
        if (animatorCharacter != null)
            animatorCharacter.speed = speed;
    }

    public void Shtraf()
    {
        if (textCount == null)
            return;

        textCount.DOKill(true);
        textCount.DOColor(Color.red, 0.3f).SetLoops(3, LoopType.Yoyo).OnComplete(() =>
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
