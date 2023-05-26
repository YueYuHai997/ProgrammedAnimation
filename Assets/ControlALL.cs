using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ControlALL : MonoBehaviour
{
    public static ControlALL Instance;

    public Control[] Controls;

    public Control[] Group_1;
    public Control[] Group_2;

    [SerializeField]
    float OffY;

    private void Awake()
    {
        Instance = this;

        foreach (var item in Group_1)
        {
            item.CanMove = true;
            item.FinishMove = false;
        }
    }


    private void Start()
    {
        foreach (var item in Controls)
        {
            OffY += item.Original.y - item.LegOffset.y;
        }
        OffY /= Controls.Length;
    }




    public float MoveSpeed;
    public float RotSpeed;
    public float CueentAngle;
    void COntrol()
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
        COntrol();
        if (Chake(Group_1))
        {
            foreach (var item in Group_2)
            {
                item.CanMove = true;
                item.FinishMove = false;
            }
        }
        if (Chake(Group_2))
        {
            foreach (var item in Group_1)
            {
                item.CanMove = true;
                item.FinishMove = false;
            }
        }
    }

    public bool Chake(Control[] controls)
    {
        foreach (var item in controls)
        {
            if (!item.FinishMove)
            {
                return false;
            }
        }


        foreach (var item in controls)
        {
            item.FinishMove = false;
        }

        return true;
    }

    float Temp;
    public void UpdatPostion()
    {
        Temp = 0;
        foreach (var item in Controls)
        {
            Temp += item.footPos.y;
        }
        Temp /= Controls.Length;

        Debug.Log(Temp);
        Debug.Log(OffY);

        this.transform.position += Vector3.one * (Temp - OffY);
        OffY = Temp;
    }

    public Vector3 RotAxis;
    private void LateUpdate()
    {
        Vector3 DirA = Controls[0].footPos - Controls[2].footPos;
        Vector3 DirB = Controls[1].footPos - Controls[3].footPos;

        Vector3 dir = Vector3.Cross(DirA, DirB);

        dir = Vector3.Dot(dir, Vector3.up) < 0 ? -dir : dir;


        RotAxis = dir;


        if (Input.GetKey(KeyCode.Q))
        {
            CueentAngle += RotSpeed;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            CueentAngle -= RotSpeed;
        }


        //Matrix4x4 m = Matrix4x4.Rotate(Quaternion.Euler(0, CueentAngle,0));

        //dir = m.MultiplyPoint3x4(dir);

        //this.transform.rotation = Quaternion.Euler(dir);

        this.transform.up = dir;
        this.transform.rotation *= Quaternion.Euler(0, CueentAngle, 0);


    }





}
