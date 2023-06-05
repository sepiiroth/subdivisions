using System.Collections;
using System.Collections.Generic;
using Structure;
using UnityEngine;

public class MathManager : MonoBehaviour
{
    public static MathManager Instance()
    {
        return _singleton;
    }
    private static MathManager _singleton;

    public List<Polygon> Polygons = new List<Polygon>();

    // Start is called before the first frame update
    void Awake()
    {
        if (!_singleton)
        {
            _singleton = this;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
