public class ABonusPoints: SpinItemBehaviour
{
    public int bonus = 50;
    public override string GetTextValue => bonus.ToString();

    public override void ExecuteItem(SpinItemBehContext context)
    {
        if (context.ActionType != BehActionType.Get)
            return;

        G.GamerManager.PlayerProgress(bonus);
    }
}