using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SpinObserver;

public class SpinGameFlow : MonoBehaviour
{
    private bool isWin;
    private bool isDead;
    private bool isGamePlay;
    private int round;

    private Dictionary<int, int> killAtRound = new Dictionary<int, int>()
    {
        { 0, 5 },
        { 1, 5 },
        { 2, 5 },
        { 3, 5 },
        { 4, 5 },
        { 5, 5 },
        { 6, 5 }
    };


    int[] fortuneOption1 = new int[] { 0, 2 };
    int[] fortuneOption2 = new int[] { 3, 6 };
    int[] fortuneOption3 = new int[] { 7 };
    int[] fortuneOption4 = new int[] { 5 };

    private float delayKill = 10f;
    private float randomEventsDealy = 16f;

    public void Init()
    {
        G.FortuneWhell.gameObject.SetActive(false);

        G.spinGamePlay.OnSpin += OnSpinWork;

        StartCoroutine(GameFlowRoutine());
        InitFortuneWhell();
    }


    private IEnumerator GameFlowRoutine()
    {
        if (TestBooleans.GetValue("IsPlayIntro"))
            yield return IntroRoutine(); //Вступление

        while (!isWin && !isDead)
        {
            if (TestBooleans.GetValue("PlayGame"))
            {
                yield return WaitPlayerGrap(); //Игра готова к началу, ждём игрока
                yield return GameRoutine(); //Процесс игры ("Волки проснулись")
            }

            if (isWin || isDead)
                    break;

            if (TestBooleans.GetValue("PlayQuiz"))
            {
                yield return QuizRoutine(); //Викторина ("Волки засыпают")
            }
        }

        if (isWin)
            yield return WinRoutine(); //Игрок победил
    }

    private IEnumerator IntroRoutine()
    {
        Debug.Log("IntroRoutine");

        var list = DictorSpeachManager.Speeches["Narrative_Intro"];
        foreach (var item in list)
            yield return Speak(item.GetText(DictorSpeachManager.language), item.AnimID);

        //Катсцена
    }

    private IEnumerator WaitPlayerGrap()
    {
        StartCoroutine(SpeakIDRoutine("Wait_Player"));

        bool playerGrap = false;
        Action<ISpinInterHold> action = (spinHold) =>
        {
            ITaggable taggable = spinHold as ITaggable;
            //Debug.Log($"taggable {taggable != null} {(taggable != null ? taggable.Tags.Contains("MainHandle") : (""))}");
            if (taggable != null && taggable.Tags.Contains("MainHandle"))
                playerGrap = true;
        };
        G.SpinInterManager.OnHold += action;

        yield return new WaitWhile(() => !playerGrap);

        G.SpinInterManager.OnHold -= action;
    }

    private IEnumerator GameRoutine()
    {
        isGamePlay = true;
        StartCoroutine(RandomEventsRoutine());
        StartCoroutine(FortuneWhellRoutine());

        StartCoroutine(SpeakIDRoutine("Dogs_Awake"));
        G.MusicManager.PlayMusic("GamePlay", 5f);

        float waitKill = delayKill;
        int killAtRound = this.killAtRound[round];

        if (G.GamerManager.CountGamers == 2)
            waitKill = killAtRound * delayKill;

        int targetCount = G.GamerManager.CountGamers - killAtRound;
        while (G.GamerManager.CountGamers > 1 && G.GamerManager.CountGamers > targetCount)
        {
            yield return new WaitForSeconds(waitKill);
            KillLast();
            Debug.Log($"{round}: {G.GamerManager.CountGamers}/{targetCount}");
        }

        if (G.GamerManager.CountGamers == 1)
            SetWin();

        isGamePlay = false;
        G.MusicManager.StopMusic();
        round++;
    }

    private void SetWin()
    {
        isWin = true;
    }

    private void KillLast()
    {
        G.GamerManager.KillLast();
    }

