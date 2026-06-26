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
    public List<string> Tags => tags;

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

    private IEnumerator UpdatePos()
    {
        while (true)
        {
            if (!isMove)
            {
                yield return null;
                continue;
            }

            Vector3 offsetVector = new Vector3(Input.GetAxis("Mouse X") * (MenuManager.Snsitivity + 0.1f), 0, 
                Input.GetAxis("Mouse Y") * (MenuManager.Snsitivity + 0.1f))
                * speedOffset * Time.fixedDeltaTime;

            if (coefRadius <= 1f)
                pointTarget = Vector3.Lerp(spinMain.transform.position, pointTarget, coefRadius);
            else
                pointTarget = spinMain.transform.position + ((pointTarget - spinMain.transform.position) * coefRadius);
            pointTarget += referenceTr.TransformVector(offsetVector);

            pointTarget = (Vector3.ProjectOnPlane(pointTarget - spinMain.transform.position, referenceTr.up).normalized * distHandle) +
                spinMain.transform.position;

            spinMain.SetRotation(pointTarget, directionHandle);

            yield return new WaitForSeconds(Time.fixedDeltaTime / 2f);
        }
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