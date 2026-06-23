using UnityEngine;

public class SpinInterMoveble : MonoBehaviour, ISpinInterHold
{
    public bool IsBlockMouse => true;
    public bool isMove { get; private set; }

    public virtual void InterHoldEnter()
    {
        isMove = true;
    }

    public virtual void InterHoldExit()
    {
        isMove = false;
    }
}