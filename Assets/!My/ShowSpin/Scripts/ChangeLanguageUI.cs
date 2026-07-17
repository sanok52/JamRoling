using TMPro;
using UnityEngine;

public class ChangeLanguageUI : MonoBehaviour
{
    private void Start()
    {
         GetComponent<TMP_Dropdown>().onValueChanged.AddListener((n) => ChangeLanguage(n));
    }

    private void ChangeLanguage(int n)
    {
        if (n == 0)
            DictorSpeachManager.Language = Language.EN;
        else
            DictorSpeachManager.Language = Language.RU;
    }
}
