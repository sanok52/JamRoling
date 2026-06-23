using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpinGamerView : MonoBehaviour 
{
    public string ID => gameObject.name;
    public TMP_Text textCount;

    public void TryUpdateProgress(string id, int value)
    {
        if (id != ID)
            return;

        if(textCount != null)
            textCount.text = value.ToString();
    }
}
