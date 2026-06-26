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

        int start = Mathf.Clamp(playerIndex + distance, 0, leaders.Length);
        Debug.Log($"{playerIndex} => {playerIndex + distance}");
        for (int i = 0; i < distance && leaders.Length > start - i; i++)
        {
            G.GamerManager.Broke(leaders[start - i].ID, true);
        }
    }
}