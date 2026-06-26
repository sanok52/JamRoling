public class FortuneFixBeh : SpinItemBehaviour
{
    public int procent = 60;
    public override string GetTextValue => procent.ToString();

    public override void ExecuteItem(SpinItemBehContext context)
    {
        if (context.ActionType != BehActionType.FortuneWhellEnd)
            return;

        if (PocketRandomazer.GetRandomElement<int>("Random") >= procent)
            return;

        G.GamerManager.Broke("Player", false);
    }
}