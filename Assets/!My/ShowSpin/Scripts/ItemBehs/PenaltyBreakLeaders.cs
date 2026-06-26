using UnityEngine;

public class PenaltyLeadersBeh : SpinItemBehaviour
{
    public int procent = 60;
    public int leaders = 3;

    public override string GetTextValue => procent.ToString();
    public override string GetTextValue2 => leaders.ToString();

    public override void ExecuteItem(SpinItemBehContext context)
    {
        base.ExecuteItem(context);

        if (context.ActionType != BehActionType.Penalty)
            return;

        if (PocketRandomazer.GetRandomElement<int>("Random") >= procent)
            return;

        SpinGamerManager.SpinGamer[] gamers = G.GamerManager.GetLeaders(leaders);
        foreach (var item in gamers)
        {
            G.GamerManager.Broke(item.ID, true);
        }
    }
}