using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static SpinObserver;

public class SpinGameFlow : MonoBehaviour
{
    private bool isWin;
    private bool isDead;
    private bool isGamePlay;
    private int round;

    private Dictionary<int, int> killAtRound = new Dictionary<int, int>()
    {
        { 0, 10 },
        { 1, 10 },
        { 2, 10 },
        { 3, 10 },
        { 4, 10 },
        { 5, 10 },
        { 6, 10 }
    };

    private Dictionary<int, (float, float)> radiusAtRound = new Dictionary<int, (float, float)>()
    {
        { 0, (0.45f, 0.75f) },
        { 1, (0.45f, 0.75f) },
        { 2, (0.45f, 0.75f)  },
        { 3, (0.45f, 0.75f)  },
        { 4, (0.55f, 2.1f)  },
        { 5,  (0.55f, 2.1f) },
        { 6, (0.55f, 2.1f) },
        { 7, (0.55f, 2.1f) }
    };


    int[] fortuneOption1 = new int[] { 0, 2 };
    int[] fortuneOption2 = new int[] { 3, 5 };
    int[] fortuneOption3 = new int[] { 7, 6 };
    int[] fortuneOption4 = new int[] { 5 };

    private float delayKill = 7f;
    private float randomEventsDealy = 16f;

    private float playerDeltaObesrv;

    private List<SpinGameMode> tutorModes = new List<SpinGameMode>()
    {
        SpinGameMode.Clock,
        SpinGameMode.Fog,
        SpinGameMode.Fog,
        SpinGameMode.DevicePlayerBreak
    };

    public void Init()
    {
        Time.timeScale = 1f;

        G.FortuneWhell.gameObject.SetActive(false);
        G.spinGamePlay.OnSpin += OnSpinWork;
        G.GamerManager.OnGamerDead += OnGamerDeadWork;
        G.LeaderBoardUI.OnLeadersChanged += LeadersChangedWork;

        G.GamerManager.OnGamerBroke += GamerBrokeWork;
        G.ScreenRemont.OnRemont += () => G.GamerManager.BrokePlayer(false);
        G.GamerManager.OnGamerProgressDelta += GamerDeltaProgressWork;

        DictorSpeachManager.SetVariable("Player_Name", "N-451");

        StartCoroutine(GameFlowRoutine());

        PocketRandomazer.CreatePocket<SpinGameMode>("RandomEvents", SpinGameMode.Fog, SpinGameMode.Fog, SpinGameMode.Unclock, SpinGameMode.Clock,
            SpinGameMode.Clock, SpinGameMode.Unclock, SpinGameMode.Fog, SpinGameMode.Unclock, SpinGameMode.Clock, 
            SpinGameMode.DevicePlayerBreak);

        InitFortuneWhell();
        G.LeaderBoardUI.ChangeProgeressWork(G.GamerManager.GamersScores);
    }

    private void GamerDeltaProgressWork(string id, int progress)
    {
        if (id != "Player")
            return;

        if(progress < 0)
        {

        }
    }

    private IEnumerator GameFlowRoutine()
    {
        if (!isSkipStart)
            yield return MenuRoutine();
        else
            G.MenuManager.SetMenuState(false);

        //G.spinGamePlay.gameObject.SetActive(true);
        G.MusicManager.StopMusic();

        yield return new WaitForSeconds(1.5f);

        if (TestBooleans.GetValue("IsPlayIntro") && !isSkipStart)
            yield return IntroRoutine(); //Вступление

        if (TestBooleans.GetValue("IsPlayGame"))
        {
            while (!isWin && !isDead)
            {
                if (TestBooleans.GetValue("PlayGame"))
                {
                    yield return WaitPlayerGrap(); //Игра готова к началу, ждём игрока
                    yield return GameRoutine(); //Процесс игры ("Волки проснулись")
                }

                isFortune = false;

                G.VictorineAnserAnimation.AnimationUp();
                G.FortuneWhell.UpNow = true;
                BreakMultyplay();

                if (isWin || isDead)
                    break;

                yield return new WaitForSeconds(2.5f);
                yield return SpinGrow(radiusAtRound[round].Item1, radiusAtRound[round].Item2);

                if (TestBooleans.GetValue("PlayQuiz"))
                {
                    yield return QuizGameStateRoutine(); //Викторина ("Волки засыпают")
                }
            }
        }

        G.FortuneWhell.Break();
        StopCoroutine("RandomEventsRoutine");
        StopCoroutine("FortuneWhellRoutine");
        StopCoroutine("FortuneWhellImmidiatlyRoutine"); 
        StopCoroutine("GamerUpdateRoutine");
        StopCoroutine("QuizWaitAnserRoutine");
        StopCoroutine("QuizRoutine");
        isFortune = false;

        if (isWin || TestBooleans.GetValue("IsWin"))
            yield return WinRoutine(); //Игрок победил
        else
            yield return LoseRoutine();
    }

