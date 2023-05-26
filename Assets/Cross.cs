using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Cross : MonoBehaviour
{

    public Transform A;
    public Transform B;
    public Transform C;

    public Transform Light;

    Vector3 AB;
    Vector3 AC;
    Vector3 BC;

    private Mesh mesh;

    private void Update()
    {
        AB = B.position - A.position;
        AC = C.position - A.position;
        BC = C.position - B.position;

        Vector3 nor1 = Vector3.Cross(AB, AC);
        Vector3 nor2 = Vector3.Cross(AC, BC);
        Vector3 nor3 = Vector3.Cross(AB, BC);

        Vector3 core = (A.transform.position + B.transform.position + C.transform.position) / 3;
        Debug.DrawLine(core, core + nor1);
        Debug.DrawLine(core, core + nor2);
        Debug.DrawLine(core, core + nor3);



        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";
        mesh.vertices = new Vector3[] { C.transform.position, A.transform.position, B.transform.position };
        mesh.triangles = new int[] { 0, 1, 2 };



        //模拟光照
        Plane plane = new Plane(C.position, A.position, B.position);
        Vector3 touchPoint = Vector3.zero;

        //判断平面和射线相交
        if (plane.Raycast(new Ray(Light.position, Light.forward), out float distance))
        {
            touchPoint = Light.position + Light.forward * distance;
        }

        if (touchPoint != Vector3.zero)
        {
            Vector3 Light_Dir = Light.forward.normalized;

            Vector3 Light_reflect = Vector3.Reflect(Light_Dir, nor1).normalized;
            Light_reflect *=1- Vector3.Dot(Light_reflect, Light_Dir);

            Debug.DrawLine(touchPoint - Light_Dir, touchPoint);
            Debug.DrawLine(touchPoint, touchPoint + Light_reflect);
        }

    }

}
