using System.Collections.Generic;
using UnityEngine;
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
        G.ItemExecuter.Init();

        PocketRandomazer.CreatePocket<int>("Random", 15, 21, 31, 45, 55, 69, 81, 99);

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
    public static GameModeUI GameModeUI;

    public static SpinObserver spinGamePlay;

    public static FortuneSpinPresenter FortuneWhell;
    public static FortuneSpinPresenter VictorinChoiceWhell;

    public static ChoiceContent FortuneContent;
    public static ChoiceContent VictorinChoiceContent;
    public static ChoiceContent VictorineAnserContent;
    public static TextTyper VictorinTextTyper;

    public static SpinUpObjectAnimation VictorineAnserAnimation;
    public static ScreenRemont ScreenRemont;
    public static ScreenVictorin ScreenVictorin;
    public static SpinItemExecuter ItemExecuter;
    public static List<SpinHandle> handlesFixes;

    public static void Init()
    {
        Find();
        CreateGO();
        Create();
        FindAll();
    }

    private static void Find()
    {
        SpinInterManager = Object.FindFirstObjectByType<SpinInterManager>(FindObjectsInactive.Include);
        DictorAnimation = Object.FindFirstObjectByType<DictorAnimation>(FindObjectsInactive.Include);
        LeaderBoardUI = Object.FindFirstObjectByType<SpinLeaderBoardUI>(FindObjectsInactive.Include);
        GameModeUI = Object.FindFirstObjectByType<GameModeUI>(FindObjectsInactive.Include);
        ScreenRemont = Object.FindFirstObjectByType<ScreenRemont>(FindObjectsInactive.Include);
        ScreenVictorin = Object.FindFirstObjectByType<ScreenVictorin>(FindObjectsInactive.Include);
    }

    private static void Create()
    {        GamerManager = new SpinGamerManager();
        MusicManager = new MusicManager();
    }

    private static void CreateGO()
    {
        SpinGameFlow = new GameObject("SpinGameFlow").AddComponent<SpinGameFlow>();
        ItemExecuter = new GameObject("SpinItemExecuter").AddComponent<SpinItemExecuter>();
    }

    private static void FindAll()
    {
        SpinObserver[] spinObservs = Object.FindObjectsByType<SpinObserver>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
        foreach (var spin in spinObservs)
        {
            if (spin.Tags.Contains("GamePlay"))
                spinGamePlay = spin;
        }

        SpinHandle[] handles = Object.FindObjectsByType<SpinHandle>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
        handlesFixes = new List<SpinHandle>();
        foreach (var handle in handles)
        {
            if (handle.Tags.Contains("FixHandle"))
                handlesFixes.Add(handle);
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

        TextTyper[] textTypers = Object.FindObjectsByType<TextTyper>(FindObjectsInactive.Include,
            FindObjectsSortMode.InstanceID);
        foreach (var textTyper in textTypers)
        {
            if (textTyper.gameObject.name == "TextTyperDevil")
                DictorTextTyper = textTyper;
            else if (textTyper.gameObject.name == "TextTyperV")
                VictorinTextTyper = textTyper;
        }
    }
}