using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using DG.Tweening;
using Unity.VisualScripting;
using System.Threading.Tasks;

public class Control_LimbIk : MonoBehaviour
{
    public LimbIK Ik;
    public bool CanMove = false;
    //脚的位置
    public Vector3 footPos;
    Vector3 localOriginal;

    //初始位置
    public Vector3 Original;

    //腿部Ik偏移
    public float LegOffsetY = 2;

    private bool _IsMoving;

    public void Init()
    {
        if (Physics.Raycast(Ik.transform.position + Vector3.up *10,  -Vector3.up, out hit, 100, LimbIk_Manager.Instance.layerMask))
        {
            Ik.transform.position = hit.point + (this.transform.up * LegOffsetY);
            Original = hit.point + this.transform.up * LegOffsetY;
            footPos = hit.point;

            Debug.Log(footPos+this.gameObject.name);
        }

        Debug.DrawLine(Ik.transform.position, hit.point, Color.red);

        localOriginal = Ik.transform.localPosition;
    }

    void ReInitialize()
    {
        Original = Ik.transform.position;
    }

    Vector3 NextPosition;
    RaycastHit hit;
    // Update is called once per frame
    void Update()
    {

        Vector3 RayPoint = this.transform.position - this.transform.right * Mathf.Abs(localOriginal.x) + this.transform.up * 10;
        if (Physics.Raycast(RayPoint, -this.transform.up, out hit, 100, LimbIk_Manager.Instance.layerMask))
        {
            NextPosition = hit.point + this.transform.up * LegOffsetY + LimbIk_Manager.Instance.MDir.MultiplyVector(LimbIk_Manager.Instance.Dir).normalized * LimbIk_Manager.Instance.DirWeight;
            
            //NextPosition = hit.point + this.transform.up * LegOffsetY + LimbIk_Manager.Instance.Dir.normalized * LimbIk_Manager.Instance.DirWeight;

            Debug.DrawLine(RayPoint, hit.point);
        }

        Debug.DrawLine(NextPosition + new Vector3(0, 10, 0), NextPosition, Color.green);

        Debug.DrawLine(this.transform.up * LegOffsetY - Ik.transform.position + new Vector3(0, 10, 0),
            this.transform.up * LegOffsetY - Ik.transform.position, Color.magenta);

        if (CanMove)
        {
            if (Mathf.Abs((NextPosition - this.transform.up * LegOffsetY - Ik.transform.position).x) > LimbIk_Manager.Instance.MaxStep.x
                || Mathf.Abs((NextPosition - this.transform.up * LegOffsetY - Ik.transform.position).z) > LimbIk_Manager.Instance.MaxStep.z)
            {
                _IsMoving = true;
                Ik.transform.DOMove(NextPosition, LimbIk_Manager.Instance.MoveTime);
                Ik.transform.DOMoveY(NextPosition.y + 1f, LimbIk_Manager.Instance.MoveTime / 2).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    Ik.transform.DOMoveY(NextPosition.y, LimbIk_Manager.Instance.MoveTime / 2).OnComplete(() =>
                    {
                        _IsMoving = false;
                        ReInitialize();
                        CanMove = false;
                        footPos = hit.point;
                        LimbIk_Manager.Instance.UpdatPostion(this);
                    });
                });
            }
        }
    }

    private void LateUpdate()
    {
        if(!_IsMoving)
            Ik.transform.position = Original;
    }
}
