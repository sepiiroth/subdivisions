using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathManager : MonoBehaviour
{
    public static MathManager Instance()
    {
        return _singleton;
    }
    private static MathManager _singleton;

    public List<Polygon> Polygons = new List<Polygon>();
    public List<Vertex> Vertices = new List<Vertex>();
    public List<Edge> Edges = new List<Edge>();

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
