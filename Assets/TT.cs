using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TT : MonoBehaviour
{
    public GameObject A;

    void Start()
    {


    }

    float CueentAngle;
    public float Speed;
    void Update()
    {

        if (Physics.Raycast(A.transform.position, -A.transform.up, out RaycastHit hit))
        {
            A.transform.up = hit.normal;
            Debug.DrawLine(A.transform.position, hit.point);
        }


        if (Input.GetKey(KeyCode.Q))
        {
            CueentAngle += Time.deltaTime * Speed;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            CueentAngle -= Time.deltaTime * Speed;
        }

        A.transform.up = hit.normal;
        A.transform.rotation *= Quaternion.Euler(0,CueentAngle,0);


    }
}
