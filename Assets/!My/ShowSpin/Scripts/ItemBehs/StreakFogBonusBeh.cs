using UnityEngine.Rendering.PostProcessing;

public class StreakFogBonusBeh : SpinItemBehaviour
{
    public float coef = 0.15f;

    public override void ExecuteItem(SpinItemBehContext context)
    {
        base.ExecuteItem(context);

        if (context.ActionType == BehActionType.Penalty && G.SpinGameFlow.GameMode == SpinGameFlow.SpinGameMode.Fog)
        {
            G.SpinGameFlow.countNoPenalty = 0;
            G.SpinGameFlow.SetFogMultyply(1f);
        }
        else if (context.ActionType == BehActionType.FogRotate)
        {
            if(G.SpinGameFlow.countNoPenalty >= 2)
            {
                G.SpinGameFlow.SetFogMultyply(1f + (G.SpinGameFlow.countNoPenalty * coef));
            }
            G.SpinGameFlow.countNoPenalty++;
        }
    }
}