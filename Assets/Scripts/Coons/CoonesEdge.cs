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
}
