public class FortuneFogDoubleBeh : SpinItemBehaviour
{
    public int procent = 60;
    public override string GetTextValue => procent.ToString();

    public override void ExecuteItem(SpinItemBehContext context)
    {
        if (context.ActionType != BehActionType.FortuneWhellInvoke)
            return;

        if (G.SpinGameFlow.GameMode != SpinGameFlow.SpinGameMode.Fog)
            return;

        if (PocketRandomazer.GetRandomElement<int>("Random") >= procent)
            return;

        G.SpinGameFlow.delayFortune++;
    }
}