    public IEnumerator SpinGrow(float visual, float radius)
    {
        if (G.handlesMain[0].CoefRadius == radius)
            yield break;

        Transform trScale = G.spinGamePlay.SpinMain.transform;
        Vector3 targetScale = new Vector3(visual, trScale.localScale.y, visual);
        trScale.DOScale(targetScale, 2.5f).OnComplete(() => trScale.DOPunchScale(Vector3.one * 0.15f, 0.4f));        
        trScale.GetComponent<AudioSource>().Play();

        foreach (var item in G.handlesMain)
        {
            item.CoefRadius = radius;
        }

        G.GamerManager.CoefHard = 0.7f;

        yield return new WaitForSeconds(3.5f);
    }

    private IEnumerator LoseRoutine()
    {
        yield return new WaitForSeconds(6f);
        CanvasGroup canvasGroup = GameObject.Find("BlackScreen").GetComponent<CanvasGroup>();
        Image image = canvasGroup.GetComponent<Image>();
        image.color = Color.black;

        yield return canvasGroup.DOFade(1f, 4f).WaitForCompletion();
        Reload(true);
    }

    private IEnumerator MenuRoutine()
    {
        yield return G.MenuManager.WaitPlayerAction();
    }

    private IEnumerator IntroRoutine()
    {
        var list = DictorSpeachManager.Speeches["Narrative_Intro"];
        foreach (var item in list)
            yield return Speak(item.GetText(DictorSpeachManager.Language), item.AnimID);

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
        G.FortuneWhell.UpNow = false;
        G.handlesFixes.ForEach(x => x.enabled = true);

        SetGameMode(SpinGameMode.None, true);

        StartCoroutine(RandomEventsRoutine());
        StartCoroutine(FortuneWhellRoutine());
        StartCoroutine(GamerUpdateRoutine());

        StartCoroutine(SpeakIDRoutine("Dogs_Awake"));
        G.MusicManager.PlayMusic("GamePlay", 5f);

        float waitKill = delayKill;
        int killAtRound = this.killAtRound[round];
;

        G.GamerManager.SetPlayState(true);
        
        foreach (var gamer in G.GamerManager.SpinGamers.Values)
        {
            if (gamer.IsDead)
                continue;
            gamer.View.WhellAnim();
        }

        int targetCount = G.GamerManager.CountGamers - killAtRound;
        if (G.GamerManager.CountGamers == 2 || round >= 4)
        {
            waitKill = this.killAtRound[0] * 0.5f * delayKill / G.GamerManager.CountGamers;
            targetCount = 1;
        }

        while (G.GamerManager.CountGamers > 1 && G.GamerManager.CountGamers > targetCount && !IsSkipGamePlay)
        {
            yield return new WaitForSeconds(waitKill);

            var gamer = G.GamerManager.GetLast();
            if (gamer.ID == "Player")
            {
                yield return new WaitForSeconds(12f / (round + 1));
                gamer = G.GamerManager.GetLast();
            }

            G.GamerManager.Kill(gamer);

            Debug.Log($"{round}: {G.GamerManager.CountGamers}/{targetCount}");

            if (isDead)
                yield break;
        }
        IsSkipGamePlay = false;

        G.GamerManager.SetPlayState(false);

        isGamePlay = false;

        G.MusicManager.StopMusic();
        SetGameMode(SpinGameMode.None, true);

        if (G.GamerManager.CountGamers == 1)
        {
            SetWin();
            yield break;
        }

        foreach (var gamer in G.GamerManager.SpinGamers.Values)
        {
            if (gamer.IsDead)
                continue;
            gamer.View.IdleAnim();
        }

        G.handlesFixes.ForEach(x => x.enabled = false);

        yield return SpeakIDRoutine("Dogs_Sleep");

        round++;
    }

