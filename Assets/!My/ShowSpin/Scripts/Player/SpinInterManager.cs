using System;
using System.Collections.Generic;
using UnityEngine;

public class SpinInterManager : MonoBehaviour
{
    [SerializeField] private LayerMask layersInteraction;
    private CameraControl control;
    private new Camera camera;

    private ISpinInterHold currentHold;

    public event Action<ISpinInterHold> OnHold;

    void Start()
    {
        camera = Camera.main;
        control = FindFirstObjectByType<CameraControl>();
    }

    void Update()
    {
        if (currentHold != null && (currentHold as MonoBehaviour).enabled == false)
        {
            UnHoldCurrent();
            return;
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (currentHold != null)
                UnHoldCurrent();
        }

        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, layersInteraction))
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (currentHold != null)
                    return;

                if (hitInfo.transform.TryGetComponent(out ISpinInterUse spinInterUse) && (spinInterUse as MonoBehaviour).enabled)
                    spinInterUse.InterUse();


                if (hitInfo.transform.TryGetComponent(out ISpinInterHold spinInterHold) && (spinInterHold as MonoBehaviour).enabled)
                {
                    Hold(spinInterHold);
                    Debug.Log($"Hold {hitInfo.transform.name}");
                }
            }
        }
    }

    private void Hold(ISpinInterHold spinInterHold)
    {
        currentHold = spinInterHold;
        spinInterHold.InterHoldEnter();
        if (currentHold.IsBlockMouse)
            control.enabled = false;

        OnHold?.Invoke(spinInterHold);
    }
    private void UnHoldCurrent()
    {
        currentHold.InterHoldExit();
        currentHold = null;
        control.enabled = true;
    }
}

public interface ISpinInterUse
{
    public void InterUse();
}

public interface ISpinInterHold
{
    public bool IsBlockMouse { get; }

    public void InterHoldEnter();
    public void InterHoldExit();
}

public interface ITaggable
{
    public List<string> Tags { get; }
}