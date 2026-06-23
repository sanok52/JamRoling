using NUnit.Framework;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using Object = UnityEngine.Object;

public static class SpinEntryPoint
{

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitGame()
    {
        G.Init();

        PocketRandomazer.Clear();
        DictorSpeachManager.LoadFiles();
        G.MusicManager.Init();
        G.GamerManager.Init(42);
        G.GamerManager.OnChangeProgress += G.LeaderBoardUI.ChangeProgeressWork;

        G.SpinGameFlow.Init();
    }
}

public static class G
{
    public static TextTyper DictorTextTyper;
    public static SpinGameFlow SpinGameFlow;
    public static SpinGamerManager GamerManager;
    public static SpinInterManager SpinInterManager;
    public static MusicManager MusicManager;
    public static DictorAnimation DictorAnimation;
    public static SpinLeaderBoardUI LeaderBoardUI;

    public static SpinObserver spinGamePlay;

    public static FortuneSpinPresenter FortuneWhell;
    public static FortuneSpinPresenter VictorinChoiceWhell;

    public static ChoiceContent FortuneContent;
    public static ChoiceContent VictorinChoiceContent;
    public static ChoiceContent VictorineAnserContent;

    public static SpinUpObjectAnimation VictorineAnserAnimation;

    public static void Init()
    {
        DictorTextTyper = Object.FindFirstObjectByType<TextTyper>();
        SpinInterManager = Object.FindFirstObjectByType<SpinInterManager>();
        DictorAnimation = Object.FindFirstObjectByType<DictorAnimation>();
        LeaderBoardUI = Object.FindFirstObjectByType<SpinLeaderBoardUI>();

        SpinGameFlow = new GameObject("SpinGameFlow").AddComponent<SpinGameFlow>();

        GamerManager = new SpinGamerManager();
        MusicManager = new MusicManager();

        SpinObserver[] spinObservs = Object.FindObjectsByType<SpinObserver>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
        foreach (var spin in spinObservs)
        {
            if(spin.Tags.Contains("GamePlay"))
                spinGamePlay = spin;
        }

        FortuneSpinPresenter[] fortunes = Object.FindObjectsByType<FortuneSpinPresenter>(FindObjectsInactive.Include, 
            FindObjectsSortMode.InstanceID);
        foreach (var fortune in fortunes)
        {
            if (fortune.Tags.Contains("Fortune"))
                FortuneWhell = fortune;
            else if (fortune.Tags.Contains("VictorinChoice"))
                VictorinChoiceWhell = fortune;
        }

        ChoiceContent[] choiceContents = Object.FindObjectsByType<ChoiceContent>(FindObjectsInactive.Include,
            FindObjectsSortMode.InstanceID);
        foreach (var content in choiceContents)
        {
            if (content.Tags.Contains("Fortune"))
                FortuneContent = content;
            else if (content.Tags.Contains("VictorinChoice"))
                VictorinChoiceContent = content;
            else if (content.Tags.Contains("VictorineAnser"))
                VictorineAnserContent = content;
        }

        SpinUpObjectAnimation[] spinUpAnimations = Object.FindObjectsByType<SpinUpObjectAnimation>(FindObjectsInactive.Include,
            FindObjectsSortMode.InstanceID);
        foreach (var spinUp in spinUpAnimations)
        {
            if (spinUp.Tags.Contains("VictorineAnser"))
                VictorineAnserAnimation = spinUp;
        }
    }
}