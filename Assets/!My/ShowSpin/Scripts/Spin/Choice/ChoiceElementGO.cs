using System;
using TMPro;
using UnityEngine;

public class ChoiceElementGO : MonoBehaviour
{
    [SerializeField] private TMP_Text tmp;
    [SerializeField] private float maxSize = 50;
    [SerializeField] private SpriteRenderer sprRenSelect;
    [SerializeField] private Transform scaleRoot;
    private Color color;

    public void SetChoice(ChoicesInChoiceContent choice)
    {
        SetChoice(choice.text, choice.color);
    }

    public void SetChoice(string text, Color color, float angle = 180)
    {
        SetAngle(angle);
        tmp.text = text;

        this.color = color;
        SetSelectState(false);
    }

    public void SetAngle (float angle)
    {
        scaleRoot.localScale = new Vector3(Mathf.Pow(angle / 180, 2f) * maxSize * 0.5f, scaleRoot.localScale.y, scaleRoot.localScale.z);
    }

    public void SetSelectState(bool isChoice)
    {
        sprRenSelect.color = isChoice ? color : new Color(color.r * 0.35f, color.g * 0.35f, color.b * 0.35f, 1f);
    }
}
