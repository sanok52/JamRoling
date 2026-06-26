public class FortuneOnBreakBeh : SpinItemBehaviour
{
    public int procent = 60;
    public override string GetTextValue => procent.ToString();

    public override void ExecuteItem(SpinItemBehContext context)
    {
        base.ExecuteItem(context);

        if (context.ActionType != BehActionType.PlayerBroke)
            return;

        if (PocketRandomazer.GetRandomElement<int>("Random") >= procent)
            return;

        G.SpinGameFlow.FortuneWhellImmidiatly();
    }
}