using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScreenRemont : MonoBehaviour
{
    [SerializeField] private Transform[] arrows;
    [SerializeField] private Gradient gradient = new Gradient();
    [SerializeField] private Color colorGood = Color.yellow;

    [Space]
    [SerializeField] private SpinObserver[] spinObservers;
    private List<float[]> coefs = new List<float[]>();
    private List<Vector3> fwds = new List<Vector3>();

    public event Action OnRemont;
    private bool needRemont = false;

    private int step = 0;
    private int maxStep = 3;
    private int indexArrow = 0;

    private void Start()
    {
        for (int i = 0; i < spinObservers.Length; i++)
        {
            int n = i;
            spinObservers[i].OnSpin += (info) => SpinWork(info, n);
        }

        Hide();
    }

    public void StartRemont()
    {
        needRemont = true;
        coefs.Clear();
        fwds.Clear();

        for (int i = 0; i < arrows.Length; i++)
        {
            arrows[i].gameObject.SetActive(true);

            arrows[i].transform.localRotation = Quaternion.Euler(0f, Random.Range(0, 360), 0);
            fwds.Add(arrows[i].transform.forward);

            coefs.Add(new float[arrows.Length]);
        }

        step = -1;
        StepNext();
    }

    private void StepNext()
    {
        if (!needRemont)
            return;

        goodAngleTime = 0f;
        step++;
        if(step >= maxStep)
        {
            step = -1;
            Hide();

            OnRemont?.Invoke();
            //maxStep += 1;
            return;
        }

        int prev = indexArrow;
        while (prev == indexArrow)
        {
            indexArrow = Random.Range(0, arrows.Length);
        }

        for (int i = 0; i < arrows.Length; i++)
        {
            if(i == prev)
            {
                arrows[i].DOPunchScale(Vector3.one * 0.4f, 0.4f).OnComplete(() => arrows[prev].gameObject.SetActive(false));
            }else
            arrows[i].gameObject.SetActive(i == indexArrow);
        }

        fwds[indexArrow] = Vector3.ProjectOnPlane(Random.onUnitSphere, arrows[indexArrow].up).normalized;
        arrows[indexArrow].transform.rotation = Quaternion.LookRotation(fwds[indexArrow], arrows[indexArrow].up);
        AddRotate(indexArrow, Random.Range(60f, 300f));

        ColorazeAngles();
    }

    private void Hide()
    {
        needRemont = false;
        for (int i = 0; i < arrows.Length; i++)
        {
            arrows[i].gameObject.SetActive(false);
        }
    }

    private void AddRotate(int index, float angle)
    {
        if (!needRemont)
            return;

        arrows[index].Rotate(new Vector3(0f, angle, 0));

        /*for (int i = 0; i < arrows.Length; i++)
        {
            if (i == index)
                continue;

            float coef = coefs[index][i];
            arrows[i].Rotate(new Vector3(0f, angle * coef, 0));
        }*/
    }

    private void ColorazeAngles()
    {
        for (int i = 0; i < arrows.Length; i++)
        {
            float angle = Vector3.Angle(arrows[i].forward, fwds[i]);

            if (Mathf.Abs(angle) > 25)            
                SetBadColor(i, angle);            
            else
                SetGoodColor(i);
        }
    }

    float goodAngleTime = 0;
    private void TestAngles()
    {
        if (!needRemont)
            return;

        /*for (int i = 0; i < arrows.Length; i++)
        {
            float angle = Vector3.Angle(arrows[i].forward, fwds[i]);
            if (angle > 10)
                return;
        }*/

        //OnRemont?.Invoke();

        float angle = Vector3.Angle(arrows[indexArrow].forward, fwds[indexArrow]);
        if (angle < 25)
        {
            goodAngleTime += Time.deltaTime;
            if (goodAngleTime >= 0.5f)
                StepNext();
        }
        else
            goodAngleTime = 0f;
    }

    private void SetGoodColor(int i)
    {
        arrows[i].GetChild(0).GetComponent<SpriteRenderer>().color = colorGood;
        arrows[i].GetChild(1).GetComponent<SpriteRenderer>().color = colorGood;
    }

    private void SetBadColor(int i, float angle)
    {
        Color color = gradient.Evaluate(1f - (angle / 170f));

        arrows[i].GetChild(0).GetComponent<SpriteRenderer>().color = color;
        arrows[i].GetChild(1).GetComponent<SpriteRenderer>().color = color;
    }

    private void SpinWork(SpinObserver.SpinEventInfo info, int i)
    {
        if (!needRemont)
            return;

        AddRotate(i, info.delta * 0.5f);
        ColorazeAngles();
    }

    private void Update()
    {
        if (!needRemont) 
            return;

        TestAngles();
    }
}
