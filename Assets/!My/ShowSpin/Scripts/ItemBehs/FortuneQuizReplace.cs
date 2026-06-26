public class FortuneQuizReplace : SpinItemBehaviour
{
    public override void ExecuteItem(SpinItemBehContext context)
    {
        if(context.ActionType == BehActionType.Get)
            G.SpinGameFlow.replaceRotateFortune = true;
    }
}