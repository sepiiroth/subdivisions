using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : MonoBehaviour
{
    public List<Vertex> points;
    public List<Polygon> Polygons;
    
    // Start is called before the first frame update
    void Start()
    {
        points = new List<Vertex>();
        Polygons = new List<Polygon>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetEdge(Vertex v1, Vertex v2, Polygon p)
    {
        points.Add(v1);
        points.Add(v2);
        Polygons.Add(p);
    }

    public void SetEdge(Vertex v1, Vertex v2)
    {
        points.Add(v1);
        points.Add(v2);
    }
}
