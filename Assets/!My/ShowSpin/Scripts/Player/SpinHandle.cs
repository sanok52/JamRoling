using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SpinHandle : SpinInterMoveble, ITaggable
{
    [SerializeField] private SpinMain spinMain;
    [SerializeField] private Transform referenceTr;
    [SerializeField] private float speedOffset = 100f;
    [SerializeField] private float coefRadius = 1f;

    private Vector3 pointTarget;
    private Vector3 directionHandle;
    private float distHandle;

    [Space]
    [SerializeField] private List<string> tags = new List<string>();

    private Vector2 spinInput; // хранит текущую дельту мыши дл€ спина

    public List<string> Tags => tags;
    public float CoefRadius { get => coefRadius; set => coefRadius = value; }

    private void Start()
    {
        Vector3 worldDir = (transform.position - spinMain.transform.position).normalized;
        Vector3 projWorld = Vector3.ProjectOnPlane(worldDir, referenceTr.up);
        directionHandle = spinMain.transform.InverseTransformDirection(projWorld).normalized;

        distHandle = Vector3.Distance(transform.position, spinMain.transform.position);
        pointTarget = transform.position;

    }
    private void OnEnable()
    {
        StartCoroutine(UpdatePos());
    }

    float webCradCoef = 10f;
    private IEnumerator UpdatePos()
    {
        while (true)
        {
            if (!isMove)
            {
                // если не двигаемс€ Ч ждЄм следующий FixedUpdate, чтобы не блокировать цикл
                yield return new WaitForFixedUpdate();
                continue;
            }

            // используем заранее считанные значени€ (обновл€ютс€ в Update)
            Vector3 offsetVector = new Vector3(spinInput.x, 0f, spinInput.y)
                * speedOffset * Time.fixedDeltaTime / 2f;

            float crad = coefRadius / 1.2f;

#if UNITY_WEBGL
            crad *= webCradCoef;
#endif

            float sens = SettingsManager.SensitivitySpin;

            if (sens < 0.5f)
            {
                float value = Mathf.Lerp(1f, 2.5f, Mathf.Exp(Mathf.Abs(0.5f - sens) / 2.7f));
                crad *= value;
            }
            else
            {
                float value = Mathf.Lerp(1f, 3f, Mathf.Exp(Mathf.Abs(0.5f - sens) / 2.7f));
                crad /= value;
            }

            if (crad <= 1f)
                pointTarget = Vector3.Lerp(spinMain.transform.position, pointTarget, crad);
            else
                pointTarget = spinMain.transform.position + ((pointTarget - spinMain.transform.position) * crad);

            pointTarget += referenceTr.TransformVector(offsetVector);

            pointTarget = (Vector3.ProjectOnPlane(pointTarget - spinMain.transform.position, referenceTr.up).normalized * distHandle) +
                          spinMain.transform.position;

            spinMain.SetRotation(pointTarget, directionHandle);

            // ждЄм следующий FixedUpdate
            yield return new WaitForFixedUpdate();
        }
    }

    // ќбновл€ем ввод в Update, чтобы не тер€ть дельты между FixedUpdate
    private void Update()
    {
        // SettingsManager.MouseXSpin/MouseYSpin уже = Input.GetAxis(...) * SensitivitySpin * coefSpin
        spinInput.x = SettingsManager.MouseXSpin;
        spinInput.y = SettingsManager.MouseYSpin;

        /*float acc = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(acc) > 0.05f)
        {
            webCradCoef += Mathf.Clamp(acc, 0.2f, 1f);
            G.ReciveItemText.ShowItemInfo(new SpinItemInfo() { Description = $"webCradCoef {webCradCoef}" });
        }

        if(Input.GetKeyUp(KeyCode.Q))
            G.ReciveItemText.ShowItemInfo(new SpinItemInfo() { Description = $"webCradCoef {webCradCoef}" });*/
    }

    public override void InterHoldEnter()
    {
        base.InterHoldEnter();
        pointTarget = transform.position;
    }

    private void OnDrawGizmos()
    {
        if(isMove)
            Gizmos.DrawWireSphere(pointTarget, 0.05f);
    }

}