    private void SetWin()
    {
        isWin = true;
    }

    private IEnumerator QuizGameStateRoutine()
    {
        SetGameMode(SpinGameMode.None, true);
        isGamePlay = false;

        foreach (var gamer in G.GamerManager.SpinGamers.Values)
        {
            if (gamer.IsDead || gamer.ID == "Player")
                continue;

            G.GamerManager.Broke(gamer.ID, false);
        }

        yield return QuizRoutine();
    }

    private IEnumerator QuizRoutine(float wait = -1)
    {
        while(isFortune)
        {
            yield return new WaitForEndOfFrame();
        }

        isFortune = true;

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
        G.VictorinChoiceWhell.Restart(isGamePlay);

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

        yield return QuizWaitAnserRoutine(quiz, isHard ? 2 : (isMiddle ? 1 : 0), wait);

        isFortune = false;
    }

    private IEnumerator QuizWaitAnserRoutine(SpinVictorinQuest quiz, int hardQ, float wait = -1)
    {
        List<ChoicesInChoiceContent> choices = new List<ChoicesInChoiceContent>();
        choices.Add(new ChoicesInChoiceContent(0, "I don't know", Color.gray));

        int right = UnityEngine.Random.Range(1, 5);

        List<string> wrong = new List<string>();
        wrong.AddRange(quiz.GetWrongAnswers(DictorSpeachManager.Language));

        for (int i = 1; i < 5; i++)
        {
            int n = UnityEngine.Random.Range(0, wrong.Count);
            if (right == i)
                choices.Add(new ChoicesInChoiceContent(i, quiz.GetRightText(DictorSpeachManager.Language), i % 2 == 0 ? Color.yellow : Color.blue));
            else {
                choices.Add(new ChoicesInChoiceContent(i, wrong[n], i % 2 == 0 ? Color.yellow : Color.blue));
                wrong.RemoveAt(n);
            }
        }
        G.VictorineAnserContent.SetChoices(choices.ToArray());

        //StartCoroutine(SpeakWork("", "Empty"));
        G.VictorineAnserAnimation.gameObject.SetActive(true);
        G.VictorineAnserAnimation.AnimationDown();

        yield return G.ScreenVictorin.StartQuiz(quiz, wait);

        int index = G.VictorineAnserContent.GetCurrentChoiceIndex();

        if (right == index)
        {
            G.ScreenVictorin.SetQuizText("That's the right answer!");
            G.ItemExecuter.InvokeEvent(BehActionType.QuizRight, quiz, hardQ);
        }
        else
        {
            G.ScreenVictorin.SetQuizText("This is the wrong answer!");
            G.ItemExecuter.InvokeEvent(BehActionType.QuizWrong, quiz, hardQ);
        }

        G.ScreenVictorin.PlayAnserSound(right == index);
        if (right == index)
        {
            if (isGamePlay)
            {
                yield return new WaitForSeconds(1f);

                int point = (int)(110 * (1f + (0.2f * hardQ))) + quizCorrectBonus;
                G.ScreenVictorin.SetQuizText($"You get {point} points!");
                G.GamerManager.PlayerProgress(point);

                yield return new WaitForSeconds(3f);
            }
            else
            {
                yield return new WaitForSeconds(2f);

                var itemInfo = G.ItemExecuter.GetRandomVictorinItem();
                var itemInfoPure = G.ItemExecuter.GetPureInfo(itemInfo);
                G.ItemExecuter.AddItemInList(itemInfo.ID);

                int point = (int)(20 * (1f + (0.2f * hardQ))) + quizCorrectBonus;
                G.GamerManager.PlayerProgress(point);

                yield return G.ScreenVictorin.SetGetItemObject(itemInfoPure);
            }
        }
        else
        {
            if (isGamePlay)
                yield return new WaitForSeconds(2f);
            else
                yield return new WaitForSeconds(3f);
        }

        yield return new WaitForSeconds(0.5f);

        G.VictorineAnserAnimation.AnimationUp();
        if (!isGamePlay)
            yield return new WaitForSeconds(1f);
    }

