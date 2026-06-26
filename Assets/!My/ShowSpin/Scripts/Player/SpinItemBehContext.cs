public class SpinItemBehContext
{
    public BehActionType ActionType;
    internal object[] items;
}
public enum BehActionType { Get,
    Penalty,
    PlayerBroke,
    FortuneWhellMiss,
    FortuneWhellEnd,
    FortuneWhellInvoke,
    FortuneWhellOptionEmpty,
    FogRotate,
    QuizRight,
    QuizWrong,
    GamerDead
}