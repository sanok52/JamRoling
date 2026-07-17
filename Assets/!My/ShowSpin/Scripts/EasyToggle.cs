using UnityEngine;
using UnityEngine.UI;

public class EasyToggle : MonoBehaviour
{
    void Start()
    {
        Toggle toggle = GetComponent<Toggle>();
        toggle.isOn = SpinGamerManager.EasyMode;
        toggle.onValueChanged.AddListener((on) => SpinGamerManager.EasyMode = on);
    }
}
