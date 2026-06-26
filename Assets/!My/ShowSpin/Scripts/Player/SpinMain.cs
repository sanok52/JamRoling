using System;
using System.Collections.Generic;
using UnityEngine;

public class SpinMain : MonoBehaviour, ITaggable
{
    [SerializeField] private Transform referenceTr;
    [SerializeField] private float lerpRotate = 100f;
    private Vector3 defaultUp;

    [SerializeField] private List<string> tags;
    public float CurrentAngle { get; private set; }

    public List<string> Tags => tags;

    public event Action<float> onSpin;

    private void Start()
    {
        defaultUp = referenceTr.up;
    }

    public void SetRotation(Vector3 pointTarget, Vector3 directionHandle, bool testEnabled = true)
    {
        if (enabled == false && testEnabled)
            return;

        directionHandle = new Vector3(
            ClampFloatInVector(directionHandle.x),
             ClampFloatInVector(directionHandle.y),
              ClampFloatInVector(directionHandle.z));

        Vector3 direction = pointTarget - transform.position;

        Vector3 worldHandle = transform.TransformDirection(directionHandle);

        Vector3 projHandle = Vector3.ProjectOnPlane(worldHandle, referenceTr.up);
        Vector3 projTarget = Vector3.ProjectOnPlane(direction, referenceTr.up);

        projHandle.Normalize();
        projTarget.Normalize();

        float angle = Vector3.SignedAngle(projHandle, projTarget, referenceTr.up);

        AddRotation(angle);
    }

    public void AddRotation (float angle, bool testEnabled = true)
    {
        if (enabled == false && testEnabled)
            return;

        //Debug.Log($"angle {angle}");
        Quaternion deltaRotation = Quaternion.AngleAxis(angle, referenceTr.up);
        Quaternion targetRotation = deltaRotation * transform.rotation;

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation,
            lerpRotate * Time.deltaTime);

        float angleEv = Vector3.SignedAngle(transform.TransformDirection(new Vector3(0, 0, 1)), referenceTr.forward, referenceTr.up);
        if (angleEv >= 0)
            CurrentAngle = angleEv;
        else
            CurrentAngle = 360 + angleEv;

        onSpin?.Invoke(CurrentAngle);
    }

    private float ClampFloatInVector(float z)
    {
        if (Mathf.Abs(z) < 0.01f)
            return 0f;
        return z;
    }
}