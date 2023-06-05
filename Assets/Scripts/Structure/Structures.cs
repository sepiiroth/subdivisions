using System.Collections.Generic;
using UnityEngine;

namespace Structure
{
    public class Vertex
    {
        public Vector3 vec3;

        public Vertex(Vector3 vec3)
        {
            this.vec3 = vec3;
        }
    }

    public class Edge
    {
        public List<Vertex> points;
        public List<Polygon> Polygons;

        public Edge(Vertex v1, Vertex v2, Polygon p)
        {
            points = new List<Vertex>();
            Polygons = new List<Polygon>();
            points.Add(v1);
            points.Add(v2);
            Polygons.Add(p);
        }
    }

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
}