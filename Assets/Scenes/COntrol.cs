using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class COntrol : MonoBehaviour
{
    public Transform Tag1;
    public Transform Player1;
    public Transform Player1_Copy;


    public Transform Tag2;
    public Transform Player2;

    void Update()
    {
        Debug.Log(Player1.localToWorldMatrix);
        Debug.Log(Tag1.worldToLocalMatrix);

        //Tag2的本地坐标转世界坐标矩阵  Tag1的世界坐标转本地坐标矩阵   Player1的本地坐标转世界坐标矩阵
        Matrix4x4 rot = Tag2.localToWorldMatrix * Tag1.worldToLocalMatrix * Player1.localToWorldMatrix;
        Player2.SetPositionAndRotation(rot.GetColumn(3), rot.rotation);


        Player1_Copy.SetPositionAndRotation(Player1.localToWorldMatrix.GetColumn(3), rot.rotation);


        Debug.Log(rot);
        Debug.Log(rot.GetColumn(3));

    }
}
