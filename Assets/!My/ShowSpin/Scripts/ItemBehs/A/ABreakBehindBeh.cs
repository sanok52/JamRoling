using UnityEngine;

public class ABreakBehindBeh : SpinItemBehaviour
{
    public int distance = 10;
    public override string GetTextValue => distance.ToString();

    public override void ExecuteItem(SpinItemBehContext context)
    {
        if (context.ActionType != BehActionType.Get)
            return;

        var leaders = G.GamerManager.GetLeaders(40);

        int index = leaders.Length;
        int playerIndex = -1;
        for (int i = 0; i < leaders.Length; i++)
        {
            SpinGamerManager.SpinGamer lead = leaders[i];
            if (lead.ID == "Player")
            {
                playerIndex = i;
                break;
            }
        }

        for (int i = 1; i <= distance && playerIndex + i < leaders.Length; i++)
        {
            G.GamerManager.Broke(leaders[playerIndex + i].ID, true);
        }
    }
}