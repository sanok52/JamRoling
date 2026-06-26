public class QuizCorrectBonusBeh : SpinItemBehaviour
{
    public int bonus = 30;
    public override string GetTextValue => bonus.ToString();

    public override void ExecuteItem(SpinItemBehContext context)
    {
        if (context.ActionType != BehActionType.Get)
            return;

        G.SpinGameFlow.quizCorrectBonus += bonus;
    }

}