using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using DG.Tweening;

public class Control : MonoBehaviour
{
    public TwoBoneIKConstraint tbIk;
    public bool CanMove = false;
    //脚的位置
    public Vector3 footPos;
    Vector3 localOriginal;

    //初始位置
    public Vector3 Original;

    //腿部Ik偏移
    public float LegOffsetY = 2;

    private bool _IsMoving;

    private void Awake()
    {
        if (Physics.Raycast(tbIk.transform.position, -this.transform.up, out hit, 100, ControlALL.Instance.layerMask))
        {
            tbIk.transform.position = hit.point + (this.transform.up * LegOffsetY);
            Original = hit.point + this.transform.up * LegOffsetY;
            footPos = hit.point;
        }

        localOriginal = tbIk.transform.localPosition;
    }

    void ReInitialize()
    {
        Original = tbIk.transform.position;
    }

    Vector3 NextPosition;
    RaycastHit hit;
    // Update is called once per frame
    void Update()
    {
        if (!_IsMoving)
            tbIk.transform.position = Original;

        //Vector3 RayPoint = this.transform.position - this.transform.right * Mathf.Abs(localOriginal.x) + this.transform.up * 10;
        //if (Physics.Raycast(RayPoint, -this.transform.up, out hit, 100, ControlALL.Instance.layerMask))
        //{
        //    NextPosition = hit.point + this.transform.up * LegOffsetY + ControlALL.Instance.Dir.normalized * 1f;
        //    Debug.DrawLine(RayPoint, hit.point);
        //}

        //Debug.DrawLine(NextPosition + new Vector3(0, 10, 0), NextPosition, Color.green);

        //Debug.DrawLine(this.transform.up * LegOffsetY - tbIk.transform.position + new Vector3(0, 10, 0),
        //    this.transform.up * LegOffsetY - tbIk.transform.position, Color.red);

        //if (CanMove)
        //{
        //    if (Mathf.Abs((NextPosition - this.transform.up * LegOffsetY - tbIk.transform.position).x) > ControlALL.Instance.MaxStep.x
        //        || Mathf.Abs((NextPosition - this.transform.up * LegOffsetY - tbIk.transform.position).z) > ControlALL.Instance.MaxStep.z)
        //    {
        //        _IsMoving = true;
        //        tbIk.transform.DOMove(NextPosition, ControlALL.Instance.MoveTime);
        //        tbIk.transform.DOMoveY(NextPosition.y + 1f, ControlALL.Instance.MoveTime / 2).SetEase(Ease.OutBack).OnComplete(() =>
        //        {
        //            tbIk.transform.DOMoveY(NextPosition.y, ControlALL.Instance.MoveTime / 2).OnComplete(() =>
        //            {
        //                _IsMoving = false;
        //                ReInitialize();
        //                CanMove = false;
        //                footPos = hit.point;
        //                ControlALL.Instance.UpdatPostion(this);
        //            });
        //        });
        //    }
        //}

    }

    private void LateUpdate()
    {
        if(!_IsMoving)
            tbIk.transform.position = Original;
    }
}