    public enum SpinGameMode { None, Clock, Unclock, Fog,
        DevicePlayerBreak
    }
    public SpinGameMode GameMode { get; private set; }
    public bool IsGamePlay { get => isGamePlay; set => isGamePlay = value; }
    public bool IsSkipGamePlay { get; private set; }

    [HideInInspector] public bool replaceRotateFortune;

    bool isTutorWhell;
    private IEnumerator RandomEventsRoutine()
    {
        SetGameMode(SpinGameMode.None, true);

        if (isTutorWhell == false && tutorModes.Count > 0)
        {
            yield return new WaitForSeconds(randomEventsDealy);
            FortuneWhellImmidiatly();
            isTutorWhell = true;
            yield return new WaitWhile(() => isFortune && isGamePlay);
        }

        while (isGamePlay)
        {
            if (tutorModes.Count > 0)
            {
                yield return new WaitForSeconds(randomEventsDealy / 1.4f);
                if (isGamePlay)
                {
                    SetGameMode(tutorModes[0], false);
                    tutorModes.RemoveAt(0);
                    yield return new WaitForSeconds(randomEventsDealy / 1.35f);
                    SetGameMode(SpinGameMode.None, false);
                }
            }
            else
            {
                yield return new WaitForSeconds(randomEventsDealy);
                yield return new WaitWhile(() => !PlayerSpinIsWork() && isGamePlay);
                if (isGamePlay)
                    RandomGameEventInvoke();
            }
        }
    }

    private bool isFortune = false;
    private IEnumerator FortuneWhellRoutine()
    {
        while (isGamePlay && tutorModes.Count > 0)        
            yield return new WaitForEndOfFrame();        

        if (!isGamePlay)        
            yield break;        

        int killAtRound = this.killAtRound[round];
        yield return new WaitForSeconds(UnityEngine.Random.Range(delayKill, (delayKill * killAtRound) * 0.8f));
        FortuneWhellImmidiatly();
    }

    public void FortuneWhellImmidiatly()
    {
        if (isFortune)
            return;

        if (!isGamePlay)
            return;

        if (replaceRotateFortune && (GameMode == SpinGameMode.Clock || GameMode == SpinGameMode.Unclock))
        {
            StartCoroutine(QuizRoutine(15));
        }else
            StartCoroutine(FortuneWhellImmidiatlyRoutine());
    }

    private IEnumerator FortuneWhellImmidiatlyRoutine()
    {
        if (isFortune)
            yield break;

        isFortune = true;
        PlayerFortuneWhell();

        bool isWait = true;
        bool noChoice = false;

        Action action = () => isWait = false;
        G.FortuneWhell.OnEndSpin += action;

        Action actionNoChoice = () => noChoice = true;
        G.FortuneWhell.OnNoChoice += actionNoChoice;

        yield return new WaitWhile(() => isWait && !noChoice); //Ждём, что игрок сделает выбор на колесе

        G.FortuneWhell.OnEndSpin -= action;

        if (noChoice)
        {
            G.ItemExecuter.InvokeEvent(BehActionType.FortuneWhellMiss);
        }
        else
        {
            int index = G.FortuneContent.GetCurrentChoiceIndex();
            ExecuteFortuneWhellOption(index);
        }

        isFortune = false;
        G.ItemExecuter.InvokeEvent(BehActionType.FortuneWhellEnd);

        if(delayFortune > 0)
        {
            if (!isGamePlay)
            {
                delayFortune = 0;
                yield break;
            }

            delayFortune--;
            FortuneWhellImmidiatly();
        }
    }

