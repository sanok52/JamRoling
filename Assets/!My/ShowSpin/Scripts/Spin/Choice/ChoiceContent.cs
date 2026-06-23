using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChoiceContent : MonoBehaviour, ITaggable
{
    [SerializeField] private ChoiceElementGO elementGOPref;
    [SerializeField] private Transform content;
    [SerializeField] private SpinObserver spinObserver;
    [SerializeField] private float angleTarget = 45f;
    [SerializeField] private float angleCoef = 1f;

    private Dictionary<ChoicesInChoiceContent, ChoiceElementGO> elementGOs = new Dictionary<ChoicesInChoiceContent, ChoiceElementGO>();

    [SerializeField] private List<string> tags;
    public List<string> Tags => tags;

    private void Start()
    {
        /*int[] hard = new int[] { 0, 2 };
        int[] middle = new int[] { 3, 4, 6 };
        int countQuest = 7;
        List<ChoicesInChoiceContent> choices = new List<ChoicesInChoiceContent>();

        bool isHard;
        bool isMiddle;

        for (int i = 0; i < countQuest; i++)
        {
            isHard = hard.Contains(i);
            isMiddle = middle.Contains(i);
            choices.Add(new ChoicesInChoiceContent(i,
                 (isHard ? "Hard" : (isMiddle ? "Medium" : "Easy")),
                 (isHard ? Color.red : (isMiddle ? Color.purple : Color.green))));
        }

        SetChoices(choices.ToArray());*/
        //spinObserver.OnSpin += OnSpinWork;
    }

    private void Update()
    {
        UpdateChoiceTarget();
    }

    public void SetChoices (params ChoicesInChoiceContent[] choicesInChoices)
    {
        ClearChoices();

        foreach (var choice in choicesInChoices)
            CreateChoiceView(choice);

        UpdateChoicesView();
    }

    private void CreateChoiceView(ChoicesInChoiceContent choice)
    {
        var nchoice = Instantiate(elementGOPref, content.transform.position, Quaternion.identity, content);
        elementGOs.Add(choice, nchoice);
        nchoice.SetChoice(choice);
    }

    private void UpdateChoicesView()
    {
        float angle = 360f / elementGOs.Count;
        int n = 0;
        foreach (var item in elementGOs.Values)
        {
            item.SetAngle(angle * angleCoef);
            item.transform.localRotation = Quaternion.Euler(0, 0, angle * n);
            n++;
        }

        UpdateChoiceTarget();
    }

    private void UpdateChoiceTarget()
    {
        var index = GetCurrentChoiceIndex();

        string debug = "";
        foreach (var item in elementGOs)
        {
            item.Value.SetSelectState(index == item.Key.index);
            debug += $"{item.Key.index} == {index} ({index == item.Key.index});";
        }

        //Debug.Log(gameObject.name + ": " + debug);
    }

    public int GetCurrentChoiceIndex()
    {
        float angle = 360f / elementGOs.Count;
        float angleHalf = angle / 2;
        float currentAngle = spinObserver.SpinMain.CurrentAngle;

        int n = 0;
        foreach (var item in elementGOs.Values)
        {
            float angleThis = currentAngle + (angle * n);
            if (angleThis >= 360)
                angleThis -= 360;

            //Debug.Log($"{n} => {angleThis} && {angleTarget} => {SpinObserver.GetAngleDelta(angleThis, angleTarget)} <= {angleHalf}");
            if (SpinObserver.GetAngleDelta(angleThis, angleTarget) <= angleHalf)
                return n;
            n++;
        }

        return 0;
    }

    private void ClearChoices()
    {
        foreach (var choice in elementGOs.Values)
            Destroy(choice.gameObject);
        elementGOs.Clear();
    }
}

[Serializable]
public class ChoicesInChoiceContent
{
    public int index = 0;
    public string text;
    public Color color;

    public ChoicesInChoiceContent(int choice, string text, Color color)
    {
        this.index = choice;
        this.text = text;
        this.color = color;
    }
}
