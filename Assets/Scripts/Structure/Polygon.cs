using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon
{
    public List<Vertex> Vertices;
    public List<Edge> Edges;
    
    public Polygon()
    {
        Vertices = new List<Vertex>();
        Edges = new List<Edge>();
    }
    
}
