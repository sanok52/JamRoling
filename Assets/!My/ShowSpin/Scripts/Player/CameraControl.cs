using DG.Tweening;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private Vector2 lookSpeed = new Vector2(10f, 10f);
    [SerializeField] private Vector2 rotateClampX = new Vector2(-90f, 90f);
    [SerializeField] private Vector2 rotateClampY = new Vector2(-90f, 90f);

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

       DOVirtual.DelayedCall(Time.deltaTime, () =>  
       transform.rotation = Quaternion.Euler((rotateClampX.x + rotateClampX.y) / 2,
            (rotateClampY.x + rotateClampY.y) / 2, 
            transform.rotation.eulerAngles.z));
    }


    void Update()
    {
        float x = transform.rotation.eulerAngles.x;
        float y = transform.rotation.eulerAngles.y;

        x += Input.GetAxis("Mouse Y") * lookSpeed.x * (MenuManager.Snsitivity + 0.1f) * Time.deltaTime;
        y += Input.GetAxis("Mouse X") * lookSpeed.y * (MenuManager.Snsitivity + 0.1f) * Time.deltaTime;

        if(x > 180)
            x = -(360 - x);
        if(y > 180)
            y = -(360 - y);

        if (x < 0f) 
            x = Mathf.Clamp(360 + x, 360 + rotateClampX.x, 360);
        else
            x = Mathf.Clamp(x, 0, rotateClampX.y);

        if (y < 0f)
            y = Mathf.Clamp(360 + y, 360 + rotateClampY.x, 360);
        else
            y = Mathf.Clamp(y, 0, rotateClampY.y);

        transform.rotation = Quaternion.Euler(x, y, transform.rotation.eulerAngles.z);
    }
}