    private void ExecuteFortuneWhellOption(int index)
    {
        if (fortuneOption1.Contains(index))
        {
            G.ItemExecuter.AddItemInList(G.ItemExecuter.FortuneItems[0].ID);
            G.ReciveItemText.ShowItemInfo(G.ItemExecuter.GetPureInfo(G.ItemExecuter.FortuneItems[0]));
            Debug.Log($"{G.ItemExecuter.FortuneItems[0].ID} option");
        }
        else if (fortuneOption2.Contains(index))
        {
            G.ItemExecuter.AddItemInList(G.ItemExecuter.FortuneItems[1].ID);
            G.ReciveItemText.ShowItemInfo(G.ItemExecuter.GetPureInfo(G.ItemExecuter.FortuneItems[1]));
            Debug.Log($"{G.ItemExecuter.FortuneItems[1].ID} option");
        }
        else if (fortuneOption3.Contains(index))
        {
            G.ItemExecuter.AddItemInList(G.ItemExecuter.FortuneItems[2].ID);
            G.ReciveItemText.ShowItemInfo(G.ItemExecuter.GetPureInfo(G.ItemExecuter.FortuneItems[2]));
            Debug.Log($"{G.ItemExecuter.FortuneItems[2].ID} option");
        }
        else if (fortuneOption4.Contains(index))
        {
            G.ItemExecuter.AddItemInList(G.ItemExecuter.FortuneItems[4].ID);
            G.ReciveItemText.ShowItemInfo(G.ItemExecuter.GetPureInfo(G.ItemExecuter.FortuneItems[4]));
            Debug.Log($"{G.ItemExecuter.FortuneItems[4].ID} option");
        }
        else
        {
            G.ItemExecuter.InvokeEvent(BehActionType.FortuneWhellOptionEmpty);
        }
    }

    private float lastRandomQuips = 0;
    private IEnumerator GamerUpdateRoutine()
    {
        G.GamerManager.SetPlayState(true);
        while (isGamePlay)
        {
            G.GamerManager.Update();

            if(Time.time - lastTimeSpeach > 6f && Time.time - lastRandomQuips > 20f)
            {
                StartCoroutine(SpeakIDRoutine("Random_Quips"));
                lastRandomQuips = Time.time;
            }

            yield return new WaitForEndOfFrame();
        }
        G.GamerManager.SetPlayState(false);
    }

    private void PlayerFortuneWhell()
    {
        G.FortuneWhell.Restart();
        StartCoroutine(SpeakIDRoutine("Fortune_Wheel"));
        G.ItemExecuter.InvokeEvent(BehActionType.FortuneWhellInvoke);
    }

    private bool PlayerSpinIsWork()
    {
        return true; //Работает ли колесо игрока
    }

    private IEnumerator WinRoutine()
    {
        SetGameMode(SpinGameMode.None, true);
        G.MusicManager.StopMusic();
        G.LeaderBoardUI.ChangeProgeressWork(G.GamerManager.GamersScores);

        var list = DictorSpeachManager.Speeches["Final_Win_Speech"];
        for (int i = 0; i < list.Count - 3; i++)
        {
            SpinDictorSpeech item = list[i];
            yield return Speak(item.GetText(DictorSpeachManager.Language), item.AnimID);
        }

        LightAnim lightAnim = GameObject.FindFirstObjectByType<LightAnim>(FindObjectsInactive.Include);
        lightAnim.gameObject.SetActive(true);

        yield return Speak(list[list.Count - 3].GetText(DictorSpeachManager.Language), list[list.Count - 3].AnimID);
        yield return Speak(list[list.Count - 2].GetText(DictorSpeachManager.Language), list[list.Count - 2].AnimID);
        yield return Speak(list[list.Count - 1].GetText(DictorSpeachManager.Language), list[list.Count - 1].AnimID);

        yield return new WaitUntil(() => lightAnim.IsEnd);
        yield return new WaitForSeconds(2.5f);

        var thansk = GameObject.Find("Text (End)").gameObject;
        Vector3 targetScale = thansk.transform.localScale;

        thansk.transform.localScale = Vector3.zero;
        thansk.GetComponent<TMP_Text>().text = "Thanks for playing!";

        yield return thansk.transform.DOScale(targetScale, 1f).WaitForCompletion();

        yield return new WaitForSeconds(3f);

        Reload(false);
    }

    private IEnumerator SpeakIDRoutine (string id)
    {
        var speech = DictorSpeachManager.GetRandomSpeech(id);

        string text = speech.GetText(DictorSpeachManager.Language);
        bool next = false;
        Action<string> action = (txt) => { if (txt == text) next = true; };
        OnLocalSpeakEnd += action;
        StartCoroutine(Speak(text, speech.AnimID));

        yield return new WaitWhile(() => next);

        OnLocalSpeakEnd -= action;
    }


    event Action<string> OnLocalSpeakEnd;
    string textPrevSpeak = "";
    private Coroutine coroutineSpeak;

