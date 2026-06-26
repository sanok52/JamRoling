using UnityEngine;

public class SpinItemBehaviour : MonoBehaviour
{
    public virtual string GetTextValue { get; }
    public virtual string GetTextValue2 { get; }

    public virtual void ExecuteItem(SpinItemBehContext context)
    {

    }
}
