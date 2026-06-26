public class QuizWrongBreakAllBeh : SpinItemBehaviour
{
    public override void ExecuteItem(SpinItemBehContext context)
    {
        if (context.ActionType != BehActionType.QuizWrong)
            return;

        int hard = (int)context.items[1];
        if (hard >= 2)
            return;

        foreach (var gamer in G.GamerManager.SpinGamers.Values)
        {
            G.GamerManager.Broke(gamer.ID, true);
        } 
    }
}