    private IEnumerator Speak(string text, string dictorAnim)
    {
        if (coroutineSpeak != null)
        {
            OnLocalSpeakEnd?.Invoke(textPrevSpeak);
            StopCoroutine(coroutineSpeak);
        }

        lastTimeSpeach = Time.time;
        textPrevSpeak = text;
        coroutineSpeak = StartCoroutine(SpeakWork(text, dictorAnim));
        yield return coroutineSpeak;

        coroutineSpeak = null;
        yield return new WaitForSeconds(1f);
        G.DictorAnimation.SetAnimation("Empty");

        OnLocalSpeakEnd?.Invoke(text);
    }

    private IEnumerator SpeakWork(string text, string dictorAnim)
    {
        G.DictorAnimation.SetAnimation(dictorAnim);
        yield return G.DictorTextTyper.ClearAndTypeText(text, 100);
        yield return new WaitForSeconds(Mathf.Clamp(text.Length * 0.027f, 3f, float.MaxValue));
        G.DictorTextTyper.ClearText(100);
    }

    private void RandomGameEventInvoke()
    {
        SetGameMode(PocketRandomazer.GetRandomElement<SpinGameMode>("RandomEvents"), false);
    }

    public void SetGameMode(SpinGameMode spinMode, bool withoutDictor)
    {
        if (!isGamePlay)
        {
            GameMode = SpinGameMode.None;
            G.GameModeUI.SetMode(spinMode);
            return;
        }

        var prevMode = GameMode;
        prevGameModeTime = Time.time;

        if (playerDeltaObesrv > 0f && spinMode == SpinGameMode.Unclock)
            spinMode = SpinGameMode.Clock;
        else if(playerDeltaObesrv < 0f && spinMode == SpinGameMode.Clock)
            spinMode = SpinGameMode.Unclock;


        GameMode = spinMode;
        G.GameModeUI.SetMode(spinMode);

        Vector2 speedAnim =  new Vector2(0, 0);
        List<string> triggerAnim = new List<string>() { };
        switch (spinMode)
        {
            case SpinGameMode.None:
                switch (prevMode)
                {
                    case SpinGameMode.Clock:
                        //StartCoroutine(SpeakIDRoutine("Direction_Change"));
                        break;
                    case SpinGameMode.Unclock:
                        //StartCoroutine(SpeakIDRoutine("Direction_Change"));
                        break;
                    case SpinGameMode.Fog:
                        if (!withoutDictor)
                            StartCoroutine(SpeakIDRoutine("Fog_End"));
                        break;

                    case SpinGameMode.DevicePlayerBreak:
                        if (!withoutDictor)
                            StartCoroutine(SpeakIDRoutine("Device_Fixed"));
                        break;

                    default:
                        break;
                }

                speedAnim = isGamePlay ? new Vector2(1.4f, 2f) : new Vector2(0.95f, 1.1f);
                if (isGamePlay)
                {
                    triggerAnim.Add("WhellRight");
                    triggerAnim.Add("WhellLeft");
                }else
                    triggerAnim.Add("Idle");

                break;

            case SpinGameMode.Clock:
                if (!withoutDictor)
                    StartCoroutine(SpeakIDRoutine("Direction_Change"));

                speedAnim = new Vector2(1.4f, 2f);
                triggerAnim.Add("WhellLeft");

                break;

            case SpinGameMode.Unclock:
                if (!withoutDictor)
                    StartCoroutine(SpeakIDRoutine("Direction_Change"));

                speedAnim = new Vector2(1.4f, 2f);
                triggerAnim.Add("WhellRight");

                break;

            case SpinGameMode.Fog:
                if (!withoutDictor)
                    StartCoroutine(SpeakIDRoutine("Fog_Start"));

                speedAnim = new Vector2(0.15f, 0.3f);
                triggerAnim.Add("WhellRight");
                triggerAnim.Add("WhellLeft");

                break;

            case SpinGameMode.DevicePlayerBreak:
                G.GamerManager.BrokePlayer(true);

                break;

            default:
                break;
        }


        if (speedAnim != Vector2.zero)
        {
            foreach (var gamer in G.GamerManager.SpinGamers.Values)
            {
                if (gamer.IsDead)
                    continue;

                gamer.View.TriggerAnimation(triggerAnim.ToArray().RandomElement());
                gamer.View.SetSpeedAnim(UnityEngine.Random.Range(speedAnim.x, speedAnim.y));
            }
        }
    }

