public class FortuneEmptyFogBeh : SpinItemBehaviour
{
    public override void ExecuteItem(SpinItemBehContext context)
    {
        base.ExecuteItem(context);

        if (context.ActionType != BehActionType.FortuneWhellOptionEmpty)
            return;

        G.SpinGameFlow.SetGameMode(SpinGameFlow.SpinGameMode.Fog, false);
    }
}