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
    public bool FinishMove = false;

    bool IsMove = false;

    //½ÅµÄÎ»ÖÃ
    public Vector3 footPos;

    Vector3 localOriginal;

    //ÒÆ¶¯³¯Ïò
    public Vector3 dir;
    Vector3 OldPos;

    //³õÊ¼Î»ÖÃ
    public Vector3 Original;

    //Åö×²¼ì²â²ã
    public LayerMask layerMask;

    //²½³¤
    public Vector3 MaxStep;

    //ÍÈ²¿IkÆ«ÒÆ
    public Vector3 LegOffset = new Vector3(0, 1.92f, 0);


    public float MoveTime = 0.3f;



    private void Awake()
    {
        if (Physics.Raycast(tbIk.transform.position, -this.transform.up, out hit, 100, layerMask))
        {
            tbIk.transform.position = hit.point + LegOffset;
            Original = hit.point + LegOffset;
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
        CanMove = true;

        if (OldPos != transform.position)
        {
            dir = this.transform.position - OldPos;
            OldPos = transform.position;
        }


        Vector3 RayPoint = this.transform.position - this.transform.right * Mathf.Abs(localOriginal.x) + this.transform.up * 10;
        if (Physics.Raycast(RayPoint, -this.transform.up, out hit, 100, layerMask))
        {
            NextPosition = hit.point + LegOffset + dir.normalized * 1f;
            Debug.DrawLine(RayPoint, hit.point);
        }

        if(!IsMove)
        tbIk.transform.position = Original;


        if (CanMove)
        {
            if (Mathf.Abs((NextPosition - LegOffset - tbIk.transform.position).x) > MaxStep.x || Mathf.Abs((NextPosition - LegOffset - tbIk.transform.position).z) > MaxStep.z)
            {
                IsMove = true;
                tbIk.transform.DOMove(NextPosition, MoveTime);
                tbIk.transform.DOMoveY(NextPosition.y + 1f, MoveTime / 2).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    tbIk.transform.DOMoveY(NextPosition.y, MoveTime / 2).OnComplete(() =>
                    {
                        ReInitialize();
                        IsMove = false;
                        CanMove = false;
                        FinishMove = true;
                        footPos = hit.point;
                        ControlALL.Instance.UpdatPostion();
                    });
                });
            }
        }

    }
}
