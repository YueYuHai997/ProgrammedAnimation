using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class matri : MonoBehaviour
{
    public GameObject Tag;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Matrix4x4 T = new Matrix4x4(
            new Vector4(1, 0, 0, 1),
            new Vector4(0, 1, 0, 1),
            new Vector4(0, 0, 1, 1),
            new Vector4(0, 0, 0, 1));

            Vector4 Pos = new Vector4(Tag.transform.position.x, Tag.transform.position.y, Tag.transform.position.z, 1);

            Pos = T.transpose * Pos;
            Tag.transform.position = new Vector3(Pos.x, Pos.y, Pos.z);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log(Tag.transform.localToWorldMatrix);
        }
    }
}
