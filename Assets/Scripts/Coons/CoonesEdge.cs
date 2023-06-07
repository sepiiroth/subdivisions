using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoonesEdge : MonoBehaviour
{
    [HideInInspector] public CoonesVertice pointA;
    [HideInInspector] public CoonesVertice pointB;

    public void Start()
    {
        //gameObject.GetComponent<LineRenderer>().SetPosition(0, pointA.transform.position);
        //gameObject.GetComponent<LineRenderer>().SetPosition(1, pointB.transform.position);

    }

    public Vector3 GetVector()
    {
        return (pointB.transform.position - pointA.transform.position).normalized;
    }

    public Vector3 GetReverseVector()
    {
        return (pointA.transform.position - pointB.transform.position).normalized;
    }
}
