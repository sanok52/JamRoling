public class AIgnorePenalty: SpinItemBehaviour
{
    public int count = 4;

    public override void ExecuteItem(SpinItemBehContext context)
    {
        if (context.ActionType != BehActionType.Get)
            return;

        G.SpinGameFlow.ignorePenalty += count;
    }
}