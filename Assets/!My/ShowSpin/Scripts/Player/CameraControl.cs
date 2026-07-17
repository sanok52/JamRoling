using DG.Tweening;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Vector2 lookSpeed = new Vector2(10f, 10f); // масштаб для X и Y
    [SerializeField] private Vector2 rotateClampX = new Vector2(-90f, 90f); // pitch
    [SerializeField] private Vector2 rotateClampY = new Vector2(-90f, 90f); // yaw

    private float pitch; // x
    private float yaw;   // y

    void Start()
    {
        Vector3 e = transform.rotation.eulerAngles;
        pitch = NormalizeAngle(e.x);
        yaw = NormalizeAngle(e.y);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        DOVirtual.DelayedCall(Time.deltaTime, () =>
        {
            pitch = (rotateClampX.x + rotateClampX.y) * 0.5f;
            yaw = (rotateClampY.x + rotateClampY.y) * 0.5f;
            transform.rotation = Quaternion.Euler(pitch, yaw, transform.rotation.eulerAngles.z);
        });
    }

    void Update()
    {
        if (G.MenuManager.IsMenu || G.MenuManager.IsPause)
            return;

        // Берём уже подготовленные значения из SettingsManager
        float mouseX = SettingsManager.MouseXLook; // = Input.GetAxis("Mouse X") * SensitivityLook * coefLook
        float mouseY = SettingsManager.MouseYLook; // = Input.GetAxis("Mouse Y") * SensitivityLook * coefLook

        // НЕ умножаем на Time.deltaTime — это дельта за кадр
        yaw += mouseX * lookSpeed.y;
        pitch += -mouseY * lookSpeed.x; // минус для стандартного поведения (поднять мышь — смотреть вверх)

        // Ограничения
        pitch = Mathf.Clamp(pitch, rotateClampX.x, rotateClampX.y);
        yaw = NormalizeAngle(yaw);
        yaw = Mathf.Clamp(yaw, rotateClampY.x, rotateClampY.y);

        transform.rotation = Quaternion.Euler(pitch, yaw, transform.rotation.eulerAngles.z);
    }

    private float NormalizeAngle(float angle)
    {
        return Mathf.Repeat(angle + 180f, 360f) - 180f;
    }
}