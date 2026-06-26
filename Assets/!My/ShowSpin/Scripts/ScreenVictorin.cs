using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ScreenVictorin : MonoBehaviour
{
    [SerializeField] private TMP_Text tmpTimer;
    [SerializeField] private float waitAnser = 15f;
    [SerializeField] private TextTyper textTyper;

    public IEnumerator StartQuiz(SpinVictorinQuest quest, float waitTarget = -1f)
    {
        SetQuizText(quest.GetText(DictorSpeachManager.language));

        float wait = 0f;
        if (waitTarget <= 0)
            waitTarget = waitAnser;

        while (wait < waitTarget) 
        {
            tmpTimer.text = System.Math.Round(waitTarget - wait, 0).ToString();

            yield return null;
            wait += Time.deltaTime;
        }

        tmpTimer.text = $"0!";

        yield return textTyper.PlayClearText();
    }

    public void SetQuizText (string text)
    {
        StartCoroutine(textTyper.ClearAndTypeText(text, 70));
    }

    public IEnumerator SetQuizTextRoutine (string text)
    {
        yield return textTyper.ClearAndTypeText(text, 70);
    }

    public IEnumerator SetGetItemObject(SpinItemInfo itemInfoPure)
    {
        string info = $"YOU HAVE WON...\n\n" +
            $"<wave=1,1>{itemInfoPure.Name}";
        SetQuizText(info);

        yield return new WaitForSeconds(2f);

        yield return SetQuizTextRoutine($"{itemInfoPure.Description}");

        yield return new WaitForSeconds(1f + (itemInfoPure.Description.Length * 0.05f));

        yield return textTyper.PlayClearText();
    }
}