    private IEnumerator QuizRoutine()
    {
        isGamePlay = false;

        int[] hard = new int[] { 0, 2 };
        int[] middle = new int[] { 3, 4, 6 };
        int countQuest = 7;
        List<ChoicesInChoiceContent> choices = new List<ChoicesInChoiceContent>();

        bool isHard;
        bool isMiddle;

        for (int i = 0; i < countQuest; i++)
        {
            isHard = hard.Contains(i);
            isMiddle = middle.Contains(i);
            choices.Add(new ChoicesInChoiceContent(i,
                 (isHard ? "Hard" : (isMiddle ? "Medium" : "Easy")),
                 (isHard ? Color.red : (isMiddle ? Color.purple : Color.green))));
        }

        G.VictorinChoiceContent.SetChoices(choices.ToArray());

        G.VictorinChoiceWhell.Restart();

        bool isWait = true;
        Action action = () => isWait = false;
        G.VictorinChoiceWhell.OnEndSpin += action;

        yield return new WaitWhile(() => isWait); //Ждём, что игрок выеберет себе вопрос

        G.VictorinChoiceWhell.OnEndSpin -= action;

        int index = G.VictorinChoiceContent.GetCurrentChoiceIndex();
        isHard = hard.Contains(index);
        isMiddle = middle.Contains(index);

        SpinVictorinQuest quiz = null;

        if (isHard)
            quiz = DictorSpeachManager.GetRandomVictorin("Hard");
        else if (isMiddle)
            quiz = DictorSpeachManager.GetRandomVictorin("Medium");
        else
            quiz = DictorSpeachManager.GetRandomVictorin("Easy");

        yield return QuizWaitAnserRoutine(quiz);

    }

    private IEnumerator QuizWaitAnserRoutine(SpinVictorinQuest quiz)
    {
        ///Анимация появления колеса с выбором

        List<ChoicesInChoiceContent> choices = new List<ChoicesInChoiceContent>();
        choices.Add(new ChoicesInChoiceContent(0, "I don't know", Color.gray));

        int right = UnityEngine.Random.Range(1, 5);

        List<string> wrong = new List<string>();
        wrong.AddRange(quiz.GetWrongAnswers(DictorSpeachManager.language));

        for (int i = 1; i < 5; i++)
        {
            int n = UnityEngine.Random.Range(0, wrong.Count);
            if (right == i)
                choices.Add(new ChoicesInChoiceContent(i, quiz.GetRightText(DictorSpeachManager.language), i % 2 == 0 ? Color.yellow : Color.blue));
            else {
                choices.Add(new ChoicesInChoiceContent(i, wrong[n], i % 2 == 0 ? Color.yellow : Color.blue));
                wrong.RemoveAt(n);
            }
        }
        G.VictorineAnserContent.SetChoices(choices.ToArray());       

        StartCoroutine(SpeakWork(quiz.GetText(DictorSpeachManager.language), "Empty"));
        G.VictorineAnserAnimation.gameObject.SetActive(true);
        G.VictorineAnserAnimation.AnimationDown();

        yield return new WaitForSeconds(15f);
        //yield return new WaitForSeconds(2000f);

        int index = G.VictorineAnserContent.GetCurrentChoiceIndex();

        if(right == index) 
        {
            Debug.Log("И это правильный ответ!");
        }
        else
        {
            Debug.Log("И это НЕправильный ответ!");
        }
        yield return new WaitForSeconds(1f);

        G.VictorineAnserAnimation.AnimationUp();
        yield return new WaitForSeconds(1f);
    }

    public enum SpinGameMode { None }
    public SpinGameMode GameMode { get; private set; }

    private IEnumerator RandomEventsRoutine()
    {
        SetGameMode(SpinGameMode.None, true);

        while (isGamePlay)
        {
            yield return new WaitForSeconds(randomEventsDealy);
            yield return new WaitWhile(() => !PlayerSpinIsWork());
            RandomGameEventInvoke();
        }
    }

    private IEnumerator FortuneWhellRoutine()
    {
        int killAtRound = this.killAtRound[round];
        yield return new WaitForSeconds(UnityEngine.Random.Range(delayKill, (delayKill * killAtRound) * 0.8f));
        PlayerFortuneWhell();

        bool isWait = true;
        Action action = () => isWait = false;
        G.FortuneWhell.OnEndSpin += action;

        yield return new WaitWhile(() => isWait); //Ждём, что игрок сделает выбор на колесе

        G.FortuneWhell.OnEndSpin -= action;

        int index = G.FortuneContent.GetCurrentChoiceIndex();

        if (fortuneOption1.Contains(index))
        {
            Debug.Log("FortuneOption 1");
        }
        else if (fortuneOption2.Contains(index))
        {
            Debug.Log("FortuneOption 2");
        }
        else if (fortuneOption3.Contains(index))
        {
            Debug.Log("FortuneOption 3");
        }
        else if (fortuneOption4.Contains(index))
        {
            Debug.Log("FortuneOption 4");
        }
        else
        {
            Debug.Log("FortuneOption Empty");
        }
    }

