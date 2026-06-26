public class DeathRangeMultiplierBeh : SpinItemBehaviour
{
    public int distance = 5;
    public override string GetTextValue => distance.ToString();

    public override void ExecuteItem(SpinItemBehContext context)
    {
        if (context.ActionType != BehActionType.GamerDead)
            return;


        SpinGamerManager.SpinGamer gamer = (SpinGamerManager.SpinGamer)context.items[0];
        var leaders = G.GamerManager.GetLeaders(40);

        int index = leaders.Length;
        int palyerIndex = -1;
        for (int i = 0; i < leaders.Length; i++)
        {
            SpinGamerManager.SpinGamer lead = leaders[i];
            if (lead.ID == "Player")
                palyerIndex = i;
        }

        if (index - palyerIndex <= distance)
        {
            G.SpinGameFlow.AddSuperMultyply();
        }

    }
}