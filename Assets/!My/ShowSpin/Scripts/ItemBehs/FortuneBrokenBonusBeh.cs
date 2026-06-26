using UnityEngine;

public class FortuneBrokenBonusBeh : SpinItemBehaviour
{
    public int coins = 80;
    public override string GetTextValue => coins.ToString();

    public override void ExecuteItem(SpinItemBehContext context)
    {
        base.ExecuteItem(context);

        Debug.Log($"{context.ActionType} {G.GamerManager.SpinGamers["Player"].IsBroke}");

        if (context.ActionType != BehActionType.FortuneWhellMiss)
            return;

        if (!G.GamerManager.SpinGamers["Player"].IsBroke)
            return;

        G.GamerManager.PlayerProgress(coins);
    }
}