using System;
using System.Collections.Generic;
using UnityEngine;

public class SpinObserver : MonoBehaviour, ITaggable
{
    [SerializeField] private SpinMain spinMain;
    private List<SpinPointsEvent> spinPointsEvents = new List<SpinPointsEvent>();

    public event Action<SpinEventInfo> OnSpin;

    private float anglePrev;

    [SerializeField] private List<string> tags;
    public List<string> Tags => tags;

    public List<SpinPointsEvent> SpinPointsEvents => spinPointsEvents;
    public SpinMain SpinMain => spinMain;

    void Start()
    {
        spinMain.onSpin += OnSpinWork;
    }

    private void OnSpinWork(float anlge)
    {
        //Debug.Log($"{anlge}");
        List<SpinPointsEvent> spinPointsDes = new List<SpinPointsEvent>();
        foreach (var spinPoints in spinPointsEvents)
        {
            spinPoints.SpinAngle(anlge);
            if(spinPoints.currentAngleAt == -1)
                spinPointsDes.Add(spinPoints);
        }

        spinPointsDes.ForEach(x => spinPointsEvents.Remove(x));

        OnSpin?.Invoke(new SpinEventInfo()
        {
            angle = anlge,
            ID = "OnSpin",
            delta = GetAngleDelta(anlge, anglePrev) * Mathf.Sign(anglePrev - anlge)
        });

        anglePrev = anlge;
    }

    public void AddPointEvent (SpinPointsEvent ev)
    {
        spinPointsEvents.Add(ev);
    }

    [Serializable]
    public class SpinPointsEvent
    {
        public string ID;
        public float[] angles;
        public bool clock;
        public bool destroyOnEnd;
        public int currentAngleAt;
        public bool beakble = true;

        public event Action<SpinEventInfo> OnStep;
        public event Action<SpinEventInfo> OnEnd;

        public SpinPointsEvent (float[] angles, bool clock, bool destroyOnEnd, bool beakble = true)
        {
            this.angles = angles;
            this.destroyOnEnd = destroyOnEnd;
            this.clock = clock;
            currentAngleAt = 0;

            this.beakble = beakble;
        }

        public void SpinAngle(float anlge)
        {
            Debug.Log($"anlge {anlge}");

            float currentAngle = angles[currentAngleAt];

            //Debug.Log($"{currentAngle} {anlge} => {SpinObserver.ClockmoveForAngles(currentAngle, anlge)}");
            if (beakble && currentAngleAt >= 1)
            {
                float prevAngle = angles[currentAngleAt - 1];
                if (SpinObserver.ClockmoveForAngles(prevAngle, anlge) == !clock)
                {
                    Debug.Log($"{anlge} {prevAngle} {SpinObserver.ClockmoveForAngles(prevAngle, anlge)} {clock}");
                    currentAngleAt = 0;
                    OnStep?.Invoke(GetSpinInfo(anlge));
                    return;
                }
            }

            if(SpinObserver.ClockmoveForAngles(currentAngle, anlge) == clock)
            {
                currentAngleAt++;
                if(currentAngleAt >= angles.Length)
                {
                    currentAngleAt = 0;
                    if (destroyOnEnd)
                        currentAngleAt = -1;
                    OnEnd?.Invoke(GetSpinInfo(anlge, true));
                }
                else
                {
                    OnStep?.Invoke(GetSpinInfo(anlge));
                }
            }
        }

        private SpinEventInfo GetSpinInfo(float angle, bool isEnd = false)
        {
            return new SpinEventInfo()
            {
                angle = angle,
                angleState = currentAngleAt,
                ID = ID,
                isEnd = isEnd,
                delta = currentAngleAt != -1 ? 
                    GetAngleDelta(angles[currentAngleAt], angle) : 
                    GetAngleDelta(angles[angles.Length - 1], angle)
            };
        }
    }

    private static bool ClockmoveForAngles(float angleA, float angleB)
    {
        float clockwiseDist = angleA - angleB;
        if (clockwiseDist < 0) clockwiseDist += 360f;
        return clockwiseDist < 180f;
    }

    private static int GetQuat(float angle)
    {
        if (angle <= 90)
            return 0;
        else if (angle <= 180)
            return 1;
        else if(angle <= 270)
            return 2;
        else
            return 3;
    }

    public static float GetAngleDelta(float angleA, float angleB)
    {
        float diff = Mathf.Abs(angleA - angleB);
        if (diff > 180f) diff = 360f - diff;
        return diff;
    }

    public struct SpinEventInfo
    {
        public string ID;
        public float angle;
        public int angleState;
        public bool isEnd;
        public float delta;
    }
}