    float deltaSumQuater = 0f;
    private void OnSpinWork(SpinEventInfo info)
    {
        if (G.GamerManager.SpinGamers["Player"].IsBroke)
            return;

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

        playerDeltaObesrv += delta;
        playerDeltaObesrv = Mathf.Clamp(playerDeltaObesrv, -1000f, 1000f);

        if (GameMode == SpinGameMode.Fog && Mathf.Abs(delta) >= 8f)
        {
            Shtraf(0.18f);
            return;
        }

        deltaSumQuater += delta;

        while (Mathf.Abs(deltaSumQuater) >= 90f)
        {
            AddQuatarForEvent(deltaSumQuater, Mathf.Sign(deltaSumQuater) == -1);
            deltaSumQuater -= deltaSumQuater > 0 ? 90f : -90f;
        }
    }

    private float prevShtraf = 0f;
    private float prevGameModeTime = 0f;
    private void Shtraf(float coef)
    {
        if(Time.time - prevShtraf < 1f || Time.time - prevGameModeTime < 0.8f)
            return;

        if (ignorePenalty > 0)
        {
            ignorePenalty--;
            G.GamerManager.PlayerProgress(12);
            G.GamerManager.SpinGamers["Player"].View.Shtraf(Color.white);
            return;
        }

        if (Time.time - prevGameModeTime > 1.6f)
        {
            G.GamerManager.PlayerProgress(-(int)(50 * coef));
            G.GamerManager.SpinGamers["Player"].View.Shtraf();
        }
        G.ItemExecuter.InvokeEvent(BehActionType.Penalty);

        prevShtraf = Time.time;
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
        if (!isGamePlay)
            return;

        bool equalGameMode = true;
        switch (GameMode) { 
            case SpinGameMode.Clock:
                equalGameMode = isClock;
                break;
            case SpinGameMode.Unclock:
                equalGameMode = !isClock;
                break;
            case SpinGameMode.Fog:
                break;
            //case SpinGameMode.DevicePlayerBreak:
                //return;
            default:
                break;
        }

        switch (r4)
        {
            case SpinRotateEventType.R4:
                if (GameMode == SpinGameMode.Fog && Time.time - prevShtraf > 1.5f)
                {
                    G.GamerManager.PlayerProgress((int)(12 * fogMultyply) + (int)(multyplySup > 1 ? MathF.Pow(2, multyplySup) : 0));
                }
                break;
            case SpinRotateEventType.R2:
                break;
            case SpinRotateEventType.R1:
                if (equalGameMode)
                    G.GamerManager.PlayerProgress((int)(1 + (multyplySup > 1 ? MathF.Pow(1.15f, multyplySup) : 0f)));
                else
                    Shtraf(0.3f);
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
                choices.Add(new ChoicesInChoiceContent(i, G.ItemExecuter.GetPureInfo(G.ItemExecuter.FortuneItems[0]).Name,
                     Color.red));
            } else if (fortuneOption2.Contains(i))
            {
                choices.Add(new ChoicesInChoiceContent(i, G.ItemExecuter.GetPureInfo(G.ItemExecuter.FortuneItems[1]).Name,
                     Color.yellow));
            }
            else if (fortuneOption3.Contains(i))
            {
                choices.Add(new ChoicesInChoiceContent(i, G.ItemExecuter.GetPureInfo(G.ItemExecuter.FortuneItems[2]).Name,
                     Color.blue));
            }
            else if (fortuneOption4.Contains(i))
            {
                choices.Add(new ChoicesInChoiceContent(i, G.ItemExecuter.GetPureInfo(G.ItemExecuter.FortuneItems[4]).Name,
                     Color.purple));
            }else
            {
                choices.Add(new ChoicesInChoiceContent(i, "",
                     Color.gray));
            }
        }

