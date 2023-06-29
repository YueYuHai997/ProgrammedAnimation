using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using RootMotion.FinalIK;
using Unity.Mathematics;

public class LimbIk_Manager : MonoBehaviour
{
    private static LimbIk_Manager instance;

    public static LimbIk_Manager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LimbIk_Manager>();
            }
            return instance;
        }
    }

    //腿部组
    public Control_LimbIk[] Controls;

    //同步组一
    public List<Control_LimbIk> Group_1 = new List<Control_LimbIk>();

    //同步组二
    public List<Control_LimbIk> Group_2 = new List<Control_LimbIk>();

    float _offY;

    //碰撞检测层
    public LayerMask layerMask;

    public float Body_High = 2;

    //移动朝向
    public Vector3 Dir;

    public Matrix4x4 MDir;

    //移动朝向权重
    public float DirWeight;

    private Vector3 _oldPos;

    private Vector3 _oldDir;

    //步长
    public Vector3 MaxStep;

    //移动过度时间
    public float MoveTime = 0.3f;

    private bool[] _canMove;

    //当前速度
    public float MoveSpeed;
    //旋转速度
    public float RotSpeed;
    //当前角度
    public float CueentAngle;

    private void Awake()
    {
        _canMove = new bool[Group_1.Count];
        foreach (var item in Group_1)
        {
            item.CanMove = true;
        }

        if (Physics.Raycast(this.transform.position, Vector3.down, out RaycastHit hit, 20, layerMask))
        {
            this.transform.position = hit.point + this.transform.up * Body_High;
        }

        foreach (var item in Controls)
        {
            item.Init();
            Debug.Log(item.name);
        }
    }

    float hight;
    private void Start()
    {
        foreach (var item in Controls)
        {
            _offY += item.Original.y - item.LegOffsetY;
        }
        _offY /= (float)Controls.Length;
    }


    void MoveControl()
    {
        if (Input.GetKey(KeyCode.W))
        {
            this.transform.position += Vector3.forward * Time.deltaTime * MoveSpeed;
        }

        if (Input.GetKey(KeyCode.S))
        {
            this.transform.position -= Vector3.forward * Time.deltaTime * MoveSpeed;
        }

        if (Input.GetKey(KeyCode.A))
        {
            this.transform.position -= Vector3.right * Time.deltaTime * MoveSpeed;
        }

        if (Input.GetKey(KeyCode.D))
        {
            this.transform.position += Vector3.right * Time.deltaTime * MoveSpeed;
        }
    }


    private void Update()
    {
        if (_oldPos != transform.position)
        {
            Dir = this.transform.position - _oldPos;
            Dir.y = 0;
        }
        _oldPos = transform.position;

        if (Vector3.Dot(_oldDir, Dir) < 0) //移动方向反向了
        {
            foreach (var item in Controls)
            {
                item.CanMove = !item.CanMove;
            }

            for (int i = 0; i < _canMove.Length; i++)
            {
                _canMove[i] = !_canMove[i];
            }

            Debug.Log("Change Dir");
        }

        _oldDir = Dir;

        MoveControl();

        RotAndBodyRot();

    }

    public bool? Chake()
    {
        for (int i = 1; i < _canMove.Length; i++)
        {
            if (_canMove[i] != _canMove[0])
            {
                return null;
            }
        }
        return _canMove[0];
    }

    Vector3 GizmosPos;
    float Temp;
    private Vector3 OldPos;
    private Vector3 InnerPos;
    public void UpdatPostion(Control_LimbIk control)
    {
        Temp = 0;
        GizmosPos = Vector3.zero;
        foreach (var item in Controls)
        {
            Temp += item.footPos.y;
            GizmosPos += item.footPos;
        }
        Temp /= (float)Controls.Length;
        GizmosPos /= (float)Controls.Length;
        Debug.Log(Temp - _offY);


        this.transform.position += Vector3.up * (Temp - _offY);

        Debug.Log(Vector3.up);
        Debug.Log(this.transform.up);

        _offY = Temp;

        if (Group_1.Contains(control))
        {
            _canMove[Group_1.FindIndex((x) => x == control)] = true;
        }
        else
        {
            _canMove[Group_2.FindIndex((x) => x == control)] = false;
        }

        if (Chake() != null)
        {
            if (!Chake().Value)
            {
                foreach (var item in Group_1)
                {
                    item.CanMove = true;
                }
            }
            else
            {
                foreach (var item in Group_2)
                {
                    item.CanMove = true;
                }
            }
        }

    }
    public Vector3 Rot;
    private void RotAndBodyRot()
    {
        Vector3 DirA = Controls[0].footPos - Controls[3].footPos;
        Vector3 DirB = Controls[1].footPos - Controls[2].footPos;

        Debug.DrawLine(Controls[0].footPos, Controls[3].footPos);
        Debug.DrawLine(Controls[1].footPos, Controls[2].footPos);

        Vector3 dir = Vector3.Cross(DirA, DirB);

        dir = Vector3.Dot(dir, Vector3.up) < 0 ? -dir : dir;
        Rot = dir;

        MDir = Matrix4x4.Rotate(quaternion.Euler(0, Vector3.Angle(dir, Vector3.up)/2, 0));
        Debug.Log(Vector3.Angle(dir, Vector3.up));


        if (Input.GetKey(KeyCode.Q))
        {
            CueentAngle += RotSpeed * Time.deltaTime * 40;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            CueentAngle -= RotSpeed * Time.deltaTime * 40;
        }

        this.transform.up = dir;
        this.transform.rotation *= Quaternion.Euler(0, CueentAngle, 0);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(this.transform.position + this.transform.up * (Temp - _offY), 1f); //世界坐标
    }
}