    private void PlayerFortuneWhell()
    {
        G.FortuneWhell.Restart();

    }

    private void RandomGameEventInvoke()
    {
        //Вызываем случайное событие/смену правил
    }

    private bool PlayerSpinIsWork()
    {
        return true; //Работает ли колесо игрока
    }

    private IEnumerator WinRoutine()
    {
        Debug.Log("WIN!");
        yield break;
        //Катсцена
    }

    private IEnumerator SpeakIDRoutine (string id)
    {
        var speech = DictorSpeachManager.GetRandomSpeech(id);
        yield return Speak(speech.GetText(DictorSpeachManager.language), speech.AnimID);
    }

    private Coroutine coroutineSpeak;

    private IEnumerator Speak(string text, string dictorAnim)
    {
        if (coroutineSpeak != null)
            StopCoroutine(coroutineSpeak);

        coroutineSpeak = StartCoroutine(SpeakWork(text, dictorAnim));
        yield return coroutineSpeak;

        coroutineSpeak = null;
    }

    private IEnumerator SpeakWork(string text, string dictorAnim)
    {
        G.DictorAnimation.SetAnimation(dictorAnim);
        yield return G.DictorTextTyper.ClearAndTypeText(text, 100);
        yield return new WaitForSeconds(Mathf.Clamp(text.Length * 0.02f, 3f, float.MaxValue));
        G.DictorTextTyper.ClearText(100);
    }

    private void SetGameMode(SpinGameMode spinMode, bool withoutDictor)
    {
        var prevMode = GameMode;

        GameMode = spinMode;

        if (withoutDictor)
            return;

        ///Слова диктора, обозначающие стадию
    }

    float deltaSumQuater = 0f;
    private void OnSpinWork(SpinEventInfo info)
    {
        float delta = info.delta;

        if (Mathf.Sign(deltaSumQuater) != Mathf.Sign(delta))
        {
            if (Mathf.Abs(delta) > 0.5f)
            {
                deltaSumQuater = delta;
                return;
            }
            delta = -delta;
        }

        deltaSumQuater += delta;

        while (Mathf.Abs(deltaSumQuater) >= 90f)
        {
            AddQuatarForEvent(deltaSumQuater, Mathf.Sign(deltaSumQuater) == -1);
            deltaSumQuater -= deltaSumQuater > 0 ? 90f : -90f;
        }
    }

    private int r4;
    private int r2;
    private void AddQuatarForEvent(float deltaSumQuater, bool isClock)
    {
        InvokeRotateEvent(SpinRotateEventType.R4, isClock);
        r4++;
        if(r4 >= 2)
        {
            r4 = 0;
            InvokeRotateEvent(SpinRotateEventType.R2, isClock);
            r2++;

            if (r2 >= 2)
            {
                r2 = 0;
                InvokeRotateEvent(SpinRotateEventType.R1, isClock);
            }
        }
    }

    private void InvokeRotateEvent(SpinRotateEventType r4, bool isClock)
    {
        switch (r4)
        {
            case SpinRotateEventType.R4:
                break;
            case SpinRotateEventType.R2:
                break;
            case SpinRotateEventType.R1:
                G.GamerManager.PlayerProgress(1);
                break;
            default:
                break;
        }
    }

    private void InitFortuneWhell()
    {
        int countQuest = 10;
        List<ChoicesInChoiceContent> choices = new List<ChoicesInChoiceContent>();

        for (int i = 0; i < countQuest; i++)
        {
            if (fortuneOption1.Contains(i))
            {
                choices.Add(new ChoicesInChoiceContent(i, "option1",
                     Color.red));
            } else if (fortuneOption2.Contains(i))
            {
                choices.Add(new ChoicesInChoiceContent(i, "option2",
                     Color.yellow));
            }
            else if (fortuneOption3.Contains(i))
            {
                choices.Add(new ChoicesInChoiceContent(i, "option3",
                     Color.blue));
            }
            else if (fortuneOption4.Contains(i))
            {
                choices.Add(new ChoicesInChoiceContent(i, "option4",
                     Color.purple));
            }else
            {
                choices.Add(new ChoicesInChoiceContent(i, "",
                     Color.gray));
            }    
        }

        G.FortuneContent.SetChoices(choices.ToArray());
    }
}

public enum SpinRotateEventType { R4 , R2 , R1 };