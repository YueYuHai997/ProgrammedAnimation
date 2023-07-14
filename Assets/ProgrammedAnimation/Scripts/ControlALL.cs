using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;

public class ControlALL : MonoBehaviour
{
    private static ControlALL instance;

    public static ControlALL Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ControlALL>();
            }
            return instance;
        }
    }

    //腿部组
    public Control[] Controls;

    //同步组一
    public List<Control> Group_1 = new List<Control>();

    //同步组二
    public List<Control> Group_2 = new List<Control>();

    float _offY;

    //碰撞检测层
    public LayerMask layerMask;

    public float Body_High = 2;

    //移动朝向
    public Vector3 Dir;

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

        if (Physics.Raycast(this.transform.position, Vector3.down, out RaycastHit hit, 100, layerMask))
        {
            this.transform.position = hit.point + this.transform.up * Body_High;
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
            this.transform.position += this.transform.forward * Time.deltaTime * MoveSpeed;
        }

        if (Input.GetKey(KeyCode.S))
        {
            this.transform.position -= this.transform.forward * Time.deltaTime * MoveSpeed;
        }

        if (Input.GetKey(KeyCode.A))
        {
            this.transform.position -= this.transform.right * Time.deltaTime * MoveSpeed;
        }

        if (Input.GetKey(KeyCode.D))
        {
            this.transform.position += this.transform.right * Time.deltaTime * MoveSpeed;
        }

    }


    private void Update()
    {

        if (_oldPos != transform.position)
        {
            Dir = this.transform.position - _oldPos;
            Dir.y = 0;
            _oldPos = transform.position;
        }

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

    float Temp;
    private Vector3 OldPos;
    private Vector3 InnerPos;
    public void UpdatPostion(Control control)
    {
        Temp = 0;
        foreach (var item in Controls)
        {
            Temp += item.footPos.y;
        }
        Temp /= (float)Controls.Length;

        InnerPos = this.transform.position - OldPos;

        this.transform.position += Vector3.up * (Temp - _offY);

        //减去输入已经导致的Y轴移动
        this.transform.position -= InnerPos * Mathf.Sin(20 * Mathf.Deg2Rad);


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

        OldPos = this.transform.position;
    }

    public Vector3 RotAxis;


    private void RotAndBodyRot()
    {
        Vector3 DirA = Controls[0].footPos - Controls[2].footPos;
        Vector3 DirB = Controls[1].footPos - Controls[3].footPos;

        Vector3 dir = Vector3.Cross(DirA, DirB);

        dir = Vector3.Dot(dir, Vector3.up) < 0 ? -dir : dir;
        RotAxis = dir;

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
}