        G.FortuneContent.SetChoices(choices.ToArray());
    }

    float lastTimeSpeach = 0f;
    public int fixBonus = 45;

    private void OnGamerDeadWork(SpinGamerManager.SpinGamer gamer)
    {
        DictorSpeachManager.SetVariable("Last_Kill_Name", gamer.Name);
        if (gamer.ID == "Player")
        {
            PlayerDead();
            return;
        }

        if (UnityEngine.Random.Range(0, 100f) > 80f && Time.time - lastTimeSpeach  > 2f)
            StartCoroutine(SpeakIDRoutine("Someone_Killed"));

        G.ItemExecuter.InvokeEvent(BehActionType.GamerDead, gamer);
    }

    private void PlayerDead()
    {
        isDead = true;
        StartCoroutine(SpeakIDRoutine("Someone_Killed"));
    }

    private void LeadersChangedWork(string name)
    {
        DictorSpeachManager.SetVariable("Leader_Name", name);
        if (Time.time - lastTimeSpeach > 12f)
            StartCoroutine(SpeakIDRoutine("New_Leader"));
    }

    private void GamerBrokeWork(SpinGamerManager.SpinGamer gamer, bool isBroke)
    {
        if (!isGamePlay)
            return;

        DictorSpeachManager.SetVariable("Broke_Player", gamer.Name);
        DictorSpeachManager.SetVariable("Fix_Player", gamer.Name);

        if (gamer.ID == "Player")
        {
            if (isBroke)
            {
                StartCoroutine(SpeakIDRoutine("Device_Broken"));
                G.ItemExecuter.InvokeEvent(BehActionType.PlayerBroke);
            }
            else
            {
                StartCoroutine(SpeakIDRoutine("Device_Fixed"));
                G.GamerManager.PlayerProgress(fixBonus);
            }
            return;
        }

        //StartCoroutine(FixDevice(gamer));
        if (UnityEngine.Random.Range(0, 100) > 75 || Time.time - lastTimeSpeach > 12f)
        {
            if (isBroke)
            {
                StartCoroutine(SpeakIDRoutine("Device_Broken"));
            }
            else
                StartCoroutine(SpeakIDRoutine("Device_Fixed"));
        }
    }

    float fogMultyply = 1f;
    [HideInInspector] public int countNoPenalty;
    public int delayFortune;
    public int quizCorrectBonus;
    private int multyplySup = 1;
    [HideInInspector] public int ignorePenalty;

    public void SetFogMultyply(float coef = 1f)
    {
        fogMultyply = Mathf.Clamp(coef, 1f, 4f);
    }

    public void BreakMultyplay()
    {
        StopCoroutine("AddSuperMultyplyRoutine");
        multyplySup = 1;
        G.GamerManager.SpinGamers["Player"].View.TryUpdateMulty(multyplySup);
    }

    public void AddSuperMultyply()
    {
        StartCoroutine(AddSuperMultyplyRoutine());
    }

    private IEnumerator AddSuperMultyplyRoutine()
    {
        multyplySup++;
        G.GamerManager.SpinGamers["Player"].View.TryUpdateMulty(multyplySup);
        yield return new WaitForSeconds(40f / (multyplySup + 1));
        //multyplySup--;
        G.GamerManager.SpinGamers["Player"].View.TryUpdateMulty(multyplySup);
    }

    private static bool isSkipStart = false;
    public void Reload(bool isSkipStart)
    {
        StopCoroutine("QuizWaitAnserRoutine");
        StopCoroutine("QuizRoutine");
        StopCoroutine("GamerUpdateRoutine");
        StopCoroutine("RandomEventsRoutine");
        StopCoroutine("FortuneWhellRoutine");

        SpinGameFlow.isSkipStart = isSkipStart;
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyUp(KeyCode.R))
            SetGameMode(SpinGameMode.Clock, false);

        if (Input.GetKeyUp(KeyCode.T))
            SetGameMode(SpinGameMode.Fog, false);

        if (Input.GetKeyUp(KeyCode.C))
            FortuneWhellImmidiatly();

        if (Input.GetKeyUp(KeyCode.Q))
            G.GamerManager.PlayerProgress(1000);

        if (Input.GetKeyUp(KeyCode.Space))
            IsSkipGamePlay = !IsSkipGamePlay;
#endif

        if (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Tab))
        {
            G.MenuManager.SwitchPause();
        }

    }
}

public enum SpinRotateEventType { R4 , R2 , R1 };
