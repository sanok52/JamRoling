using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpinInterManager : MonoBehaviour
{
    [SerializeField] private LayerMask layersInteraction;

    [Space]
    [SerializeField] private Image imagePoint;
    [SerializeField] private Sprite spriteNone;
    [SerializeField] private Sprite spriteCanGrap;
    [SerializeField] private Sprite spriteGrap;

    private CameraControl control;
    private new Camera camera;

    private ISpinInterHold currentHold;
    private Vector3 offset;

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
            if (currentHold != null)
            {
                UpdatePointUIGrap();
                return;
            }

            if (hitInfo.transform.TryGetComponent(out ISpinInterUse spinInterUse) && (spinInterUse as MonoBehaviour).enabled)
            {
                if (Input.GetKey(KeyCode.Mouse0))
                    spinInterUse.InterUse();
            }else if (hitInfo.transform.TryGetComponent(out ISpinInterHold spinInterHold) && (spinInterHold as MonoBehaviour).enabled)
            {
                imagePoint.sprite = spriteCanGrap;
                imagePoint.SetNativeSize();
                imagePoint.color = Color.white;
                BreakPointToCenter();

                if (Input.GetKey(KeyCode.Mouse0))
                {
                    Hold(spinInterHold);
                    offset = hitInfo.point - hitInfo.transform.position;
                }
            }
            else
                BreakPointUI();
        }
        else
            BreakPointUI();        
    }

    private void BreakPointUI()
    {
        imagePoint.sprite = spriteNone;
        imagePoint.SetNativeSize();
        imagePoint.color = Color.yellow;
        BreakPointToCenter();
    }


    private void UpdatePointUIGrap()
    {
        imagePoint.sprite = spriteGrap;
        imagePoint.SetNativeSize();
        imagePoint.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        Transform target = (currentHold as MonoBehaviour).transform;
        Vector2 pointOnScreen = Camera.main.WorldToScreenPoint(target.position + offset);

        Canvas canvas = imagePoint.transform.parent.GetComponent<Canvas>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, pointOnScreen, cam, out Vector2 localPoint))
        {
            imagePoint.transform.localPosition = localPoint;
        }
    }
    private void BreakPointToCenter()
    {
        imagePoint.transform.localPosition = Vector2.zero;
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
        BreakPointUI();
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